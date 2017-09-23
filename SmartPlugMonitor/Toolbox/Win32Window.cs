using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;

namespace SmartPlugMonitor.Toolbox
{
    public class Win32Window : Form, IWindow
    {
        public event EventHandler OnClose;

        public Point Position {
            get { return base.Location; }
            set { base.Location = value; }
        }

        public void Show (Point position)
        {
            var sizeTmp = base.Size;
            base.Size = new Size ();
            base.Visible = false;
            base.Show ();
            Position = position;
            base.Size = sizeTmp;
        }

        protected override void Dispose (bool disposing)
        {
            if (OnClose != null) {
                OnClose (this, EventArgs.Empty);
            }

            base.Dispose (disposing);
        }
    }
}
