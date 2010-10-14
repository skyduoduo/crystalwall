using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrystalWall
{

    /// <summary>
    /// 权限控制执行上下文
    /// </summary>
    public abstract class AbstractDecider : IAccessDecider
    {

        public event EventHandler<AccessExceptionEventArgs> AccessDenyed;

        /// <summary>
        /// 配置冲突解决器
        /// </summary>
        private Elect confuseElect;

        public Elect ConfuseElect
        {
            get { return confuseElect; }
            set { confuseElect = value; }
        }

        private IList<IPointResolveStrategy> pointResolves = new List<IPointResolveStrategy>();

        public IList<IPointResolveStrategy> PointResolves
        {
            get { return pointResolves; }
            set { pointResolves = value; }
        }

        /// <summary>
        ///  授权不通过，则执行不通过时的事件处理
        /// </summary>
        protected void OnAccessException(IPrincipalToken principal, object check)
        {
            if (AccessDenyed != null)
                AccessDenyed(this, new AccessExceptionEventArgs(principal, check));
        }

        /// <summary>
        /// 根据当前访问对象获取其上的权限点
        /// 注意：一般上下文对象建议实现IPermissionPointProvider接口，自己提供当前运行期间的权限点列表
        /// </summary>
        public PermissionPoint[] GetPoint(object context)
        {
            if (context is IPermissionPointProvider)
            {
                return ((IPermissionPointProvider)context).GetPoint();
            }
            try
            {
                IPointResolveStrategy strategy = PointResolves.First(ps =>
                {
                    if (ps.Support(context.GetType()))
                        return true;
                    return false;
                });
                return strategy.Resolve(context);
            }
            catch (InvalidOperationException e)
            {
                return null;
            }
        }


        public void Decide(IPrincipalToken principal, object check)
        {
            PermissionInfoCollection pc = principal.GetGrandedPermission();
            if (ConfuseElect != null)
                pc.ElectVisitor = ConfuseElect;
            //资源上没有配置当前权限点指定的权限，则不允许任何人访问
            PermissionPoint[] point = GetPoint(check);
            if (point == null)
                return;//程序没有定义权限点，不做任何权限控制！
            foreach (PermissionPoint p in point)
            {//在当前对象上定义了多个权限点，每一个都需要进行权限检测
                PermissionInfo checkPermission = p.NewPermission();
                if (!pc.Contains(checkPermission))
                {
                    AccessException ae = new AccessException("there is no access for " + PrincipalTokenHolder.CurrentPrincipal.Name);
                    ae.CheckObject = check;
                    throw ae;
                }
            }
        }
    }

    public class AccessExceptionEventArgs : EventArgs
    {
        private IPrincipalToken principal;

        public IPrincipalToken Principal
        {
            get { return principal; }
        }
        private object check;

        public object Check
        {
            get { return check; }
        }

        public AccessExceptionEventArgs(IPrincipalToken principal, object check)
        {
            this.principal = principal;
            this.check = check;
        }


    }
}
