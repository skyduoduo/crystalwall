using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QJSoft.ECBC.Authority
{
    /// <summary>
    /// 无授权时抛出的异常
    /// </summary>
    public class AccessException: ApplicationException
    {
        //被检测的对象
        private object checkObject;

        public object CheckObject
        {
            get { return checkObject; }
            set { checkObject = value; }
        }

        public AccessException()
            : base("授权异常")
        {
        }

        public AccessException(string message)
            : base(message)
        {
        }
    }
}
