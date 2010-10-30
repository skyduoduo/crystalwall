using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CrystalWall.Web
{
    /// <summary>
    /// ASP.NET Web应用程序的身份令牌持久存储实现。他在当前web会员中的__CURRENT_USER_KEY__中存储当前的身份令牌对象
    /// </summary>
    public class WebPrincipalTokenStorage: IPrincipalTokenStorage
    {
        //[ThreadStatic]
        //private static IPrincipalToken CurrentToken;

        public const string __CURRENT_USER_KEY__ ="__CURRENT_USER_KEY__";

        public void SetCurrentToken(IPrincipalToken token)
        {
            if (token != null)
                HttpContext.Current.Session.Add(WebPrincipalTokenStorage.__CURRENT_USER_KEY__, token);
        }

        public IPrincipalToken GetCurrentToken()
        {
            return (IPrincipalToken)HttpContext.Current.Session[WebPrincipalTokenStorage.__CURRENT_USER_KEY__];
        }

        public void ClearToken()
        {
            HttpContext.Current.Session.Remove(WebPrincipalTokenStorage.__CURRENT_USER_KEY__);
        }
    }
}
