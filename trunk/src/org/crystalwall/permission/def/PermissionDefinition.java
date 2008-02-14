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

import java.util.Collection;
import java.util.Collections;

/**
 * 包含权限信息对象的定义集合
 * @author vincent valenlee
 */
public interface PermissionDefinition {

    public Collection<PermissionInfo> getPermissionInfos();
    
    public void addPermissionInfo(PermissionInfo pinfo);
    
    public PermissionDefinition combine(PermissionDefinition pdef);
    
    public boolean contains(PermissionInfo info);
    
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
            throw new UnsupportedOperationException("Not supported yet.");
        }

        public boolean contains(PermissionInfo info) {
            if (info == PermissionInfo.ALL_PERMISSION_INFO) 
                return true;
            return false;
        }
    };
}
