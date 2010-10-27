using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Reflection;
using CrystalWall.Utils;
using System.Xml;

namespace CrystalWall.Config
{
    /// <summary>
    /// 用于在xml配置文件中解析principal提供者的处理器
    /// </summary>
    public class PrincipalProviderSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            if (section.Attributes["class"] == null)
                throw new ConfigurationException(section.Name, "身份提供者配置节点中必须具有class属性指定提供者类型");
            IPrincipalProvider provider = null;
            try
            {
                if (section.Attributes["assembly"] == null)
                {
                    //没有指定程序集，但可能在class中指定全限定名。首先使用当前运行的程序集加载，然后使用crystalwall程序集加载，最后使用全限定名加载
                    provider = (IPrincipalProvider)Assembly.GetExecutingAssembly().CreateInstance(section.Attributes["class"].Value);
                    if (provider == null)
                        provider = (IPrincipalProvider)Assembly.GetAssembly(typeof(IPrincipalProvider)).CreateInstance(section.Attributes["class"].Value);
                    if (provider == null)
                        provider = (IPrincipalProvider)Type.GetType(section.Attributes["class"].Value, true).GetConstructor(new Type[0]).Invoke(new object[0]);
                }
                else
                {
                    provider = (IPrincipalProvider)Assembly.LoadFrom(section.Attributes["assembly"].Value).CreateInstance(section.Attributes["class"].Value);
                }
                if (provider == null)
                    throw new ConfigurationException(section.Name, "身份提供者配置节点中指定提供者类型无法加载");
            }
            catch (Exception e)
            {
                ServiceManager.LoggingService.Error("无法加载指定的身份提供者，请检查配置文件是否正确", e);
                throw e;
            }
            //初始化provider数据（将根据具体的provider并根据xml中的配置进行初始化，由子类决定）
            provider.InitData(section, null, null);
            return provider;
        }
    }
}
