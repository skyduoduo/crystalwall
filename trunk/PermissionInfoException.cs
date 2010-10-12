using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace QJSoft.ECBC.Authority
{
    /// <summary>
    /// 权限信息操作时抛出的异常
    /// </summary>
    public class PermissionInfoException: ApplicationException
    {
         public PermissionInfoException()
            : base()
        {
        }

        public PermissionInfoException(string message)
            : base(message)
        {
        }

        public PermissionInfoException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PermissionInfoException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
