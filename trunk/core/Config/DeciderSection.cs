using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using CrystalWall.Utils;

namespace CrystalWall.Config
{
    /// <summary>
    /// 配置在site中的权限决定者配置
    /// </summary>
    public class DeciderSection : ConfigurationElement, IExecutingElement
    {
        [ConfigurationProperty("class", DefaultValue = "false", IsRequired = false)]
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

        [ConfigurationProperty("resolves", IsDefaultCollection = false, IsRequired=false)]
        [ConfigurationCollection(typeof(PointResolvesCollection), AddItemName="resolve")]
        public PointResolvesCollection Resolves
        {
            get
            {
                 return (PointResolvesCollection)base["resolves"];
            }
        }
      

        /// <summary>
        /// 超类只是根据class属性调用无参构造函数，子类应该根据需要重写
        /// </summary>
        public virtual object GetExecutingObject()
        {
            if (Class == null)
                return new DefaultDecider();
            Type t = Type.GetType(Class, true);
            if (!typeof(IAccessDecider).IsAssignableFrom(t))
                return new ConfigurationException("decider", "指定的class类型不是IAccessDecider类型！decider无法构造");
            try
            {
                IAccessDecider decider = (IAccessDecider)t.GetConstructor(new Type[0]).Invoke(new object[0]);
                InitDecider(decider);
                return decider;
            }
            catch (Exception e)
            {
                ServiceManager.LoggingService.Fatal("decider无法构造，请仔细检查配置是否正确");
                throw new ConfigurationException("decider", "decider无法构造，请仔细检查配置是否正确", e);
            }
        }

        /// <summary>
        /// 子类应该重写初始化decider的方法,例如：添加ConfuseElect冲突选择器、添加异常后时间等。
        /// 默认实现只解析IPointResolve接口，因此，子类重写时应该首先调用base.InitDecider()
        /// </summary>
        protected virtual void InitDecider(IAccessDecider decider)
        {
            AbstractDecider de = decider as AbstractDecider;
            if (de == null)
                return;
            if (Resolves != null && Resolves.Count > 0)
            {
                try
                {
                    //解析resolve
                    foreach (PointResolveElement resolve in Resolves)
                    {
                        IPointResolveStrategy strategy = (IPointResolveStrategy)Type.GetType(resolve.Class).GetConstructor(new Type[0]).Invoke(new object[0]);
                        InitResolve(strategy);
                        de.AddPointResolve(strategy);
                    }
                }
                catch
                {
                    ServiceManager.LoggingService.Error("无法构造resolve，请仔细检查配置是否正确");
                    throw new ConfigurationException("resolve", "无法构造resolve，请仔细检查配置是否正确");
                }
            }
        }

        /// <summary>
        /// 默认为空，子类应该重写以便初始化解析器
        /// </summary>
        protected void InitResolve(IPointResolveStrategy resolve)
        {
        }

    }

    public class PointResolveElement : ConfigurationElement
    {
        [ConfigurationProperty("class", DefaultValue = "false", IsRequired = true, IsKey = true)]
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
    }

    public class PointResolvesCollection : ConfigurationElementCollection
    {

        protected override ConfigurationElement CreateNewElement()
        {
            return new PointResolveElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PointResolveElement)element).Class;
        }
    }

}
