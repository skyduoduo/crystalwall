/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

package org.crystalwall.permission.def;

import java.security.AllPermission;
import java.security.Permission;

/**
 * 描述权限信息的对象
 * @author vincent valenlee
 */
public class PermissionInfo {

    private String name;
    private String actions;
    //java.security.Permission的Class全称,不能为null否则抛出异常
    private String type;
    private final static Permission allPermission = new AllPermission();
    /**
     * 这个权限信息标示AllPermission权限，要小心使用此权限信息，这意味着使用此权限
     * 信息做的任何操作，没有任何限制！
     */
    public final static PermissionInfo ALL_PERMISSION_INFO = new PermissionInfo(allPermission.getName(), allPermission.getActions(), allPermission.getClass().getName());

    public PermissionInfo(String name, String action, String type) {
        this.name = name;
        this.actions = action;
        this.type = type;
        if (type == null) {
            throw new NullPointerException("type is null");
        }
    }

    public PermissionInfo() {
    }
    
    public String getAction() {
        return actions;
    }

    public void setAction(String action) {
        this.actions = action;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getType() {
        return type;
    }

    public void setType(String type) {
        this.type = type;
    }

    /**
     * 根据字符串形式的权限信息将其解码成对象，字符串的格式为：
     * "(type \"name\" \"actions\")"
     * @param encodedPermission
     * @return
     */
    public static PermissionInfo decode(String encodedPermission) {
        PermissionInfo pdef = new PermissionInfo();
        if (encodedPermission == null) {
            throw new NullPointerException("missing encoded permission");
        }
        if (encodedPermission.length() == 0) {
            throw new IllegalArgumentException("empty encoded permission");
        }
        try {
            char[] encoded = encodedPermission.toCharArray();
            int length = encoded.length;
            int pos = 0;

            /* 跳过空格 */
            while (Character.isWhitespace(encoded[pos])) {
                pos++;
            }

            if (encoded[pos] != '(') {//第一个字符必须为"("
                throw new IllegalArgumentException(
                        "expecting open parenthesis");
            }
            pos++;
            
            while (Character.isWhitespace(encoded[pos])) {
                pos++;
            }
            
            int begin = pos;
            while (!Character.isWhitespace(encoded[pos]) && (encoded[pos] != ')')) {
                pos++;
            }
            if (pos == begin || encoded[begin] == '"') {
                throw new IllegalArgumentException("expecting type");
            }
            //获取指定的类型
            pdef.type = new String(encoded, begin, pos - begin);

            while (Character.isWhitespace(encoded[pos])) {
                pos++;
            }

            /* type之后是名字，或者直接括号*/
            if (encoded[pos] == '"') {
                pos++;
                begin = pos;
                while (encoded[pos] != '"') {
                    if (encoded[pos] == '\\') {
                        pos++;
                    }
                    pos++;
                }
                pdef.name = unescapeString(encoded, begin, pos);
                pos++;

                if (Character.isWhitespace(encoded[pos])) {
                    
                    while (Character.isWhitespace(encoded[pos])) {
                        pos++;
                    }

                    /* name之后是actions或者没有actions，直接括号*/
                    if (encoded[pos] == '"') {
                        pos++;
                        begin = pos;
                        while (encoded[pos] != '"') {
                            if (encoded[pos] == '\\') {
                                pos++;
                            }
                            pos++;
                        }
                        pdef.actions = unescapeString(encoded, begin, pos);
                        pos++;

                        while (Character.isWhitespace(encoded[pos])) {
                            pos++;
                        }
                    }
                }
            }

            /* 最后的字符必须为')' */
            char c = encoded[pos];
            pos++;
            while ((pos < length) && Character.isWhitespace(encoded[pos])) {
                pos++;
            }
            if ((c != ')') || (pos != length)) {
                throw new IllegalArgumentException("expecting close parenthesis");
            }
            return pdef;
        } catch (ArrayIndexOutOfBoundsException e) {
            throw new IllegalArgumentException("parsing terminated abruptly");
        }
    }

    /**
     * @return 将权限信息以指定的编码格式返回
     */
    public String toString() {
        StringBuffer output = new StringBuffer(
                8 + type.length() + ((((name == null) ? 0 : name.length()) + ((actions == null) ? 0
                : actions.length())) << 1));
        output.append('(');
        output.append(type);
        if (name != null) {
            output.append(" \"");
            escapeString(name, output);
            if (actions != null) {
                output.append("\" \"");
                escapeString(actions, output);
            }
            output.append('\"');
        }
        output.append(')');
        return output.toString();
    }

    public boolean equals(Object obj) {
        if (obj == this) {
            return true;
        }
        if (!(obj instanceof PermissionInfo)) {
            return false;
        }
        PermissionInfo other = (PermissionInfo) obj;
        if (!type.equals(other.type) || ((name == null) ^ (other.name == null)) || ((actions == null) ^ (other.actions == null))) {
            return false;
        }
        if (name != null) {
            if (actions != null) {
                return name.equals(other.name) && actions.equals(other.actions);
            } else {
                return name.equals(other.name);
            }
        } else {
            return true;
        }
    }

    public int hashCode() {
        int hash = type.hashCode();
        if (name != null) {
            hash ^= name.hashCode();
            if (actions != null) {
                hash ^= actions.hashCode();
            }
        }
        return hash;
    }

    private static void escapeString(String str, StringBuffer output) {
        int len = str.length();
        for (int i = 0; i < len; i++) {
            char c = str.charAt(i);
            switch (c) {
                case '"':
                case '\\':
                    output.append('\\');
                    output.append(c);
                    break;
                case '\r':
                    output.append("\\r");
                    break;
                case '\n':
                    output.append("\\n");
                    break;
                default:
                    output.append(c);
                    break;
            }
        }
    }

    private static String unescapeString(char[] str, int begin, int end) {
        StringBuffer output = new StringBuffer(end - begin);
        for (int i = begin; i < end; i++) {
            char c = str[i];
            if (c == '\\') {
                i++;
                if (i < end) {
                    c = str[i];
                    switch (c) {
                        case '"':
                        case '\\':
                            break;
                        case 'r':
                            c = '\r';
                            break;
                        case 'n':
                            c = '\n';
                            break;
                        default:
                            c = '\\';
                            i--;
                            break;
                    }
                }
            }
            output.append(c);
        }
        return output.toString();
    }
}
