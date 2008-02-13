/*
 *  Copyright 2008 the original athe oruthor or authors.
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

package org.crystalwall;

import com.google.common.collect.Lists;
import java.util.List;
import org.crystalwall.permission.PermissionDefRegistry;
import org.crystalwall.permission.PermissionResolver;

/**
 * 访问策略接口的抽象实现，他使用一个权限定义注册表和一个权限解析器列表供子类使用，子类只需要实现
 * doDecision抽象方法执行认证即可
 * @author vincent valenlee
 */
public abstract class AbstractAccessDecisionStrategy implements IAccessDecisionStrategy{

    private PermissionDefRegistry permissionDefRegistry;
    
    private List<PermissionResolver> resolvers;
    
    public PermissionDefRegistry getPermissionDefRegistry() {
        return permissionDefRegistry;
    }

    public void setPermissionDefRegistry(PermissionDefRegistry permissionDefRegistry) {
        this.permissionDefRegistry = permissionDefRegistry;
    }
    
    public List<PermissionResolver> getResolvers() {
        return resolvers;
    }

    public void setResolvers(List<PermissionResolver> resolvers) {
        this.resolvers = resolvers;
    }

    public void checkAndPretreatment(AuthenticationToken token) throws AuthenticationTokenInvalid {
        //空方法，不执行任何操作，子类可以实现
    }

    public void registPermissionResolver(PermissionResolver resolver) {
        if (resolver != null) {
            if (resolvers == null)
                resolvers = Lists.newArrayList();
            resolvers.add(resolver);
        }
    }
    
    /**
     * 如果令牌无效或没有认证，则抛出AuthenticationTokenInvalid异常
     * @param token 令牌
     * @param secur 要保护的安全对象
     * @throws org.crystalwall.AccessDeniedException
     * @throws org.crystalwall.AuthenticationTokenInvalid
     */
    public void decidedAccess(AuthenticationToken token, Object secur) throws AccessDeniedException, AuthenticationTokenInvalid {
        if (token == null || !token.isAuthenticated()) {
            throw new AuthenticationTokenInvalid("the authentication token do not enticated or it is invalid...");
        }
        doDecision(token, secur, getPermissionDefRegistry());
    }
    
    protected abstract void doDecision(AuthenticationToken token, Object secur, PermissionDefRegistry registry) throws AccessDeniedException;
}
