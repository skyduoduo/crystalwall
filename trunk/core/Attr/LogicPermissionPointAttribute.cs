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
using CrystalWall.Permissions;

namespace CrystalWall.Attr
{
    /// <summary>
    /// 定义进行逻辑And或者or操作的权限点元特性。他禁止定义type，而改成定义leftType，rightType
    /// </summary>
    public class LogicPermissionPointAttribute: PermissionPointAttribute
    {
        public override string Type
        {
            get
            {
                return null;
            }
        }

        private string leftType;

        public string LeftType
        {
            get { return leftType; }
            set { leftType = value; }
        }

        private string rightType;

        public string RightType
        {
            get { return rightType; }
            set { rightType = value; }
        }

        private string rightName;

        public string RightName
        {
            get { return rightName; }
            set { rightName = value; }
        }

        private string rightAction;

        public string RightAction
        {
            get { return rightAction; }
            set { rightAction = value; }
        }

        //private string rightResource;

        //public string RightResource
        //{
        //    get { return rightResource; }
        //    set { rightResource = value; }
        //}

        private LogicPoint logic = LogicPoint.AND;

        public LogicPoint Logic
        {
            get { return logic; }
            set { logic = value; }
        }

        protected override PermissionPoint InternalPoint() 
        {
            LogicPermissionPoint p = new LogicPermissionPoint();
            p.LeftType = this.LeftType;
            p.RightType = this.RightType;
            p.RightName = this.RightName;
            p.RightAction = this.RightAction;
            p.LogicPoint = this.Logic;
            return p;
        }
    }
}
