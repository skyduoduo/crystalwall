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
            return false;
        }
    }
}
