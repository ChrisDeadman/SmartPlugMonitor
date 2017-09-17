using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace SmartPlugMonitor.Ui
{
    public class ConfigurationUi : Form
    {
        private System.ComponentModel.IContainer components = null;

        public ConfigurationUi ()
        {
            InitializeComponent ();

            this.IpAddressTextBox.Text = Globals.ConfigFile.TpLinkConfig.IpAddress;
            this.WattageCheckBox.Checked = Globals.ConfigFile.TpLinkConfig.MonitorWattage;
            this.VoltageCheckBox.Checked = Globals.ConfigFile.TpLinkConfig.MonitorVoltage;
            this.CurrentCheckBox.Checked = Globals.ConfigFile.TpLinkConfig.MonitorCurrent;
        }

        protected override void OnClosing (CancelEventArgs e)
        {
            Globals.ConfigFile.TpLinkConfig.IpAddress = this.IpAddressTextBox.Text;
            Globals.ConfigFile.TpLinkConfig.MonitorWattage = this.WattageCheckBox.Checked;
            Globals.ConfigFile.TpLinkConfig.MonitorVoltage = this.VoltageCheckBox.Checked;
            Globals.ConfigFile.TpLinkConfig.MonitorCurrent = this.CurrentCheckBox.Checked;
            Globals.SaveConfigFile ();

            base.OnClosing (e);
        }

        protected override void Dispose (bool disposing)
        {
            if (disposing && (components != null)) {
                components.Dispose ();
            }
            base.Dispose (disposing);
        }

        private System.Windows.Forms.Label IpAddressTextBoxLabel;
        private System.Windows.Forms.TextBox IpAddressTextBox;

        private System.Windows.Forms.Label WattageCheckBoxLabel;
        private System.Windows.Forms.CheckBox WattageCheckBox;

        private System.Windows.Forms.Label VoltageCheckBoxLabel;
        private System.Windows.Forms.CheckBox VoltageCheckBox;

        private System.Windows.Forms.Label CurrentCheckBoxLabel;
        private System.Windows.Forms.CheckBox CurrentCheckBox;

        void InitializeComponent ()
        {
            this.components = new System.ComponentModel.Container ();
            this.SuspendLayout ();
            this.MaximizeBox = false;
            this.Size = new System.Drawing.Size (280, 180);
            this.Text = "Configure";
            this.Icon = Globals.ApplicationIcon;

            int x_offset = 20;
            int y_offset = 20;

            int x = x_offset;
            int y = y_offset;

            this.IpAddressTextBoxLabel = new System.Windows.Forms.Label ();
            this.IpAddressTextBoxLabel.AutoSize = true;
            this.IpAddressTextBoxLabel.Location = new System.Drawing.Point (x, y);
            this.IpAddressTextBoxLabel.Text = "IP Address:";
            this.Controls.Add (this.IpAddressTextBoxLabel);

            x += 100;
            this.IpAddressTextBox = new System.Windows.Forms.TextBox ();
            this.IpAddressTextBox.Location = new System.Drawing.Point (x, y);
            this.IpAddressTextBox.Size = new System.Drawing.Size (130, 20);
            this.IpAddressTextBox.TabIndex = 0;
            this.Controls.Add (this.IpAddressTextBox);

            x = x_offset;
            y += 30;
            this.WattageCheckBoxLabel = new System.Windows.Forms.Label ();
            this.WattageCheckBoxLabel.AutoSize = true;
            this.WattageCheckBoxLabel.Location = new System.Drawing.Point (x, y);
            this.WattageCheckBoxLabel.Text = "Wattage:";
            this.Controls.Add (this.WattageCheckBoxLabel);

            x += 100;
            this.WattageCheckBox = new System.Windows.Forms.CheckBox ();
            this.WattageCheckBox.Location = new System.Drawing.Point (x, y);
            this.WattageCheckBox.TabIndex = 1;
            this.Controls.Add (this.WattageCheckBox);

            x = x_offset;
            y += 30;
            this.VoltageCheckBoxLabel = new System.Windows.Forms.Label ();
            this.VoltageCheckBoxLabel.AutoSize = true;
            this.VoltageCheckBoxLabel.Location = new System.Drawing.Point (x, y);
            this.VoltageCheckBoxLabel.Text = "Voltage:";
            this.Controls.Add (this.VoltageCheckBoxLabel);

            x += 100;
            this.VoltageCheckBox = new System.Windows.Forms.CheckBox ();
            this.VoltageCheckBox.Location = new System.Drawing.Point (x, y);
            this.VoltageCheckBox.TabIndex = 1;
            this.Controls.Add (this.VoltageCheckBox);

            x = x_offset;
            y += 30;
            this.CurrentCheckBoxLabel = new System.Windows.Forms.Label ();
            this.CurrentCheckBoxLabel.AutoSize = true;
            this.CurrentCheckBoxLabel.Location = new System.Drawing.Point (x, y);
            this.CurrentCheckBoxLabel.Text = "Current:";
            this.Controls.Add (this.CurrentCheckBoxLabel);

            x += 100;
            this.CurrentCheckBox = new System.Windows.Forms.CheckBox ();
            this.CurrentCheckBox.Location = new System.Drawing.Point (x, y);
            this.CurrentCheckBox.TabIndex = 1;
            this.Controls.Add (this.CurrentCheckBox);

            this.ResumeLayout (false);
            this.PerformLayout ();
        }
    }
}
