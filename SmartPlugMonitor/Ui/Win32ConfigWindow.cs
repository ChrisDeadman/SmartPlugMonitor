using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using SmartPlugMonitor.Toolbox;

namespace SmartPlugMonitor.Ui
{
    public class Win32ConfigWindow : Win32Window
    {
        private readonly IList<Win32ConfigPage> configPages;

        public Win32ConfigWindow(IEnumerable<Win32ConfigPage> configPages)
        {
            this.configPages = configPages.ToList();
            Initialize();
        }

        private void Initialize()
        {
            base.SuspendLayout();
            base.Text = "Configure";
            base.Icon = Globals.ApplicationIcon;
            base.MinimumSize = new Size(200, 200);
            base.MaximumSize = new Size(250, 220);
            base.AutoSize = true;
            base.AutoSizeMode = AutoSizeMode.GrowOnly;
            base.MinimizeBox = false;
            base.MaximizeBox = false;

            var tabControl = new TabControl {
                Dock = DockStyle.Fill
            };

            base.Controls.Add(tabControl);

            foreach (var page in configPages) {
                tabControl.TabPages.Add(page);
            }

            base.ResumeLayout(false);
            base.PerformLayout();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                foreach (var page in configPages) {
                    page.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}
