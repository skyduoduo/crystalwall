using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  CrystalWall.Config
{
    /// <summary>
    /// 配置异常，客户端不要使用，这是权限框架内部使用的异常
    /// </summary>
    public class ConfigurationException: ApplicationException
    {
        private string node;//配置节点名称

        public string Node
        {
            set { node = value; }
        }

        public ConfigurationException(string node, string message)
            : base(message)
        {
            this.node = node;
        }

        public ConfigurationException(string node, string message, Exception e)
            : base(message, e)
        {
            this.node = node;
        }


    }
}
