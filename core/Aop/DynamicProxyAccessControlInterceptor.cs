using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;

namespace CrystalWall.Aop
{
    /// <summary>
    /// 此类使用Castle的动态代理拦截器的方式拦截代理的对象上的公共虚方法，如果在虚
    /// 方法之上配置了PermissionPoint权限点指定的权限，则进行权限检查。要使用此类，
    /// 需要在配置文件中配置：
    /// <code>
    /// <sites>
    ///   <site context="Castle.DynamicProxy.IInvocation, Castle.Core , Version=2.5.1.00, Culture=neutral, PublicKeyToken=null">
    ///      <!--其他配置-->
    ///   </site>
    ///   <!--其他site配置-->
    /// </sites>
    /// </code>
    /// </summary>
    public class DynamicProxyAccessControlInterceptor: IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            CrystalWallSite site = CrystalWallSite.Find(invocation);
            site.InitSite();
            //使用DynamicProxyMethodPointResolver解析invocation动态拦截的代理方法上定义的权限点
            //并根据site对象上配置的decider判断权限
            site.Decider.Decide(PrincipalTokenHolder.CurrentPrincipal, invocation);
            //权限通过，继续执行原方法
            invocation.Proceed();
        }
    }
}
