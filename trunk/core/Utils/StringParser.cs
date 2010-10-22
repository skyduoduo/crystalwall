using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using CrystalWall.Property;
using CrystalWall.FileUtils;
using CrystalWall.Message;

///<remarks>this file is from Sharpdevelop</remarks>
namespace CrystalWall.Utils
{
    /// <summary>
    /// 用于解析系统内部的${xyz}标签的字符解析器类，他能够根据${key:value}的各种类型
    /// 进行解析，例如：${DATE:日期格式化字符}、${DATE}、${TIME}、${ProductName}、${GUID}、
    /// ${自定义标签}、${SDKTOOLPATH:SDK工具路径}、${ENV:环境变量名}、
    /// ${RES:资源key}、${PROPERTY:配置属性服务key}、${对象属性名}
    /// </summary>
    public static class StringParser
    {
        readonly static Dictionary<string, string> properties;
        readonly static Dictionary<string, IStringTagProvider> stringTagProviders;
        readonly static Dictionary<string, object> propertyObjects;

        public static Dictionary<string, string> Properties
        {
            get
            {
                return properties;
            }
        }

        public static Dictionary<string, object> PropertyObjects
        {
            get
            {
                return propertyObjects;
            }
        }

        /// <summary>
        /// 1、记录可执行程序集的文件版本信息属性exe
        /// 2、记录环境用户名USER和版本属性Version
        /// 3、记录Platform平台属性
        /// </summary>
        static StringParser()
        {
            properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            stringTagProviders = new Dictionary<string, IStringTagProvider>(StringComparer.OrdinalIgnoreCase);
            propertyObjects = new Dictionary<string, object>();

            // 获取默认应用程序域中第一个可执行文件
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                //获取入口程序集exe文件名版本信息
                string exeName = entryAssembly.Location;
                propertyObjects["exe"] = FileVersionInfo.GetVersionInfo(exeName);
            }

            if (IntPtr.Size == 4)
            {
                properties["Platform"] = "Win32";
            }
            else if (IntPtr.Size == 8)
            {
                properties["Platform"] = "Win64";
            }
            else
            {
                properties["Platform"] = "unknown";
            }
        }

        /// <summary>
        /// 解析${xyz}类型的字符
        /// </summary>
        public static string Parse(string input)
        {
            return Parse(input, null);
        }

        public static void Parse(string[] inputs)
        {
            for (int i = 0; i < inputs.Length; ++i)
            {
                inputs[i] = Parse(inputs[i], null);
            }
        }

        public static void RegisterStringTagProvider(IStringTagProvider tagProvider)
        {
            foreach (string str in tagProvider.Tags)
            {
                stringTagProviders[str] = tagProvider;
            }
        }

        /// <summary>
        /// 解析${xyz}类型的属性值
        /// </summary>
        public static string Parse(string input, string[,] customTags)
        {
            if (input == null)
                return null;
            int pos = 0;
            StringBuilder output = null; 
            do
            {
                int oldPos = pos;
                pos = input.IndexOf("${", pos, StringComparison.Ordinal);
                if (pos < 0)
                {
                    if (output == null)
                    {
                        return input;
                    }
                    else
                    {
                        if (oldPos < input.Length)
                        {
                            // 存储匹配标签之后的字符
                            output.Append(input, oldPos, input.Length - oldPos);
                        }
                        return output.ToString();
                    }
                }
                if (output == null)
                {
                    if (pos == 0)
                        output = new StringBuilder();
                    else
                        output = new StringBuilder(input, 0, pos, pos + 16);
                }
                else
                {
                    if (pos > oldPos)
                    {
                        // 将两个标签之间字符加入
                        output.Append(input, oldPos, pos - oldPos);
                    }
                }
                int end = input.IndexOf('}', pos + 1);
                if (end < 0)
                {
                    output.Append("${");
                    pos += 2;
                }
                else
                {
                    string property = input.Substring(pos + 2, end - pos - 2);
                    //获取属性值
                    string val = GetValue(property, customTags);
                    if (val == null)
                    {//不存在属性值
                        output.Append("${");
                        output.Append(property);
                        output.Append('}');
                    }
                    else
                    {
                        output.Append(val);
                    }
                    pos = end + 1;
                }
            } while (pos < input.Length);
            return output.ToString();
        }

