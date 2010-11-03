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

namespace  CrystalWall.Config
{
    /// <summary>
    /// 配置异常，客户端不要使用，这是权限框架内部使用的异常
    /// </summary>
    /// <author>vincent valenlee</author>
    public class ConfigurationException: ApplicationException
    {
        private string node;//配置节点名称

        public string Node
        {
            set { node = value; }
        }

        public ConfigurationException(string node, string message)
            : base(message)
        {
            this.node = node;
        }

        public ConfigurationException(string node, string message, Exception e)
            : base(message, e)
        {
            this.node = node;
        }


    }
}
