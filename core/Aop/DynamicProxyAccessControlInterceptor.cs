/*
 * Copyright 2008-2010 the original author or authors.
 *
 * Licensed under the Eclipse Public License v1.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.eclipse.org/legal/epl-v10.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
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
    /// <author>vincent valenlee</author>
    public class DynamicProxyAccessControlInterceptor: IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            CrystalWallSite site = CrystalWallSite.Find(invocation);
            site.InitSite();
            //使用DynamicProxyMethodPointResolver解析invocation动态拦截的代理方法上定义的权限点
            //并根据site对象上配置的decider判断权限
            bool result = true;
            site.Decider.Decide(PrincipalTokenHolder.CurrentPrincipal, invocation, out result);
            //权限通过，继续执行原方法
            invocation.Proceed();
        }
    }
}
