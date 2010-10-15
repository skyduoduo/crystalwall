using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrystalWall.Attr
{
    //设置决定者类型的元属性
    public class DeciderAttribute: Attribute
    {
        private string type;

        public string Type
        {
            get { return type; }
        }

        public DeciderAttribute(string type)
        {
            this.type = type;
        }


    }
}
