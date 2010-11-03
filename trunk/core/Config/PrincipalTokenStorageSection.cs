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
using System.Configuration;
using CrystalWall.Utils;

namespace CrystalWall.Config
{
    /// <summary>
    /// 身份令牌存储器的配置对象，子类可以重写此配置类
    /// </summary>
    /// <author>vincent valenlee</author>
    public class PrincipalTokenStorageSection : ConfigurationSection, IExecutingElement
    {

        [ConfigurationProperty("class", DefaultValue = "false", IsRequired = true)]
        public string Class
        {
            get
            {
                return (string)this["class"];
            }
            set
            {
                this["class"] = value;
            }
        }

        public virtual object GetExecutingObject()
        {
            Type t = Type.GetType(Class, true);
            if (!typeof(IPrincipalTokenStorage).IsAssignableFrom(t))
                return new ConfigurationException(PrincipalTokenHolder.PRINCIPAL_TOKEN_STORAGE_SECTION, "身份提供者的配置类必须实现IPrincipalTokenStorage接口");
            try
            {
                return (IPrincipalTokenStorage)t.GetConstructor(new Type[0]).Invoke(new object[0]);
            }
            catch (Exception e)
            {
                ServiceManager.LoggingService.Fatal("身份提供者配置无法构建，请仔细检查配置文件中身份提供者的class类型是否为完全限定名（名称空间+程序集)");
                throw new ConfigurationException(PrincipalTokenHolder.PRINCIPAL_TOKEN_STORAGE_SECTION, "身份提供者配置无法构建", e);
            }
           
        }
    }
}
