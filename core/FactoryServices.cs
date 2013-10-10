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
using CrystalWall.Auths;
using CrystalWall.Aop;

namespace CrystalWall
{
    /// <summary>
    /// 提供各种抽象工厂的服务类，他是系统所有工厂、常数对象的访问入口,在程序启动时，需要调用相关Set方法
    /// 设置初始值。外部客户端应该使用此静态服务获取各种需要的工厂接口
    /// </summary>
    /// <author>vincent valenlee</author>
    public static class FactoryServices
    {
        public static readonly IPermissionFactory PermissionFactory = null; //TODO:编写权限工厂实现

        public static AnonyPrincipalToken ANONY_PRINCIPAL_TOKEN
        {
            get
            {
                //TODO:添加从配置中读取匿名用户的权限
                return new AnonyPrincipalToken(null);
            }
        }

        /// <summary>
        /// 默认的权限决定者，实际每次都将返回一个新的对象
        /// </summary>
        public static  IAccessDecider DEFAULT_DECIDER 
        {
            get
            {
                DefaultDecider decider =  new DefaultDecider();
                //TODO:从应用程序启动配置中获取默认权限决定者的IPointResolveStrategy解析器配置设置到默认决定者中
                //目前只添加使用Castle动态代理的IInvocation对象解析出源方法上定义的权限点的解析器。但此时获取对象
                //需要使用Castle的动态代理方式生成对象
                decider.AddPointResolve(new DynamicProxyMethodPointResolver());
                return decider;
            }
        }

        ///// <summary>
        ///// 根据指定的权限字符，获取常数类型提供者
        ///// </summary>
        //public static IGrandedPermissionProvider GetGrandedProvider(params string[] permissions)
        //{
        //    if (permissions != null && permissions.Length > 0)
        //    {
        //        return new DefaultGrandedPermissionProvider(permissions);
        //    }
        //    else
        //    {
        //        return EMPTY_PERMISSIONINFO_PROVIDER;
        //    }
        //}

        //public static readonly IResourceRegistry ResourceRegistry = null;//TODO:编写资源注册表实现

        private static IPrincipalTokenStorageFactory principalStorageFactory;

        public static IPrincipalTokenStorageFactory PrincipalStorageFactory
        {
            get 
            { 
                return FactoryServices.principalStorageFactory; 
            }
            set 
            {
                if (FactoryServices.principalStorageFactory != null)
                    throw new ArgumentException("身份存储工厂已经设置且不允许改变！");
                FactoryServices.principalStorageFactory = value; 
            }
        }
    }
}
