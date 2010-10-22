using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Reflection;
using CrystalWall.Utils;

namespace CrystalWall.Config
{
    /// <summary>
    /// 用于在xml配置文件中解析principal提供者的处理器
    /// </summary>
    public class PrincipalProviderSectionHandler : IConfigurationSectionHandler 
    {
        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
              try
              {
                  IPrincipalProvider provider;
                  if (section.Attributes["assembly"] == null)
                  {
                      //没有指定程序集，但可能在class中指定全限定名。首先使用当前的程序集加载，否则使用全限定名加载
                      provider = (IPrincipalProvider)Assembly.GetExecutingAssembly().CreateInstance(section.Attributes["class"].Name);
                      if (provider == null)
                          provider = (IPrincipalProvider)Type.GetType(section.Attributes["class"].Name, true).GetConstructor(new Type[0]).Invoke(new object[0]);
                  }
                  else
                  {
                      provider = (IPrincipalProvider)Assembly.LoadFrom(section.Attributes["assembly"].Name).CreateInstance(section.Attributes["class"].Name);
                  }
                  //初始化provider数据（将根据具体的provider并根据xml中的配置进行初始化，由子类决定）
                  provider.InitData(section, null, null);
                  return provider;
              }
              catch (Exception e)
              {
                  ServiceManager.LoggingService.Error("无法加载指定的身份提供者，请检查配置文件是否正确", e);
                  throw e;
              }
        }
    }
}
