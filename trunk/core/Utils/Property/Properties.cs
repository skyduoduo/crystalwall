using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Globalization;
using System.Xml;
using System.Collections;
using System.ComponentModel;
using CrystalWall.Message;

///<remarks>this file is from Sharpdevelop</remarks>
namespace CrystalWall.Property
{
    /// <summary>
    /// 可记录的接口用于标记对象是可备忘录的。它使用了备忘录模式。用于保存和恢复对象的状态。
    /// 状态信息存储在Properties中。他提供了以下职责：
    /// </summary>
    /// <description>
    ///  <item>
    ///   1、根据当前状态创建一个新的备忘录
    ///  </item>
    ///  <item>
    ///   2、设置当前状态到指定的备忘录中
    ///  </item>
    /// </description>
    public interface IMementoCapable
    {
        /// <summary>
        /// 根据当前状态创建一个新的备忘录
        /// </summary>
        Properties CreateMemento();

        /// <summary>
        /// 设置当前状态到指定的备忘录中
        /// </summary>
        void SetMemento(Properties memento);
    }

    /// <summary>
    /// 描述属性组，支持触发属性改变事件，也支持从xml文件中序列化和反序列化属性。
    /// 在获取和设置属性时，也支持根据属性的各种类型转换器转换为不可变字符
    /// </summary>
    public class Properties
    {
        /// 反序列化指定的xml字符内容的类，构造此类时，使用一个包含xml字符的
        /// 序列化字符串。此类使用Deserialize<T>方法将指定的content xml字符内容
        /// 反序列化为指定泛型的对象 
        class SerializedValue
        {
            string content;

            public string Content
            {
                get { return content; }
            }

            public T Deserialize<T>()
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                //将指定内容反序列化为T对象
                return (T)serializer.Deserialize(new StringReader(content));
            }

            public SerializedValue(string content)
            {
                this.content = content;
            }
        }

        Dictionary<string, object> properties = new Dictionary<string, object>();

        /// <summary>
        /// 获取指定属性时，使用不变的文化格式化转换
        /// </summary>
        public string this[string property]
        {
            get
            {
                return Convert.ToString(Get(property), CultureInfo.InvariantCulture);
            }
            set
            {
                Set(property, value);
            }
        }

        /// <summary>
        /// 获取字典属性中的所有key的值
        /// </summary>
        public string[] Elements
        {
            get
            {
                lock (properties)
                {
                    List<string> ret = new List<string>();
                    foreach (KeyValuePair<string, object> property in properties)
                        ret.Add(property.Key);
                    return ret.ToArray();
                }
            }
        }

        /// <summary>
        /// 从字典中获取指定属性的值
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public object Get(string property)
        {
            lock (properties)
            {
                object val;
                properties.TryGetValue(property, out val);
                return val;
            }
        }

        /// <summary>
        /// 改变新属性值，并触发OnPropertyChanged事件
        /// </summary>
        /// <param name="property">属性名</param>
        public void Set<T>(string property, T value)
        {
            T oldValue = default(T);
            lock (properties)
            {
                if (!properties.ContainsKey(property))
                {
                    properties.Add(property, value);
                }
                else
                {
                    oldValue = Get<T>(property, value);
                    properties[property] = value;
                }
            }
            OnPropertyChanged(new PropertyChangedEventArgs(this, property, oldValue, value));
        }

        public bool Contains(string property)
        {
            lock (properties)
            {
                return properties.ContainsKey(property);
            }
        }

        public int Count
        {
            get
            {
                lock (properties)
                {
                    return properties.Count;
                }
            }
        }

        public bool Remove(string property)
        {
            lock (properties)
            {
                return properties.Remove(property);
            }
        }

