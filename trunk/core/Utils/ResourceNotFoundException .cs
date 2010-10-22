using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using CrystalWall.Logging;

///<remarks>this file is from Sharpdevelop</remarks>
namespace CrystalWall.Utils
{
    /// <summary>
    /// 当GlobalResource全局资源管理器无法找到请求的资源时发生的异常
    /// </summary>
    [Serializable()]
    public class ResourceNotFoundException : LoggingException
    {
        public ResourceNotFoundException(string resource)
            : base("Resource not found : " + resource)
        {
        }

        public ResourceNotFoundException()
            : base()
        {
        }

        public ResourceNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ResourceNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
