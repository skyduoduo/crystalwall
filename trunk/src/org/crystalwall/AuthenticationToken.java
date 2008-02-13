/*
 *Licensed to the Apache Software Foundation (ASF) under one or more
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

package org.crystalwall;

import java.io.Serializable;
import java.security.PermissionCollection;

/**
 * 需要被认证的身份令牌，他标识一个运行实体（人、机器或其他）的一组相关安全信息
 * @author vincent valenlee
 */
public interface AuthenticationToken extends Serializable {

    /**
     * @return 获取令牌的唯一名字
     */
    public String getName();
    
    /**
     * @return 令牌是否已经被认证通过
     */
    public boolean isAuthenticated(); 
    
    /**
     * 设置此令牌是否已经被认证
     * @param authicated
     */
    public void setAutenticated(boolean authicated);
    
    /**
     * @return 获取令牌的身份证明对象，通常为密码，但也可以为其他形式，例如：数字证书
     */
    public Object getCertificates();
    
    /**
     * @return 获取令牌的身份标识对象，通常是一个用户名字、登陆id等，但也可以是数字密钥
     */
    public Object getPrincipal();
    
    /**
     * @return 获取令牌的其他详细信息
     */
    public Object getDetails();
    
    /**
     * @return 获取令牌在认证之后拥有的被授予的许可对象，如果认证不通过，则此方法返回null
     */
    public PermissionCollection getGrantedAuthorization();
}
