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

package org.crystalwall.permission.def;

import org.crystalwall.permission.PermissionDefinitionException;

/**
 * 合并时发生的异常
 * @author vincent valenlee
 */
public class CombineException extends PermissionDefinitionException {

    public CombineException(Throwable cause) {
        super(cause);
    }

    public CombineException(String message, Throwable cause) {
        super(message, cause);
    }

    public CombineException(String message) {
        super(message);
    }
}
