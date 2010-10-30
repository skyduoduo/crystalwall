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
    }
}
