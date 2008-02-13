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

/**
 * 投票者链接口
 * @author vincent valenlee
 */
public interface VoterChain {

    /**
   * 在链最后添加投票者
   * @param voter 要添加的投票者
   */
    public void addVoter(Voter voter);
    
    /**
   * 在before之前添加added投票者
   * @param before 要在之前插入的投票者
   * @param added 要插入的投票者
   */
    public void addBefore(Voter before, Voter added);
    
    /**
   * 在after之后添加added投票者
   * @param after 要在之后插入的投票者
   * @param added 要插入的投票者
   */
    public void addAfter(Voter after, Voter added);
    
    public void removeVoter(Voter voter);
    
    /**
   * 获取支持对指定安全类进行投票的投票者
   * @param securClazz 安全对象类
   */
    public List<Voter> getVoters(Class securClazz);
    
    /**
   * @return 获取所有链中的投票者
   */
    public List<Voter> getVoters();

}
