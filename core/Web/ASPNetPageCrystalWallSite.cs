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
using System.Web.UI;
using System.Reflection;
using CrystalWall.Utils;

namespace CrystalWall.Web
{
    /// <summary>
    /// 用于处理ASP.NET页面的Site。PagePoint权限点标注在页面的各个控件上，如：
    /// <code>
    /// public YourPage: Page {
    ///   [PermissionPoint(name="控件id" type="ControlPermissionPoint", action="visiable(可不标注，默认为不隐藏）,event:operation,event2:operation2..."]
    ///   private Button button;
    ///   .....
    /// }
    /// </code>
    /// 此权限上下文读取当前页面上所有使用以上point标注的控件，将检查控件的是否隐藏，各种事件的action等解析为PagePermissionInfo，然后在控件的相应
    /// 渲染事件上添加一个事件，这个事件对应action中的原始事件，每次执行原始事件时，都将调用对应的PageControlDecider检测权限
    /// 注意：如果PermissionPoint定义的控件id是在其他容器控件的子控件，请使用$符号分隔，例如panel1$panel2$labelId。
    /// 此site内部具有两个decider，第一个decider将在Init事件中安装，检测具有visiable的权限点的控件，如果当前用户不具有此权限，将其visiable设置为false。
    /// 第二个decider在PreLoad事件中安装，用于检测相应事件权限
    /// </summary>
    /// <author>vincent valenlee</author>
    public class ASPNetPageCrystalWallSite : CrystalWallSite
    {
        public override IAccessDecider Decider
        {
            get
            {
                InitSite();
                if (DeciderSection.Class.Equals(CrystalWall.Config.DeciderSection.DEFAULT_DECIDER_CLASS) && Decider.GetType() != typeof(AspPageDecider))
                {
                    //默认的配置或者没有配置，则使用AspPageDecider覆盖
                    Decider = new AspPageDecider(new AspPageControlEventDecider(this), new AspPageControlViewDecider(this));
                    return Decider;
                }
                return base.Decider;
            }
        }

        /// <summary>
        /// 获取容器内的子控件
        /// </summary>
        /// <param name="controlIDInPointName">
        /// id的命名规范为使用$分隔。如果控件在母版页中，则uniqueID第一部分为最顶层的ContentPlaceHolder的id。
        /// 例如：最顶层母版页的ContentPlaceHolder-id$容器1id$容器2id$控件id
        /// </param>
        public static Control FindControlInContainer(Page page, string controlIDInPointName)
        {
            MasterPage top = page.Master;
            if (top == null)
            {
                //没有使用母版页
                return FindControlInContainerWithNoMastPage(page, controlIDInPointName);
            }
            while (top.Master != null)
            {
                //找到最顶层模板页
                top = top.Master;
            }
            //具有母版页，解析第一部分$为ContentPlaceHolder
            int first = controlIDInPointName.IndexOf("$");
            Control container = top.FindControl(controlIDInPointName.Substring(0, first));
            return FindControlInContainerWithNoMastPage(container, controlIDInPointName.Substring(first + 1));
        }

        private static Control FindControlInContainerWithNoMastPage(Control container, string uniqueID)
        {
            if (!uniqueID.Contains("$"))
                return container.FindControl(uniqueID);
            string[] ids = uniqueID.Split('$');
            Control parent = container.FindControl(ids[0]);
            if (parent != null)
            {
                for (int i = 1; i < ids.Length; i++)
                {
                    parent = parent.FindControl(ids[i]);
                }
            }
            return parent;
        }

        /// <summary>
        /// 获取指定Page页面中控件域上定义的权限点信息列表。如果没有定义则返回空列表
        /// </summary>
        public IList<PermissionPoint> GetPoints(Page page, Func<PermissionPoint, bool> predicate)
        {
            FieldInfo[] fields = page.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            IList<PermissionPoint> points = new List<PermissionPoint>();
            foreach (FieldInfo info in fields)
            {
                if (info.FieldType.IsSubclassOf(typeof(Control)) && info.GetCustomAttributes(typeof(PermissionPointAttribute), false).Length != 0)
                {
                    //在控件字段上定义了PermissionPointAttribute元特性(目前只支持一个）
                    PermissionPointAttribute pattr = (PermissionPointAttribute)info.GetCustomAttributes(typeof(PermissionPointAttribute), false)[0];
                    if (pattr.Action != null)
                    {
                        string[] action;
                        if (pattr.Action.Contains(","))
                        {
                            //解析以逗号分隔的action列表
                            action = pattr.Action.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        }
                        else
                        {
                            //没有逗号，单独一个point action
                            action = new string[1];
                            action[0] = pattr.Action;
                        }

                        foreach (string act in action)
                        {
                            ControlPermissionPoint point;
                            if (pattr.Type != null)
                            {
                                Type t = Type.GetType(pattr.Type);
                                point = (ControlPermissionPoint)t.GetConstructor(new Type[0]).Invoke(new object[0]);
                            }
                            else
                            {
                                point = new ControlPermissionPoint();
                            }
                            point.Name = pattr.Name.Substring(pattr.Name.LastIndexOf('#') + 1);//控件id
                            point.Resource = page.Request.Path;//权限点资源对应所在页面的虚拟路径
                            point.Action = act;//定义的操作
                            point.Control = ASPNetPageCrystalWallSite.FindControlInContainer(page, pattr.Name);//运行时加入控件实例
                            points.Add(point);
                        }
                    }
                }
            }
            if (predicate != null)
            {
                return points.Where(predicate).ToList();
            }
            else
            {
                return points;
            }
        }
    }
}
