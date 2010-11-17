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
using System.Text.RegularExpressions;

namespace CrystalWall.Permissions
{
    /// <summary>
    /// 用于按照路径的方式匹配资源权限的路径匹配器，他按照如下方式进行匹配：
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
    /// </summary>
    public class PathMatcher
    {
        /// 中文unicode字符范围
        public const string CHINESE_UNICODE = @"[\u4e00-\u9fa5]";

        private string unicodeRegex = null;

        public const string PATH_SEPARATOR = "/";

        /// <summary>
        /// 构造路径匹配器
        /// </summary>
        /// <param name="unicodeRegex">基于unicode的多国语言单字符正则表达式</param>
        public PathMatcher(string unicodeRegex)
        {
            this.unicodeRegex = unicodeRegex;
        }

        /// <summary>
        /// 默认使用中文unicode字符正则表达式
        /// </summary>
        public PathMatcher()
        {
        }

        private string[] DevidePath(string path)
        {
            string[] devided = path.Trim().Split(new string[] {PATH_SEPARATOR}, StringSplitOptions.RemoveEmptyEntries);
            IList<string> trimed = new List<string>(devided.Length);
            foreach (string d in devided)
            {
                trimed.Add(d.Trim());
            }
            return trimed.ToArray();
        }

        /// <summary>
        /// 判断指定的表达式是否匹配指定路径
        /// </summary>
        /// <param name="regex">？表示匹配单个字符（包括指定的多国语言），*匹配多个字符，**匹配多个目录</param>
        /// <param name="path">需要匹配的资源路径</param>
        public bool Match(string regex, string path)
        {
            if (path.StartsWith(PATH_SEPARATOR) != regex.StartsWith(PATH_SEPARATOR))
                return false;
            string[] pattDirs = DevidePath(regex);
            string[] pathDirs = DevidePath(path);

            int pattIdxStart = 0;
            int pattIdxEnd = pattDirs.Length - 1;
            int pathIdxStart = 0;
            int pathIdxEnd = pathDirs.Length - 1;
            
            //如果找到第一个**，则跳出
            while (pattIdxStart <= pattIdxEnd && pathIdxStart <= pathIdxEnd) 
            {
                string patDir = pattDirs[pattIdxStart];
			    if ("**".Equals(patDir)) 
				    break;
			    if (!IsMatch(patDir, pathDirs[pathIdxStart])) 
                {
			        //如果路径不为**，则路径段必须匹配
				    return false;
			    }
			    pattIdxStart++;
			    pathIdxStart++;
		    }
            
            if (pathIdxStart > pathIdxEnd) 
            {
                // 路径长度小于匹配模式长度，则模式剩余部分必须为*或者**才可能匹配
                if (pattIdxStart > pattIdxEnd) 
                {
                    //路径和模式长度正好相同
                    return (regex.EndsWith(PATH_SEPARATOR) ? path.EndsWith(PATH_SEPARATOR) : !path.EndsWith(PATH_SEPARATOR));
                }
                if (pattIdxStart == pattIdxEnd && pattDirs[pattIdxStart].Equals("*") && path.EndsWith(PATH_SEPARATOR))
                    return true;//模式结束于*，需要匹配任意资源而非目录，则路径必须结束于分隔符（资源是具有路径的）
                for (int i = pattIdxStart; i <= pattIdxEnd; i++) 
                {
				    if (!pattDirs[i].Equals("**"))
                    {
					    return false;//剩余部分必须为**
				    }
                }
                return true;
            }
            else if (pattIdxStart > pattIdxEnd) 
            {
                // 模式中没有**，且模式长度小于路径长度，则必然不匹配
                return false;
            }

            //模式部分上一个循环由于遇到**跳出则开始倒着找最后一个**，将最后的部分匹配完
            while (pattIdxStart <= pattIdxEnd && pathIdxStart <= pathIdxEnd) 
            {
                string patDir = pattDirs[pattIdxEnd];
                if (patDir.Equals("**"))
                    break;
                if (!IsMatch(patDir, pathDirs[pathIdxEnd]))
                    return false;
                pattIdxEnd--;
                pathIdxEnd--;
            }
            
            // 路径长度比模式短（从路径最后倒数），模式每段必须为**
            if (pathIdxStart > pathIdxEnd) 
            {
                for (int i = pattIdxStart; i <= pattIdxEnd; i++) 
                {
                    if (!pattDirs[i].Equals("**"))
                        return false;
                }
                return true;
            }
            
            //模式最后和开头都匹配完毕，开始匹配中间部分（pattIdxStart指向**）
            //模式部分倒数遇到**且模式比路径长度短
            while (pattIdxStart != pattIdxEnd && pathIdxStart <= pathIdxEnd) 
            {
                int patIdxTmp = -1;
                //模式倒数部分遇到**才进入此循环
                for (int i = pattIdxStart + 1; i <= pattIdxEnd; i++) 
                {
                    if (pattDirs[i].Equals("**")) 
                    {
                        //找到模式start和end之间第一个**位置
                        patIdxTmp = i;
                        break;
                    }
                }
                if (patIdxTmp == pattIdxStart + 1) 
                {
                    // 出现连续的任意目录'**/**'可以跳过（类似一个**）
                    pattIdxStart++;
                    continue;
                }
                
                // 找到模式中**之后的子路径匹配
			    int patLength = (patIdxTmp - pattIdxStart - 1);
			    int strLength = (pathIdxEnd - pathIdxStart + 1);
			    int foundIdx = -1;

                strLoop: for (int i = 0; i <= strLength - patLength; i++) 
                {
				    for (int j = 0; j < patLength; j++) 
                    {
					    string subPat = pattDirs[pattIdxStart + j + 1];
					    string subStr = pathDirs[pathIdxStart + i + j];
					    if (!IsMatch(subPat, subStr)) 
						    goto strLoop;//path中找第一个不匹配的位置（计入模式中的**）
				    }
                    //后面都匹配，则子串匹配跳出循环
				    foundIdx = pathIdxStart + i;
				    break;
			    }
                if (foundIdx == -1)
                    return false;
                //继续查找下一个子串
                pattIdxStart = patIdxTmp;
                pathIdxStart = foundIdx + patLength;
            }

		    for (int i = pattIdxStart; i <= pattIdxEnd; i++) 
            {
			    if (!pattDirs[i].Equals("**"))
				    return false;
		    }
            return true;
        }

