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

/**
 * 权限定义注册者对象用于以自己的方式注册自定义的权限定义对象到指定的
 * 权限定义注册表中。此对象将注册算法过程与注册表分离，提供开发者灵活
 * 的方式管理注册过程
 * @author vincent valenlee
 */
public interface PermissionDefinitionRegistrar {

    /**
     * 在指定的权限定义注册表中注册权限定义
     * @param pregistry 权限定义注册表
     */
    public void registerPermissionDefinitions(PermissionDefRegistry pregistry);
    
    /**
     * 以自定义的方式注销指定注册表中的相关权限定义
     * @param pregistry
     */
    public void unRegisterPermissionDefinitions(PermissionDefRegistry pregistry);
    
}