        static string GetValue(string propertyName, string[,] customTags)
        {
            if (propertyName.StartsWith("res:", StringComparison.OrdinalIgnoreCase))
            {
                try
                {//如果要解析的属性为资源字符，则从资源管理器服务中获取输入字符，并解析
                    return Parse(ResourceService.GetString(propertyName.Substring(4)), customTags);
                }
                catch (ResourceNotFoundException)
                {
                    return null;
                }
            }
            if (propertyName.StartsWith("DATE:", StringComparison.OrdinalIgnoreCase))
            {//如果解析的属性为日期前缀，则使用属性格式化当前日期
                try
                {
                    return DateTime.Now.ToString(propertyName.Split(':')[1]);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            //解析日期、时间、产品名、GUID属性
            if (propertyName.Equals("DATE", StringComparison.OrdinalIgnoreCase))
                return DateTime.Today.ToShortDateString();
            if (propertyName.Equals("TIME", StringComparison.OrdinalIgnoreCase))
                return DateTime.Now.ToShortTimeString();
            if (propertyName.Equals("ProductName", StringComparison.OrdinalIgnoreCase))
                return MessageService.ProductName;
            if (propertyName.Equals("GUID", StringComparison.OrdinalIgnoreCase))
                return Guid.NewGuid().ToString().ToUpperInvariant();

            if (customTags != null)
            {
                for (int j = 0; j < customTags.GetLength(0); ++j)
                {
                    if (propertyName.Equals(customTags[j, 0], StringComparison.OrdinalIgnoreCase))
                    {
                        return customTags[j, 1];//从自定义标签中获取值
                    }
                }
            }

            if (properties.ContainsKey(propertyName))
            {
                return properties[propertyName];
            }
            //标签提供者中具有属性名
            if (stringTagProviders.ContainsKey(propertyName))
            {
                return stringTagProviders[propertyName].Convert(propertyName);
            }

            int k = propertyName.IndexOf(':');
            if (k <= 0)
                return null;
            string prefix = propertyName.Substring(0, k);
            propertyName = propertyName.Substring(k + 1);
            //解析SDK工具路径、环境变量值、资源、PropertyService属性服务中的属性
            switch (prefix.ToUpperInvariant())
            {
                case "SDKTOOLPATH":
                    return FileUtil.GetSdkPath(propertyName);
                case "ENV":
                    return Environment.GetEnvironmentVariable(propertyName);
                case "RES":
                    try
                    {
                        return Parse(ResourceService.GetString(propertyName), customTags);
                    }
                    catch (ResourceNotFoundException)
                    {
                        return null;
                    }
                case "PROPERTY":
                    return GetProperty(propertyName);
                default:
                    if (propertyObjects.ContainsKey(prefix))
                    {
                        return Get(propertyObjects[prefix], propertyName);
                    }
                    else
                    {
                        return null;
                    }
            }
        }

        /// <summary>
        /// 按照下面的语法获取指定属性的值：
        /// ${property:PropertyName}
        /// ${property:PropertyName??DefaultValue}
        /// ${property:ContainerName/PropertyName}
        /// ${property:ContainerName/PropertyName??DefaultValue}
        /// 属性都存储在PropertyService容器中
        /// </summary>
        static string GetProperty(string propertyName)
        {
            string defaultValue = "";
            int pos = propertyName.LastIndexOf("??", StringComparison.Ordinal);
            if (pos >= 0)
            {
                defaultValue = propertyName.Substring(pos + 2);
                propertyName = propertyName.Substring(0, pos);
            }
            pos = propertyName.IndexOf('/');
            if (pos >= 0)
            {
                Properties properties = PropertyService.Get(propertyName.Substring(0, pos), new Properties());
                propertyName = propertyName.Substring(pos + 1);
                pos = propertyName.IndexOf('/');
                while (pos >= 0)
                {
                    properties = properties.Get(propertyName.Substring(0, pos), new Properties());
                    propertyName = propertyName.Substring(pos + 1);
                }
                return properties.Get(propertyName, defaultValue);
            }
            else
            {
                return PropertyService.Get(propertyName, defaultValue);
            }
        }

        static string Get(object obj, string name)
        {
            Type type = obj.GetType();
            PropertyInfo prop = type.GetProperty(name);
            if (prop != null)
            {
                return prop.GetValue(obj, null).ToString();
            }
            FieldInfo field = type.GetField(name);
            if (field != null)
            {
                return field.GetValue(obj).ToString();
            }
            return null;
        }
    }
}
