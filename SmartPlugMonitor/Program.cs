using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using log4net;

using SmartPlugMonitor.Platform;
using SmartPlugMonitor.Toolbox;
using SmartPlugMonitor.Workers;

namespace SmartPlugMonitor
{
    public class Program
    {
        private static readonly ILog Log = LogManager.GetLogger (typeof(Program));

        public static void Main (string[] args)
        {
            IPlatform platform;
            if (Globals.IsUnix) {
                platform = new UnixPlatform ();
            } else {
                platform = new Win32Platform ();
            }
            platform.Init ();

            try {
                new Program (platform).Run ();
            } catch (AggregateException ex) {
                var message = ex.Flatten ().InnerException.ToString ();
                Log.Error (message);
                throw;
            } catch (Exception ex) {
                var message = (ex.InnerException != null) ? ex.InnerException.ToString () : ex.ToString ();
                Log.Error (message);
                throw;
            }
        }

        private readonly IPlatform Platform;
        private readonly TextIconRenderer TextIconRenderer;
        private readonly ITrayIconStrip TrayIconStrip;
        private readonly IDictionary<string, EventHandler> ContextMenu;
        private readonly SensorWorkerRunner SensorWorkerRunner;

        private IWindow configWindow;

        public Program (IPlatform platform)
        {
            this.Platform = platform;

            this.TextIconRenderer = new TextIconRenderer (
                Globals.ConfigFile.TrayIconConfig.FontFamily,
                Globals.ConfigFile.TrayIconConfig.FontSize,
                Globals.ConfigFile.TrayIconConfig.IconSize);

            this.TrayIconStrip = Platform.CreateTrayIconStrip ();

            this.ContextMenu = new Dictionary<string, EventHandler> {
                { "Configure", OnToggleConfigWindow },
                { "Quit", OnQuit }
            };

            this.SensorWorkerRunner = new SensorWorkerRunner ();
            this.SensorWorkerRunner.SensorResultsChanged += OnUpdateTrayIcon;
        }

        public void Run ()
        {
            SensorWorkerRunner.Start (Platform.CreateSensorWorkers ());
            Platform.ApplicationRun ();
        }

        public void OnQuit (object sender, EventArgs e)
        {
            Log.Info ("OnQuit");

            if (configWindow != null) {
                configWindow.Close ();
            }

            SensorWorkerRunner.Dispose ();
            TrayIconStrip.Dispose ();

            Platform.ApplicationExit ();
        }

        public void OnToggleConfigWindow (object sender, EventArgs e)
        {
            Log.Info ($"OnToggleConfigWindow ({configWindow == null})");

            if (configWindow != null) {
                configWindow.Close ();
            } else {
                configWindow = Platform.CreateConfigWindow ();
                configWindow.OnClose += OnConfigWindowClosed;

                var cursorPos = Platform.GetCursorPosition (configWindow);

                var windowX = cursorPos.X - (configWindow.Width / 2);
                var windowY = cursorPos.Y - configWindow.Height - 20;
                if (windowY < 0) {
                    windowY = cursorPos.Y + 20;
                }

                configWindow.Show (new Point (windowX, windowY));
            }
        }

        public void OnConfigWindowClosed (object sender, EventArgs e)
        {
            Log.Info ("OnConfigWindowClosed");

            configWindow = null;
            Globals.SaveConfigFile ();

            SensorWorkerRunner.Stop ();
            SensorWorkerRunner.Start (Platform.CreateSensorWorkers ());
        }

        public void OnUpdateTrayIcon (object sender, SensorWorkerRunner.SensorResultsChangedEventArgs e)
        {
            Platform.ApplicationInvoke (() => {
                if (e.SensorResults.Count <= 0) {
                    TrayIconStrip.Update (new Action<ITrayIcon>[] { trayIcon => {
                            trayIcon.ToolTipText = Globals.ApplicationName;
                            trayIcon.Icon = Globals.ApplicationIcon.ToBitmap ();
                            trayIcon.ContextMenu = ContextMenu;
                            trayIcon.OnActivate = OnToggleConfigWindow;
                            if (!trayIcon.Visible) {
                                trayIcon.Visible = true;
                            }
                        }
                    });
                    return;
                }

                TrayIconStrip.Update (e.SensorResults.Select (result => new Action<ITrayIcon> (trayIcon => {
                    var icon = TextIconRenderer.Render (result.Value, Color.White);

                    if (Log.IsDebugEnabled) {
                        using (var fs = File.Create ("trayIcon.png")) {
                            icon.Save (fs, ImageFormat.Png);
                        }
                    }

                    trayIcon.ToolTipText = $"{result.ValueName} ({result.SensorName})";
                    trayIcon.Icon = icon;
                    trayIcon.ContextMenu = ContextMenu;
                    trayIcon.OnActivate = OnToggleConfigWindow;
                    if (!trayIcon.Visible) {
                        trayIcon.Visible = true;
                    }
                })));
            });
        }
    }
}
