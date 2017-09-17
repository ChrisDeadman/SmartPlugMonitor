using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SmartPlugMonitor.Ui
{
    public class TrayIconStrip : IDisposable
    {
        private readonly List<NotifyIcon> trayIcons;

        public TrayIconStrip (int size)
        {
            trayIcons = new List<NotifyIcon> ();
            for (int idx = 0; idx < size; idx++) {
                trayIcons.Add (new NotifyIcon ());
            }
        }

        public ICollection<NotifyIcon> TrayIcons { get { return trayIcons.AsReadOnly (); } }

        public void Dispose ()
        {
            trayIcons.ForEach (icon => {
                icon.Visible = false;
                icon.Dispose ();
            });
        }
    }
}
