using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CodePlex.CrystalWall.Logging
{
    /// <summary>
    /// 日志异常
    /// </summary>
    [Serializable()]
    public class LoggingException : Exception
    {
        public LoggingException()
            : base()
        {
        }

        public LoggingException(string message)
            : base(message)
        {
        }

        public LoggingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected LoggingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
