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

namespace CrystalWall.Permissions
{
    /// <summary>
    /// 所有属性必须完全相等的权限信息默认实现
    /// </summary>
    public class DefaultPermissionInfo: PermissionInfo
    {
        public DefaultPermissionInfo(string name, string action)
            : base(name, action)
        {
        }

        public override bool Contains(PermissionInfo permission)
        {
            return this.Name == permission.Name && this.Action == permission.Action && this.GetType() == permission.GetType();
        }
    }
}
