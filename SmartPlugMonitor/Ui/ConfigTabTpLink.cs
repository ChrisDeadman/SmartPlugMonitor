using System;
using SmartPlugMonitor.Config;

namespace SmartPlugMonitor.Ui
{
    public class ConfigTabTpLink : ConfigTab
    {
        public ConfigTabTpLink () : base (CreateUiItems ())
        {
            this.Text = "tp-link";
        }

        private static UiItem[] CreateUiItems ()
        {
            return new [] {
                new UiItem (TpLinkConfig.XmlStrings.IpAddressElementName, typeof(String), Globals.ConfigFile.TpLinkConfig.IpAddress),
                new UiItem (TpLinkConfig.XmlStrings.PortNumberElementName, typeof(int), Globals.ConfigFile.TpLinkConfig.PortNumber),
                new UiItem (TpLinkConfig.XmlStrings.MonitorWattageElementName, typeof(Boolean), Globals.ConfigFile.TpLinkConfig.MonitorWattage),
                new UiItem (TpLinkConfig.XmlStrings.MonitorVoltageElementName, typeof(Boolean), Globals.ConfigFile.TpLinkConfig.MonitorVoltage),
                new UiItem (TpLinkConfig.XmlStrings.MonitorCurrentElementName, typeof(Boolean), Globals.ConfigFile.TpLinkConfig.MonitorCurrent)
            };
        }

        public override void Save ()
        {
            foreach (var item in base.ConfigItems) {
                switch (item.Name) {
                case TpLinkConfig.XmlStrings.IpAddressElementName:
                    Globals.ConfigFile.TpLinkConfig.IpAddress = item.GetValue<String> ();
                    break;
                case TpLinkConfig.XmlStrings.PortNumberElementName:
                    Globals.ConfigFile.TpLinkConfig.PortNumber = item.GetValue<int> ();
                    break;
                case TpLinkConfig.XmlStrings.MonitorWattageElementName:
                    Globals.ConfigFile.TpLinkConfig.MonitorWattage = item.GetValue<Boolean> ();
                    break;
                case TpLinkConfig.XmlStrings.MonitorVoltageElementName:
                    Globals.ConfigFile.TpLinkConfig.MonitorVoltage = item.GetValue<Boolean> ();
                    break;
                case TpLinkConfig.XmlStrings.MonitorCurrentElementName:
                    Globals.ConfigFile.TpLinkConfig.MonitorCurrent = item.GetValue<Boolean> ();
                    break;
                }
            }
        }
    }
}
