using log4net;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

using SmartPlugMonitor.Ui;
using SmartPlugMonitor.Workers;
using SmartPlugMonitor.Toolbox;
using SmartPlugMonitor.Platform;
using SmartPlugMonitor.Sensors.TpLink;

namespace SmartPlugMonitor
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger (typeof(Program));

        private static IPlatform Platform;

        private readonly ITrayIconStrip trayIconStrip;
        private readonly TextIconRenderer textIconRenderer;
        private readonly IDictionary<string, EventHandler> contextMenu;
        private readonly SensorWorkerRunner sensorWorkerRunner;

        private IWindow configWindow;

        public static void Main (string[] args)
        {
            if (Globals.IsUnix) {
                Platform = new UnixPlatform ();
            } else {
                Platform = new Win32Platform ();
            }
            Platform.Init ();

            try {
                new Program ().Start ();
                Platform.ApplicationRun ();
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

        public Program ()
        {
            trayIconStrip = Platform.CreateTrayIconStrip ();
            textIconRenderer = new TextIconRenderer ();

            contextMenu = new Dictionary<string, EventHandler> {
                { "Configure", OnToggleConfigWindow },
                { "Quit", OnQuit }
            };

            sensorWorkerRunner = new SensorWorkerRunner ();
            sensorWorkerRunner.SensorResultsChanged += OnUpdateTrayIcon;
        }

        public void Start ()
        {
            sensorWorkerRunner.Start (Platform.CreateSensorWorkers ());
        }

        public void OnQuit (object sender, EventArgs e)
        {
            Log.Info ("OnQuit");

            if (configWindow != null) {
                configWindow.Close ();
            }

            sensorWorkerRunner.Dispose ();
            trayIconStrip.Dispose ();

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

            sensorWorkerRunner.Stop ();
            sensorWorkerRunner.Start (Platform.CreateSensorWorkers ());
        }

        public void OnUpdateTrayIcon (object sender, SensorWorkerRunner.SensorResultsChangedEventArgs e)
        {
            Platform.ApplicationInvoke (() => {
                if (e.SensorResults.Count <= 0) {
                    trayIconStrip.Update (new Action<ITrayIcon>[] { trayIcon => {
                            trayIcon.ToolTipText = Globals.ApplicationName;
                            trayIcon.Icon = Globals.ApplicationIcon.ToBitmap ();
                            trayIcon.ContextMenu = contextMenu;
                            trayIcon.OnActivate = OnToggleConfigWindow;
                            if (!trayIcon.Visible) {
                                trayIcon.Visible = true;
                            }
                        }
                    });
                    return;
                }

                trayIconStrip.Update (e.SensorResults.Select (result => new Action<ITrayIcon> (trayIcon => {
                    var icon = textIconRenderer.Render (result.Value);

                    if (Log.IsDebugEnabled) {
                        using (var fs = File.Create ("trayIcon.png")) {
                            icon.Save (fs, ImageFormat.Png);
                        }
                    }

                    trayIcon.ToolTipText = $"{result.ValueName} ({result.SensorName})";
                    trayIcon.Icon = icon;
                    trayIcon.ContextMenu = contextMenu;
                    trayIcon.OnActivate = OnToggleConfigWindow;
                    if (!trayIcon.Visible) {
                        trayIcon.Visible = true;
                    }
                })));
            });
        }
    }
}
