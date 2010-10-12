using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QJSoft.ECBC.Authority.Web
{
    /// <summary>
    /// 用于web控件的权限信息对象
    /// name的格式为：/.../.../xx.aspx#控件id
    /// action为:   事件名:操作名
    /// 注意：在默认情况下，当name与action全部相等时权限才互相包含，
    /// 但子类可以实现ContainsIfNotEquals方法用于特殊的权限包含关系，
    /// 例如：action为edit编辑权限包含action为view的查看权限等
    /// 注意：此控件权限包含一个特殊的visiable控件可见性权限，他的action为单词visiable。
    /// 可见性权限只能通过静态方法GetVisiablePermissionInfo获得，不能通过直接构造获得！
    /// </summary>
    public class ControlPermissionInfo: PermissionInfo
    {

        public static readonly string VISIABLE_PERMISSION_NAME = "visiable";
       
        //页面路径
        private string page;

        public string Page
        {
            get { return page; }
        }

        //控件id
        private string control;

        public string Control
        {
            get { return control; }
        }

        private string eventName;

        public string EventName
        {
            get { return eventName; }
        }

        //操作名
        private string operation;

        public string Operation
        {
            get { return operation; }
        }


        /// <param name="name">必须为     /.../.../xx.aspx#控件id   格式</param>
        /// <param name="action">必须为   事件名:操作名   格式或者为单词"visiable"</param>
        /// <exception cref="ArgumentException">如果name与action不符合格式将抛出参与异常</exception>
        public ControlPermissionInfo(string name, string action)
            : base(name, action)
        {
            if (name == null || action == null)
                throw new ArgumentNullException("the name or action must not be null");
            if (name.LastIndexOf("#") == -1)
                throw new ArgumentException("name must have '#' char to split page path and control id");
            if (action.LastIndexOf(":") == -1)
                throw new ArgumentException("action must have ':' char to split event and operation");
            Decompose();
        }

        /// <summary>
        /// 分解权限名与action
        /// </summary>
        private void Decompose()
        {
            page = Name.Substring(0, Name.LastIndexOf("#"));
            control = Name.Substring(Name.LastIndexOf("#") + 1);
            if (action.Equals("visiable"))
            {
                eventName = "visiable";
                operation = "visiable";
            }
            else
            {
                eventName = action.Split(':')[0];
                operation = action.Split(':')[1];
            }
        }

        private ControlPermissionInfo(string name)
            : base(name)
        {
        }

        /// <summary>
        /// 获取控件可见性权限。只能通过此方法获取visiable控件可见性权限信息对象
        /// </summary>
        public static ControlPermissionInfo GetVisiablePermissionInfo(string name)
        {
            ControlPermissionInfo p =  new ControlPermissionInfo(name);
            p.action = VISIABLE_PERMISSION_NAME;
            p.Decompose();
            return p;
        }

        public override bool Contains(PermissionInfo permission)
        {
            if (!(permission is ControlPermissionInfo))
                return false;
            if (this.Equals(permission))
                return true;
            return ContainsIfNotEquals(permission);
        }

        /// <summary>
        /// 子类可以实现此方法以便根据特殊控件的设置判断在名称与action都不相等的情形下是否包含指定权限。默认返回false
        /// </summary>
        protected virtual bool ContainsIfNotEquals(PermissionInfo permission)
        {
            return false;
        }
    }
}
