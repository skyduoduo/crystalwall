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
