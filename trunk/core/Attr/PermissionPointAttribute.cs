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
    /// 用于定义权限点的元特性，他可以用在类中的域及方法之上：
    /// <code>
    /// public class YouClass {
    ///   [PermissionPoint(type="权限点全限定名称", resource="控制的资源id", name="权限名"， action="执行方法或类域代表的动作")]
    ///   private Button button;
    /// }
    /// </code>
    /// </summary>
    /// <author>vincent valenlee</author>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public class PermissionPointAttribute : Attribute
    {
        //将创建的PermissionInfo的类型
        private string type;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        //资源id
        private string resource;

        public string Resource
        {
            get { return resource; }
            set { resource = value; }
        }

        private string action;

        public string Action
        {
            get { return action; }
            set { action = value; }
        }


    }
}
