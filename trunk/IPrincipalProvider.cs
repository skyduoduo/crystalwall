using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CrystalWall
{
    /// <summary>
    /// 授权信息集的提供者，权限框架可以根据此身份提供者获取对应身份名的授权信息。例如可以根据以下配置文件获取授权信息;
    /// <code>
    ///    <principal-providers>
    ///       <provider class="SQLServerDBPrincipalProvider" assembly="程序集文件名">
    ///         <connection>Data Source=**;Initial Catalog=***;User ID=sa;Password=***;</connection>
    ///       </privider>
    ///       <provider class="LDAPPrincipalProvider"/>
    ///       <provider class="XmlPricipalProvider">
    ///          <file>~/web/principal/principal.xml</file>
    ///       </provider>
    ///    </principal-providers>
    /// </code>
    /// 注意：多提供者支持提供多个获取不同身份及授权的方法。但不要在多个提供者上提供同一个身份，保持同一身份由唯一的一个提供者提供，
    /// 这样能够保持程序信息存储在单一的位置且也避免了不必要的复杂性
    /// </summary>
    public interface IPrincipalProvider
    {
        bool HasPrincipal(string name);

        /// <summary>
        /// 获取指定身份名的身份令牌
        /// </summary>
        IPrincipalToken this[string name]
        {
            get;
        }

        /// <summary>
        /// 根据配置文件中此提供者的元素信息初始化此提供者，具体如何初始化由实现决定
        /// </summary>
        /// <param name="element">此提供者类的xml节点</param>
        /// <param name="attribute">要初始化的属性</param>
        /// <param name="data">相关数据，由实现决定传递具体的对象</param>
        void InitData(XmlNode element, string attribute, object data);

        /// <summary>
        /// 提供指定身份的授权信息
        /// </summary>
        PermissionInfoCollection GetPermissions(string name);
    }
}
