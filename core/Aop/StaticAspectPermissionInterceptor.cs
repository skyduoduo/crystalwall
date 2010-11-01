using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetGuru.AspectDNG.Config;
using DotNetGuru.AspectDNG.Joinpoints;
using CrystalWall.Attr;
using System.Web.UI;
using CrystalWall.Web;

namespace CrystalWall.Aop
{
    /// <summary>
    /// 使用aspectdng的静态植入点，由于aspectdng定义静态切入点时无法指定元注释标记。因此，默认的植入点
    /// 为所有包含InAccessControl的方法。这要求开发者在加入[PermissionPoint]元注释的方法按照惯例***InAccessControl命名！
    /// 当然，开发者也可以通过aspectdng.xml配置文件修改
    /// </summary>
    public class StaticAspectPermissionInterceptor
    {
        ////const string Target = "xpath: //Type[@FullName='Base.TestAroundCall']/Methods/*/Body/Instructions/*";
        ////Method[contains(@Name, 'ToBeDeletedMethod')]
        ////const string Target = "xpath: //Method[contains(@Name, 'InAccessControl')]";

        //const string Target = "* *::*InAccessControl(*)";//默认的植入点为包含InAccessControl的方法

        //[AroundCall(Target)]
        //public static object AccessControl(MethodJoinPoint jp)
        //{
           
        //    PermissionPointAttribute[] attrs = (PermissionPointAttribute[])jp.RealTarget.GetType().GetCustomAttributes(typeof(PermissionPointAttribute), true);
        //    if (attrs == null || attrs.Length == 0)
        //        return jp.Proceed();
        //    PermissionPoint[] points = new PermissionPoint[attrs.Length];
        //    int i = 0;
        //    foreach (PermissionPointAttribute attr in attrs)
        //    {
        //        Type t = Type.GetType(attr.Type);
        //        PermissionPoint point = (PermissionPoint)t.GetConstructor(new Type[0]).Invoke(new object[0]);
        //        point.Name = attr.Name;
        //        point.Resource = attr.Resource;
        //        point.Action = attr.Action;
        //        point.Context = jp.RealTarget;
        //        point.Member = jp.TargetOperation;
        //        //获取参数
        //        point.Args = new object[jp.NbParameters];
        //        for (int j = 0; j < jp.NbParameters; j++)
        //        {
        //            point.Args[j] = jp[j];
        //        }
        //        points[i++] = point;
        //    }
        //    IAccessDecider decider;
        //    DeciderAttribute[] deciderAttr = (DeciderAttribute[])jp.RealTarget.GetType().GetCustomAttributes(typeof(DeciderAttribute), true);
        //    if (deciderAttr == null || deciderAttr.Length == 0)
        //        decider = new DefaultDecider();//使用默认的权限决定器
        //    else
        //    {
        //        //TODO:添加冲突解决者的构造
        //        decider = (IAccessDecider)Type.GetType(deciderAttr[0].Type).GetConstructor(new Type[0]).Invoke(new object[0]);
        //    }
        //    //检查权限
        //    decider.Decide(PrincipalTokenHolder.CurrentPrincipal, new ConstPointProvider(points));
        //    //权限通过，正常执行
        //    return jp.Proceed();
        //}

        ///// <summary>
        ///// 添加ASP.NET页面权限修饰
        ///// TODO:test
        ///// </summary>
        //[AroundCall("* Page::.ctor(*)")]
        //public static object AccessASPPage(JoinPoint jp)
        //{
        //    Page page = (Page)jp.Proceed();
        //    new PageSecurityContext(page);
        //    return page;
        //}

        //private class ConstPointProvider : IPermissionPointProvider
        //{
        //    private PermissionPoint[] points;

        //    public ConstPointProvider(PermissionPoint[] points)
        //    {
        //        this.points = points;
        //    }

        //    public PermissionPoint[] GetPoint()
        //    {
        //        return points;
        //    }
        //}
    }
}
