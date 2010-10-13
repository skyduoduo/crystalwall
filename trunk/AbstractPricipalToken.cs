using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrystalWall
{
    /// <summary>
    /// 抽象身份令牌的实现，他使用一个授权提供者获取授权的信息
    /// </summary>
    public abstract class AbstractPricipalToken: IPrincipalToken
    {
        private PermissionInfoCollection permissions;

        private IPrincipalProvider provider;

        public IPrincipalProvider Provider
        {
            get { return provider; }
        }

        private string name;

        /// <summary>
        /// 直接使用指定的权限构造身份令牌
        /// </summary>
        public AbstractPricipalToken(string name, IList<PermissionInfo> permissions)
        {
            this.name = name;
            if (permissions == null || permissions.Count == 0)
            {
                this.permissions = PermissionInfoCollection.EMPTY_PERMISSIONINFO_COLLECTION;
            }
            else
            {
                if (permissions == null)
                {
                    this.permissions = new PermissionInfoCollection();
                }
                foreach (PermissionInfo p in permissions)
                {
                    this.permissions.Add(p);
                }
            }
        }

        public AbstractPricipalToken(string name, IPrincipalProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException("principal's provider must not be null");
            this.name = name;
            this.provider = provider;
        }

        public  string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public abstract object Certificate
        {
            get;
            set;
        }

        public abstract object Information
        {
            get;
            set;
        }

        public PermissionInfoCollection GetGrandedPermission()
        {
            if (permissions != null)
                return (PermissionInfoCollection)permissions.Clone();
            return provider.GetPermissions(this.name);
        }

        public override bool Equals(object o)
        {
            if (o is IPrincipalToken)
            {
                IPrincipalToken p = (IPrincipalToken)o;
                if (this.name.Equals(p.Name) && this.Certificate.Equals(p.Certificate))
                    return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.name.GetHashCode() ^ this.Certificate.GetHashCode();
        }
    }
}
