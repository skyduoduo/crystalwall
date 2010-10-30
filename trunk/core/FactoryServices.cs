﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrystalWall.Auths;

namespace CrystalWall
{
    /// <summary>
    /// 提供各种抽象工厂的服务类，他是系统所有工厂、常数对象的访问入口。外部客户端应该使用此静态服务获取各种需要的工厂接口
    /// </summary>
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
        /// 默认的权限决定者
        /// </summary>
        public static  IAccessDecider DEFAULT_DECIDER 
        {
            get
            {
                return  new DefaultDecider();
                //TODO:从应用程序启动配置中获取默认权限决定者的IPointResolveStrategy解析器配置设置到默认决定者中
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

        public static readonly IResourceRegistry ResourceRegistry = null;//TODO:编写资源注册表实现
    }
}
