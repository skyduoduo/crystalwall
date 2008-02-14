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

import java.security.AllPermission;
import java.security.Permission;

/**
 * 描述权限信息的对象
 * @author vincent valenlee
 */
class PermissionInfo {

    private String name;
    
    private String action;
    
    //java.security.Permission的Class全称
    private String type;

    
    private final static Permission allPermission = new AllPermission();
    
    /**
   * 这个权限信息标示AllPermission权限，要小心使用此权限信息，这意味着使用此权限
   * 信息做的任何操作，没有任何限制！
   */
    public final static PermissionInfo ALL_PERMISSION_INFO = new PermissionInfo(allPermission.getName(), allPermission.getActions(), allPermission.getClass().getName());

    public PermissionInfo(String name, String action, String type) {
        this.name = name;
        this.action = action;
        this.type = type;
    }
    
    public String getAction() {
        return action;
    }

    public void setAction(String action) {
        this.action = action;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getType() {
        return type;
    }

    public void setType(String type) {
        this.type = type;
    }
    
    /**
   * 根据字符串形式的权限信息将其解码成对象，字符串的格式为：
   * 
   * @param permissionInfo
   * @return
   */
    public static PermissionInfo decode(String permissionInfo) {
        return null;
    }
    
    /**
   * @return 将权限信息以指定的编码格式返回
   */
    public String toString() {
        return null;
    }

}
