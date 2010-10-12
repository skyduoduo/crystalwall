using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QJSoft.ECBC.Authority.Auths
{
    /// <summary>
    /// 用于标识对一个授权的决定：允许、拒绝、弃权
    /// </summary>
    public enum AccessDecision
    {
        ALLOWED = 1,//允许
        DENY = -1,//拒绝
        ABSTAIN = 0//弃权

    }
}
