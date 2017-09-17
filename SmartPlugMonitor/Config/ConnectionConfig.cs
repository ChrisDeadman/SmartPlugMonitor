using System.Xml.Linq;
using Toolbox.NetExtensions;

namespace SmartPlugMonitor.Config
{
    public class TpLinkConfig
    {
        static class XmlStrings
        {
            public const string RootElementName = "Connection";
            public const string DownloadUrlElementName = "DownloadUrl";
            public const string UploadUrlElementName = "UploadUrl";
        }

        /// <summary>
        /// Gets or sets the download URL.
        /// </summary>
        /// <value>
        /// The download URL.
        /// </value>
        public string DownloadUrl { get; set; }

        /// <summary>
        /// Gets or sets the upload URL.
        /// </summary>
        /// <value>
        /// The upload URL.
        /// </value>
        public string UploadUrl { get; set; }

        /// <summary>
        /// Parses the configuration from XML.
        /// </summary>
        /// <param name="xmlRoot">The XML root.</param>
        /// <returns></returns>
        public static TpLinkConfig FromXml (XElement xmlRoot)
        {
            var config = new TpLinkConfig {
                DownloadUrl =
                    xmlRoot.OptionalElement (XmlStrings.RootElementName).OptionalElement (XmlStrings.DownloadUrlElementName).OptionalValue<string> (),
                UploadUrl =
                    xmlRoot.OptionalElement (XmlStrings.RootElementName).OptionalElement (XmlStrings.UploadUrlElementName).OptionalValue<string> (),
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
                new XElement (XmlStrings.DownloadUrlElementName, DownloadUrl),
                new XElement (XmlStrings.UploadUrlElementName, UploadUrl));
        }
    }
}