        /// <summary>
        /// 判断指定正则表达式是否匹配指定字符
        /// </summary>
        /// <param name="regex">包含*或者？的正则表达式</param>
        /// <param name="str">要匹配的字符</param>
        private bool IsMatch(string regex, string str)
        {
            PreRegexString matcher = new PreRegexString(regex, str, unicodeRegex);
            return matcher.IsMatch();
        }

       
    }

    /// <summary>
    /// 预编译的字符，他将？*替换为标准的正则表达式，表达式需要能够支持unicode字符范围
    /// </summary>
    public class PreRegexString
    {
        private static readonly Regex PATH_REGEX = new Regex(@"\?|\*");

        private Regex regex;

        private string path;

        private string unicodeRegex = PathMatcher.CHINESE_UNICODE;

        /// <summary>
        /// 使用指定的正则表达式构建
        /// </summary>
        /// <param name="regex">？表示任意一个字符，*表示任意多个字符，**表示任意目录</param>
        /// <param name="path">要匹配的路径字符</param>
        /// <param name="unicodeRegex">支持多国语言的unicode字符范围正则表达式，如果为空则为中文unicode范围</param>
        public PreRegexString(string regex, string path, string unicodeRegex)
        {
            this.path = path;
            if (!string.IsNullOrEmpty(unicodeRegex))
                this.unicodeRegex = unicodeRegex;
            this.regex = CreateRegex(regex);
        }

        private string SubString(string s, int start, int length)
        {
            if (length == 0)
            {
                return "";
            }
            return s.Substring(start, length);
        }

        /// <summary>
        /// 将？*替换为标准的正则表达式，表达式需要能够支持中文字符
        /// </summary>
        private Regex CreateRegex(string regex)
        {
            StringBuilder regexBuilder = new StringBuilder();
            Match m = PATH_REGEX.Match(regex);
            int end = 0;
            while (m.Success)
            {
                regexBuilder.Append(SubString(regex, end, m.Index - end));
                string match = m.Value;
                if ("?".Equals(match))
                {
                    //匹配单个字符（包括任何中文字符）
                    regexBuilder.Append(".|").Append(unicodeRegex);
                }
                else if ("*".Equals(match))
                {
                    regexBuilder.Append("(.|").Append(unicodeRegex).Append(")*");
                }
                end = m.Index + m.Length;
                m = m.NextMatch();
            }
            regexBuilder.Append(SubString(regex, end, regex.Length - end));
            return new Regex(regexBuilder.ToString());
        }

        public bool IsMatch()
        {
            if (this.regex.IsMatch(path))
            {
                return true;
            }
            return false;
        }
    }
}
