using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrystalWall
{
    /// <summary>
    /// 能够提供权限点的提供者，在decider进行权限检查的过程中，如果传入的被检测对象实现此
    /// 接口，则使用decider会根据当前运行的对象提供的权限点判断权限
    /// </summary>
    public interface IPermissionPointProvider
    {
        /// <summary>
        /// 自身提供当前运行时的权限点，例如可以返回当前运行方法上的元注释PermissionPoint标记所标注的权限点等
        /// </summary>
        PermissionPoint[] GetPoint();
    }
}
