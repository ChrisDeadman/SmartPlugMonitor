using System;
using System.Collections.Generic;

using SmartPlugMonitor.Config;
using SmartPlugMonitor.Ui;

namespace SmartPlugMonitor.Sensors.HS110
{
    public class HS110ConfigController : IConfigController
    {
        public string Title { get; } = "TP-LINK HS110";

        public IEnumerable<UiItem> UiItems { get; } = new [] {
            new UiItem (HS110Config.XmlStrings.IpAddressElementName, typeof(string), Globals.ConfigFile.HS110Config.IpAddress),
            new UiItem (HS110Config.XmlStrings.PortElementName, typeof(int), Globals.ConfigFile.HS110Config.Port),
            new UiItem (HS110Config.XmlStrings.MonitorWattageElementName, typeof(Boolean), Globals.ConfigFile.HS110Config.MonitorWattage),
            new UiItem (HS110Config.XmlStrings.MonitorVoltageElementName, typeof(Boolean), Globals.ConfigFile.HS110Config.MonitorVoltage),
            new UiItem (HS110Config.XmlStrings.MonitorCurrentElementName, typeof(Boolean), Globals.ConfigFile.HS110Config.MonitorCurrent)
        };

        public void Save ()
        {
            foreach (var item in UiItems) {
                switch (item.Name) {
                case HS110Config.XmlStrings.IpAddressElementName:
                    Globals.ConfigFile.HS110Config.IpAddress = item.GetValue<string> ();
                    break;
                case HS110Config.XmlStrings.PortElementName:
                    Globals.ConfigFile.HS110Config.Port = item.GetValue<int> ();
                    break;
                case HS110Config.XmlStrings.MonitorWattageElementName:
                    Globals.ConfigFile.HS110Config.MonitorWattage = item.GetValue<Boolean> ();
                    break;
                case HS110Config.XmlStrings.MonitorVoltageElementName:
                    Globals.ConfigFile.HS110Config.MonitorVoltage = item.GetValue<Boolean> ();
                    break;
                case HS110Config.XmlStrings.MonitorCurrentElementName:
                    Globals.ConfigFile.HS110Config.MonitorCurrent = item.GetValue<Boolean> ();
                    break;
                }
            }
        }
    }
}
