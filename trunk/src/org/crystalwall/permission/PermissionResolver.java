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

import java.security.Permission;
import org.crystalwall.permission.def.PermissionDefinition;

/**
 * 用于根据指定安全对象（包括安全对象上的操作）解析出Permission权限的解析器
 * @author vincent valenlee
 */
public interface PermissionResolver {

    /**
     * 根据指定安全对象解析出访问安全对象应该具有的权限
     * @param secur 要解析权限的安全对象
     * @param pdef 权限定义
     * @return 返回对安全对象进行保护的权限对象
     * @throws java.lang.IllegalArgumentException 无效的参数异常
     * @throws org.crystalwall.permission.PermissionResolvedException 解析时发生错误，无法解析权限时抛出的异常,如果抛出此异常，将留给后续解析器机会解析权限
     */
    public Permission resolve(Object secur, PermissionDefinition pdef) throws IllegalArgumentException, PermissionResolvedException;

    /**
     * 测试此解析器是否支持解析指定的安全对象类型
     * @param clazz 安全对象类型
     * @return 如果为true，则resolve方法不会抛出java.lang.IllegalArgumentException异常
     */
    public boolean support(Class clazz);
}
