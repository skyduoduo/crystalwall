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

package org.crystalwall.permission;

import com.google.common.collect.Maps;
import java.util.Iterator;
import java.util.Map;
import org.crystalwall.permission.def.CombineException;
import org.crystalwall.permission.def.PermissionDefinition;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

/**
 * 使用安全对象的Class作为key，存储对应的权限定义对象的注册表实现。
 * 如果重复注册相同Class上的权限定义，默认将使用权限定义的combine
 * 方法合并相同Class安全对象上的权限定义
 * @author vincent valenlee
 */
public class DefaultPermissionDefRegistry implements PermissionDefRegistry{

    protected static final Logger logger = LoggerFactory.getLogger(DefaultPermissionDefRegistry.class);
    private String name;
    private Map<Class, PermissionDefinition> definitions = Maps.newHashMap();
    
    public DefaultPermissionDefRegistry(String name) {
        this.name = name;
    }

    public Map<Class, PermissionDefinition> getDefinitions() {
        return definitions;
    }

    public void setDefinitions(Map<Class, PermissionDefinition> definistions) {
        this.definitions = definistions;
    }

    public String getName() {
        return name;
    }

    public PermissionDefinition findPermissionDefinition(Object secur) throws PermissionDefinitionException {
        if (secur == null) {
            logger.debug("the security object is null, the registry[{}] will throw IllegalArgumentException...", getName());
            throw new PermissionDefinitionException(new IllegalArgumentException("registry " + getName() + " do not find a null security object permission definition..."));
        }
        if (getDefinitions().containsKey(secur.getClass())) {
            return getDefinitions().get(secur.getClass());
        }
        logger.info("the registry[{}] do not contain security object [{}]-[{}]'s ", new Object[] {getName(), secur.toString(), secur.getClass().getName()});
        return null;
    }

    public Iterator getPermissionDefinitions() {
        return getDefinitions().values().iterator();
    }

    public void registerPermissionDefinition(Object secur, PermissionDefinition def) throws PermissionDefinitionException {
        if (secur == null) {
            logger.debug("the security object is null, so the registry return direct...");
            return;
        }
        if (def == null) {
            logger.error("the register permissionDefinition [{}] is null...", def.toString());
            throw new PermissionDefinitionException(new NullPointerException("the register permissionDefinition is null..."));
        }
        if (!getDefinitions().containsKey(secur.getClass())) {
            getDefinitions().put(secur.getClass(), def);
        } else {
            //注册表中已经存在，则将合并权限定义
            PermissionDefinition old = getDefinitions().get(secur.getClass());
            PermissionDefinition combine = old;
            try {
                combine = old.combine(def);
                getDefinitions().put(combine.getClass(), combine);
            } catch (CombineException e) {
                logger.warn("register permission definition[" + def.toString() + "] to old definition[" + old.toString() + "] exception...", e);
                try {
                    //原有权限定义不能联合新权限定义，则让新权限定义联合旧权限定义
                    combine = def.combine(old);
                    getDefinitions().put(combine.getClass(), combine);
                } catch (CombineException ce) {
                    //新权限定义也不能联合旧权限定义
                    logger.error("the new definition[" + def.toString() + "] do not combine the old definition[" + old.toString() + "]", ce);
                    throw new CombineException("can not register a new permission definition[" + def.toString() +
                            "] into an exit permission definition which the same security object Class, because the old or new all " +
                            " do not combine another", ce.getCause());
                }
            } catch (PermissionDefinitionException e) {
                logger.error("combine the new definition to old definition" +
                        " catch exception(not combineException,no need to use new definition combine old definition)...");
                throw e;
            }
        }
    }
}
