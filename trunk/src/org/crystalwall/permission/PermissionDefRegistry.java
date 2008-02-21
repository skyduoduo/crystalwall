/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

package org.crystalwall.permission;

import java.util.Iterator;
import org.crystalwall.permission.def.PermissionDefinition;

/**
 * 权限信息注册表接口用于注册权限定义对象
 * @author vincent valenlee
 */
public interface PermissionDefRegistry  {
    
    /**
     * 获取权限定义注册表的名字。这个名字在系统中可以唯一，也可以不唯一。
     * 具体情况由开发者决定
     * @return
     */
    public String getName();
     /**
    * 获取指定安全对象上匹配的权限定义
    * @param secur
    * @return
    */
     public PermissionDefinition getPermissionDefinition(Object secur) throws IllegalArgumentException;
     
     /**
    * 获取此注册表中的所有权限定义集合的迭代
    * @return
    */
     public Iterator getPermissionDefinitions();
}
