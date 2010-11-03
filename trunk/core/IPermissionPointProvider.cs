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

namespace CrystalWall
{
    /// <summary>
    /// 能够提供权限点的提供者，在decider进行权限检查的过程中，如果传入的被检测对象实现此
    /// 接口，则使用decider会根据当前运行的对象提供的权限点判断权限
    /// </summary>
    /// <author>vincent valenlee</author>
    public interface IPermissionPointProvider
    {
        /// <summary>
        /// 自身提供当前运行时的权限点，例如可以返回当前运行方法上的元注释PermissionPoint标记所标注的权限点等
        /// </summary>
        PermissionPoint[] GetPoint();
    }
}
