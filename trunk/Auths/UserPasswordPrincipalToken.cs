using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodePlex.CrystalWall.Auths
{
    /// <summary>
    /// 使用用户名和密码的身份令牌实现
    /// </summary>
    public class UserPasswordPrincipalToken : AbstractPricipalToken
    {

        private string password;

        private object information;

        public override object Certificate
        {
            get
            {
                return password;
            }
            set
            {
                password = value.ToString();
            }
        }

        public override object Information
        {
            get
            {
                return information;
            }
            set
            {
                information = value;
            }
        }

         /// <summary>
        /// 直接使用指定的权限构造身份令牌
        /// </summary>
        public UserPasswordPrincipalToken(string name, string password, IList<PermissionInfo> permissions): base(name, permissions)
        {
            this.password = password;
        }

        public UserPasswordPrincipalToken(string name, string password, IPrincipalProvider provider): base(name, provider)
        {
            this.password = password;
        }
    }
}
