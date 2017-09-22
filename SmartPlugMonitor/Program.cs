using log4net;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;

using SmartPlugMonitor.Ui;
using SmartPlugMonitor.Workers;
using SmartPlugMonitor.Config;
using SmartPlugMonitor.Workers.TpLink;
using SmartPlugMonitor.Toolbox;

namespace SmartPlugMonitor
{
    static class Program
    {
        private static readonly ILog Log = LogManager.GetLogger (typeof(Program));

        [STAThread]
        static void Main ()
        {
            Application.EnableVisualStyles ();
            Application.SetCompatibleTextRenderingDefault (false);

            try {
                var appContext = new AppContext ();
                Application.Run (appContext);
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

        private class AppContext: ApplicationContext
        {
            private readonly SensorWorkerRunner sensorWorkerRunner;

            private readonly TrayIconStrip trayIconStrip = new TrayIconStrip ();
            private readonly ContextMenuStrip contextMenuStrip = new ContextMenuStrip ();
            private readonly TextIconRenderer trayIconTextWriter = new TextIconRenderer ();

            private Form configWindow;

            public AppContext ()
            {
                Application.ApplicationExit += new EventHandler (OnQuit);
               
                InitContextMenuStrip ();

                sensorWorkerRunner = new SensorWorkerRunner ();
                sensorWorkerRunner.SensorResultsChanged += OnUpdateTrayIcon;
                sensorWorkerRunner.Start (CreateSensorWorkers ());
            }

            private void InitContextMenuStrip ()
            {
                var configureItem = new ToolStripMenuItem { Text = "Configure" };
                configureItem.Click += new EventHandler (OnToggleConfigWindow);

                var quitItem = new ToolStripMenuItem { Text = "Quit" };
                quitItem.Click += new EventHandler (OnQuit);

                contextMenuStrip.Items.AddRange (new ToolStripMenuItem[] { configureItem, quitItem });
            }

            private static ConfigForm CreateConfigForm ()
            {
                return new ConfigForm (new ConfigTab[] {
                    new ConfigTabTpLink ()
                });
            }

            private static IEnumerable<ISensorWorker> CreateSensorWorkers ()
            {
                return new ISensorWorker[] {
                    new TpLinkSensorWorker (Globals.ConfigFile.TpLinkConfig)
                };
            }

            public void OnQuit (object sender, EventArgs args)
            {
                Log.Debug ("OnQuit");

                if (configWindow != null) {
                    configWindow.Close ();
                }

                sensorWorkerRunner.Dispose ();
                trayIconStrip.Dispose ();

                Application.Exit ();
            }

            void OnToggleConfigWindow (object sender, EventArgs args)
            {
                var mouseArgs = args as MouseEventArgs;
                if ((mouseArgs == null) || (mouseArgs.Button != MouseButtons.Left)) {
                    return;
                }

                Log.Debug ("OnToggleConfigWindow");

                if (configWindow != null) {
                    configWindow.Close ();
                } else {
                    configWindow = CreateConfigForm ();
                    configWindow.FormClosed += OnConfigWindowClosed;
                    configWindow.Show ();
                    configWindow.Location = new Point (Cursor.Position.X - configWindow.Width, Cursor.Position.Y - configWindow.Height - 50);
                    configWindow.Focus ();
                }
            }

            void OnConfigWindowClosed (object sender, EventArgs args)
            {
                Log.Debug ("OnConfigWindowClosed");

                configWindow = null;

                sensorWorkerRunner.Stop ();
                sensorWorkerRunner.Start (CreateSensorWorkers ());
            }

            void OnUpdateTrayIcon (object sender, SensorWorkerRunner.SensorResultsChangedEventArgs args)
            {
                if (args.SensorResults.Count <= 0) {
                    trayIconStrip.Update (new Action<NotifyIcon>[] { trayIcon => {
                            trayIcon.Text = Globals.ApplicationName;
                            trayIcon.Icon = Globals.ApplicationIcon;
                            trayIcon.ContextMenuStrip = contextMenuStrip;
                            trayIcon.Click += new EventHandler (OnToggleConfigWindow);
                            trayIcon.Visible = true;
                        }
                    });
                    return;
                }

                trayIconStrip.Update (args.SensorResults.Select (result => new Action<NotifyIcon> (trayIcon => {
                    trayIcon.Text = $"{result.ValueName} ({result.SensorName})";
                    trayIcon.Icon = trayIconTextWriter.Render (result.Value);
                    trayIcon.ContextMenuStrip = contextMenuStrip;
                    trayIcon.Click += new EventHandler (OnToggleConfigWindow);
                    trayIcon.Visible = true;

                    if (Log.IsDebugEnabled) {
                        using (var fs = File.Create ("trayIcon.ico")) {
                            trayIcon.Icon.Save (fs);
                        }
                    }
                })));
            }
        }
    }
}
