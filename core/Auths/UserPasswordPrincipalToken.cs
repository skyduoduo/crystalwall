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
    /// 使用用户名和密码的身份令牌实现
    /// </summary>
    /// <author>vincent valenlee</author>
    public class UserPasswordPrincipalToken : AbstractPricipalToken
    {

        private string password;

        private object information;

        public override object Certificate
        {
            get
            {
                return password;
            }
            set
            {
                password = value.ToString();
            }
        }

        public override object Information
        {
            get
            {
                return information;
            }
            set
            {
                information = value;
            }
        }

         /// <summary>
        /// 直接使用指定的权限构造身份令牌
        /// </summary>
        public UserPasswordPrincipalToken(string name, string password, IList<PermissionInfo> permissions): base(name, permissions)
        {
            this.password = password;
        }

        public UserPasswordPrincipalToken(string name, string password, IPrincipalProvider provider): base(name, provider)
        {
            this.password = password;
        }
    }
}