        public override string ToString()
        {
            lock (properties)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("[Properties:{");
                foreach (KeyValuePair<string, object> entry in properties)
                {
                    sb.Append(entry.Key);
                    sb.Append("=");
                    sb.Append(entry.Value);
                    sb.Append(",");
                }
                sb.Append("}]");
                return sb.ToString();
            }
        }

        /// <summary>
        /// 从xml读取器中读取当前位置的属性构造为属性对象返回（读取器在读取完成后会恢复到元素位置）
        /// </summary>
        public static Properties ReadFromAttributes(XmlReader reader)
        {
            Properties properties = new Properties();
            if (reader.HasAttributes)
            {
                for (int i = 0; i < reader.AttributeCount; i++)
                {
                    reader.MoveToAttribute(i);
                    properties[reader.Name] = reader.Value;
                }
                reader.MoveToElement(); 
            }
            return properties;
        }

        /// <summary>
        /// 读取xml中的属性，直到指定结束元素。他将根据之间的元素类型分别进行读取：
        /// <item>如果xml元素为Properties，则递归调用读取器中的元素</item>
        /// <item>元素为数组，则读取数组元素ArrayList设置到properties对应的此元素名的属性中</item>
        /// <item>元素为可序列化值则读取内部的xml内容创建SerializedValue设置到属性中</item>
        /// <item>其他元素读取第一个属性</item>
        /// </summary>
        internal void ReadProperties(XmlReader reader, string endElement)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        if (reader.LocalName == endElement)
                        {
                            return;
                        }
                        break;
                    case XmlNodeType.Element:
                        string propertyName = reader.LocalName;
                        if (propertyName == "Properties")
                        {
                            //如果xml元素为Properties，则递归调用读取器中的元素
                            propertyName = reader.GetAttribute(0);
                            Properties p = new Properties();
                            p.ReadProperties(reader, "Properties");
                            properties[propertyName] = p;
                        }
                        else if (propertyName == "Array")
                        {
                            //元素为数组，则读取数组元素ArrayList设置到properties对应的此元素名的属性中
                            propertyName = reader.GetAttribute(0);
                            properties[propertyName] = ReadArray(reader);
                        }
                        else if (propertyName == "SerializedValue")
                        {
                            //元素为可序列化值则读取内部的xml内容创建SerializedValue设置到属性中
                            propertyName = reader.GetAttribute(0);
                            properties[propertyName] = new SerializedValue(reader.ReadInnerXml());
                        }
                        else
                        {//其他元素读取第一个属性
                            properties[propertyName] = reader.HasAttributes ? reader.GetAttribute(0) : null;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 循环读取Array元素中子元素的第一个属性加入到ArrayList中
        /// </summary>
        ArrayList ReadArray(XmlReader reader)
        {
            if (reader.IsEmptyElement)
                return new ArrayList(0);
            ArrayList l = new ArrayList();
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        if (reader.LocalName == "Array")
                        {
                            return l;
                        }
                        break;
                    case XmlNodeType.Element:
                        l.Add(reader.HasAttributes ? reader.GetAttribute(0) : null);
                        break;
                }
            }
            return l;
        }

        /// <summary>
        /// 将属性写入到xml中，根据属性类型分别写入元素：
        /// <code>
        /// <Properties name='dfdf'>
        ///   ....
        /// </Properties>
        /// <Array name="">
        ///   <Element></Element>
        /// </Array>
        /// <SerializedValue name="">
        ///   ....
        /// </SerializedValue>
        /// <convert_to_string value="能转换为字符串的值">
        ///   
        /// </convert_to_string>
        /// </code>
        /// </summary>
        /// <param name="writer"></param>
        public void WriteProperties(XmlWriter writer)
        {
            lock (properties)
            {
                List<KeyValuePair<string, object>> sortedProperties = new List<KeyValuePair<string, object>>(properties);
                sortedProperties.Sort((a, b) => StringComparer.OrdinalIgnoreCase.Compare(a.Key, b.Key));
                foreach (KeyValuePair<string, object> entry in sortedProperties)
                {
                    object val = entry.Value;
                    if (val is Properties)
                    {
                        writer.WriteStartElement("Properties");
                        writer.WriteAttributeString("name", entry.Key);
                        ((Properties)val).WriteProperties(writer);
                        writer.WriteEndElement();
                    }
                    else if (val is Array || val is ArrayList)
                    {
                        writer.WriteStartElement("Array");
                        writer.WriteAttributeString("name", entry.Key);
                        foreach (object o in (IEnumerable)val)
                        {
                            writer.WriteStartElement("Element");
                            WriteValue(writer, o);
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                    }
                    else if (System.ComponentModel.TypeDescriptor.GetConverter(val).CanConvertFrom(typeof(string)))
                    {
                        //值如果能转换为字符串
                        writer.WriteStartElement(entry.Key);
                        WriteValue(writer, val);
                        writer.WriteEndElement();
                    }
                    else if (val is SerializedValue)
                    {
                        writer.WriteStartElement("SerializedValue");
                        writer.WriteAttributeString("name", entry.Key);
                        writer.WriteRaw(((SerializedValue)val).Content);
                        writer.WriteEndElement();
                    }
                    else
                    {
                        writer.WriteStartElement("SerializedValue");
                        writer.WriteAttributeString("name", entry.Key);
                        XmlSerializer serializer = new XmlSerializer(val.GetType());
                        serializer.Serialize(writer, val, null);
                        writer.WriteEndElement();
                    }
                }
            }
        }

        /// <summary>
        /// 如果写入的值不为字符串类型，则获取值的类型转换器将值转换为区域不变字符写入
        /// <...value="..."></...value="...">中
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="val"></param>
        void WriteValue(XmlWriter writer, object val)
        {
            if (val != null)
            {
                if (val is string)
                {
                    writer.WriteAttributeString("value", val.ToString());
                }
                else
                {
                    System.ComponentModel.TypeConverter c = System.ComponentModel.TypeDescriptor.GetConverter(val.GetType());
                    writer.WriteAttributeString("value", c.ConvertToInvariantString(val));
                }
            }
        }

        /// <summary>
        /// 将此属性内容保存到指定文件中，他将以xml格式的数据写入
        /// </summary>
        public void Save(string fileName)
        {
            using (XmlTextWriter writer = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.WriteStartElement("Properties");
                WriteProperties(writer);
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// 以xml的方式读取属性
        /// </summary>
        public static Properties Load(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return null;
            }
            using (XmlTextReader reader = new XmlTextReader(fileName))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.LocalName)
                        {
                            case "Properties":
                                Properties properties = new Properties();
                                properties.ReadProperties(reader, "Properties");
                                return properties;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 获取指定属性的值，并根据属性值的类型与泛型参数T指定的类型，
        /// 使用类型转换器转换后重新存入缓存中
        /// </summary>
        /// <param name="property">属性名</param>
        /// <param name="defaultValue">默认值</param>
        public T Get<T>(string property, T defaultValue)
        {
            lock (properties)
            {
                object o;
                if (!properties.TryGetValue(property, out o))
                {
                    //如果获取的属性不存在，则加入默认值
                    properties.Add(property, defaultValue);
                    return defaultValue;
                }

                if (o is string && typeof(T) != typeof(string))
                {
                    //属性值为字符，但T不是字符型，则转换为不变字符存入
                    System.ComponentModel.TypeConverter c = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
                    try
                    {
                        o = c.ConvertFromInvariantString(o.ToString());
                    }
                    catch (Exception ex)
                    {
                        MessageService.ShowWarning("Error loading property '" + property + "': " + ex.Message);
                        o = defaultValue;
                    }
                    properties[property] = o; // store for future look up
                }
                else if (o is ArrayList && typeof(T).IsArray)
                {
                    //如果T泛型为列表，则获取列表元素的类型转换器转换值
                    ArrayList list = (ArrayList)o;
                    Type elementType = typeof(T).GetElementType();
                    Array arr = System.Array.CreateInstance(elementType, list.Count);
                    System.ComponentModel.TypeConverter c = System.ComponentModel.TypeDescriptor.GetConverter(elementType);
                    try
                    {
                        for (int i = 0; i < arr.Length; ++i)
                        {
                            if (list[i] != null)
                            {
                                arr.SetValue(c.ConvertFromInvariantString(list[i].ToString()), i);
                            }
                        }
                        o = arr;
                    }
                    catch (Exception ex)
                    {
                        MessageService.ShowWarning("Error loading property '" + property + "': " + ex.Message);
                        o = defaultValue;
                    }
                    properties[property] = o;
                }
                else if (!(o is string) && typeof(T) == typeof(string))
                {
                    //属性值不为字符，但T为字符，则获取T的转换器转换属性值为字符
                    System.ComponentModel.TypeConverter c = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
                    if (c.CanConvertTo(typeof(string)))
                    {
                        o = c.ConvertToInvariantString(o);
                    }
                    else
                    {
                        o = o.ToString();
                    }
                }
                else if (o is SerializedValue)
                {
                    try
                    {
                        o = ((SerializedValue)o).Deserialize<T>();
                    }
                    catch (Exception ex)
                    {
                        MessageService.ShowWarning("Error loading property '" + property + "': " + ex.Message);
                        o = defaultValue;
                    }
                    properties[property] = o; // store for future look up
                }
                try
                {
                    return (T)o;
                }
                catch (NullReferenceException)
                {
                    return defaultValue;
                }
            }
        }
        /// <summary>
        /// 属性改变事件的触发器
        /// </summary>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }
        /// <summary>
        /// 属性改变事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
