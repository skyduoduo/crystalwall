using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CrystalWall
{
    /// <summary>
    /// 此权限点上的资源id、名称、action都是用如下语法动态获取：
    /// 在字符名称中如果出现{....}中间的内容都将是从定义权限点的上下文对象中调用方法获取。
    /// 资源以路径方式命名，可以使用?, *表示的通配符。例如：
    /// resource={GetResource}表示资源的获取使用上下文对象上的GetResource方法获得，一般
    /// 说来，获取方法不接受参数，但如果需要参数，只能当权限点定义在执行上下文对象上的某个
    /// 方法上时候才能使用，可以使用如下方式传入参数：
    /// public class ContextObject {
    /// 
    ///     [PermissionPoint(resource="{GetResource(0, 2)}"]
    ///     public Delete(object p1, object p2, object p3, object p4) {
    ///       ...
    ///     }
    ///  }
    ///  这表示当执行delete方法时，权限点将使用GetResource且传递p1, p3获得资源路径
    /// </summary>
    public abstract class DynamicPermissionPoint: PermissionPoint
    {

        private string resourceMethod;

        private string nameMethod;

        private string actionMethod;

        public override string Name
        {
            get
            {
                if (nameMethod != null && nameMethod.Equals(string.Empty))
                {
                    return GetResultName(nameMethod);
                }
                 return base.Name;
            }
            set
            {
                base.Name = value;
                if (base.Name.IndexOf("{") == 0 && base.Name.IndexOf("}") == base.Name.Length - 1)
                {
                    //使用动态方法
                    nameMethod = base.Name.Substring(base.Name.IndexOf("{") + 1, base.Name.Length - 2);
                }
            }
        }

        public override string Resource
        {
            get
            {
                if (resourceMethod != null && resourceMethod.Equals(string.Empty))
                {
                    return GetResultName(resourceMethod);
                }
                return base.Resource;
            }
            set
            {
                base.Resource = value;
                if (base.Resource.IndexOf("{") == 0 && base.Resource.IndexOf("}") == base.Resource.Length - 1)
                {
                    //使用动态方法
                    resourceMethod = base.Resource.Substring(base.Resource.IndexOf("{") + 1, base.Resource.Length - 2);
                }
            }
        }

        public override string Action
        {
            get
            {
                if (actionMethod != null && actionMethod.Equals(string.Empty))
                {
                    return GetResultName(actionMethod);
                }
                return base.Action;
            }
            set
            {
                base.Action = value;
                if (base.Action.IndexOf("{") == 0 && base.Action.IndexOf("}") == base.Action.Length - 1)
                {
                    //使用动态方法
                    actionMethod = base.Action.Substring(base.Action.IndexOf("{") + 1, base.Action.Length - 2);
                }
            }
        }

        //根据名称从此权限点中获取值
        protected string GetResultName(string methodName)
        {
            string method = methodName.Substring(0, methodName.IndexOf("("));
            int paramLength = methodName.IndexOf(")") - methodName.IndexOf("(") - 1;
            object[] args = null;
            if (paramLength == 0)
            {
                //无参方法
                args = new object[0];
            }
            string paramString = methodName.Substring(methodName.IndexOf("(") + 1, paramLength);
            if (!paramString.Contains(","))
            {
                //只有一个参数，解析数字
                args = new object[1];
                args[0] = Args[int.Parse(paramString)];
            }
            else
            {
                string[] p = paramString.Split(',');
                args = new object[p.Length];
                for (int i = 0; i < p.Length; i++)
                {
                    args[i] = Args[int.Parse(p[i].Trim())];
                }
            }
            return (string)((MethodInfo)Member).Invoke(Context, args);
        }
        
        //public abstract PermissionInfo NewPermission();
    }
}
