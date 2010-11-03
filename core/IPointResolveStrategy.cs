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
    /// 用于根据上下文对象解析权限点的策略接口
    /// </summary>
    /// <author>vincent valenlee</author>
    public interface IPointResolveStrategy
    {
        /// <summary>
        /// 解析传入对象表示的当前运行状态的权限点
        /// 注意：此方法获取权限点期间需要自己处理异常，如果无法获取则返回null
        /// </summary>
        PermissionPoint Resolve(object context);
    }
}
