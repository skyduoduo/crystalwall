using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using CrystalWall.Utils;
using System.Reflection;
using System.ComponentModel;

namespace CrystalWall.Web
{
    /// <summary>
    /// 用于Asp.NET页面的权限决定者，他检查的对象为Page，决定方法在page页面上安装
    /// AspPageControlEventDecider和AspPageControlViewDecider。此决定着是ASPNetPageCrystalWallSite
    /// 默认安装的决定者。但在配置文件中可以修改（但crystalwall不建议这么做，因为修改后的decider将在页面显示前立即执行）
    /// </summary>
    public class AspPageDecider : DefaultDecider
    {
        private AspPageControlEventDecider eventDecider;

        private AspPageControlViewDecider viewDecider;

        public AspPageDecider(AspPageControlEventDecider eventDecider, AspPageControlViewDecider viewDecider)
        {
            this.eventDecider = eventDecider;
            this.viewDecider = viewDecider;
        }

        public override void Decide(IPrincipalToken principal, object check)
        {
            viewDecider.Decide(principal, check);
            eventDecider.Decide(principal, check);
        }

    }

    public class AspPageControlEventDecider : DefaultDecider
    {
        private ASPNetPageCrystalWallSite site;

        public AspPageControlEventDecider(ASPNetPageCrystalWallSite site)
        {
            this.site = site;
        }

        /// <summary>
        /// 在页面的指定事件中添加事件哨兵进行权限检查
        /// </summary>
        public override void Decide(IPrincipalToken principal, object check)
        {
            Page page = check as Page;
            if (page == null)
                return;
            page.PreLoad += (sender, e) =>
            {
                try
                {
                    IEnumerable<PermissionPoint> eventPoint = site.GetPoints(page, p =>
                    {
                        if (!p.Action.Equals(ControlPermissionInfo.VISIABLE_PERMISSION_NAME))
                            return true;
                        return false;
                    });
                    foreach (ControlPermissionPoint point in eventPoint)
                    {
                        Control c = ASPNetPageCrystalWallSite.FindControlInContainer(page, point.Name);
                        EventInfo eventInfo = c.GetType().GetEvent(point.EventName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        //加入权限检查事件
                        EventHandler deciderMethod = (s, ee) =>
                        {
                            base.Decide(principal, new ControlEventContextObject(point.Name, point, c, point.EventName));
                        };
                        //无法动态创建委托！
                        //Delegate d = Delegate.CreateDelegate(eventInfo.EventHandlerType, deciderMethod.Method);
                        //获取控件中的指定事件对象。无法获取，.NET反射行为非常不一致！(只能通过Events列表属性获取)
                        //Delegate eventObject = (Delegate)eventInfo.DeclaringType.GetField(eventInfo.Name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).GetValue(c);
                        EventHandlerList eventHandlerList = (EventHandlerList)c.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).GetValue(c, null);
                        object eventkey = c.GetType().GetField("Event" + eventInfo.Name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).GetValue(c);
                        Delegate eventObject = eventHandlerList[eventkey];
                        if (eventObject == null || eventObject.GetInvocationList() == null || eventObject.GetInvocationList().Length == 0)
                        {
                            //eventInfo.AddEventHandler(c, d);
                            eventInfo.AddEventHandler(c, deciderMethod);
                        }
                        else
                        {
                            //将原有列表存储,然后插入权限检查事件为第一个执行的事件
                            foreach (Delegate de in eventObject.GetInvocationList())
                            {
                                eventInfo.RemoveEventHandler(c, de);
                            }
                            //eventInfo.AddEventHandler(c, d);
                            eventInfo.AddEventHandler(c, deciderMethod);
                            eventInfo.AddEventHandler(c, Delegate.Combine(eventObject.GetInvocationList()));
                        }
                    }
                }
                catch
                {
                    ServiceManager.LoggingService.Error("检查页面：" + page.Request.Url + "中事件的权限时出错");
                }
            };
        }

    }

    /// <summary>
    /// 用于控制控件中指定控件可见权限的决定者
    /// </summary>
    public class AspPageControlViewDecider : DefaultDecider
    {
        private ASPNetPageCrystalWallSite site;

        public AspPageControlViewDecider(ASPNetPageCrystalWallSite site)
        {
            this.site = site;
            this.AccessDenyed += HideControl;
        }

        //隐藏无可见权限的控件
        private void HideControl(object sender, AccessExceptionEventArgs args)
        {
            ControlEventContextObject check = args.Check as ControlEventContextObject;
            check.control.Visible = false;
        }

        /// <summary>
        /// 添加init事件， 页面加载事件中检测具有visiable的权限点的控件，如果当前用户不具有此权限，将其visiable设置为false
        /// </summary>
        /// <param name="check">必须为Page对象</param>
        public override void Decide(IPrincipalToken principal, object check)
        {
            Page page = check as Page;
            if (page == null)
                return;
            page.Init += (sender, e) =>
            {
                try
                {
                    //查找权限点中具有可见权限的权限点
                    IEnumerable<PermissionPoint> controlCheckView = site.GetPoints(page, p =>
                    {
                        if (p.Action.Equals(ControlPermissionInfo.VISIABLE_PERMISSION_NAME))
                            return true;
                        return false;
                    });
                    foreach (ControlPermissionPoint point in controlCheckView)
                    {
                        Control c = ASPNetPageCrystalWallSite.FindControlInContainer(page, point.Name);
                        try
                        {
                            base.Decide(principal, new ControlEventContextObject(point.Name, point, c, point.EventName));
                        }
                        catch (AccessException ae)
                        {
                            //DO NOTHING
                            ServiceManager.LoggingService.Debug("当前用户对页面：" + page.Request.Url + "中控件：" + c.ID + "没有可见权限");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ServiceManager.LoggingService.Error("检查页面：" + page.Request.Url + "中对象的可见权限时出错");
                }
            };
        }
    }

    internal class ControlEventContextObject : IPermissionPointProvider
    {
        internal Control control;

        internal string eventName;

        internal string fullPath;//完整路径

        internal ControlPermissionPoint point;

        internal ControlEventContextObject(string fullPath, ControlPermissionPoint point, Control control, string eventName)
        {
            this.fullPath = fullPath;
            this.control = control;
            this.eventName = eventName;
            this.point = point;
        }

        public PermissionPoint[] GetPoint()
        {
            return new PermissionPoint[] { point };
        }
    }
}
