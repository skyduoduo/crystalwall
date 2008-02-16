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

import java.util.List;
import org.crystalwall.AuthenticationToken;
import org.crystalwall.permission.PermissionResolver;
import org.crystalwall.permission.def.PermissionDefinition;

/**
 * 少数服从多数的投票者链表
 * @author vincent valenlee
 */
public class ConsensusVoter extends AbstractVoterChain{

    private VoterChain parent;

    public ConsensusVoter(VoterChain parent) {
        this.parent = parent;
    }

    protected int doVote(AuthenticationToken token, Object secur, PermissionDefinition permissionDef) {
        int grant = 0, denied = 0, resu = Voter.ABSTAIN;
        PermissionDefinition pdef = permissionDef;
        for (int i = 0; i < getVoters().size(); i++) {
            Voter voter = getVoters().get(i);
            if (voter.getDefaultPermissionDefinition() != null) {
                pdef = voter.getDefaultPermissionDefinition().combine(permissionDef);
            }
            if (voter.support(secur.getClass())) {
                int result = voter.vote(token, secur, pdef);
               if (result == Voter.APPROVED) {
                   grant++;
               } else if (result == Voter.DENIED) {
                   denied++;
               }
            }
        }
        if (grant > denied) {
            resu = Voter.APPROVED;
        } else if (grant < denied) {
            resu = Voter.DENIED;
        }
        return resu;
    }

    public boolean support(Class clazz) {
        for (Voter voter : getVoters()) {
            if (voter.support(clazz))
                return true;
        }
        return false;
    }

    public VoterChain getVoterChain() {
       return parent;
    }
}
