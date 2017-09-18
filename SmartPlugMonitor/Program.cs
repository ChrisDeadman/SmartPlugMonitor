﻿using log4net;
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
            private readonly ISensorWorkerBuilder[] sensorWorkerBuilders;

            private readonly TrayIconStrip trayIconStrip = new TrayIconStrip (3);
            private readonly TextIconRenderer trayIconTextWriter = new TextIconRenderer ();

            private Form configWindow;

            public AppContext ()
            {
                Application.ApplicationExit += new EventHandler (Quit);
               
                var configureItem = new ToolStripMenuItem { Text = "Configure" };
                configureItem.Click += new EventHandler (ToggleConfigWindow);

                var quitItem = new ToolStripMenuItem { Text = "Quit" };
                quitItem.Click += new EventHandler (Quit);

                var contextMenuStrip = new ContextMenuStrip ();
                contextMenuStrip.Items.AddRange (new ToolStripMenuItem[] { configureItem, quitItem });

                foreach (var trayIcon in trayIconStrip.TrayIcons) {
                    trayIcon.Text = Globals.ApplicationName;
                    trayIcon.ContextMenuStrip = contextMenuStrip;
                    trayIcon.Click += new EventHandler ((s, e) => {
                        var args = e as MouseEventArgs;
                        if (args.Button == MouseButtons.Left) {
                            ToggleConfigWindow (this, e);
                        }
                    });
                }

                sensorWorkerBuilders = new ISensorWorkerBuilder[] {
                    new TpLinkSensorWorker.TpLinkSensorWorkerBuilder ()
                };
                sensorWorkerRunner = new SensorWorkerRunner ();
                sensorWorkerRunner.SensorResultsChanged += OnUpdateTrayIcon;
                sensorWorkerRunner.Start (sensorWorkerBuilders);
            }

            public void Quit (object sender, EventArgs args)
            {
                Log.Debug ("Quit");

                if (configWindow != null) {
                    configWindow.Close ();
                }

                sensorWorkerRunner.Dispose ();
                trayIconStrip.Dispose ();

                Application.Exit ();
            }

            void ToggleConfigWindow (object sender, EventArgs args)
            {
                Log.Debug ("ShowConfigWindow");

                if (configWindow != null) {
                    configWindow.Close ();
                } else {
                    configWindow = new ConfigurationUi ();
                    configWindow.FormClosed += OnConfigWindowClosed;
                    configWindow.Show ();
                    configWindow.Location = new Point (Cursor.Position.X - configWindow.Width, Cursor.Position.Y - configWindow.Height - 50);
                }
            }

            void OnConfigWindowClosed (object sender, EventArgs args)
            {
                Log.Debug ("OnConfigWindowClosed");

                configWindow = null;

                sensorWorkerRunner.Stop ();
                sensorWorkerRunner.Start (sensorWorkerBuilders);
            }

            void OnUpdateTrayIcon (object sender, SensorWorkerRunner.SensorResultsChangedEventArgs args)
            {
                var trayIconEnumerator = trayIconStrip.TrayIcons.GetEnumerator ();

                if (args.SensorResults.Count <= 0) {
                    if (trayIconEnumerator.MoveNext ()) {
                        var trayIcon = trayIconEnumerator.Current;
                        trayIcon.Text = Globals.ApplicationName;
                        trayIcon.Icon = Globals.ApplicationIcon;
                        trayIcon.Visible = true;
                    }
                }

                foreach (var sensorEntry in args.SensorResults) {
                    if (!trayIconEnumerator.MoveNext ()) {
                        break;
                    }
                    var trayIcon = trayIconEnumerator.Current;
                    trayIcon.Text = $"{sensorEntry.SensorName}:{sensorEntry.ValueName}";
                    trayIcon.Icon = trayIconTextWriter.Render (sensorEntry.Value);
                    if (!trayIcon.Visible) {
                        Thread.Sleep (100); // Ensure correct tray icon order
                    }
                    trayIcon.Visible = true;

                    if (Log.IsDebugEnabled) {
                        using (var fs = File.Create ("trayIcon.ico")) {
                            trayIcon.Icon.Save (fs);
                        }
                    }
                }

                while (trayIconEnumerator.MoveNext ()) {
                    trayIconEnumerator.Current.Visible = false;
                }
            }
        }
    }
}
