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

package org.crystalwall.permission.def;

import org.crystalwall.permission.PermissionDefinitionException;
import java.util.Collection;
import java.util.Collections;

/**
 * 包含权限信息对象的权限定义集合
 * @author vincent valenlee
 */
public interface PermissionDefinition {

    public Collection<PermissionInfo> getPermissionInfos();
    
    /**
     * 在权限定义中添加新的权限信息对象
     * @param pinfo 新权限信息对象
     * @throws org.crystalwall.permission.PermissionDefinitionException 如果是联合异常，则抛出CombineException异常，
     * 否则抛出PermissionDefinitionException，其他的运行时异常除UnsupportedOperationException将封装到此异常中外，均直接抛出
     */
    public void addPermissionInfo(PermissionInfo pinfo) throws PermissionDefinitionException;
    
    /**
     * 联合其他权限定义
     * @param pdef 其他权限定义
     * @return 联合之后的新权限定义
     * @throws org.crystalwall.permission.PermissionDefinitionException 如果是联合异常，则抛出CombineException异常，
     * 否则抛出PermissionDefinitionException，其他的运行时异常除UnsupportedOperationException将封装到此异常中外，均直接抛出
     */
    public PermissionDefinition combine(PermissionDefinition pdef) throws PermissionDefinitionException;
    
    public boolean contains(PermissionInfo info);
    
    public void removePermissionInfo(PermissionInfo pinfo) throws PermissionDefinitionException;
    
    /**
     * 获取匹配指定名字和指定类型的权限信息对象集合。如果名字为null或者名字中没有匹配
     * 的权限，则返回指定类型的全部权限集合
     * @param name 权限信息对象的名字，具体由实现解析
     * @param type 权限类型全名
     */
    public Collection<PermissionInfo> getPermissionInfos(String name, String type);
    
    /**
   * 只包含PermissionInfo.allPermissionInfo权限信息的权限定义，使用此权限定义要非常小心，因为
   * 此权限定义包含的权限没有任何限制！
   */
    public static PermissionDefinition  ALL_PERMISSION_DEF = new PermissionDefinition () {

        public Collection<PermissionInfo> getPermissionInfos() {
            return Collections.singleton(PermissionInfo.ALL_PERMISSION_INFO);
        }

        public void addPermissionInfo(PermissionInfo pinfo) {
            //throw new UnsupportedOperationException("Not supported yet.");
        }

        public PermissionDefinition combine(PermissionDefinition pdef) {
            throw new CombineException(new UnsupportedOperationException(
                    "the ALL_PERMISSION_DEF do not support combine another permission definition."));
        }

        public boolean contains(PermissionInfo info) {
            if (info == PermissionInfo.ALL_PERMISSION_INFO) 
                return true;
            return false;
        }

        public void removePermissionInfo(PermissionInfo pinfo) {
//            throw new UnsupportedOperationException("Not supported yet.");
        }

        public Collection<PermissionInfo> getPermissionInfos(String name, String type) {
            if (name == null) {
                if (PermissionInfo.ALL_PERMISSION_INFO.getType().equalsIgnoreCase(type)) {
                    return getPermissionInfos();
                }
            } else if (PermissionInfo.ALL_PERMISSION_INFO.getName().equalsIgnoreCase(name)) {
                return getPermissionInfos();
            }
            return Collections.emptyList();
        }
    };
}
