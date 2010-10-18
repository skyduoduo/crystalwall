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
