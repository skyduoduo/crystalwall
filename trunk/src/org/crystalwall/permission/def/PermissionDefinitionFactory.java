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

/**
 * 用于创建权限定义的工厂
 * @author vincent valenlee
 */
public interface PermissionDefinitionFactory {

    /**
   * @return 创建权限定义工厂
   */
    public PermissionDefinition getDefinition();
    
    /**
   * 生成PermissionDefinition.ALL_PERMISSION_DEF的工厂
   */
    public static PermissionDefinitionFactory ALL_PERMISSION_DEF_FACTORY =  new PermissionDefinitionFactory() {
        public PermissionDefinition getDefinition() {
            return PermissionDefinition.ALL_PERMISSION_DEF;
        }
    };
}
