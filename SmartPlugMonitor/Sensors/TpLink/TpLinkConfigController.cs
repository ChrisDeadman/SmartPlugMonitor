using System;
using System.Collections.Generic;

using SmartPlugMonitor.Config;
using SmartPlugMonitor.Ui;

namespace SmartPlugMonitor.Sensors.TpLink
{
    public class TpLinkConfigController : IConfigController
    {
        public string Title { get; } = "tp-link";

        public IEnumerable<UiItem> UiItems { get; } = new [] {
            new UiItem (TpLinkConfig.XmlStrings.IpAddressElementName, typeof(string), Globals.ConfigFile.TpLinkConfig.IpAddress),
            new UiItem (TpLinkConfig.XmlStrings.PortNumberElementName, typeof(int), Globals.ConfigFile.TpLinkConfig.PortNumber),
            new UiItem (TpLinkConfig.XmlStrings.MonitorWattageElementName, typeof(Boolean), Globals.ConfigFile.TpLinkConfig.MonitorWattage),
            new UiItem (TpLinkConfig.XmlStrings.MonitorVoltageElementName, typeof(Boolean), Globals.ConfigFile.TpLinkConfig.MonitorVoltage),
            new UiItem (TpLinkConfig.XmlStrings.MonitorCurrentElementName, typeof(Boolean), Globals.ConfigFile.TpLinkConfig.MonitorCurrent)
        };

        public void Save ()
        {
            foreach (var item in UiItems) {
                switch (item.Name) {
                case TpLinkConfig.XmlStrings.IpAddressElementName:
                    Globals.ConfigFile.TpLinkConfig.IpAddress = item.GetValue<string> ();
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
