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
    /// 标识访问资源的一个身份
    /// </summary>
    /// <author>vincent valenlee</author>
    public interface IPrincipalToken
    {
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 身份凭证
        /// </summary>
        object Certificate
        {
            get;
            set;
        }

        /// <summary>
        /// 身份的其他信息，如邮件地址等
        /// </summary>
        object Information
        {
            get;
            set;
        }

        /// <summary>
        /// 获取身份的授权集
        /// </summary>
        PermissionInfoCollection GetGrandedPermission();

    }
}
