using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrystalWall.Auths
{
    /// <summary>
    /// 表示一个匿名身份
    /// </summary>
    public class AnonyPrincipalToken : AbstractPricipalToken
    {
        public AnonyPrincipalToken(IList<PermissionInfo> permissions)
            : base("AnonyUser", permissions)
        {
        }

        private AnonyPrincipalToken(string name, IPrincipalProvider provider)
            : base(name, provider)
        {
        }

        public override object Certificate
        {
            get
            {
                return "";
            }
            set
            {
                
            }
        }

        public override object Information
        {
            get
            {
                return "匿名用户";
            }
            set
            {
            }
        }
    }
}
