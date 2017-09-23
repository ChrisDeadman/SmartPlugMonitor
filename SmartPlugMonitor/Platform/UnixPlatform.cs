using System;
using System.Drawing;
using System.Collections.Generic;

using Gtk;

using SmartPlugMonitor.Ui;
using SmartPlugMonitor.Toolbox;
using SmartPlugMonitor.Workers;
using SmartPlugMonitor.Sensors.TpLink;

namespace SmartPlugMonitor.Platform
{
    public class UnixPlatform : IPlatform
    {
        public void Init ()
        {
            Application.Init ();
        }

        public void ApplicationRun ()
        {
            Application.Run ();
        }

        public void ApplicationExit ()
        {
            Application.Quit ();
        }

        public void ApplicationInvoke (System.Action action)
        {
            Application.Invoke ((s, e) => action ());
        }

        public Point GetCursorPosition (IWindow window)
        {
            int cursorX;
            int cursorY;
            ((GtkConfigWindow)window).Display.GetPointer (out cursorX, out cursorY);
            return new Point (cursorX, cursorY);
        }

        public ITrayIconStrip CreateTrayIconStrip ()
        {
            return new TrayIconStrip<GtkTrayIcon> ();
        }

        public IWindow CreateConfigWindow ()
        {
            return new GtkConfigWindow (new GtkConfigPage[] {
                new GtkConfigPage (new TpLinkConfigController ())
            });
        }

        public IEnumerable<ISensorWorker> CreateSensorWorkers ()
        {
            yield return new TpLinkSensorWorker (Globals.ConfigFile.TpLinkConfig);
        }
    }
}
