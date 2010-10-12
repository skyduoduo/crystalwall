using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodePlex.CrystalWall.Config
{
    /// <summary>
    /// 配置异常，客户端不要使用，这是权限框架内部使用的异常
    /// </summary>
    public class ConfigurationException: ApplicationException
    {
        public ConfigurationException(string message)
            : base(message)
        {
        }
    }
}
