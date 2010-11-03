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
using System.Reflection;
using CrystalWall.Utils;
using System.Xml;
using System.Collections;
using System.Windows.Forms;

namespace CrystalWall.Config
{
    /// <summary>
    /// 用于在xml配置文件中解析principal提供者的处理器，但目前.NET的版本已经不支持使用接口创建配置节，
    /// 但crystalwall认为，保持使用此接口能增加程序的灵活性。因此，crystalwall另外提供了一个ConfigurationFile
    /// 的类，如果使用自定义配置，请使用此类的GetSection方法获取各种节点
    /// </summary>
    /// <author>vincent valenlee</author>
    public class PrincipalProviderSectionHandler : IConfigurationSectionHandler
    {
        public const string CLASS_ATTR = "class";

        public const string ASSEMBLY_ATTR = "assembly";

        public const string PROVIDER_CHILD_NODE = "provider";

        public object Create(object parent, object configContext, XmlNode sections)
        {
            if (!sections.HasChildNodes)
            {
                throw new ConfigurationException(sections.Name, "principal-provider配置下必须具有provider子节点");
            }
            IList<IPrincipalProvider> providers = new List<IPrincipalProvider>(sections.ChildNodes.Count);
            foreach (XmlNode section in sections.ChildNodes)
            {
                if (section.NodeType != XmlNodeType.Element || !section.Name.Equals(PROVIDER_CHILD_NODE))
                    continue;
                if (section.Attributes[CLASS_ATTR] == null)
                    throw new ConfigurationException(section.Name, "身份提供者配置节点中必须具有class属性指定提供者类型");
                IPrincipalProvider provider = null;
                try
                {
                    if (section.Attributes[ASSEMBLY_ATTR] == null)
                    {
                        //没有指定程序集，但可能在class中指定全限定名。首先使用当前运行的程序集加载，然后使用crystalwall程序集加载，最后使用全限定名加载
                        provider = (IPrincipalProvider)Assembly.GetExecutingAssembly().CreateInstance(section.Attributes[CLASS_ATTR].Value);
                        if (provider == null)
                            provider = (IPrincipalProvider)Assembly.GetAssembly(typeof(IPrincipalProvider)).CreateInstance(section.Attributes[CLASS_ATTR].Value);
                        if (provider == null)
                            provider = (IPrincipalProvider)Type.GetType(section.Attributes[CLASS_ATTR].Value, true).GetConstructor(new Type[0]).Invoke(new object[0]);
                    }
                    else
                    {
                        provider = (IPrincipalProvider)Assembly.LoadFrom(section.Attributes[ASSEMBLY_ATTR].Value).CreateInstance(section.Attributes[CLASS_ATTR].Value);
                    }
                    if (provider == null)
                        throw new ConfigurationException(section.Name, "身份提供者配置节点中指定提供者类型无法加载");
                    //初始化provider数据（将根据具体的provider并根据xml中的配置进行初始化，由子类决定）
                    provider.InitData(section, null, null);
                    providers.Add(provider);
                }
                catch (Exception e)
                {
                    ServiceManager.LoggingService.Error("无法加载指定的身份提供者，请检查配置文件是否正确", e);
                    throw e;
                }
            }
            return providers;
        }
    }
}
