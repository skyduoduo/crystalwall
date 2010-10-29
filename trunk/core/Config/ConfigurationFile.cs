using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;
using CrystalWall.Utils;

namespace CrystalWall.Config
{
    /// <summary>
    /// 封装一个配置文件的类，他的GetSection方法将使用IConfigurationSectionHandler获取相应的对象。
    /// 注意：此类只是为了解决微软弃用IConfigurationSectionHandler的问题，但crystalwall认为IConfigurationSectionHandler
    /// 接口的形式更加灵活，使用SectionElement元注释的方式虽然简单，但必须强制继承关系。因此，此类只用于使用
    /// IConfigurationSectionHandler获取节点对应的对象的情况，如果你使用继承元注释的ConfigurationSection，请不要使用此类！！
    /// </summary>
    public class ConfigurationFile
    {
        private Configuration configuration;

        private string configFile;

        private IDictionary<string, ConfigSection> configSections = new Dictionary<string, ConfigSection>();//section定义的缓存，key为路径

        public ConfigurationFile(string path)
        {
            configuration = ConfigurationManager.OpenExeConfiguration(path);
            configFile = configuration.FilePath;
        }

        public object GetSection(string sectionPath)
        {
            sectionPath = Normalize(sectionPath);
            ConfigSection configSection;
            if (configSections.ContainsKey(sectionPath))//缓存中存在，直接从缓存中获取
            {
                configSection = configSections[sectionPath];
            }
            else
            {
                configSection = GetConfigSection(sectionPath);
                if (configSection != null)
                    configSections.Add(sectionPath, configSection);
            }
            if (configSection == null)
            {
                ServiceManager.LoggingService.Debug("配置中找不到" + sectionPath + "对应的Handler处理器");
                return null;
            }
            try
            {
                IConfigurationSectionHandler handler = (IConfigurationSectionHandler)Type.GetType(configSection.Type).GetConstructor(new Type[0]).Invoke(new object[0]);
                string rowXml = configuration.GetSection(sectionPath).SectionInformation.GetRawXml();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(rowXml);
                return handler.Create(null, null, doc.DocumentElement);
            }
            catch (Exception e)
            {
                ServiceManager.LoggingService.Fatal("无法创建配置处理器，请检查配置处理器是否正确", e);
                throw e;
            }
        }

        private string Normalize(string path)
        {
            return path.TrimStart('/').TrimEnd('/');
        }

        private ConfigSection GetConfigSection(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(configFile);
            XmlNode configSectionNode = doc.DocumentElement.ChildNodes[0];//获取configSections节点
            string[] paths = path.Split('/');
            XmlNode finded = configSectionNode;
            for (int i = 0; i < paths.Length; i++)
            {
                finded = FindNode(finded, paths[i]);
                if (finded == null)
                    return null;
            }
            ConfigSection section =  new ConfigSection(finded.Attributes["name"].Value, finded.Attributes["type"].Value);
            section.Path = path;
            return section;
        }

        private XmlNode FindNode(XmlNode parent, string name)
        {
            foreach (XmlNode node in parent.ChildNodes)
            {
                if ((node.NodeType == XmlNodeType.Element 
                            && node.Attributes["name"] != null 
                            && node.Attributes["name"].Value.Equals(name)))
                {
                    return node;
                }
            }
            return null;
        }
    }

    /// <summary>
    /// 描述配置文件中一个configSections配置头中一个配置section的类，包含一个name和一个type
    /// </summary>
    internal class ConfigSection
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string path;

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        private string type;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public ConfigSection(string name, string type)
        {
            this.name = name;
            this.type = type;
        }
    }
}
