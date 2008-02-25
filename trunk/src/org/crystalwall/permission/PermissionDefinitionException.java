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

package org.crystalwall.permission;

import org.crystalwall.CrystalWallException;

/**
 * 权限定义操作中的异常，他包括添加、删除、合并(CombineException)、查找、注册权限定义时的异常。
 * 但也包括了UnsupportedOperationException这个特殊的异常(当执行不支持的操作时）
 * @author vincent valenlee
 */
public class PermissionDefinitionException extends CrystalWallException {

    public PermissionDefinitionException(Throwable cause) {
        super(cause);
    }

    public PermissionDefinitionException(String message, Throwable cause) {
        super(message, cause);
    }

    public PermissionDefinitionException(String message) {
        super(message);
    }
}
