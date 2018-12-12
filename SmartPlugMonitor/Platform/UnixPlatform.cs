using System.Collections.Generic;
using System.Drawing;
using Gtk;

using SmartPlugMonitor.Sensors.HS110;
using SmartPlugMonitor.Toolbox;
using SmartPlugMonitor.Ui;
using SmartPlugMonitor.Workers;

namespace SmartPlugMonitor.Platform
{
    public class UnixPlatform : IPlatform
    {
        public void Init()
        {
            Application.Init();
        }

        public void ApplicationRun()
        {
            Application.Run();
        }

        public void ApplicationExit()
        {
            Application.Quit();
        }

        public void ApplicationInvoke(System.Action action)
        {
            Application.Invoke((s, e) => action());
        }

        public Point GetCursorPosition(IWindow window)
        {
            int cursorX;
            int cursorY;
            ((GtkConfigWindow)window).Display.GetPointer(out cursorX, out cursorY);
            return new Point(cursorX, cursorY);
        }

        public ITrayIconStrip CreateTrayIconStrip()
        {
            return new TrayIconStrip<GtkTrayIcon>();
        }

        public IWindow CreateConfigWindow()
        {
            return new GtkConfigWindow(new GtkConfigPage[] {
                new GtkConfigPage (new HS110ConfigController ()),
            });
        }

        public IEnumerable<ISensorWorker> CreateSensorWorkers()
        {
            yield return new HS110SensorWorker(Globals.ConfigFile.HS110Config);
        }
    }
}
