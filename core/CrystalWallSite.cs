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
using CrystalWall.Config;
using CrystalWall.Utils;

namespace CrystalWall
{
    /// <summary>
    /// 配置文件中的sites配置节，包括site配置元素集合
    /// </summary>
    /// <author>vincent valenlee</author>
    public class CrystalWallSites : ConfigurationSection
    {
        [ConfigurationProperty("sites", IsDefaultCollection = false, IsRequired = true)]
        [ConfigurationCollection(typeof(CrystalWallSiteCollection), AddItemName = "site")]
        public CrystalWallSiteCollection Sites
        {
            get
            {
                return (CrystalWallSiteCollection)base["sites"];
            }
        }
    }

    /// <summary>
    /// sites配置节中的site元素集合
    /// </summary>
    /// <author>vincent valenlee</author>
    public class CrystalWallSiteCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CrystalWallSite();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CrystalWallSite)element).Class;
        }
    }

    /// <summary>
    /// 组装权限检查的上下文对象以及decider配置的对象，系统根据此对象获取对应的decider进行权限判断：
    /// <code>
    /// CrystalWallSite site = CrystalWallSite.Find(object context);
    /// site.InitSite();//可选
    /// site.Decider.decider(principal, context);
    /// </code>
    /// 此对象可以用于静态AOP和动态代理框架中将权限检查插入到IL代码中，通常如果使用AOP注入，则context对象
    /// 应该为封装方法的对象，例如MethodInvocation。site应该按照如下方式配置到应用中：
    /// <code>
    /// <sites>
    ///   <site context="Crystalwall.AOP.MethodInvocation, CrystalWall , Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" type="SiteType类型">
    ///      <!--其他配置-->
    ///      <decider class="可选">
    ///         <!--其他配置-->
    ///         <resolves>
    ///            <resolve class="权限点解析器类全名1">
    ///            <resolve class="权限点解析器类全名2">
    ///         </resolves>
    ///     </decider>
    ///   </site>
    ///   <!--其他site配置-->
    /// </sites>
    /// </code>
    /// </summary>
    /// <author>vincent valenlee</author>
    public class CrystalWallSite : ConfigurationElement
    {
        private static IDictionary<Type, CrystalWallSite> sites = new Dictionary<Type, CrystalWallSite>();

        public readonly static CrystalWallSite DEFAULT_SITE;

        static CrystalWallSite()
        {
            CrystalWallSite defaultSite = new CrystalWallSite();
            defaultSite.decider = FactoryServices.DEFAULT_DECIDER;
            defaultSite.Context = null;
            defaultSite.DeciderSection = null;
            defaultSite.Class = "CrystalWall.CrystalWallSite, CrystalWall, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            defaultSite.isInit = true;
            DEFAULT_SITE = defaultSite;
        }

        private IAccessDecider decider;

        public IAccessDecider Decider
        {
            get
            {
                InitSite();
                return decider;
            }
        }

        private bool isInit = false;

        public bool IsInit
        {
            get { return isInit; }
            set { isInit = value; }
        }

        public const string CONFIG_SITES_GROUP = "sites";

        public const string CONFIG_SITE = "site";

        [ConfigurationProperty("context", DefaultValue = "false", IsRequired = true)]
        public virtual string Context
        {
            get
            {
                return (string)this["context"];
            }
            set
            {
                this["context"] = value;
            }
        }

        [ConfigurationProperty("decider", DefaultValue = "false", IsRequired = true)]
        public virtual DeciderSection DeciderSection
        {
            get
            {
                return (DeciderSection)this["decider"];
            }
            set
            {
                this["decider"] = value;
            }
        }

        [ConfigurationProperty("class", DefaultValue = "false", IsRequired = false)]
        public virtual string Class
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

        /// <summary>
        /// 如果参数为null或者配置中没有配置sites或者找不到匹配上下文类型的sites，则返回默认的Site。
        /// </summary>
        public static CrystalWallSite Find(object context)
        {
            CrystalWallSites sitesSection = PrincipalTokenHolder.ConfigFile.Configuration.Sections["sites"] as CrystalWallSites;
            if (context == null || sitesSection == null)
                return DEFAULT_SITE;
            if (sites.Keys.Contains(context.GetType()))
                return sites[context.GetType()];
            foreach (CrystalWallSite section in sitesSection.Sites)
            {
                if (Type.GetType(section.Context) == context.GetType())
                {
                    CrystalWallSite real;
                    if (section.Class == null)
                    {
                        real = section;
                    }
                    else 
                    {
                        real = (CrystalWallSite)Type.GetType(section.Class, true).GetConstructor(new Type[0]).Invoke(new object[0]);
                    }
                    sites.Add(context.GetType(), real);
                    return real;
                }
            }
            return DEFAULT_SITE;//找不到能够解析context的sites，则返回默认的sites
        }

        /// <summary>
        /// 如果需要额外的初始化，默认只从Decider配置节中获取Decider实例，
        /// 子类应该根据需要重写
        /// </summary>
        public virtual void InitSite()
        {
            if (isInit == false)
            {
                try
                {
                    decider = (IAccessDecider)DeciderSection.GetExecutingObject();
                    isInit = true;
                }
                catch
                {
                    ServiceManager.LoggingService.Error("decider无法构造，请仔细检查配置是否正确");
                    throw new CrystalWall.Config.ConfigurationException("decider", "decider无法构造，请仔细检查配置是否正确");
                }
            }
        }

        /// <summary>
        /// 当site发生更改时调用。默认为空，子类应该根据需要重写
        /// </summary>
        public virtual void Update()
        {
        }

        /// <summary>
        /// 卸载清除相关资源, 子类应该根据需要重写
        /// </summary>
        public virtual void Close()
        {
            isInit = false;
        }
    }
}
