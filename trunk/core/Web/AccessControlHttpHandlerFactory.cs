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
using System.Web;
using System.Web.UI;

namespace CrystalWall.Web
{
    /// <summary>
    /// 用于创建Page对象的处理器工厂，在构建页面后，将使用ASPNetPageCrystalWallSite将权限控制加入页面中。
    /// 创建web应用时，应该将此handler加入到web.config中system.web配置组下的httpHandlers配置节下
    /// </summary>
    /// <author>vincent valenlee</author>
    public class AccessControlHttpHandlerFactory : IHttpHandlerFactory
    {
        private static IHttpHandlerFactory pageFactory;

        private static IHttpHandlerFactory CreatePageFactory()
        {
            if (pageFactory == null)
            {
                pageFactory = Activator.CreateInstance(typeof(PageHandlerFactory), true) as IHttpHandlerFactory;
                if (pageFactory == null)
                {
                    throw new ApplicationException("无法初始化PageHandlerFactory");
                }
            }
            return pageFactory;
        }

        public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
        {
            IHttpHandlerFactory pageFactory = CreatePageFactory();
            Page page = pageFactory.GetHandler(context, requestType, url, pathTranslated) as Page;
            //加入权限控制
            CrystalWallSite site = CrystalWallSite.Find(page);
            site.InitSite();//可选
            //这里将判断权限，默认将控件可见权限和事件权限加入页面事件中，而并不立即执行检查
            bool result = true;
            site.Decider.Decide(PrincipalTokenHolder.CurrentPrincipal, context, out result);
            return page;
        }

        public void ReleaseHandler(IHttpHandler handler)
        {
            IHttpHandlerFactory pageFactory = CreatePageFactory();
            pageFactory.ReleaseHandler(handler);
        }
    }
}
