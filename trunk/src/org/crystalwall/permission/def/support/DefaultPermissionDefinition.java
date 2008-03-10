/*
 *  Copyright 2008 author or authors.
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

package org.crystalwall.permission.def.support;

import java.util.Collection;
import org.crystalwall.permission.PermissionDefinitionException;
import org.crystalwall.permission.def.PermissionDefinition;
import org.crystalwall.permission.def.PermissionInfo;

/**
 * 权限定义接口的默认实现，内部的信息将由权限定义提供者来初始化
 * @author vincent valenlee
 */
public class DefaultPermissionDefinition implements PermissionDefinition {

    public Collection<PermissionInfo> getPermissionInfos() {
        throw new UnsupportedOperationException("Not supported yet.");
    }

    public void addPermissionInfo(PermissionInfo pinfo) throws PermissionDefinitionException {
        throw new UnsupportedOperationException("Not supported yet.");
    }

    public PermissionDefinition combine(PermissionDefinition pdef) throws PermissionDefinitionException {
        throw new UnsupportedOperationException("Not supported yet.");
    }

    public boolean contains(PermissionInfo info) {
        throw new UnsupportedOperationException("Not supported yet.");
    }

    public void removePermissionInfo(PermissionInfo pinfo) throws PermissionDefinitionException {
        throw new UnsupportedOperationException("Not supported yet.");
    }

    public Collection<PermissionInfo> getPermissionInfos(String name, String type) {
        throw new UnsupportedOperationException("Not supported yet.");
    }

    @Override
    public Object clone() throws CloneNotSupportedException {
        return super.clone();
    }

    public PermissionDefinition forNew() {
        throw new UnsupportedOperationException("Not supported yet.");
    }

    public boolean isInit() {
        throw new UnsupportedOperationException("Not supported yet.");
    }

}
