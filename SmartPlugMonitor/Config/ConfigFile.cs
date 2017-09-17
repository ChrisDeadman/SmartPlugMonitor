using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using SmartPlugMonitor.Toolbox;

namespace SmartPlugMonitor.Config
{
    /// <summary>
    /// Manages the persistent configuration.
    /// </summary>
    public class ConfigFile
    {
        static class XmlStrings
        {
            public const string RootElementName = "Settings";
        }

        /// <summary>
        /// Gets or sets the tp-link config.
        /// </summary>
        /// <value>
        /// The tp-link config.
        /// </value>
        public TpLinkConfig TpLinkConfig { get; private set; }

        /// <summary>
        /// Loads the XML configuration.
        /// </summary>
        /// <param name="configFilePath">The config file path.</param>
        /// <returns></returns>
        public static ConfigFile Load (string configFilePath)
        {
            var xmlRoot = LoadXmlSettingsElement (configFilePath);
            return new ConfigFile {
                TpLinkConfig = TpLinkConfig.FromXml (xmlRoot)
            };
        }

        /// <summary>
        /// Saves the current configuration to the configuration file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void Save (string filePath)
        {
            var xmlRoot = LoadXmlSettingsElement (filePath);

            xmlRoot.AddOrReplaceElement (TpLinkConfig.ToXml ());

            using (var documentWriter = new StreamWriter (filePath, false, new UTF8Encoding (false)))
                new XDocument (FindTopRootElement (xmlRoot)).Save (documentWriter, SaveOptions.None);
        }

        /// <summary>
        /// Loads the XML settings element.
        /// </summary>
        /// <param name="configFilePath">The config file path.</param>
        /// <returns></returns>
        static XElement LoadXmlSettingsElement (string configFilePath)
        {
            XDocument document;
            using (var documentReader = new XmlTextReader (configFilePath) { WhitespaceHandling = WhitespaceHandling.Significant }) {
                document = XDocument.Load (documentReader, LoadOptions.SetLineInfo);
                document = new XDocument (document.Root);
            }
                
            return document.Root.MandatoryElement (XmlStrings.RootElementName);
        }

        /// <summary>
        /// Finds the top root element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        static XElement FindTopRootElement (XElement element)
        {
            var rootElement = element;
            while (rootElement.Parent != null)
                rootElement = rootElement.Parent;

            return rootElement;
        }
    }
}
