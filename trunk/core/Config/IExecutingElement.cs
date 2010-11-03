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

namespace CrystalWall.Config
{
    /// <summary>
    /// 实现此接口的对象能够在配置文件中进行配置，且能够根据配置获取一个可运行的对象
    /// </summary>
    /// <author>vincent valenlee</author>
    public interface IExecutingElement
    {
        /// <summary>
        /// 根据配置获取可运行的对象
        /// </summary>
        object GetExecutingObject();
    }
}
