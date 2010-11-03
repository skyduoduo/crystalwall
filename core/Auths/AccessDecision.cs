/*
 * Copyright 2008-2010 the original author or authors.
 *
 * Licensed under the Eclipse Public License v1.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.eclipse.org/legal/epl-v10.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrystalWall.Auths
{
    /// <summary>
    /// 用于标识对一个授权的决定：允许、拒绝、弃权
    /// </summary>
    /// <author>vincent valenlee</author>
    public enum AccessDecision
    {
        ALLOWED = 1,//允许
        DENY = -1,//拒绝
        ABSTAIN = 0//弃权

    }
}
