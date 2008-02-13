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

import org.crystalwall.AuthenticationToken;
import org.crystalwall.permission.def.PermissionDefinition;

/**
 * 投票者用于对其支持的安全对象的权限进行投票以允许对受保护的安全对象进行安全控制。
 * 这个类参考了ACEGI中的AccessDecisionVoter对象，但有所区别，投票者可以是一个
 * 投票者链类型的投票者，每一种投票者都有根据投票结果进行决定的策略，例如：多数票
 * 策略的投票者将其内部的投票者链的投票结果综合起来，如果赞成票大于反对票，则投票
 * 通过。而一票否决策略的投票者，则查看自己内部的投票者链是否有一个反对票，只要有
 * 反对票则反对。
 * @author vincent valenlee
 */
public interface Voter {

    public static final Integer APPROVED = 1;
    public static final Integer ABSTAIN = 0;
    public static final Integer DENIED = -1;
    
    /**
     * 对指定令牌进行投票
     * @param token 身份认证令牌
     * @param secur 安全对象
     * @param registry 权限定义注册表，如果为null则使用投票者默认支持的权限定义注册表
     * @return 1--赞同、0--弃权、-1--拒绝
     */
    public int vote(AuthenticationToken token, Object secur, PermissionDefinition permissionDef);
    
    /**
     * 是否支持对指定的安全对象类型进行投票
     * @param clazz
     * @return
     */
    public boolean support(Class clazz);
    
    /**
   * 获取此投票者关联的投票者链
   * @return 如果投票者不属于投票者链，则返回null
   */
    public VoterChain getVoterChain();
    
    /**
   * 获取投票者默认使用的权限定义注册表
   * @return
   */
    public PermissionDefinition getDefaultPermissionDefinition();

}
