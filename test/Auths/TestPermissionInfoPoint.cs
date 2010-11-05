using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrystalWall;

namespace Crystalwall.Test.Auths
{
    /// <summary>
    /// 用于测试的返回TestPermissionInfo的权限点定义类
    /// </summary>
    public class TestPermissionInfoPoint: PermissionPoint
    {
        public override PermissionInfo NewPermission()
        {
            return new TestPermissionInfo(Name, Action);
        }
    }
}
