using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrystalWall.Attr
{
    //设置决定者类型以及的元属性
    public class DeciderAttribute: Attribute
    {
        private string type;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        private string electType;

        public string ElectType
        {
            get { return electType; }
            set { electType = value; }
        }

    }
}
