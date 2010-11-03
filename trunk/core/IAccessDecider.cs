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
    /// 访问决定者，用于判断访问权限的核心接口
    /// </summary>
    /// <author>vincent valenlee</author>
    public interface IAccessDecider
    {
        /// <summary>
        /// 决定指定的身份是否能对指定资源进行访问
        /// </summary>
        /// <param name="principal">指定的身份令牌</param>
        /// <param name="cntext">请求访问的对象</param>
        /// <exception cref="AccessException">如果不允许访问资源则抛出</exception>
        void Decide(IPrincipalToken principal, object cntext);

    }
}
