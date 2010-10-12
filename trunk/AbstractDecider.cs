using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodePlex.CrystalWall
{
    /// <summary>
    /// 抽象访问决定者。此决定者包含一个子类需要实现的从资源中获取相关权限的方法
    /// 以便用于指定身份令牌的授权是否包含资源的授权权限集，如果包含则说明指定的身份具有授权访问指定资源。
    /// 否则，将抛出AccessException通知客户端指定身份没有获得授权访问指定资源
    /// </summary>
    public abstract class AbstractDecider: IAccessDecider
    {
        /// <summary>
        /// 获取冲突选举器
        /// </summary>
        public abstract Elect GetElect();

        public abstract AbstractSecurityContext GetContext();

        public void Decide(IPrincipalToken principal, object check)
        {
            PermissionInfoCollection pc = principal.GetGrandedPermission();
            if (GetElect() != null)
                pc.ElectVisitor = GetElect();
            //资源上没有配置当前权限点指定的权限，则不允许任何人访问
            PermissionPoint point = GetContext().GetPoint(check);
            if (point == null)
                return;//程序没有定义权限点，不做任何权限控制！
            PermissionInfo checkPermission = point.NewPermission();
            if (!pc.Contains(checkPermission))
            {
                AccessException ae =  new AccessException("there is no access for " + principal.Name);
                ae.CheckObject = check;
                throw ae;
            }
        }
    }
}
