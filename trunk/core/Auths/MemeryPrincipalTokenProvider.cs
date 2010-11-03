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

namespace  CrystalWall.Auths
{
    /// <summary>
    /// 使用内存存储身份令牌的提供者，用于测试的目的
    /// </summary>
    /// <author>vincent valenlee</author>
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


        public void InitData(System.Xml.XmlNode element, string attribute, object data)
        {
           //DO NOTHING
        }
    }
}
