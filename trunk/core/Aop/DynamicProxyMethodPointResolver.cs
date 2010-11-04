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
using Castle.DynamicProxy;
using CrystalWall.Utils;

namespace CrystalWall.Aop
{
    /// <summary>
    /// 通过Castle动态代理的IInvocation对象，支持获取执行的方法上定义的PermissionPointAttribute
    /// 权限点属性的权限点解析策略实现。此类需要Castle动态代理的支持。此类将默认配置到CrystalwallSite中
    /// </summary>
    public class DynamicProxyMethodPointResolver: IPointResolveStrategy
    {
        public PermissionPoint[] Resolve(object context)
        {
            IInvocation invocation = context as IInvocation;
            if (invocation == null)
                return null;//此策略只支持通过Castle动态代理对象获取方法上定义的权限点
            PermissionPointAttribute[] pointAttrs = (PermissionPointAttribute[])invocation.MethodInvocationTarget.GetCustomAttributes(typeof(PermissionPointAttribute), true);
            if (pointAttrs == null || pointAttrs.Length == 0)
              return null;//没有定义，则不解析
            PermissionPoint[] points = new PermissionPoint[pointAttrs.Length];
            int i = 0;
            foreach (PermissionPointAttribute attr in pointAttrs)
            {
                try
                {
                    Type t = Type.GetType(attr.Type);
                    PermissionPoint point = (PermissionPoint)t.GetConstructor(new Type[0]).Invoke(new object[0]);
                    point.Name = attr.Name;
                    point.Resource = attr.Resource;
                    point.Action = attr.Action;
                    point.Context = invocation.Proxy;//TODO:需测试代理目标对象
                    point.Member = invocation.MethodInvocationTarget;
                    //获取参数
                    point.Args = new object[invocation.Arguments.Length];
                    for (int j = 0; j < invocation.Arguments.Length; j++)
                    {
                        point.Args[j] = invocation.Arguments[j];
                    }
                    points[i++] = point;
                }
                catch
                {
                    //其中之一有错误，记录日志，但继续解析其他权限点
                    ServiceManager.LoggingService.Fatal("方法" + invocation.MethodInvocationTarget.Name 
                                                                              + "上的权限点：" 
                                                                              + attr.Name 
                                                                              + "[type:"
                                                                              + attr.Type
                                                                              + "] 错误，无法获得类型或调用无惨构造函数构造!");
                }
            }
            return points;
        }
    }
}
