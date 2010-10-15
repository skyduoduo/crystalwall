using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrystalWall
{
    /// <summary>
    /// 用于存储当前身份令牌的存储对象
    /// </summary>
    public interface IPrincipalTokenStorage
    {
        /// <summary>
        /// 设置当前使用系统的身份令牌
        /// </summary>
        void SetCurrentToken(IPrincipalToken token);

        /// <summary>
        /// 获取当前使用系统的身份令牌
        /// </summary>
        IPrincipalToken GetCurrentToken();

        /// <summary>
        /// 清除当前使用系统的身份令牌（用户推出系统时调用）
        /// </summary>
        void ClearToken();
    }
}
