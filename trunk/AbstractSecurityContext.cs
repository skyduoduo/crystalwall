using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrystalWall
{

    /// <summary>
    /// 权限控制执行上下文
    /// </summary>
    public abstract class AbstractSecurityContext
    {

        public event EventHandler<AccessExceptionEventArgs> AccessDenyed;

        private IList<IPrincipalProvider> principalProviders = new List<IPrincipalProvider>();

        public IList<IPrincipalProvider> PrincipalProviders
        {
            get { return principalProviders; }
            set { principalProviders = value; }//提供容器注入
        }

        /// <summary>
        /// 获取当前的有效用户身份唯一标识
        /// </summary>
        protected abstract string GetCurrentName();

        /// <summary>
        /// 通过此方法，上下文信息从身份提供者中获取存储指定名称身份的身份令牌及其授权信息。
        /// 此方法将被处理权限的方法中调用。如果没有任何提供者能够提供指定名称的身份，则返回匿名身份
        /// </summary>
        protected IPrincipalToken GetPrincipal(string name)
        {
            foreach (IPrincipalProvider provider in PrincipalProviders)
            {
                if (provider.HasPrincipal(name))
                    return provider[name];
            }
            return FactoryServices.ANONY_PRINCIPAL_TOKEN;
        }

        /// <summary>
        ///  授权不通过，则执行不通过时的事件处理
        /// </summary>
        protected void OnAccessException(IPrincipalToken principal, object check)
        {
            if (AccessDenyed != null)
                AccessDenyed(this, new AccessExceptionEventArgs(principal, check));
        }

        /// <summary>
        /// 根据当前访问对象获取其上的权限点
        /// </summary>
        public virtual PermissionPoint GetPoint(object context)
        {
            //TODO:这里对context的各种类型没有做到开放封闭原则，将来需要重构！！！！
           return FactoryServices.ResourceRegistry.Find(context).Resolve(context);
        }

    }

    public class AccessExceptionEventArgs : EventArgs
    {
        private IPrincipalToken principal;

        public IPrincipalToken Principal
        {
            get { return principal; }
        }
        private object check;

        public object Check
        {
            get { return check; }
        }

        public AccessExceptionEventArgs(IPrincipalToken principal, object check)
        {
            this.principal = principal;
            this.check = check;
        }


    }
}
