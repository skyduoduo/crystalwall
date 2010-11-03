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
    /// 用于存储当前身份令牌的存储对象
    /// </summary>
    /// <author>vincent valenlee</author>
    public interface IPrincipalTokenStorage
    {
        /// <summary>
        /// 设置当前使用系统的身份令牌,如果令牌为null，则设置当前用户为匿名用户令牌
        /// </summary>
        void SetCurrentToken(IPrincipalToken token);

        /// <summary>
        /// 获取当前使用系统的身份令牌，如果当前没有设置令牌，则返回null
        /// </summary>
        IPrincipalToken GetCurrentToken();

        /// <summary>
        /// 清除当前使用系统的身份令牌（用户退出系统时调用）
        /// </summary>
        void ClearToken();
    }
}
