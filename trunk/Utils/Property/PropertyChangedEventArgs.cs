using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

///<remarks>this file is from Sharpdevelop</remarks>
namespace CrystalWall.Property
{
    /// <summary>
    /// 属性改变代理
    /// </summary>
    public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);

    /// <summary>
    /// 包含指定属性新旧值的事件参数对象
    /// </summary>
    public class PropertyChangedEventArgs : EventArgs
    {
        Properties properties;
        string key;
        object newValue;
        object oldValue;

        public Properties Properties
        {
            get
            {
                return properties;
            }
        }

        public string Key
        {
            get
            {
                return key;
            }
        }

  
        public object NewValue
        {
            get
            {
                return newValue;
            }
        }


        public object OldValue
        {
            get
            {
                return oldValue;
            }
        }

        public PropertyChangedEventArgs(Properties properties, string key, object oldValue, object newValue)
        {
            this.properties = properties;
            this.key = key;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }
    }
}
