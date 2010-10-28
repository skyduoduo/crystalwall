using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CodePlex.CrystalWall
{
    public delegate void MyHandler(object sender, AccessExceptionEventArgs args);

    public class EventTest
    {
        public static void Main()
        {
            //MyEventObject et = new MyEventObject();
            //et.Hander += new EventHandler(et_hander);
            //EventInfo eventInfo = et.GetType().GetEvent("Hander", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            //Console.WriteLine("事件添加方法：" + eventInfo.Name);
            //EventHandler m = (s, e) =>
            //{
            //    Console.WriteLine("事件添加方法");
            //};
            //Delegate d = Delegate.CreateDelegate(eventInfo.EventHandlerType, m.Method);
            //Delegate eventObject = (Delegate)eventInfo.DeclaringType.GetField(eventInfo.Name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).GetValue(et);
            //Delegate[] events = eventObject.GetInvocationList();
            //Console.WriteLine("事件调用列表中的数目：" + events.Length);
            //foreach (Delegate de in events)
            //{
            //    eventInfo.RemoveEventHandler(et, de);
            //}
            ////MethodInfo method = eventInfo.GetRemoveMethod();
            ////foreach (Delegate de in events)
            ////{
            ////    method.Invoke(et, new object[] { (EventHandler)de  });
            ////}
            //eventObject = (Delegate)eventInfo.DeclaringType.GetField(eventInfo.Name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).GetValue(et);
            //if (eventObject != null && eventObject.GetInvocationList() != null)
            //{
            //    Console.WriteLine("删除后事件调用列表中的数目：" + eventObject.GetInvocationList().Length);
            //}
            //else
            //{
            //    Console.WriteLine("删除后事件调用列表中的数目：" + 0);
            //}
            //eventInfo.AddEventHandler(et, d);
            //eventInfo.AddEventHandler(et, Delegate.Combine(events));
            ////method = eventInfo.GetAddMethod();
            ////Delegate nevents = Delegate.Combine(d, events[0]);
            ////method.Invoke(et, new object[] { (EventHandler)nevents });
            //eventObject = (Delegate)eventInfo.DeclaringType.GetField(eventInfo.Name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).GetValue(et);
            //Console.WriteLine("增加事件调用列表中的数目：" + eventObject.GetInvocationList().Length);

            ////Delegate eventObject = (Delegate)eventInfo.DeclaringType.InvokeMember(eventInfo.Name, BindingFlags.Instance | BindingFlags.GetField, null, et, null);
            ////Console.WriteLine("事件写入方法：" + eventInfo.GetAddMethod().Name);
            ////Delegate[] events = eventObject.GetInvocationList();
            ////Console.WriteLine("事件调用列表中的数目：" + events.Length);
            ////eventInfo.DeclaringType.InvokeMember(eventInfo.Name, BindingFlags.Instance | BindingFlags.Public  | BindingFlags.SetField, null, et, new object[0]);
            ////Console.WriteLine("删除后事件调用列表中的数目：" + eventObject.GetInvocationList().Length);
            ////Delegate nd = Delegate.Combine(events[0], d);
            ////eventInfo.AddEventHandler(et, nd);
            ////Console.WriteLine("增加事件调用列表中的数目：" + eventObject.GetInvocationList() != null ? eventObject.GetInvocationList().Length : 0);
            Type t = typeof(System.Web.UI.Page);
            FieldInfo[] fs = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            //foreach (FieldInfo f in fs)
            //{
            //    Console.WriteLine(f.Name);
            //}
            MemberInfo[] f = t.GetMember("LoadComplete", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            //Console.WriteLine(f.Name);
        }

        static void et_hander(object sender, EventArgs e)
        {
            Console.WriteLine("执行事件1");
        }
    }

    public class MyEventObject
    {
        public event EventHandler Hander;//表示控件事件
    }
}
