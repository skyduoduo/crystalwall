using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Threading;
using CrystalWall.Logging;
using CrystalWall.FileUtils;
using CrystalWall.Message;

///<remarks>this file is from Sharpdevelop</remarks>
namespace CrystalWall.Property
{
    /// <summary>
    /// 属性服务是从配置目录中指定属性文件加载或将内部Properties所做
    /// 的修改存储到属性文件中的服务类。第一次实例化此服务的属性、配置文件目录时，使用InitializeService方法
    /// </summary>
    public static class PropertyService
    {
        static string propertyFileName;
        static string propertyXmlRootNodeName;

        static string configDirectory;
        static string dataDirectory;

        static Properties properties;

        public static bool Initialized
        {
            get
            {
                return properties != null;
            }
        }

        /// <summary>
        /// 初始化服务：配置目录、数据目录、属性名字（属性文件根节点名）、属性文件（属性名.xml/)s
        /// </summary>
        /// <param name="configDirectory">配置目录</param>
        /// <param name="dataDirectory">数据目录</param>
        /// <param name="propertiesName">属性文件名</param>
        public static void InitializeService(string configDirectory, string dataDirectory, string propertiesName)
        {
            if (properties != null)
                throw new InvalidOperationException("Service is already initialized.");
            if (configDirectory == null || dataDirectory == null || propertiesName == null)
                throw new ArgumentNullException();
            properties = new Properties();
            PropertyService.configDirectory = configDirectory;
            PropertyService.dataDirectory = dataDirectory;
            propertyXmlRootNodeName = propertiesName;
            propertyFileName = propertiesName + ".xml";
            properties.PropertyChanged += new PropertyChangedEventHandler(PropertiesPropertyChanged);
        }

        public static string ConfigDirectory
        {
            get
            {
                return configDirectory;
            }
        }

        public static string DataDirectory
        {
            get
            {
                return dataDirectory;
            }
        }

        public static string Get(string property)
        {
            return properties[property];
        }

        public static T Get<T>(string property, T defaultValue)
        {
            return properties.Get(property, defaultValue);
        }

        public static void Set<T>(string property, T value)
        {
            properties.Set(property, value);
        }

        /// <summary>
        /// 1、configDirectory目录不存在，则创建
        /// 2、LoadPropertiesFromStream从流中加载到属性中
        /// </summary>
        public static void Load()
        {
            if (properties == null)
                throw new InvalidOperationException("Service is not initialized.");
            if (!Directory.Exists(configDirectory))
            {
                Directory.CreateDirectory(configDirectory);
            }

            if (!LoadPropertiesFromStream(Path.Combine(configDirectory, propertyFileName)))
            {
                LoadPropertiesFromStream(FileUtil.Combine(DataDirectory, "options", propertyFileName));
            }
        }

        public static bool LoadPropertiesFromStream(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return false;
            }
            try
            {
                using (LockPropertyFile())
                {
                    using (XmlTextReader reader = new XmlTextReader(fileName))
                    {
                        while (reader.Read())
                        {
                            if (reader.IsStartElement())
                            {
                                if (reader.LocalName == propertyXmlRootNodeName)
                                {
                                    properties.ReadProperties(reader, propertyXmlRootNodeName);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            catch (XmlException ex)
            {
                MessageService.ShowError("Error loading properties: " + ex.Message + "\nSettings have been restored to default values.");
            }
            return false;
        }

        /// <summary>
        /// 将properties内容写入MemoryStream内存流后将其写入配置文件中
        /// </summary>
        public static void Save()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8);
                writer.Formatting = Formatting.Indented;
                writer.WriteStartElement(propertyXmlRootNodeName);
                properties.WriteProperties(writer);
                writer.WriteEndElement();
                writer.Flush();

                ms.Position = 0;
                string fileName = Path.Combine(configDirectory, propertyFileName);
                using (LockPropertyFile())
                {
                    using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        ms.WriteTo(fs);
                    }
                }
            }
        }

        /// <summary>
        /// 使用互斥量Mutex锁定
        /// </summary>
        public static IDisposable LockPropertyFile()
        {
            Mutex mutex = new Mutex(false, "PropertyServiceSave-30F32619-F92D-4BC0-BF49-AA18BF4AC313");
            mutex.WaitOne();
            return new CallbackOnDispose(
                delegate
                {
                    mutex.ReleaseMutex();
                    mutex.Close();
                });
        }

        static void PropertiesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(null, e);
            }
        }

        public static event PropertyChangedEventHandler PropertyChanged;
    }
}
