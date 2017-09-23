using System;
using System.Drawing;
using System.Collections.Generic;

using Gtk;

namespace SmartPlugMonitor.Toolbox
{
    public class GtkTrayIcon : ITrayIcon
    {
        private readonly StatusIcon statusIcon;

        public GtkTrayIcon ()
        {
            statusIcon = new StatusIcon ();
            statusIcon.Activate += new EventHandler (OnStatusIconClick);
            statusIcon.PopupMenu += (s, a) => OnStatusIconPopupMenu ();
        }

        public Boolean Visible {
            get { return statusIcon.Visible; }
            set { statusIcon.Visible = value; }
        }

        public string ToolTipText {
            set { statusIcon.Tooltip = value; }
        }

        public Bitmap Icon {
            set { statusIcon.Pixbuf = value.ToPixbuf (); }
        }

        public IDictionary<string, EventHandler> ContextMenu { get; set; }

        public EventHandler OnActivate { get; set; }

        void OnStatusIconClick (object sender, EventArgs e)
        {
            if (OnActivate != null) {
                OnActivate.Invoke (sender, e);
            }
        }

        void OnStatusIconPopupMenu ()
        {
            var contextMenu = ContextMenu;
            if (contextMenu == null) {
                return;
            }

            var popupMenu = new Menu ();

            foreach (var entry in contextMenu) {
                var item = new MenuItem (entry.Key);
                item.Activated += entry.Value;
                popupMenu.Add (item);
            }

            popupMenu.ShowAll ();
            statusIcon.PresentMenu (popupMenu, 0, Global.CurrentEventTime);
        }

        public void Dispose ()
        {
            statusIcon.Visible = false;
            statusIcon.Dispose ();
        }
    }
}
