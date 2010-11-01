using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrystalWall.Config
{
    /// <summary>
    /// 实现此接口的对象能够在配置文件中进行配置，且能够根据配置获取一个可运行的对象
    /// </summary>
    public interface IExecutingElement
    {
        /// <summary>
        /// 根据配置获取可运行的对象
        /// </summary>
        object GetExecutingObject();
    }
}
