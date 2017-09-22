using System;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

namespace SmartPlugMonitor.Ui
{
    public class ConfigForm : Form
    {
        private readonly IList<ConfigTab> configTabs = new List<ConfigTab> ();

        public ConfigForm (IEnumerable<ConfigTab> configTabs)
        {
            this.configTabs = configTabs.ToList ();

            InitializeComponent ();
        }

        void InitializeComponent ()
        {
            this.SuspendLayout ();
            this.MaximizeBox = false;
            this.Size = new System.Drawing.Size (320, 250);
            this.Text = "Configure";
            this.Icon = Globals.ApplicationIcon;

            var tabControl = new TabControl {
                Dock = DockStyle.Fill
            };

            this.Controls.Add (tabControl);

            foreach (var tab in configTabs) {
                tabControl.TabPages.Add (tab);
            }

            this.ResumeLayout (false);
            this.PerformLayout ();
        }

        protected override void Dispose (bool disposing)
        {
            if (disposing) {
                foreach (var tab in configTabs) {
                    tab.Dispose ();
                }

                Globals.SaveConfigFile ();
            }

            base.Dispose (disposing);
        }
    }
}
