package org.crystalwall.vote;

import java.util.List;
import org.crystalwall.AuthenticationToken;
import org.crystalwall.permission.PermissionResolver;
import org.crystalwall.permission.def.PermissionDefinition;
import org.crystalwall.permission.def.PermissionDefinitionFactory;

/**
 * 投票者的抽象类,他使用内部的权限定义工厂获取默认的权限定义
 * @author vincent valenlee
 */
public abstract class AbstractVoter implements Voter {

    private PermissionDefinitionFactory pdefFactory;
    private List<PermissionResolver> resolvers;
    private VoterChain parentChain;

    public AbstractVoter(VoterChain parentChain) {
        this.parentChain = parentChain;
    }

    public AbstractVoter() {
    }

    public List<PermissionResolver> getResolvers() {
        return resolvers;
    }

    public void setResolvers(List<PermissionResolver> resolvers) {
        this.resolvers = resolvers;
    }

    public PermissionDefinitionFactory getPdefFactory() {
        return pdefFactory;
    }

    public void setPdefFactory(PermissionDefinitionFactory pdefFactory) {
        this.pdefFactory = pdefFactory;
    }

    public int vote(final AuthenticationToken token, final Object secur, final PermissionDefinition pdef) {
        int result = Voter.ABSTAIN;
        PermissionDefinition npdef = pdef;
        if (getDefaultPermissionDefinition() != null) {
            npdef = getDefaultPermissionDefinition().combine(pdef);
        }
        if (support(secur.getClass())) {
            result = doVote(token, secur, npdef);
        }
        return result;
    }

    public PermissionDefinition getDefaultPermissionDefinition() {
        if (getPdefFactory() != null) {
            return getPdefFactory().getDefinition();
        }
        return null;
    }

    public VoterChain getVoterChain() {
        return parentChain;
    }

    /**
     * 获取具有解析指定安全对象能力的解析器，如果没有则返回null
     * <p>子类可以重写此方法，默认返回列表中第一个支持的解析器
     * @param secur 安全对象
     */
    protected PermissionResolver getResolveable(Object secur) {
        if (getResolvers() != null) {
            for (PermissionResolver resolver : getResolvers()) {
                if (resolver.support(secur.getClass())) {
                    return resolver;
                }
            }
        }
        return null;
    }

    /**
     * 子类必须实现的投票操作
     * @param token
     * @param secur
     * @param registry
     * @return
     */
    protected abstract int doVote(AuthenticationToken token, Object secur, PermissionDefinition pdef);
}
