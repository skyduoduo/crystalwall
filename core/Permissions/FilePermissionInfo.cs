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

namespace CrystalWall.Permissions
{
    /// <summary>
    /// 描述系统中某个资源的权限对象，资源的名称格式为linux path的格式类似，默认以
    /// "/"分隔符分隔。在系统中，任何资源都被认为是具有目录的文件，最上层的目录为“/"。资源权限的包含算法按照
    /// 以下进行：
    /// <br> 
    /// <ul> 
    /// <li>? 匹配一个字符</li> 
    /// <li>* 匹配0个或者多个字符</li> 
    /// <li>** 匹配0个或者多个目录</li> 
    /// </ul>
    /// </br>
    /// <br>
    /// 例如：
    /// <li>/xyz/mn/**/匹配目录/xyz/mn/pqr/</li>
    /// <li>/xyz/mn/**/*.exe匹配文件/xyz/mn/pqr/xy.exe</li>
    /// <li>/xyz/mn/**/pqr/x*.exe匹配文件/xyz/mn/op/etc/pqr/xy.exe</li>
    /// </br>
    /// 文件权限也采用linux的对于动作的结构，动作的字符用5个位表示："crwxd"
    /// <list type="li">
    /// <listheader>位数</listheader>
    /// <item>第1位c表示创建</item>
    /// <item>第2位r表示读取</item>
    /// <item>第3位w表示写入</item>
    /// <item>第4位x表示运行</item>
    /// <item>第5位d表示删除</item>
    /// </list>
    /// 如果不具有某动作的权限，则只需要在此动作位设置为字符“-"即可，例如：不具有可写权限则为："cr-xd";
    /// 具体动作的”含义“由应用程序解释
    /// </summary>
    /// <author>vincent valenlee</author>
    public class FilePermissionInfo: PermissionInfo
    {
        public const int CREATE = 0x1;//创建

        public const int READ = 0x2;//读取

        public const int WRITE = 0x4;//写入

        public const int EXCUTE = 0x8;//运行

        public const int DELETE = 0x10;//删除

        public const int NO_ACTION = 0;

        public const char CREATE_CHAR = 'c';

        public const char READ_CHAR = 'r';

        public const char WRITE_CHAR = 'w';

        public const char EXECUTE_CHAR = 'x';

        public const char DELETE_CHAR = 'd';

        public const char NO_ACTION_CHAR = '-';

        //private int fileAction = NO_ACTION;//文件权限动作

        //public int FileAction
        //{
        //    get { return fileAction; }
        //}

        public override object Clone()
        {
            return new FilePermissionInfo((string)this.name.Clone(), (string)this.Action.Clone());
        }

        protected override int ResolveAction(string action)
        {
            char[] act = action.ToCharArray();
            foreach (char ac in act)
            {
                char lac = char.ToLower(ac);
                switch (lac)
                {
                    case CREATE_CHAR:
                        RealAction |= CREATE;
                        break;
                    case READ_CHAR:
                        RealAction |= READ;
                        break;
                    case WRITE_CHAR:
                        RealAction |= WRITE;
                        break;
                    case EXECUTE_CHAR:
                        RealAction |= EXCUTE;
                        break;
                    case DELETE_CHAR:
                        RealAction |= DELETE;
                        break;
                    default:
                        break;
                }
            }
            return RealAction;
        }

        /// <summary>
        /// 检查动作字符是否符合格式要求
        /// </summary>
        /// <exception cref="FilePermissionException">如果动作格式不符合规范，则抛出此异常</exception>
        protected void CheckAction(string action)
        {
            if (action.Length != 5)
            {
                throw new FilePermissionException(Name, action, "文件权限动作字符长度必须为4");
            }
            char[] act = action.ToCharArray();
            if ((act[0] != '-' && act[0] != CREATE_CHAR)
                 || (act[1] != '-' && act[1] != READ_CHAR)
                 || (act[2] != '-' && act[2] != WRITE_CHAR)
                 || (act[3] != '-' && act[3] != EXECUTE_CHAR)
                 || (act[4] != '-' && act[4] != DELETE_CHAR))
                throw new FilePermissionException(Name, action, "文件的动作字符不符合规范！");
        }

        /// <summary>
        /// 使用路径构造文件权限，此时动作默认为"----"
        /// </summary>
        /// <param name="name">如果name为空或者空格或者null，则为根目录</param>
        /// <exception cref="FilePermissionException">如果name不以根目录/开头或者动作不符合规范则抛出异常</exception>
        public FilePermissionInfo(string name): this(name, null)
        {
        }

        /// <summary>
        /// 使用路径构造文件权限
        /// </summary>
        /// <param name="name">如果name为空或者空格或者null，则为根目录</param>
        /// <exception cref="FilePermissionException">如果name不以根目录/开头或者动作不符合规范则抛出异常</exception>
        public FilePermissionInfo(string name, string action)
            : base(name, action)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = "/";//根目录
            if (!name.StartsWith("/"))
            {
                throw new FilePermissionException(Name, action, @"文件权限的路径必须在根目录之下（必须以/）开头！");
            }
            if (string.IsNullOrWhiteSpace(action))
            {
                this.action = "----";
            }
            CheckAction(action);
            ResolveAction(this.action);
        }

        public override bool Contains(PermissionInfo permission)
        {
            if (base.Equals(permission))
                return true;//permission为空权限、引用相等或者name与action完全相同
            FilePermissionInfo fp = permission as FilePermissionInfo;
            if (fp == null)
                return false;
            PathMatcher target = new PathMatcher();
            if (target.Match(this.Name, permission.Name))
            {
                //路径匹配，则检查动作
                if ((this.RealAction & fp.RealAction) == fp.RealAction)
                    return true;
            }
            return false;
        }
    }
}
