
package org.crystalwall.vote;

import org.crystalwall.AuthenticationToken;
import org.crystalwall.permission.def.PermissionDefinition;
import org.crystalwall.permission.def.PermissionDefinitionFactory;

/**
 * 投票者的抽象类
 * @author vincent valenlee
 */
public abstract class AbstractVoter implements Voter {

    private PermissionDefinitionFactory pdefFactory;

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

   /**
    * 子类必须实现的投票操作
    * @param token
    * @param secur
    * @param registry
    * @return
    */
    protected abstract int doVote(AuthenticationToken token, Object secur, PermissionDefinition pdef);
   
}
