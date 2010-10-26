using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrystalWall.Auths;

namespace CrystalWall
{
    /// <summary>
    /// 获取当前用户以及存储身份令牌提供者的持有器
    /// </summary>
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

        private static IList<PermissionInfo> anonyPrincipalPermission;

        public static IList<PermissionInfo> AnonyPrincipalPermission
        {
            get { return anonyPrincipalPermission; }
            set { anonyPrincipalPermission = value; }
        }

        //缓存（应该使用定时情况的缓存）
        private static IDictionary<string, IPrincipalToken> tokenCache = new Dictionary<string, IPrincipalToken>();

        /// <summary>
        /// 获取指定标识的令牌，他将遍历提供者列表，因此是一个耗时的操作
        /// </summary>
        public static IPrincipalToken GetPrincipal(string indentity) 
        {
            if (tokenCache[indentity] != null)
                return tokenCache[indentity];
            foreach (IPrincipalProvider provider in PrincipalProviders)
            {
                if (provider.HasPrincipal(indentity))
                {
                    tokenCache[indentity] = provider[indentity];
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
                return new AnonyPrincipalToken(AnonyPrincipalPermission);
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
