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
using CrystalWall;

namespace Crystalwall.Test
{
    /// <summary>
    /// 仅用于测试的当前身份令牌的存储器实现
    /// </summary>
    /// <author>vincent valenlee</author>
    public class TestPrincipalTokenStorage: IPrincipalTokenStorage
    {
        private IDictionary<string, IPrincipalToken> principals = new Dictionary<string, IPrincipalToken>();

        public const string CURRENT_KEY = "__current_principal_key__";

        public void SetCurrentToken(IPrincipalToken token)
        {
            if (principals.ContainsKey(CURRENT_KEY))
            {
                principals[CURRENT_KEY] = token;
                return;
            }
            if (token != null)
            {
                principals.Add(CURRENT_KEY, token);
            }
            else
                principals.Add(CURRENT_KEY, FactoryServices.ANONY_PRINCIPAL_TOKEN);
        }

        public IPrincipalToken GetCurrentToken()
        {
            if (principals.ContainsKey(CURRENT_KEY))
                return principals[CURRENT_KEY];
            return null;
        }

        public void ClearToken()
        {
            if (principals.ContainsKey(CURRENT_KEY))
                principals.Remove(CURRENT_KEY);
        }
    }
}
