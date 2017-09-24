using System;
using System.Xml.Linq;

using SmartPlugMonitor.Toolbox;

namespace SmartPlugMonitor.Config
{
    public class TrayIconConfig
    {
        public static class XmlStrings
        {
            public const string RootElementName = "TrayIcon";
            public const string FontFamilyElementName = "FontFamily";
            public const string FontSizeElementName = "FontSize";
            public const string IconSizeElementName = "IconSize";
        }

        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>
        /// The font family for tray icon text.
        /// </value>
        public string FontFamily { get; set; }

        /// <summary>
        /// Gets or sets the font size.
        /// </summary>
        /// <value>
        /// The font size for tray icon text.
        /// </value>
        public int FontSize { get; set; }

        /// <summary>
        /// Gets or sets the icon size.
        /// </summary>
        /// <value>
        /// The size of the tray icon.
        /// </value>
        public int IconSize { get; set; }

        /// <summary>
        /// Parses the configuration from XML.
        /// </summary>
        /// <param name="xmlRoot">The XML root.</param>
        /// <returns></returns>
        public static TrayIconConfig FromXml (XElement xmlRoot)
        {
            var config = new TrayIconConfig {
                FontFamily = xmlRoot.OptionalElement (XmlStrings.RootElementName).OptionalElement (XmlStrings.FontFamilyElementName).OptionalValue<string> ("Tahoma"),
                FontSize = xmlRoot.OptionalElement (XmlStrings.RootElementName).OptionalElement (XmlStrings.FontSizeElementName).OptionalValue<int> (22),
                IconSize = xmlRoot.OptionalElement (XmlStrings.RootElementName).OptionalElement (XmlStrings.IconSizeElementName).OptionalValue<int> (32)
            };

            return config;
        }

        /// <summary>
        /// Returns the current settings as XML.
        /// </summary>
        /// <returns></returns>
        public XElement ToXml ()
        {
            return new XElement (
                XmlStrings.RootElementName,
                new XElement (XmlStrings.FontFamilyElementName, FontFamily),
                new XElement (XmlStrings.FontSizeElementName, FontSize),
                new XElement (XmlStrings.IconSizeElementName, IconSize));
        }
    }
}
