using System;
using System.Linq;
using System.Drawing;

using Gtk;

namespace SmartPlugMonitor.Toolbox
{
    public class GtkWindow : Window, IWindow
    {
        public event EventHandler OnClose;

        public GtkWindow (string title) : base (title)
        {
        }

        public int Width {
            get { 
                int width;
                int height;
                base.GetSize (out width, out height);
                return width;
            }
        }

        public int Height {
            get { 
                int width;
                int height;
                base.GetSize (out width, out height);
                return height;
            }
        }

        public Point Position {
            get {
                int x;
                int y;
                base.GetPosition (out x, out y);
                return new Point (x, y);
            }
            set {
                base.Move (value.X, value.Y);
            }
        }

        public void Show (Point position)
        {
            base.Show ();
            Position = position;
        }

        public void Close ()
        {
            this.Destroy ();
        }

        protected override void OnDestroyed ()
        {
            if (OnClose != null) {
                OnClose (this, EventArgs.Empty);
            }

            base.OnDestroyed ();
        }
    }
}
