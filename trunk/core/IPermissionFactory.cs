using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrystalWall
{
    /// <summary>
    /// 根据字符权限信息构造权限对象的工厂
    /// </summary>
    public interface IPermissionFactory
    {
        /// <summary>
        /// 解析权限字符成权限对象，字符的格式由实现决定
        /// </summary>
        PermissionInfo getPermission(string content);
    }

}
