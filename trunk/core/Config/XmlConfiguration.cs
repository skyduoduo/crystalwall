using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading;
using System.Reflection;
using System.IO;
using CrystalWall.Utils;

namespace  CrystalWall.Config
{
    /// <summary>
    /// 使用xml文件实现的配置接口
    /// </summary>
    public class XmlConfiguration: IConfiguration
    {
        private XmlReader reader;

        private XmlReader childReader;//子树读取器

        private IPermissionResource owner;

        private object parent; 

        private int index;//元素在父元素之下的索引号

        private string name;//元素名称

        private string content;

        private IDictionary<string, string> attributes = new Dictionary<string, string>();

        private IConfiguration[] child;

        /// <summary>
        /// 此配置元素为资源的直接子元素，xmlreader指向当前对应的元素
        /// </summary>
        /// <param name="parent"></param>
        public XmlConfiguration(XmlReader reader, IPermissionResource parent, int index)
        {
            if (parent == null)
            {
                throw new ConfigurationException(null, "xml configuration element's parent must not be null");
            }
            this.reader = reader;
            this.owner = parent;
            this.parent = parent;
            this.index = index;
            ReadSection(reader);
        }

        /// <summary>
        /// 使用父元素构造, xmlreader指向当前对应的元素
        /// </summary>
        public XmlConfiguration(XmlReader reader, XmlConfiguration parent, int index)
        {
            if (parent == null)
            {
                throw new ConfigurationException(null, "xml configuration element's parent must not be null");
            }
            this.reader = reader;
            this.parent = parent;
            this.index = index;
            this.owner = FindResource();
            ReadSection(reader);
        }

        
        /// <summary>
        /// 读取此配置元素的名称、属性
        /// </summary>
        private void ReadSection(XmlReader reader)
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.IsStartElement())
                        {
                            this.name = reader.LocalName;//读取此配置元素名称
                            ReadAttribute(reader);
                            string childText = reader.ReadInnerXml();
                             if (!string.Empty.Equals(childText) && (childText.Contains("</") || childText.Contains("/>")))
                            {
                                //具有子节点
                                childReader = XmlReader.Create(new StringReader(childText));
                            }
                             else if(!string.Empty.Equals(childText))
                             {
                                  //读取内容
                                 this.content = childText;
                             }
                            return;
                        }
                        //reader.Skip();
                        break;
                    //case XmlNodeType.EndElement:
                    //    //读取到资源的关闭节点也没读完
                    //    throw new ConfigurationException("there is no configuration element in reader");
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 读取属性
        /// </summary>
        /// <param name="reader"></param>
        private void ReadAttribute(XmlReader reader)
        {
            if (reader.HasAttributes)
            {
                for (int i = 0; i < reader.AttributeCount; i++)
                {
                    reader.MoveToAttribute(i);
                    attributes[reader.Name] = reader.Value;
                }
                reader.MoveToElement();
            }
        }

        IPermissionResource FindResource()
        {
            object p = parent;
            while (!(p is IPermissionResource))
            {
                p = ((IConfiguration)p).Parent;
            }
            return (IPermissionResource)p;
        }

        /// <summary>
        /// 使用当前程序集加载实例
        /// </summary>
        /// <param name="propertyName">指定属性的值必须具有类完全限定名</param>
        public object CreateObject(string propertyName)
        {
            string loadedAssembly = this.owner.LoadedAssembly;
            try
            {
                if (loadedAssembly != null)
                {
                    return AppDomain.CurrentDomain.CreateInstanceAndUnwrap(loadedAssembly, this.GetAttribute(propertyName));
                }
                return Assembly.GetExecutingAssembly().CreateInstance(GetAttribute(propertyName));
            }
            catch (Exception e)
            {
                ServiceManager.LoggingService.Error("加载程序集时出现错误！", e);
                throw new ConfigurationException(null, string.Format("创建属性{0}指定的对象时发生程序集加载错误", propertyName));
            }
        }

        public string GetAttribute(string name)
        {
            return attributes[name];
        }

        public string[] GetAttributeNames()
        {
            return attributes.Keys.ToArray();
        }

        public IConfiguration[] Children()
        {
            if (childReader == null)
                return new IConfiguration[0];
            List<IConfiguration> list = new List<IConfiguration>();
            for (int i = 0; childReader.Read() == true; i++)
            {
                XmlConfiguration conf = new XmlConfiguration(childReader, this, i);
                list.Add(conf);
            }
            return list.ToArray();
        }

        public IConfiguration[] GetChildren(string name)
        {
            return Children().ToList().FindAll(
                c => c.Name.Equals(name))
                .ToArray();
        }

        public string Name
        {
            get { return name; }
        }

        public IPermissionResource Owner
        {
            get { return owner; }
        }

        public object Parent
        {
            get { return parent; }
        }

        public string GetValue()
        {
            if (content != null)
                return content;
            return "";
        }
    }
}
