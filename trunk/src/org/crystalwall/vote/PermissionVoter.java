/*
 *  Copyright 2008 the original author or authors.
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

package org.crystalwall.vote;

import com.google.common.collect.Lists;
import java.security.Permission;
import java.util.List;
import org.crystalwall.AuthenticationToken;
import org.crystalwall.permission.resolve.AllPermissionResolver;
import org.crystalwall.permission.PermissionResolvedException;
import org.crystalwall.permission.PermissionResolver;
import org.crystalwall.permission.def.PermissionDefinition;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

/**
 * 根据解析的权限进行投票的投票者
 * @author vincent valenlee
 */
public class PermissionVoter extends AbstractVoter {

    protected final static Logger logger = LoggerFactory.getLogger(PermissionVoter.class);
    
    @Override
    protected int doVote(AuthenticationToken token, Object secur, PermissionDefinition pdef) {
        PermissionResolver resolver = getResolveAbility(secur);
        if (resolver == null) {
            resolver = new AllPermissionResolver();//使用AllPermission权限解析器
        }
        List<ResolverIndex> oldResolvers = Lists.newArrayList();
        Permission securityLock = null;
        do {
            try {
                securityLock = resolver.resolve(secur, pdef);
                reback(oldResolvers, getResolvers());
                oldResolvers = null;
            } catch (IllegalArgumentException e) {
                logger.warn("the security object[]" + secur.getClass() + "-" + secur.toString() + " is illegal...", e);
                return Voter.DENIED;//能够解析，但无效的参数将投反对票
            } catch (PermissionResolvedException e) {
            //解析异常，将根据设置决定是否循环到解析器列表的后续解析器进行解析
                if (!e.isAllowNextResolve()) {
                    logger.debug("the resolver:{}-{} resolve the security object:{}-{} " +
                            "catch resolveException, and the exception set do not allow next resolver resolve" +
                            " so the voter will vote DENIED!", new Object[]{resolver.getClass(), resolver, secur.getClass(), secur});
                    reback(oldResolvers, getResolvers());
                    return Voter.DENIED;
                }
                getResolvers().remove(resolver);
                oldResolvers.add(new ResolverIndex(getResolvers().indexOf(resolver), resolver));
            }
        } while (oldResolvers == null);
       
        if (token.getGrantedAuthorization() != null) {
            if (token.getGrantedAuthorization().implies(securityLock)) {
                //认证令牌隐含安全对象上的权限，则赞成票
                return Voter.APPROVED;
            }
        }
        return Voter.DENIED;
    }

    public boolean support(Class clazz) {
        if (getResolvers() != null) {
            for (PermissionResolver resolver : getResolvers()) {
                if (resolver.support(clazz)) {
                    return true;
                }
            }
            return false;
        }
        //投票者没有设置权限解析器列表，则将在投票中判断权限定义是否包含AllPermission权限，因此这里返回true
        return true;
    }
    
    /**
     * 恢复resolver解析器
     * @param oldResolvers 暂时被删除的解析器
     * @param resolvers 原有解析器列表
     */
    private void reback(List<ResolverIndex> oldResolvers, List<PermissionResolver> resolvers) {
        if (oldResolvers == null || resolvers == null)
            return;
        for (ResolverIndex r : oldResolvers) {
                oldResolvers.remove(r);
                resolvers.add(r.index, r.resolver);
            }
    }

    private class ResolverIndex {
        private PermissionResolver resolver;
        private int index;
        
        public ResolverIndex(int index, PermissionResolver resolver) {
            this.resolver = resolver;
            this.index = index;
        }
    }
    
    
}
