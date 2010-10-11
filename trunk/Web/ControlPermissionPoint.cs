using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace QJSoft.ECBC.Authority.Web
{
    /// <summary>
    /// 定义页面中控件的权限点.
    /// <code>
    /// name--控件id
    /// resource--控件所在页面路径
    /// action--visiable或者event:operation
    /// </code>
    /// 注意：此权限点在运行时获得相关的控件对象，这个对象提供子类可以扩展的NewPermissionByControl方法，
    /// 以便根据各种不同的web控件的特殊性质获取不同的权限信息对象
    /// </summary>
    public class ControlPermissionPoint: PermissionPoint
    {
        private Control control;

        public Control Control
        {
            get { return control; }
            set { control = value; }
        }

        public string EventName
        {
            get
            {
                if (!this.Action.Contains(":"))
                {
                    return this.Action;
                }
                else
                {
                    return this.Action.Split(':')[0];
                }
            }
        }

       public override PermissionInfo NewPermission()
        {
            return NewPermissionByControl(control);
        }

        /// <summary>
        /// 子类可以根据各种不同的控件的设置返回不同权限信息对象，默认返回ControlPermissionInfo
        /// </summary>
       protected virtual ControlPermissionInfo NewPermissionByControl(Control control)
       {
           string permissionName = this.Resource + "#" + this.Name;
           if ("visiable".Equals(this.Action, StringComparison.OrdinalIgnoreCase))
               return ControlPermissionInfo.GetVisiablePermissionInfo(permissionName);
           return new ControlPermissionInfo(permissionName, this.Action);
       }
    }
}
