using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SmartPlugMonitor.Ui
{
    public abstract class ConfigTab : TabPage, IDisposable
    {
        protected readonly IList<UiItem> ConfigItems = new List<UiItem> ();

        public ConfigTab (IEnumerable<UiItem> configItems)
        {
            this.ConfigItems = configItems.ToList ();

            InitializeComponent ();
        }

        public abstract void Save ();

        protected override void Dispose (bool disposing)
        {
            Save ();

            base.Dispose (disposing);
        }

        private void InitializeComponent ()
        {
            this.SuspendLayout ();

            this.AutoScroll = true;

            const int x_offset = 20;
            const int y_offset = 20;

            var x = x_offset;
            var y = y_offset;

            foreach (var item in ConfigItems) {
                this.Controls.AddRange (CreateItemControls (item, x, ref y));
            }

            this.ResumeLayout (false);
            this.PerformLayout ();
        }

        private static Control[] CreateItemControls (UiItem configItem, int x_offset, ref int y)
        {
            Control[] controls;

            if (configItem.Type == typeof(String)) {
                controls = new Control[] {
                    CreateLabel (configItem.Name, x_offset, y),
                    CreateStringTextBox (configItem, x_offset + 130, y)
                };
            } else if (configItem.Type == typeof(int)) {
                controls = new Control[] {
                    CreateLabel (configItem.Name, x_offset, y),
                    CreateIntegerTextBox (configItem, x_offset + 130, y)
                };
            } else if (configItem.Type == typeof(Boolean)) {
                controls = new Control[] {
                    CreateLabel (configItem.Name, x_offset, y),
                    CreateCheckBox (configItem, x_offset + 130, y)
                };
            } else {
                throw new ArgumentException ($"Unsupported config item type: {configItem.Type}");
            }

            y += 30;
            return controls;
        }

        private static Label CreateLabel (string text, int x, int y)
        {
            return new System.Windows.Forms.Label {
                AutoSize = true,
                Location = new System.Drawing.Point (x, y),
                Text = text
            };
        }

        private static TextBox CreateStringTextBox (UiItem configItem, int x, int y)
        {
            var textBox = new System.Windows.Forms.TextBox {
                Size = new System.Drawing.Size (130, 20),
                Location = new System.Drawing.Point (x, y),
                Text = configItem.GetValue<String> ()
            };
            textBox.TextChanged += (s, e) => configItem.Value = textBox.Text;
            return textBox;
        }

        private static TextBox CreateIntegerTextBox (UiItem configItem, int x, int y)
        {
            var textBox = new System.Windows.Forms.TextBox {
                Size = new System.Drawing.Size (50, 20),
                Location = new System.Drawing.Point (x, y),
                Text = configItem.GetValue<int> ().ToString ()
            };
            textBox.TextChanged += (s, e) => {
                int value;
                if (int.TryParse (textBox.Text, out value)) {
                    configItem.Value = value;
                }
            };
            return textBox;
        }

        private static CheckBox CreateCheckBox (UiItem configItem, int x, int y)
        {
            var checkBox = new System.Windows.Forms.CheckBox {
                Location = new System.Drawing.Point (x, y),
                Checked = configItem.GetValue<Boolean> ()
            };
            checkBox.CheckedChanged += (s, e) => configItem.Value = checkBox.Checked;
            return checkBox;
        }
    }
}
