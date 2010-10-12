using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodePlex.CrystalWall
{
    /// <summary>
    /// 资源对象是权限点的清单，在这份清单中，他能够根据权限的名称和action解析权限信息
    /// </summary>
    public interface IPermissionResource
    {
        string UniqueIdentifier
        {
            get;
            set;
        }

        /// <summary>
        /// 获取资源的配置
        /// </summary>
        IConfiguration[] Configurations
        {
            get;
        }

        /// <summary>
        /// 定义资源类的程序集
        /// </summary>
        string LoadedAssembly
        {
            get;
        }

        /// <summary>
        /// 是否支持指定的上下文
        /// </summary>
        bool Support(object context);

        /// <summary>
        /// 资源负责根据上下文解析出权限点
        /// </summary>
        PermissionPoint Resolve(object context);

    }
}
