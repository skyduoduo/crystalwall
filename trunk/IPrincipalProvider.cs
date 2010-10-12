using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QJSoft.ECBC.Authority
{
    /// <summary>
    /// 授权信息集的提供者，权限框架可以根据此身份提供者获取对应身份名的授权信息。例如可以根据以下配置文件获取授权信息;
    /// <code>
    ///    <principal-providers>
    ///       <provider class="DBPrincipalProvider"/>
    ///       <provider class="XmlPricipalProvider">
    ///          <file>~/web/principal/principal.xml</file>
    ///       </provider>
    ///       <provider class="QJSoft.ECBC.Authority.Auths.DefaultGrandedPermissionProvider" assembly="QJSoft.ECBC.Authority">
    ///           <principal name="admin">
    ///              <permission class="......"></permission>
    ///              <permission class="......"></permission>
    ///           </principal>
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
        /// 提供指定身份的授权信息
        /// </summary>
        PermissionInfoCollection GetPermissions(string name);
    }
}
