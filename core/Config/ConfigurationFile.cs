using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;

namespace CrystalWall.Config
{
    public class ConfigurationFile
    {
        private Configuration configuration;

        public ConfigurationFile(string path)
        {
            configuration = ConfigurationManager.OpenExeConfiguration(path);
        }

        public object GetSection(string sectionPath, IConfigurationSectionHandler handler)
        {
            string rowXml = configuration.GetSection(sectionPath).SectionInformation.GetRawXml();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(rowXml);
            XmlNode node = doc.DocumentElement;
            return handler.Create(null, null, node);
        }
    }
}
