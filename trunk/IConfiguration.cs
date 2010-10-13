using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrystalWall
{
    /// <summary>
    /// 权限配置元素
    /// </summary>
    public interface IConfiguration
    {
        
        /// <summary>
        /// 根据配置属性创建配置对应的对象。属性名必须为程序集全局名称。指定的对象必须具有无参的构造函数
        /// </summary>
        /// <param name="propertyName">指定类型的属性名称</param>
        /// <returns>可运行的对象</returns>
        object CreateObject(String propertyName);
        
        /// <summary>
        /// 获取指定配置元素属性的值。属性名称为xml配置中的元素名称
        /// </summary>
        /// <param name="name">xml配置元素名称</param>
        /// <returns>属性值</returns>
        string GetAttribute(String name);
        
        /// <summary>
        /// 获取配置中所有属性名称
        /// </summary>
        string[] GetAttributeNames();
        
        /// <summary>
        /// 获取子配置元素列表
        /// </summary>
        IConfiguration[] Children();
        
        /// <summary>
        /// 获取指定名称的子配置列表
        /// </summary>
        IConfiguration[] GetChildren(String name);
        
        /// <summary>
        /// 此配置元素的名称
        /// </summary>
        string Name 
        {
            get;
        }
        
        /// <summary>
        /// 获取此配置元素的父对象，如果此配置元素是IResource资源的直接子元素则返回的对象为IResource，否则为IConfiguration
        /// </summary>
        object Parent 
        {
            get;
        }
        
        /// <summary>
        /// 获取元素的text文本值，无内容则返回空字符串
        /// </summary>
        string GetValue();
    }
}
