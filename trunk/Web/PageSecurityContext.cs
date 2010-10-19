using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Reflection;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace CrystalWall.Web
{

    //当前用户没有可见权限时执行的处理
    public delegate void HideControl(object sender, AccessExceptionEventArgs e);

    //决定控件访问的方法
    //public delegate void ControlDecideMethod(object sender, EventArgs args);

    /// <summary>
    /// 基于ASP.NET页面的权限控制上下文。构造此上下文时，page中的控件不一定加载完毕(应该在页面构造时构造此修饰性的上下文）
    /// 他能够读取Page页面上的PagePoint权限点。PagePoint权限点标注在页面的各个控件上，如：
    /// <code>
    /// public YourPage: Page {
    ///   [PermissionPoint(name="控件id" type="ControlPermissionPoint", action="visiable(可不标注，默认为不隐藏）,event:operation,event2:operation2..."]
    ///   private Button button;
    ///   .....
    /// }
    /// </code>
    /// 此权限上下文读取当前页面上（配置文件中资源id为页面路径）所有使用以上point标注的控件，将检查控件的是否隐藏，各种事件的action等解析为PagePermissionInfo，然后在控件的相应
    /// 渲染事件上添加一个事件，这个事件对应action中的原始事件，每次执行原始事件时，都将调用对应的PageControlDecider检测权限
    /// 注意：如果PermissionPoint定义的控件id是在其他容器控件的子控件，请使用$符号分隔，例如panel1$panel2$labelId
    /// </summary>
    public class PageSecurityContext : DefaultDecider
    {
        //当控件没有可见权限时触发的自定义事件处理
        public event HideControl HiddenControl;

        private Page page;

        private IList<PermissionPoint> points = new List<PermissionPoint>();//页面上各个控件上的权限点缓存


        //执行自定义隐藏控件的事件，事件参数中的check对象为需要隐藏的控件
        public void OnHideControl(AccessExceptionEventArgs e)
        {
            if (HiddenControl != null)
                HiddenControl(this, e);
        }

        public PageSecurityContext(Page page)
        {
            this.page = page;
            //principal = GetPrincipal(GetCurrentName());
            this.page.Error += new EventHandler(Page_Error);
            //页面的加载事件中，为控件指定的事件添加权限检查
            this.page.Init += new EventHandler(Page_Init);
            this.page.PreLoad += new EventHandler(Page_PreLoad);
        }

        /// <summary>
        /// 页面加载中添加事件哨兵进行权限检查
        /// </summary>
        public void Page_PreLoad(object sender, EventArgs e)
        {
            if (page.IsPostBack)
            {
                //回传时处理事件，找出控制事件的权限点
                try
                {
                    IEnumerable<PermissionPoint> eventPoint = points.Where(p =>
                    {
                        if (!p.Action.Equals(ControlPermissionInfo.VISIABLE_PERMISSION_NAME))
                            return true;
                        return false;
                    });
                    foreach (ControlPermissionPoint point in eventPoint)
                    {
                        Control c = FindControlInContainer(point.Name);
                        EventInfo eventInfo = c.GetType().GetEvent(point.EventName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        //加入权限检查事件
                        EventHandler deciderMethod = (s, ee) =>
                        {
                            Decide(PrincipalTokenHolder.CurrentPrincipal, new ControlEventContextObject(point.Name, this, c, point.EventName));
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
                catch (InvalidOperationException oe)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// 页面加载事件中检测具有visiable的权限点的控件，如果当前用户不具有此权限，将其visiable设置为false
        /// </summary>
        public void Page_Init(object sender, EventArgs e)
        {
            //页面的加载事件中，为控件指定的事件添加权限检查
            ReLoadPoints();
            try
            {
                IEnumerable<PermissionPoint> controlCheckView = points.Where(p =>
                {
                    if (p.Action.Equals(ControlPermissionInfo.VISIABLE_PERMISSION_NAME))
                        return true;
                    return false;
                });
                foreach (ControlPermissionPoint point in controlCheckView)
                {
                    Control c = FindControlInContainer(point.Name);
                    try
                    {
                        Decide(PrincipalTokenHolder.CurrentPrincipal, new ControlEventContextObject(point.Name, this, c, point.EventName));
                    }
                    catch (AccessException ae)
                    {
                        //当前用户没有控件的可见权限，则默认设置visiable为true
                        c.Visible = false;
                        OnHideControl(new AccessExceptionEventArgs(PrincipalTokenHolder.CurrentPrincipal, c));
                    }
                }
            }
            catch (InvalidOperationException oe)
            {
                return;
            }
        }

        void Page_Error(object sender, EventArgs e)
        {
            if (page.Server.GetLastError() is AccessException)
            {
                //执行抛出异常时的事件处理
                this.OnAccessException(PrincipalTokenHolder.CurrentPrincipal, (page.Server.GetLastError() as AccessException).CheckObject);
            }
        }

        /// <summary>
        /// 如果不存在控件，则首先加载
        /// </summary>
        internal PermissionPoint GetPoint(string controlId, string eventName)
        {
            try
            {
                PermissionPoint p = points.First(point =>
                {
                    ControlPermissionPoint cp = (ControlPermissionPoint)point;
                    if (cp.Name.Equals(controlId) && cp.EventName.Equals(eventName))
                    {
                        return true;
                    }
                    return false;
                });
                return p;
            }
            catch (InvalidOperationException e)
            {
                //确保加载
                ReLoadPoints();
            }
            return GetPointTwice(controlId, eventName);
        }

        private PermissionPoint GetPointTwice(string controlId, string eventName)
        {
            try
            {
                PermissionPoint p = points.First(point =>
                {
                    ControlPermissionPoint cp = (ControlPermissionPoint)point;
                    if (cp.Name.Equals(controlId) && cp.EventName.Equals(eventName))
                    {
                        return true;
                    }
                    return false;
                });
                return p;
            }
            catch (InvalidOperationException e)
            {
                return null;
            }
        }

        /// <summary>
        /// TODO:需要重构
        /// </summary>
        protected virtual void ReLoadPoints()
        {
            points.Clear();
            FieldInfo[] fields = page.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo info in fields)
            {
                if (info.FieldType.IsSubclassOf(typeof(Control)) && info.GetCustomAttributes(typeof(PermissionPointAttribute), false).Length != 0)
                {
                    //在控件字段上定义了PermissionPointAttribute元特性(目前只支持一个）
                    PermissionPointAttribute pattr = (PermissionPointAttribute)info.GetCustomAttributes(typeof(PermissionPointAttribute), false)[0];
                    if (pattr.Action != null)
                    {
                        string[] action;
                        if (pattr.Action.Contains(","))
                        {
                            //TODO:解析以逗号分隔的action列表
                            action = pattr.Action.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        }
                        else
                        {
                            //没有逗号，单独一个point action
                            action = new string[1];
                            action[0] = pattr.Action;
                        }
                        foreach (string act in action)
                        {
                            ControlPermissionPoint point;
                            if (pattr.Type != null)
                            {
                                Type t = Type.GetType(pattr.Type);
                                point = (ControlPermissionPoint)t.GetConstructor(new Type[0]).Invoke(new object[0]);
                            }
                            else
                            {
                                point = new ControlPermissionPoint();
                            }
                            point.Name = pattr.Name.Substring(pattr.Name.LastIndexOf('#') + 1);//控件id
                            point.Resource = page.Request.Path;//权限点资源对应所在页面的虚拟路径
                            point.Action = act;//定义的操作
                            point.Control = FindControlInContainer(pattr.Name);//运行时加入控件实例
                            points.Add(point);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取容器内的子控件
        /// </summary>
        /// <param name="controlIDInPointName">
        /// id的命名规范为使用$分隔。如果控件在母版页中，则uniqueID第一部分为最顶层的ContentPlaceHolder的id。
        /// 例如：最顶层母版页的ContentPlaceHolder-id$容器1id$容器2id$控件id
        /// </param>
        public Control FindControlInContainer(string controlIDInPointName)
        {
            MasterPage top = page.Master;
            if (top == null)
            {
                //没有使用母版页
                return FindControlInContainerWithNoMastPage(page, controlIDInPointName);
            }
            while (top.Master != null)
            {
                //找到最顶层模板页
                top = top.Master;
            }
            //具有母版页，解析第一部分$为ContentPlaceHolder
            int first = controlIDInPointName.IndexOf("$");
            Control container = top.FindControl(controlIDInPointName.Substring(0, first));
            return FindControlInContainerWithNoMastPage(container, controlIDInPointName.Substring(first + 1));
        }

        private Control FindControlInContainerWithNoMastPage(Control container, string uniqueID)
        {
            if (!uniqueID.Contains("$"))
                return container.FindControl(uniqueID);
            string[] ids = uniqueID.Split('$');
            Control parent = container.FindControl(ids[0]);
            if (parent != null)
            {
                for (int i = 1; i < ids.Length; i++)
                {
                    parent = parent.FindControl(ids[i]);
                }
            }
            return parent;
        }

        ///// <summary>
        ///// 只接受ControlEventContextObject对象
        ///// </summary>
        //public override PermissionPoint GetPoint(object context)
        //{
        //    ControlEventContextObject controlContext = context as ControlEventContextObject;
        //    if (controlContext == null)
        //        return PermissionPoint.EMPTY_PERMISSION_POINT;//传入的对象不是ControlEventContextObject
        //    else
        //    {
        //        PermissionPoint p = this.GetPoint(controlContext.fullPath, controlContext.eventName);
        //        return p;
        //    }
        //}

    }


    /// <summary>
    /// 用于封装当前正在执行某控件上某事件的上下文对象
    /// </summary>
    internal class ControlEventContextObject: IPermissionPointProvider
    {
        internal Control control;

        internal string eventName;

        internal string fullPath;//完整路径

        private PageSecurityContext context;

        internal ControlEventContextObject(string fullPath, PageSecurityContext context, Control control, string eventName)
        {
            this.fullPath = fullPath;
            this.control = control;
            this.eventName = eventName;
            this.context = context;
        }

        public PermissionPoint[] GetPoint()
        {
            return new PermissionPoint[] { context.GetPoint(fullPath, eventName) };
        }
    }

}
