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
     * 获取指定安全对象上匹配的权限定义。
     * <p>注意：一般来说，在权限定义注册表中应该根据Class类型获取其上匹配的权限定义对象，但这里
     * 参数类型并不是Class，这就允许实现者可以实现更多的根据安全对象动态的组装匹配的权限定义的策略
     * @param secur 安全对象，也可以是安全Class
     * @return 匹配的权限定义
     */
     public PermissionDefinition findPermissionDefinition(Object secur) throws IllegalArgumentException;
     
     /**
    * 获取此注册表中的所有权限定义集合的迭代
    * @return
    */
     public Iterator getPermissionDefinitions();
     
     /**
      * 注册能应用到指定安全对象上的权限定义对象。安全对象只是一个方便的参数，
      * 具体注册表的实现将决定是否使用此参数或不使用此参数
      * @param secur 安全对象  
      * @param def 权限信息对象
      */
     public void registerPermissionDefinition(Object secur, PermissionDefinition def);
}
