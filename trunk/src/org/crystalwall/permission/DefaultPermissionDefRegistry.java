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

package org.crystalwall.permission;

import java.util.Iterator;
import java.util.Map;
import org.crystalwall.permission.def.PermissionDefinition;

/**
 * 使用安全对象的Class作为key，存储对应的权限定义对象的注册表实现。
 * 如果重复注册相同Class上的权限定义，默认将使用权限定义的combine
 * 方法合并相同Class安全对象上的权限定义
 * @author vincent valenlee
 */
public class DefaultPermissionDefRegistry implements PermissionDefRegistry{

    private String name;
    
    private Map definistions;
    
    public DefaultPermissionDefRegistry(String name) {
        this.name = name;
    }

    public Map getDefinistions() {
        return definistions;
    }

    public void setDefinistions(Map definistions) {
        this.definistions = definistions;
    }

    
    public String getName() {
        return name;
    }

    public PermissionDefinition findPermissionDefinition(Object secur) throws IllegalArgumentException {
        throw new UnsupportedOperationException("Not supported yet.");
    }

    public Iterator getPermissionDefinitions() {
        return getDefinistions().values().iterator();
    }

    public void registerPermissionDefinition(Object secur, PermissionDefinition def) {
        throw new UnsupportedOperationException("Not supported yet.");
    }

}
