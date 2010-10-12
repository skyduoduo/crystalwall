using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodePlex.CrystalWall
{
    /// <summary>
    /// 表示当前系统访问的一个权限点。其组成全部为字符串，方便系统进行配置。
    /// 包括：资源id、名称、操作
    /// </summary>
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

        public abstract PermissionInfo NewPermission();

        public static PermissionPoint EMPTY_PERMISSION_POINT = new EmptyPermissionPoint();
    }

    public class EmptyPermissionPoint : PermissionPoint
    {
        public virtual string Name
        {
            get { return ""; }
        }

        public virtual string Resource
        {
            get { return ""; }
        }

        public virtual string Action
        {
            get { return ""; }
        }

        public override PermissionInfo NewPermission()
        {
            return PermissionInfo.EMPTY_PERMISSIONINFO;
        }
    }
}
