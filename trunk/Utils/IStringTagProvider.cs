using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

///<remarks>this file is from Sharpdevelop</remarks>
namespace CrystalWall.Utils
{
    /// <summary>
    /// 字符标签列表提供者接口，能够转换指定标签
    /// </summary>
    public interface IStringTagProvider
    {
        string[] Tags
        {
            get;
        }

        string Convert(string tag);
    }
}
