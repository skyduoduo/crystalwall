/*
 *  Copyright 2008 the original author or authors.
 * 
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 * 
 *       http://www.apache.org/licenses/LICENSE-2.0
 * 
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 *  under the License.
 */

package org.crystalwall.permission.resolve;

import org.crystalwall.permission.*;
import java.security.AllPermission;
import java.security.Permission;
import org.crystalwall.permission.def.PermissionDefinition;

/**
 * 总是解析出AllPermission的解析器，经过此权限解析的权限具有最高的权限控制级别。
 * 即：要访问指定的安全对象，必须具有AllPermission权限。
 * @author vincent valenlee
 */
public class AllPermissionResolver implements PermissionResolver {

    public Permission resolve(Object secur, PermissionDefinition pdef) throws IllegalArgumentException, PermissionResolvedException {
        return new AllPermission();
    }

    public boolean support(Class clazz) {
        return true;
    }

}
