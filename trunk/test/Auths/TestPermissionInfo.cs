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
using CrystalWall;

namespace Crystalwall.Test.Auths
{
    /// <summary>
    /// 仅用于测试的权限信息类
    /// </summary>
    /// <author>vincent valenlee</author>
    public class TestPermissionInfo: PermissionInfo
    {
        public TestPermissionInfo(string name, string action)
            : base(name, action)
        {
        }

        public override bool Contains(PermissionInfo permission)
        {
            TestPermissionInfo tp = permission as TestPermissionInfo;
            if (tp == null)
                return false;
            if (!CompareName(tp))
            {
                return false;
            }
            else
            {
                return CompareAction(tp);
            }
        }

        private bool CompareName(TestPermissionInfo permission)
        {
            if (this.Name == null)
                return permission.Name == null;
            else
                return this.Name.Equals(permission.Name);
        }

        private bool CompareAction(TestPermissionInfo permission)
        {
            if (this.Action == null)
                return permission.Action == null;
            else
                return this.Action.Equals(permission.Action);
        }

        protected override int ResolveAction(string action)
        {
            return 0;
        }
    }
}
