using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SmartPlugMonitor.Toolbox
{
    public class Win32TrayIcon : ITrayIcon
    {
        private readonly NotifyIcon notifyIcon;

        public Win32TrayIcon ()
        {
            notifyIcon = new NotifyIcon ();
            notifyIcon.Click += new EventHandler (OnNotifyIconClick);
        }

        public bool Visible {
            get { return notifyIcon.Visible; }
            set { notifyIcon.Visible = value; }
        }

        public string ToolTipText {
            set { notifyIcon.Text = value; }
        }

        public Bitmap Icon {
            set { notifyIcon.Icon = value.ToIcon (); }
        }

        public IDictionary<string, EventHandler> ContextMenu {
            set {
                var contextMenuStrip = new ContextMenuStrip ();

                foreach (var entry in value) {
                    var item = new ToolStripMenuItem { Text = entry.Key };
                    item.Click += entry.Value;
                    contextMenuStrip.Items.Add (item);
                }

                notifyIcon.ContextMenuStrip = contextMenuStrip;
            }
        }

        public EventHandler OnActivate { get; set; }

        void OnNotifyIconClick (object sender, EventArgs e)
        {
            var mouseArgs = e as MouseEventArgs;
            if (mouseArgs != null) {
                switch (mouseArgs.Button) {
                case MouseButtons.Left:
                    if (OnActivate != null) {
                        OnActivate.Invoke (sender, e);
                    }
                    break;
                }
            }
        }

        public void Dispose ()
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose ();
        }
    }
}