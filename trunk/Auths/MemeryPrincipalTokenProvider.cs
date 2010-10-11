using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QJSoft.ECBC.Authority.Auths
{
    /// <summary>
    /// 使用内存存储身份令牌的提供者，用于测试的目的
    /// </summary>
    public class MemeryPrincipalTokenProvider: IPrincipalProvider
    {
        private IDictionary<string, IPrincipalToken> principals = new Dictionary<string, IPrincipalToken>();

        public IDictionary<string, IPrincipalToken> Principals
        {
            set { principals = value; }
        }

        public IPrincipalToken GeneratePrincipalToken(string name, string password, string information)
        {
            if (HasPrincipal(name))
                return principals[name];
            UserPasswordPrincipalToken token = new UserPasswordPrincipalToken(name, password, this);
            token.Information = information;
            principals.Add(name, token);
            return token;
        }

        private IDictionary<IPrincipalToken, PermissionInfoCollection> permissions = new Dictionary<IPrincipalToken, PermissionInfoCollection>();

        public void AddPermission(string principal, PermissionInfo permission)
        {
            if (HasPrincipal(principal))
            {
                if (!permissions.Keys.Contains(this[principal]))
                    permissions.Add(this[principal], new PermissionInfoCollection());
                permissions[this[principal]].Add(permission);
            }
            else
                throw new ArgumentException("you must add " + principal + " user!");
        }

        public bool HasPrincipal(string name)
        {
            return principals.Keys.Contains(name);
        }

        public IPrincipalToken this[string name]
        {
            get { return principals[name]; }
        }

        public PermissionInfoCollection GetPermissions(string name)
        {
            if (HasPrincipal(name))
                return permissions[this[name]];
            else
                return PermissionInfoCollection.EMPTY_PERMISSIONINFO_COLLECTION;
        }
    }
}
