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

package org.crystalwall.permission.def;

import org.crystalwall.permission.PermissionDefinitionException;

/**
 * 提供加载权限信息对象的提供者，例如：从XML配置文件中或从数据库中加载。
 * 此接口可以被PermissionDefinitionFactory使用以便构造权限定义对象
 * @author vincent valenlee
 */
public interface PermissionDefProvider {
  
    /**
     * 销毁资源
     */
    public void destroy() throws PermissionDefinitionException;

    /**
     * 初始化权限定义
     * @param pdef 权限定义
     */
    public void init(PermissionDefinition pdef) throws PermissionDefinitionException;

    /**
     * 加载所有的权限定义
     */
    public void loadPermissionInfos() throws PermissionDefinitionException;

    /**
     * <p>配置提供者是否应该重新加载其配置
     */
    public boolean needsReload() throws PermissionDefinitionException;
}
