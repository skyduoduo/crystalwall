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

package org.crystalwall.vote;

import org.crystalwall.AbstractAccessDecisionStrategy;
import org.crystalwall.AccessDeniedException;
import org.crystalwall.AuthenticationToken;
import org.crystalwall.permission.PermissionDefRegistry;
import org.crystalwall.permission.def.PermissionDefinition;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

/**
 * 使用一个投票者进行访问控制决定的策略实现
 * @author vincent valenlee
 */
public class VoterDecisionStrategy extends AbstractAccessDecisionStrategy {

    protected final static Logger logger = LoggerFactory.getLogger(AbstractAccessDecisionStrategy.class);
    
    private Voter voter;
    
    private boolean exceptionWhenAbstain = true;

    public boolean isExceptionWhenAbstain() {
        return exceptionWhenAbstain;
    }

    public void setExceptionWhenAbstain(boolean exceptionWhenAbstain) {
        this.exceptionWhenAbstain = exceptionWhenAbstain;
    }

    public VoterDecisionStrategy(Voter voter) {
        if (voter == null)
            throw new IllegalArgumentException("the voterStrategy must set a nonull voter...");
        this.voter = voter;
    }

    @Override
    protected void doDecision(AuthenticationToken token, Object secur, PermissionDefRegistry registry) throws AccessDeniedException {
        if (registry == null) {
            logger.info("the permissionDef registry is null, VoterDecisionStrategy denied access {}-{} object...", secur.getClass().getName(), secur);
        }
        PermissionDefinition pdef = registry.getPermissionDefinition(secur);
        if (pdef == null) {
            logger.warn("the registry[{}] does not have permissionDefinition about {}-{}", new Object[]{registry.getName(), secur.getClass().getName(), secur});
        }
        int result = voter.vote(token, secur, pdef);
        if (result == Voter.DENIED) {
            throw new AccessDeniedException("access denied!");
        } else if (result == Voter.ABSTAIN) {
            if (isExceptionWhenAbstain()) {//如果是拒绝票，则根据设置决定是否允许访问
                logger.info("the voter vote a ABSTAIN, but the access strategy decided a abstain is denied...");
                throw new AccessDeniedException("access denied!");
            }
        }
    }

}
