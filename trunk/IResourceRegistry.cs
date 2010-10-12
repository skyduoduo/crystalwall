using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QJSoft.ECBC.Authority
{
    /// <summary>
    /// 资源注册表接口，用于注册资源对象
    /// </summary>
    public interface IResourceRegistry
    {
        IResource Find(string id);

        IResource Find(object context);
    }
}
