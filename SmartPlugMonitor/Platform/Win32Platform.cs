using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using SmartPlugMonitor.Sensors.HS110;
using SmartPlugMonitor.Toolbox;
using SmartPlugMonitor.Ui;
using SmartPlugMonitor.Workers;

namespace SmartPlugMonitor.Platform
{
    public class Win32Platform : IPlatform
    {
        private static Form dummyForm;

        public void Init()
        {
            dummyForm = new Form { Size = new Size() };
            dummyForm.Show();
            dummyForm.Visible = false;
        }

        public void ApplicationRun()
        {
            Application.Run();
        }

        public void ApplicationExit()
        {
            Application.Exit();
        }

        public void ApplicationInvoke(Action action)
        {
            dummyForm.Invoke(action);
        }

        public Point GetCursorPosition(IWindow window)
        {
            return Cursor.Position;
        }

        public ITrayIconStrip CreateTrayIconStrip()
        {
            return new TrayIconStrip<Win32TrayIcon>();
        }

        public IWindow CreateConfigWindow()
        {
            return new Win32ConfigWindow(new Win32ConfigPage[] {
                new Win32ConfigPage (new HS110ConfigController ()),
            }) {
                TopMost = true
            };
        }

        public IEnumerable<ISensorWorker> CreateSensorWorkers()
        {
            yield return new HS110SensorWorker(Globals.ConfigFile.HS110Config);
        }
    }
}
