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
using System.Collections;

namespace CrystalWall.Permissions
{
    /// <summary>
    /// 此权限只是一个权限集合，本省并不代表其他含义！其只是内部使用，不要在外部通过反射构造此类。
    /// 因为他不实现接收name和action的构造函数！
    /// TODO:将来补充接收name和action的构造函数！
    /// </summary>
    public class LogicPermissionInfo: VisitablePermissionInfo
    {
        public LogicPoint LogicPoint = LogicPoint.AND;

        private PermissionInfo left;

        public PermissionInfo Left
        {
            get { return left; }
        }

        private PermissionInfo right;

        public PermissionInfo Right
        {
            get { return right; }
        }

        public LogicPermissionInfo(PermissionInfo left, PermissionInfo right) 
        {
            this.left = left;
            this.right = right;
        }

        public override bool Contains(PermissionInfo permission)
        {
            if (permission == null)
                return false;
            switch(LogicPoint)
            {
                case LogicPoint.AND:
                default:
                    return left.Contains(permission) && right.Contains(permission);
                case LogicPoint.OR:
                    return left.Contains(permission) || right.Contains(permission);
            }
        }

        public override IContainsVisitor GetVisitor(PermissionInfoCollection pc)
        {
            if (LogicPoint == LogicPoint.AND)
                return new AndVisitor(this, pc);
            else
                return new OrVisitor(this, pc);
        }

        public class AndVisitor : IContainsVisitor
        {
            public static BitArray TRUE = new BitArray(new bool[] { true, true });

            private LogicPermissionInfo lp;

            private BitArray bits;

            private PermissionInfoCollection pc;

            public AndVisitor(LogicPermissionInfo lp, PermissionInfoCollection pc)
            {
                this.lp = lp;
                this.pc = pc;
                bits = new BitArray(2);
            }

            public void Visit(PermissionInfo contain, PermissionInfo contained)
            {
                int index = pc.Index(contain);
                if (index != -1)
                {
                    if (contain.Contains(lp.Left))
                        bits.Set(0, true);//设置左权限
                    else if (contain.Contains(lp.Right))
                        bits.Set(1, true);//设置右权限
                }
            }

            public bool Result
            {
                get 
                {
                    BitArray and = bits.And(TRUE);
                    int[] b = new int[and.Length];
                    and.CopyTo(b, 0);
                    if (b[0] == 3)
                        return true;
                    return false;
                }
            }
        }

        public class OrVisitor : IContainsVisitor
        {
            //public static BitArray TRUE = new BitArray(new bool[] { false, false });

            public bool success = false;

            private LogicPermissionInfo lp;

            private BitArray bits;

            private PermissionInfoCollection pc;

            public OrVisitor(LogicPermissionInfo lp, PermissionInfoCollection pc)
            {
                this.lp = lp;
                this.pc = pc;
                bits = new BitArray(2);
            }

            public void Visit(PermissionInfo contain, PermissionInfo contained)
            {
                int index = pc.Index(contain);
                if (index != -1)
                {
                    if (contain.Contains(lp.Left) || contain.Contains(lp.Right))
                    {
                        success = true;
                        return;
                    }
                }
            }

            public bool Result
            {
                get
                {
                    return success;
                }
            }
        }

    }

    public class LogicPermissionPoint : PermissionPoint
    {
        private string leftType = null;

        public string LeftType
        {
            get { return leftType; }
            set { leftType = value; }
        }

        private string rightType = null;

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

        private LogicPoint logicPoint = LogicPoint.AND;

        public LogicPoint LogicPoint
        {
            get { return logicPoint; }
            set { logicPoint = value; }
        }


        public override PermissionInfo NewPermission()
        {
            LogicPermissionInfo p = new LogicPermissionInfo(BuildLeft(), BuildRight());
            p.LogicPoint = this.LogicPoint;
            return p;
        }

        private PermissionInfo BuildLeft()
        {
            if (LeftType == null || !typeof(PermissionInfo).IsAssignableFrom(Type.GetType(leftType)))
            {
                return new DefaultPermissionInfo(this.Name, this.Action);
            }
            else
            {
                return (PermissionInfo)Type.GetType(leftType).GetConstructor(new System.Type[2] {typeof(string), typeof(string)}).Invoke(new object[2] { this.Name, this.Action});
            }
        }

        private PermissionInfo BuildRight()
        {
            if (RightType == null || !typeof(PermissionInfo).IsAssignableFrom(Type.GetType(rightType)))
            {
                return new DefaultPermissionInfo(this.rightName, this.rightAction);
            }
            else
            {
                return (PermissionInfo)Type.GetType(leftType).GetConstructor(new System.Type[2] { typeof(string), typeof(string) }).Invoke(new object[2] { this.rightName, this.rightAction });
            }
        }

    }

    public enum LogicPoint
    {
        AND = 1,
        OR = 0
    }



}
