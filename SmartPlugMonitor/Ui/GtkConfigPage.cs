using System;
using System.Linq;
using System.Collections.Generic;

using Gtk;

namespace SmartPlugMonitor.Ui
{
    public class GtkConfigPage : Frame
    {
        protected readonly IConfigController TabController;

        public GtkConfigPage (IConfigController tabController)
        {
            this.TabController = tabController;

            Initialize ();
        }

        public string Title { get { return TabController.Title; } }

        private void Initialize ()
        {
            var tableLayout = new Table (0, 0, false) {
                RowSpacing = 5,
                ColumnSpacing = 20
            };

            var row = 0U;
            foreach (var uiItem in TabController.UiItems) {
                var column = 0U;
                foreach (var widget in CreateItemWidgets (uiItem)) {
                    tableLayout.Attach (widget, column, column + 1, row, row + 1, AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
                    column++;
                }
                row++;
            }

            this.Add (tableLayout);
            this.ShowAll ();
        }

        protected override void OnDestroyed ()
        {
            TabController.Save ();
            base.OnDestroyed ();
        }

        private static IEnumerable<Widget> CreateItemWidgets (UiItem configItem)
        {
            yield return CreateLabel (configItem.Name);

            if (configItem.Type == typeof(string)) {
                yield return CreateStringTextBox (configItem);
            } else if (configItem.Type == typeof(int)) {
                yield return CreateIntegerTextBox (configItem);
            } else if (configItem.Type == typeof(Boolean)) {
                yield return CreateCheckButton (configItem);
            } else {
                throw new ArgumentException ($"Unsupported config item type: {configItem.Type}");
            }
        }

        private static Label CreateLabel (string text)
        {
            var label = new Label (text);
            label.Justify = Justification.Left;
            label.SetAlignment (0, 0);
            return label;
        }

        private static TextView CreateStringTextBox (UiItem configItem)
        {
            var textView = new TextView ();
            textView.Buffer.Text = configItem.GetValue<string> ();
            textView.Buffer.Changed += (s, a) => configItem.Value = textView.Buffer.Text;
            return textView;
        }

        private static TextView CreateIntegerTextBox (UiItem configItem)
        {
            var textView = new TextView ();
            textView.Buffer.Text = configItem.GetValue<int> ().ToString ();
            textView.Buffer.Changed += (s, a) => {
                int value;
                if (int.TryParse (textView.Buffer.Text, out value)) {
                    configItem.Value = value;
                }
            };
            return textView;
        }

        private static CheckButton CreateCheckButton (UiItem configItem)
        {
            var checkButton = new CheckButton ();
            checkButton.Active = configItem.GetValue<Boolean> ();
            checkButton.StateChanged += (s, a) => configItem.Value = checkButton.Active;
            return checkButton;
        }
    }
}
