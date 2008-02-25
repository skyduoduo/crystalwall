/*
 * Copyright 2008 the original author or authors.
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

package org.crystalwall.permission.def.support;

import org.crystalwall.permission.def.*;
import com.google.common.collect.Lists;
import java.util.List;
import org.apache.commons.lang.NullArgumentException;

/**
 * 使用一个不可变的List存储权限定义的工厂实现，这个实现能够支持spring中List类型的权限配置。
 * 使用此工厂返回的权限集合不能改变，否则将抛出异常。返回的权限列表中的权限可以改变，但一般
 * 此实现将缓存第一次调用getDefinition返回的结果，如果在权限信息改变之后，应该手工调用clear
 * 方法清空缓存，然后调用getDefinition返回新的结果
 * @author vincent
 */
public class ListPermissionDefinitionFactory implements PermissionDefinitionFactory {

    private List<PermissionDefinition> definitions;
    
    private PermissionDefinition cache;
    
    public ListPermissionDefinitionFactory(List<PermissionDefinition> definitions) {
        if (definitions == null) {
            throw new NullArgumentException("the definitions list must not be null...");
        }
        definitions = Lists.immutableList(definitions);
    }
    
    /**
     * 将合并的结果缓存,并返回。这意味着，第一次调用此方法之后，对集合中的权限定义的修改将无效，
     * 除非调用了clear方法情况缓存。
     * @return
     */
    public PermissionDefinition getDefinition() {
        if (cache == null) {
            PermissionDefinition first = definitions.get(0);
            List<PermissionDefinition> sub = definitions.subList(1, definitions.size());
            for (PermissionDefinition def : sub) {
                first = first.combine(def);
            }
            cache = first;
        }
        return cache;
    }
    
    public List<PermissionDefinition> getPermissionDefinitions() {
       return definitions;
    }
    
    public void clear() {
        cache = null;
    }

}
