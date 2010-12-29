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
    /// 用于特殊权限进行在集合中的权限判断，某些权限在权限集合环境下又特殊的包含行为，则通过此访问者接口扩展
    /// </summary>
    public interface IContainsVisitor
    {
        /// <summary>
        /// 访问集合中的权限
        /// </summary>
        /// <param name="contain">集合中的权限</param>
        /// <param name="contained">需要判断包含的权限</param>
        void Visit(PermissionInfo contain, PermissionInfo contained);

        /// <summary>
        /// 最后包含的结果
        /// </summary>
        bool Result
        {
            get;
        }
    }
}
