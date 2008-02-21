/*
 * Copyright 2008 the original author or authors.
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

/**
 * 支持能够动态改变内部存储的权限定义的工厂
 * @author vincent
 */
public interface MutablePermissionDefinitionFactory extends PermissionDefinitionFactory {

    /**
     * 动态注册一个权限定义到工厂中
     * @param pdef
     */
    public void registryDefinition(PermissionDefinition pdef) ;
    
    /**
     * 从工厂中动态注销一个权限定义，如果不存在，此方法不执行任何操作
     * @param pdef
     */
    public void unRegistryDefinition(PermissionDefinition pdef);
}
