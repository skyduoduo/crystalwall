﻿/*
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
using System.Reflection;

namespace CrystalWall
{
    /// <summary>
    /// 表示当前系统访问的一个权限点。其组成全部为字符串，方便系统进行配置。
    /// 包括：资源id、名称、操作
    /// </summary>
    /// <author>vincent valenlee</author>
    public abstract class PermissionPoint
    {
        private string name;

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string resource;

        public virtual string Resource
        {
            get { return resource; }
            set { resource = value; }
        }

        private string action;

        public virtual string Action
        {
            get { return action; }
            set { action = value; }
        }

        private object context;//定义权限点的对象

        public object Context
        {
            get { return context; }
            set { context = value; }
        }

        private MemberInfo member;//定义的字段、属性、方法、事件等

        public MemberInfo Member
        {
            get { return member; }
            set { member = value; }
        }

        //执行方法时的参数列表
        private object[] args;

        public object[] Args
        {
            get { return args; }
            set { args = value; }
        }

        public abstract PermissionInfo NewPermission();

        public static PermissionPoint EMPTY_PERMISSION_POINT = new EmptyPermissionPoint();
    }

    public class EmptyPermissionPoint : PermissionPoint
    {
        public override string Name
        {
            get { return ""; }
        }

        public override string Resource
        {
            get { return ""; }
        }

        public override string Action
        {
            get { return ""; }
        }

        public override PermissionInfo NewPermission()
        {
            return PermissionInfo.EMPTY_PERMISSIONINFO;
        }
    }
}
