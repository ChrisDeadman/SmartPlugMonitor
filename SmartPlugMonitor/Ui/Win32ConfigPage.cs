using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SmartPlugMonitor.Ui
{
    public class Win32ConfigPage : TabPage
    {
        protected readonly IConfigController TabController;

        public Win32ConfigPage (IConfigController tabController)
        {
            this.TabController = tabController;

            Initialize ();
        }

        private void Initialize ()
        {
            base.SuspendLayout ();
            base.Text = TabController.Title;
            base.AutoScroll = true;

            var tableLayout = new TableLayoutPanel {
                Dock = DockStyle.Fill
            };

            var row = 0;
            foreach (var uiItem in TabController.UiItems) {
                var column = 0;
                foreach (var control in CreateItemControls (uiItem)) {
                    tableLayout.Controls.Add (control, column, row);
                    column++;
                }
                row++;
            }

            base.Controls.Add (tableLayout);
            base.ResumeLayout (false);
            base.PerformLayout ();
        }

        protected override void Dispose (bool disposing)
        { 
            if (disposing) {
                TabController.Save ();
            }

            base.Dispose (disposing);
        }

        private static IEnumerable<Control> CreateItemControls (UiItem configItem)
        { 
            yield return CreateLabel (configItem.Name);

            if (configItem.Type == typeof(string)) {
                yield return CreateStringTextBox (configItem);
            } else if (configItem.Type == typeof(int)) {
                yield return CreateIntegerTextBox (configItem);
            } else if (configItem.Type == typeof(Boolean)) {
                yield return CreateCheckBox (configItem);
            } else {
                throw new ArgumentException ($"Unsupported config item type: {configItem.Type}");
            }
        }

        private static Label CreateLabel (string text)
        {
            return new Label { Text = text };
        }

        private static TextBox CreateStringTextBox (UiItem configItem)
        {
            var textBox = new TextBox {
                Text = configItem.GetValue<string> ()
            };
            textBox.TextChanged += (s, e) => configItem.Value = textBox.Text;
            return textBox;
        }

        private static TextBox CreateIntegerTextBox (UiItem configItem)
        {
            var textBox = new TextBox {
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

        private static CheckBox CreateCheckBox (UiItem configItem)
        {
            var checkBox = new CheckBox {
                Checked = configItem.GetValue<Boolean> ()
            };
            checkBox.CheckedChanged += (s, e) => configItem.Value = checkBox.Checked;
            return checkBox;
        }
    }
}
