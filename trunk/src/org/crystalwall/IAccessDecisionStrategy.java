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

package org.crystalwall;

import java.util.List;
import org.crystalwall.permission.PermissionResolver;

/**
 * 访问决定策略对象用于决定一个身份令牌对一个安全对象的访问权限是否许可
 * @author vincent valenlee
 */
public interface IAccessDecisionStrategy {

    /**
     * 检查一个令牌是否能够访问一个安全对象
     * @param token 要决定的令牌
     * @param secur 要保护的安全对象
     * @throws org.crystalwall.AccessDeniedException
     */
    public void decidedAccess(AuthenticationToken token, Object secur) throws AccessDeniedException, AuthenticationTokenInvalid;
    
//    /**
//    * 获取用于解析出Permission权限的解析器
//    * @return
//    */
//     public List<PermissionResolver> getResolvers();
//     
//     /**
//      * 注册能够解析指定
//      * @param securClazz
//      * @param resolver
//      */
//     public void registPermissionResolver(PermissionResolver resolver);
    
    /**
     * 在决定之前检查令牌是否有效并预处理令牌。这个职责可以用于支持不同类型的令牌进行处理
     * @param token
     * @throws org.crystalwall.AuthenticationTokenInvalid
     */
    public void checkAndPretreatment(AuthenticationToken token) throws AuthenticationTokenInvalid;
}
