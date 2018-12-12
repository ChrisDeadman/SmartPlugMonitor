using System;
using System.Xml.Linq;

using SmartPlugMonitor.Toolbox;

namespace SmartPlugMonitor.Config
{
    public class HS110Config
    {
        public static class XmlStrings
        {
            public const string RootElementName = "HS110";
            public const string IpAddressElementName = "IpAddress";
            public const string PortElementName = "Port";
            public const string MonitorWattageElementName = "Wattage";
            public const string MonitorVoltageElementName = "Voltage";
            public const string MonitorCurrentElementName = "Current";
        }

        /// <summary>
        /// Gets or sets the IP address.
        /// </summary>
        /// <value>
        /// The IP address of the smart plug.
        /// </value>
        public string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the port number.
        /// </summary>
        /// <value>
        /// The port number of the smart plug.
        /// </value>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets whether wattage should be monitored.
        /// </summary>
        /// <value>
        /// Whether wattage should be monitored.
        /// </value>
        public bool MonitorWattage { get; set; }

        /// <summary>
        /// Gets or sets whether voltage should be monitored.
        /// </summary>
        /// <value>
        /// Whether voltage should be monitored.
        /// </value>
        public bool MonitorVoltage { get; set; }

        /// <summary>
        /// Gets or sets whether current should be monitored.
        /// </summary>
        /// <value>
        /// Whether current should be monitored.
        /// </value>
        public bool MonitorCurrent { get; set; }

        /// <summary>
        /// Parses the configuration from XML.
        /// </summary>
        /// <param name="xmlRoot">The XML root.</param>
        /// <returns></returns>
        public static HS110Config FromXml(XElement xmlRoot)
        {
            var config = new HS110Config {
                IpAddress = xmlRoot.OptionalElement(XmlStrings.RootElementName).OptionalElement(XmlStrings.IpAddressElementName).OptionalValue(""),
                Port = xmlRoot.OptionalElement(XmlStrings.RootElementName).OptionalElement(XmlStrings.PortElementName).OptionalValue(9999),
                MonitorWattage = xmlRoot.OptionalElement(XmlStrings.RootElementName).OptionalElement(XmlStrings.MonitorWattageElementName).OptionalValue(true),
                MonitorVoltage = xmlRoot.OptionalElement(XmlStrings.RootElementName).OptionalElement(XmlStrings.MonitorVoltageElementName).OptionalValue(false),
                MonitorCurrent = xmlRoot.OptionalElement(XmlStrings.RootElementName).OptionalElement(XmlStrings.MonitorCurrentElementName).OptionalValue(false)
            };

            return config;
        }

        /// <summary>
        /// Returns the current settings as XML.
        /// </summary>
        /// <returns></returns>
        public XElement ToXml()
        {
            return new XElement(
                XmlStrings.RootElementName,
                new XElement(XmlStrings.IpAddressElementName, IpAddress),
                new XElement(XmlStrings.PortElementName, Port),
                new XElement(XmlStrings.MonitorWattageElementName, MonitorWattage),
                new XElement(XmlStrings.MonitorVoltageElementName, MonitorVoltage),
                new XElement(XmlStrings.MonitorCurrentElementName, MonitorCurrent));
        }
    }
}
