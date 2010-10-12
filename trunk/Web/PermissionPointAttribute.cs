using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodePlex.CrystalWall.Web
{
    /// <summary>
    /// 用于定义权限点的元特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class PermissionPointAttribute: Attribute
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
