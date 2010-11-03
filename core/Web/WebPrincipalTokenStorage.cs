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
using System.Web;

namespace CrystalWall.Web
{
    /// <summary>
    /// ASP.NET Web应用程序的身份令牌持久存储实现。他在当前web会员中的__CURRENT_USER_KEY__中存储当前的身份令牌对象
    /// </summary>
    /// <author>vincent valenlee</author>
    public class WebPrincipalTokenStorage: IPrincipalTokenStorage
    {
        //[ThreadStatic]
        //private static IPrincipalToken CurrentToken;

        public const string __CURRENT_USER_KEY__ ="__CURRENT_USER_KEY__";

        public void SetCurrentToken(IPrincipalToken token)
        {
            if (token != null)
                HttpContext.Current.Session.Add(WebPrincipalTokenStorage.__CURRENT_USER_KEY__, token);
        }

        public IPrincipalToken GetCurrentToken()
        {
            return (IPrincipalToken)HttpContext.Current.Session[WebPrincipalTokenStorage.__CURRENT_USER_KEY__];
        }

        public void ClearToken()
        {
            HttpContext.Current.Session.Remove(WebPrincipalTokenStorage.__CURRENT_USER_KEY__);
        }
    }
}
