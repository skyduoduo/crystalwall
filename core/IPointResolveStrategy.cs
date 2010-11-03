using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrystalWall
{
    /// <summary>
    /// 用于根据上下文对象解析权限点的策略接口
    /// </summary>
    public interface IPointResolveStrategy
    {
        /// <summary>
        /// 解析传入对象表示的当前运行状态的权限点
        /// 注意：此方法获取权限点期间需要自己处理异常，如果无法获取则返回null
        /// </summary>
        PermissionPoint Resolve(object context);
    }
}
