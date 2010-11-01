using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using CrystalWall.Utils;

namespace CrystalWall.Config
{
    /// <summary>
    /// 身份令牌存储器的配置对象，子类可以重写此配置类
    /// </summary>
    public class PrincipalTokenStorageSection : ConfigurationSection, IExecutingElement
    {

        [ConfigurationProperty("class", DefaultValue = "false", IsRequired = true)]
        public string Class
        {
            get
            {
                return (string)this["class"];
            }
            set
            {
                this["class"] = value;
            }
        }

        public virtual object GetExecutingObject()
        {
            Type t = Type.GetType(Class, true);
            if (!typeof(IPrincipalTokenStorage).IsAssignableFrom(t))
                return new ConfigurationException(PrincipalTokenHolder.PRINCIPAL_TOKEN_STORAGE_SECTION, "身份提供者的配置类必须实现IPrincipalTokenStorage接口");
            try
            {
                return (IPrincipalTokenStorage)t.GetConstructor(new Type[0]).Invoke(new object[0]);
            }
            catch (Exception e)
            {
                ServiceManager.LoggingService.Fatal("身份提供者配置无法构建，请仔细检查配置文件中身份提供者的class类型是否为完全限定名（名称空间+程序集)");
                throw new ConfigurationException(PrincipalTokenHolder.PRINCIPAL_TOKEN_STORAGE_SECTION, "身份提供者配置无法构建", e);
            }
           
        }
    }
}
