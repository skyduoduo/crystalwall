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
using CrystalWall.Config;
using CrystalWall.Utils;
using System.Web.Configuration;

namespace CrystalWall
{
    /// <summary>
    /// 获取当前用户以及存储身份令牌提供者的持有器
    /// </summary>
    /// <author>vincent valenlee</author>
    public static class PrincipalTokenHolder
    {
        private static IList<IPrincipalProvider> principalProviders = new List<IPrincipalProvider>();

        public static IList<IPrincipalProvider> PrincipalProviders
        {
          get { return PrincipalTokenHolder.principalProviders; }
          set { PrincipalTokenHolder.principalProviders = value; }
        }

        private static IPrincipalTokenStorage storage;

        public static IPrincipalTokenStorage Storage
        {
          get { return PrincipalTokenHolder.storage; }
          set { PrincipalTokenHolder.storage = value; }
        }

        private static  ConfigurationFile configFile;

        public static ConfigurationFile ConfigFile
        {
            get
            {
                return configFile;
            }
        }

        public const string PRINCIPAL_PROVIDERS_SECTION = "principal-providers";

        public const string PRINCIPAL_TOKEN_STORAGE_SECTION = "principal-storage";

        /// <summary>
        /// 清除数据，用于测试目的
        /// </summary>
        public static void Clear()
        {
            Storage = null;
            PrincipalProviders.Clear();
            ClearCurrentToken();
            configFile = null;
        }

        /// <summary>
        /// 使用普通配置文件初始化的方法，此方法只能在整个应用程序中被调用一次
        /// </summary>
        /// <param name="configPath">程序集路径</param>
        public static void Init(string configPath)
        {
            if (configFile != null)
                throw new ConfigurationException("", "系统已经初始化，不能再次进行初始化");
            configFile = new ConfigurationFile(configPath);
            InitProviderAndStorage();
        }

        private static void InitProviderAndStorage()
        {
            try
            {
                IList<IPrincipalProvider> ps = configFile.GetSection(PRINCIPAL_PROVIDERS_SECTION) as IList<IPrincipalProvider>;
                if (ps == null || ps.Count == 0)
                    throw new ConfigurationException(PRINCIPAL_PROVIDERS_SECTION, "身份提供者必须配置，请检查配置文件是否配置正确");
                PrincipalProviders = ps;
                PrincipalTokenStorageSection storageSection = configFile.Configuration.GetSection(PRINCIPAL_TOKEN_STORAGE_SECTION) as PrincipalTokenStorageSection;
                if (storageSection != null)
                    Storage = storageSection.GetExecutingObject() as IPrincipalTokenStorage;
            }
            catch (Exception e)
            {
                ServiceManager.LoggingService.Fatal("获取身份提供者配置或者当前身份存储配置错误，请仔细检查配置是否正确！");
                throw e;
            }
        }

        /// <summary>
        /// 使用web根配置文件初始化的方法，此方法只能被整个应用程序调用一次
        /// </summary>
        public static void InitWeb()
        {
            if (configFile != null)
                throw new ConfigurationException("", "系统已经初始化，不能再次进行初始化");
            configFile = new ConfigurationFile(WebConfigurationManager.OpenWebConfiguration(null));
            InitProviderAndStorage();
        }

        //private static IList<PermissionInfo> anonyPrincipalPermission;

        //public static IList<PermissionInfo> AnonyPrincipalPermission
        //{
        //    get { return anonyPrincipalPermission; }
        //    set { anonyPrincipalPermission = value; }
        //}

        //缓存（应该使用定时清空的缓存）
        private static IDictionary<string, IPrincipalToken> tokenCache = new Dictionary<string, IPrincipalToken>();

        /// <summary>
        /// 获取指定标识的令牌，他将遍历提供者列表，因此是一个耗时的操作
        /// TODO:应当做适当的缓存，缓存机制将来版本会加强
        /// </summary>
        public static IPrincipalToken GetPrincipal(string indentity) 
        {
            if (tokenCache.ContainsKey(indentity) && tokenCache[indentity] != null)
                return tokenCache[indentity];
            foreach (IPrincipalProvider provider in PrincipalProviders)
            {
                if (provider.HasPrincipal(indentity))
                {
                    tokenCache.Add(indentity, provider[indentity]);
                    return tokenCache[indentity];
                }
            }
            return null;
        }

        public static IPrincipalToken CurrentPrincipal
        {
            get
            {
                if (Storage != null)
                {
                    return Storage.GetCurrentToken();
                }
                return FactoryServices.ANONY_PRINCIPAL_TOKEN;
            }
            set
            {
                if (Storage != null)
                {
                    Storage.SetCurrentToken(value);
                }
            }
        }

        /// <summary>
        /// 清除当前使用系统的身份，用户退出时调用
        /// </summary>
        public static void ClearCurrentToken()
        {
            if (Storage != null)
                Storage.ClearToken();
        }

    }
}
