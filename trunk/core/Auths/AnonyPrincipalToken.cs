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

namespace CrystalWall.Auths
{
    /// <summary>
    /// 表示一个匿名身份
    /// </summary>
    /// <author>vincent valenlee</author>
    public class AnonyPrincipalToken : AbstractPricipalToken
    {
        public AnonyPrincipalToken(IList<PermissionInfo> permissions)
            : base("AnonyUser", permissions)
        {
        }

        private AnonyPrincipalToken(string name, IPrincipalProvider provider)
            : base(name, provider)
        {
        }

        public override object Certificate
        {
            get
            {
                return "";
            }
            set
            {
                
            }
        }

        public override object Information
        {
            get
            {
                return "匿名用户";
            }
            set
            {
            }
        }
    }
}
