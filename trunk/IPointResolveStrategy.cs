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
        /// </summary>
        PermissionPoint[] Resolve(object context);

        /// <summary>
        /// 是否支持指定运行状态对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool Support(Type type);
    }
}
