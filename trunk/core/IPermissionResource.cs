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
    /// 资源对象是权限点的清单，在这份清单中，他能够根据权限的名称和action解析权限信息
    /// </summary>
    /// <author>vincent valenlee</author>
    public interface IPermissionResource
    {
        string UniqueIdentifier
        {
            get;
            set;
        }

        /// <summary>
        /// 获取资源的配置
        /// </summary>
        IConfiguration[] Configurations
        {
            get;
        }

        /// <summary>
        /// 定义资源类的程序集
        /// </summary>
        string LoadedAssembly
        {
            get;
        }

        /// <summary>
        /// 是否支持指定的上下文
        /// </summary>
        bool Support(object context);

        /// <summary>
        /// 资源负责根据上下文解析出权限点
        /// </summary>
        PermissionPoint Resolve(object context);

    }
}
