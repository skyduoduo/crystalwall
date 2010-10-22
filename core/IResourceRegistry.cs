using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrystalWall
{
    /// <summary>
    /// 资源注册表接口，用于注册资源对象
    /// </summary>
    public interface IResourceRegistry
    {
        IPermissionResource Find(string id);

        IPermissionResource Find(object context);
    }
}
