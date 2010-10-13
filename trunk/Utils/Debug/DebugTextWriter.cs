using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

///<remarks>this file is from Sharpdevelop</remarks>
namespace CrystalWall.Debugging
{
    /// <summary>
    /// TextWriter的子类，使用Debug类将信息写入跟踪监听器中
    /// </summary>
    public class DebugTextWriter : TextWriter
    {
        public override Encoding Encoding
        {
            get
            {
                return Encoding.Unicode;
            }
        }

        public override void Write(char value)
        {
            Debug.Write(value.ToString());
        }

        public override void Write(char[] buffer, int index, int count)
        {
            Debug.Write(new string(buffer, index, count));
        }

        public override void Write(string value)
        {
            Debug.Write(value);
        }

        public override void WriteLine()
        {
            Debug.WriteLine(string.Empty);
        }

        public override void WriteLine(string value)
        {
            Debug.WriteLine(value);
        }
    }
}
