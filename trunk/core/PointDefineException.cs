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

namespace CrystalWall
{
    /// <summary>
    /// 权限点定义异常，当权限点无法解析或定义错误时发生的异常。他包含权限点元特性定义以及定义的类全名
    /// </summary>
    public class PointDefineException: ApplicationException
    {
        private PermissionPointAttribute pointAttribute;

        public PermissionPointAttribute PointAttribute
        {
            get { return pointAttribute; }
        }

        private string contextClass;

        public string ContextClass
        {
            get { return contextClass; }
        }

        public PointDefineException(PermissionPointAttribute pointAttribute, string contextClass, string message)
            : base(message)
        {
            this.pointAttribute = pointAttribute;
            this.contextClass = contextClass;
        }
    }
}
