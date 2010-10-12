using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodePlex.CrystalWall.FileUtils
{
    public delegate void FileNameEventHandler(object sender, FileNameEventArgs e);

    /// <summary>
    /// 描述文件名的事件处理器参数
    /// </summary>
    public class FileNameEventArgs : System.EventArgs
    {
        string fileName;

        public string FileName
        {
            get
            {
                return fileName;
            }
        }

        public FileNameEventArgs(string fileName)
        {
            this.fileName = fileName;
        }
    }
}
