/*
 *  Copyright 2008 vincent.
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

import com.google.common.base.Predicate;
import com.google.common.collect.Iterables;
import java.util.LinkedList;
import java.util.List;
import org.crystalwall.permission.PermissionDefRegistry;
import org.crystalwall.permission.PermissionResolver;

/**
 * 投票者链的抽象超类，他将使用内部包含的投票者链表进行投票。默认的处理结果的策略是
 * 少数服从多数投票策略（也就是说，如果内部包含的投票者链中赞成票大于反对票则投票通过）
 * @author vincent valenlee
 */
public abstract class AbstractVoterChain extends AbstractVoter implements VoterChain  {

    private List<Voter> voters = new LinkedList<Voter>();

    public List<Voter> getVoters() {
        return voters;
    }

    public void setVoters(List<Voter> voters) {
        this.voters = voters;
    }

    public void addAfter(Voter after, Voter added) {
        add(after, added, false);
    }

    public void addBefore(Voter before, Voter added) {
        add(before, added, true);
    }

    private void add(Voter v, Voter added, boolean before) {
        int index = getVoters().indexOf(v);
        if (index == -1) {
            getVoters().add(added);
        } else {
            if ( !before) {
               index++; 
            }
            getVoters().add(index, added);
        }
    }
    
    public void addVoter(Voter voter) {
       getVoters().add(voter);
    }

    public List<Voter> getVoters(Class securClazz) {
        List<Voter> v = null;
        final Class clazz = securClazz;
        synchronized(voters) {
            v = (List<Voter>)Iterables.filter(voters, new Predicate<Voter>(){
                public boolean apply(Voter source) {
                    if (source.support(clazz))
                        return true;  
                    return false;
                }
            });
        }
        return v;
    }

    public void removeVoter(Voter voter) {
        this.getVoters().remove(voter);
    }

}
