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

namespace CrystalWall
{
    /// <summary>
    /// 抽象身份令牌的实现，他使用一个授权提供者获取授权的信息
    /// </summary>
    /// <author>vincent valenlee</author>
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
            else
            {
                PermissionInfoCollection p = provider.GetPermissions(this.name);
                if ( p != null)//非空则缓存
                    permissions = p;
                else
                    permissions = PermissionInfoCollection.EMPTY_PERMISSIONINFO_COLLECTION;
                return permissions;
            }

        }

        public override bool Equals(object o)
        {
            if (this == o)
                return true;
            if (o is IPrincipalToken)
            {
                IPrincipalToken p = (IPrincipalToken)o;
                if (!this.name.Equals(p.Name) || !this.Certificate.Equals(p.Certificate))
                    return false;
                if (this.GetGrandedPermission() != null)
                {
                    if (this.GetGrandedPermission().Equals(p.GetGrandedPermission()))
                        return true;
                }
                else
                    return p.GetGrandedPermission() == null;    
            }
            return false;
        }

        public override int GetHashCode()
        {
            if (GetGrandedPermission() != null)
                return this.name.GetHashCode() ^ this.Certificate.GetHashCode() ^ GetGrandedPermission().GetHashCode();
            return this.name.GetHashCode() ^ this.Certificate.GetHashCode();
        }
    }
}
