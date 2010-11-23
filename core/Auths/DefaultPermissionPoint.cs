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
using System.Diagnostics;

namespace CrystalWall.Auths
{
    /// <summary>
    /// 权限点的默认实现，当元特性中没有指定type时，将使用此权限点类型，此权限点的type字符串必须为
    /// PermissionInfo类的全名，他直接使用type字符指定的类型并根据name和action构造权限信息对象
    /// </summary>
    /// <author>vincent valenlee</author>
    public class DefaultPermissionPoint: PermissionPoint
    {
        private Type permissionType;

        private PermissionInfo cached;

        public DefaultPermissionPoint(Type permissionType)
            : base()
        {
            Debug.Assert(permissionType != null, "类型不能为空");
            if (!typeof(PermissionInfo).IsAssignableFrom(permissionType))
            {
                throw new ApplicationException("构造默认权限点时，权限类型参数必须为PermissionInfo或其子类全名");
            }
            this.permissionType = permissionType;
        }

        public override PermissionInfo NewPermission()
        {
            if (cached == null)
                cached = (PermissionInfo)permissionType.GetConstructor(new Type[2]).Invoke(new string[2] {this.Name, this.Action});
            return cached;
        }
    }
}
