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
    /// 使用Castle动态代理生成具有权限检查功能的代理对象的代理工具类。对于任何想要进行权限控制的对象
    /// （方法中具有PermissionPoint元特性定义的对象）都必须使用此工具类生成实例（使用动态植入方式，
    /// 静态植入方式除外）
    /// </summary>
    /// <author>vincent valenlee</author>
    public static class CrystalwallDynamicProxyor
    {
        private static readonly ProxyGenerator generator = new ProxyGenerator();

        /// <summary>
        /// 生成target的代理对象
        /// </summary>
        /// <typeparam name="I">T实现的接口</typeparam>
        /// <typeparam name="T">要代理的对象类型，他必须实现I接口</typeparam>
        /// <param name="target">代理的对象，必须实现I接口</param>
        public static I ProxyInterface<I, T>(T target)
            where I : class
            where T: I
        {
            return generator.CreateInterfaceProxyWithTarget<I>(target, new ProxyGenerationOptions(new DynamicInterceptorFilter()),  new DynamicProxyAccessControlInterceptor());
        }

        /// <summary>
        /// 生成target的代理类，只有虚方法才能被代理，且代理对象类型不能为seale
        /// </summary>
        /// <typeparam name="T">代理的类型</typeparam>
        /// <param name="target">要代理的对象</param>
        public static T ProxyClass<T>(T target)
            where T: class
        {
            return generator.CreateClassProxyWithTarget<T>(target, new ProxyGenerationOptions(new DynamicInterceptorFilter()), new DynamicProxyAccessControlInterceptor());
        }

        /// <summary>
        /// 直接生成代理对象的实例
        /// </summary>
        /// <typeparam name="T">代理的类型</typeparam>
        /// <returns>被代理的类型的实例</returns>
        public static T ProxyClass<T>()
            where T : class
        {
            return generator.CreateClassProxy<T>(new ProxyGenerationOptions(new DynamicInterceptorFilter()),new DynamicProxyAccessControlInterceptor());
        }
    }

    /// <summary>
    /// 只有方法上使用PermissionPoint元特性定义的方法才被拦截的过滤器
    /// </summary>
    public class DynamicInterceptorFilter : IProxyGenerationHook
    {
        public void MethodsInspected()
        {
            //do nothing
        }

        public void NonProxyableMemberNotification(Type type, System.Reflection.MemberInfo memberInfo)
        {
            //do nothing
        }

        public bool ShouldInterceptMethod(Type type, System.Reflection.MethodInfo methodInfo)
        {
            //只有方法上使用PermissionPoint元特性定义的方法才被拦截
            object[] attrs = methodInfo.GetCustomAttributes(typeof(PermissionPointAttribute), true);
            if (attrs == null || attrs.Length == 0)
                return false;
            return true;
        }
    }

}
