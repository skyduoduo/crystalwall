/*
 * Copyright 2008-2010 the original author or authors.
 *
 * Licensed under the Eclipse Public License v1.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.eclipse.org/legal/epl-v10.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CrystalWall.Config;
using System.Data;
using System.Data.Common;
using System.Reflection;
using CrystalWall.Utils;

namespace CrystalWall.Auths
{
    /// <summary>
    /// 基于数据库的身份提供者实现，他指定存储身份令牌的表、权限表、外键、身份名称字段。
    /// 其中表的主键必须id字段，且主键必须为36位的唯一字符串。身份表必须具有name字段，
    /// 权限表必须具有name、action和class字段。子类可以根据自己的数据库设计重写。
    /// DB身份提供者配置必须按照如下方式配置：
    /// <code>
    /// <principal-providers>
    ///       <provider class="DBPrincipalProvider" assembly="程序集文件名">
    ///         <connection>Data Source=**;Initial Catalog=***;User ID=sa;Password=***;</connection>
    ///         <conn-provider>数据提供者名称</conn-provider>（可选，默认为sql server提供者）
    ///         <principal-table>user</principal-table>
    ///         <user-indentity>name</user-indentity>（可选，默认为name）
    ///         <permission-table>permission</permission-table>
    ///         <!--以下可选，关联表默认为身份表_权限表-->
    ///         <foreign-table name="user_permission">
    ///           <foreign-user>user_id</foreign-user>
    ///           <foreign-permission>permission_id</foreign-permission>
    ///         </foreign-table>
    ///       </privider>
    /// </principal-providers>
    /// </code>
    /// </summary>
    /// <author>vincent valenlee</author>
    public class DBPrincipalProvider : IPrincipalProvider
    {
        public  const string ELE_CONNECTION = "connection";

        public  const string ELE_CONN_PROVIDER = "conn-provider";

        public  const string ELE_PRINCIPAL_TABLE = "principal-table";

        public  const string ELE_PERMISSION_TABLE = "permission-table";

        public  const string ELE_FOREIGN_TABLE = "foreign-table";//外键表名

        public const string ELE_FOREIGN_USER = "foreign-user";//外键user id列

        public const string ELE_FOREIGN_PERMISSION = "foreign-permission";//外键permission id列

        public  const string ELE_USER_INDENTITY_COLUMN = "user-indentity";//唯一标识用户的列

        public const string PERMISSION_NAME_COLUMN = "name";//权限表名称字段

        public const string PERMISSION_ACTION_COLUMN = "action";//权限表动作字段

        public const string PERMISSION_TYPE_COLUMN = "class";//权限表类型字段（包括程序集的全限定名称）

        private string connectionString;

        /// <summary>
        /// 仅用于测试目的
        /// </summary>
        public string ConnectionString
        {
            get { return connectionString; }
        }

        private string principaltable;

        /// <summary>
        /// 仅用于测试目的
        /// </summary>
        public string Principaltable
        {
            get { return principaltable; }
        }

        private string userIndentity = "pname";//唯一标识用户的列

        /// <summary>
        /// 仅用于测试目的
        /// </summary>
        public string UserIndentity
        {
            get { return userIndentity; }
        }

        private string permissiontable = "permission";

        /// <summary>
        /// 仅用于测试目的
        /// </summary>
        public string Permissiontable
        {
            get { return permissiontable; }
        }

        private string foreigntable = "principal_permission";

        /// <summary>
        /// 仅用于测试目的
        /// </summary>
        public string Foreigntable
        {
            get { return foreigntable; }
        }

        private string foreignuser = "principal_id";

        /// <summary>
        /// 仅用于测试目的
        /// </summary>
        public string Foreignuser
        {
            get { return foreignuser; }
        }

        private string foreignpermission = "permission_id";

        /// <summary>
        /// 仅用于测试目的
        /// </summary>
        public string Foreignpermission
        {
            get { return foreignpermission; }
        }

        private string connProvider = "System.Data.SqlClient"; //数据提供者名称

        /// <summary>
        /// 仅用于测试目的
        /// </summary>
        public string ConnProvider
        {
            get { return connProvider; }
        }

        private DbConnection connection;

        private DbProviderFactory factory;  

        private DbProviderFactory Factory  
        {
            get
            {
                if (factory == null)
                    factory = DbProviderFactories.GetFactory(connProvider);
                return factory;
            }
        }

        public virtual bool HasPrincipal(string name)
        {
            string sql = "select  count(*) from " + principaltable + " where " +  userIndentity + "=@name";
            initConnection();
            return ExecuteBool(sql, s =>
            {
                DbCommand command = connection.CreateCommand();
                DbParameter parameter = command.CreateParameter();
                parameter.ParameterName = "name";
                parameter.DbType = DbType.String;
                parameter.Value = name;
                command.CommandText = s;
                command.Parameters.Add(parameter);
                return command;
            });
        }

        private void initConnection()
        {
            if (connection == null)
            {
                connection = Factory.CreateConnection();
                connection.ConnectionString = connectionString;
            }
            if (connection.State == ConnectionState.Broken || connection.State == ConnectionState.Closed)
            {
                //重新打开连接
                connection.ConnectionString = connectionString;
                connection.Open();
            }
        }

        private bool ExecuteBool(string sql, Func<string, DbCommand> initCommand)
        {
            using (connection)
            {
                if ((int)initCommand(sql).ExecuteScalar() > 0)
                    return true;
                return false;
            }
        }

        private DataSet ExecuteQuery(string sql, Func<string, DbCommand> initCommand)
        {
            using (connection)
            {
                DbDataAdapter adapter = Factory.CreateDataAdapter();
                adapter.SelectCommand = initCommand(sql);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                return ds;
            }
        }

        /// <summary>
        /// 首先使用GetPrincipalSelectCause获取身份查询语句，然后使用CreatePrincipalToken
        /// 默认返回UserPasswordPrincipalToken,且他默认使用数据表的password列设置令牌密码。
        /// 子类应该根据需要重写这两个方法
        /// </summary>
        public virtual IPrincipalToken this[string name]
        {
            get 
            {
                string sql = GetPrincipalSelectCause();
                initConnection();
                return CreatePrincipalToken(ExecuteQuery(sql, s =>
                {
                    DbCommand command = connection.CreateCommand();
                    DbParameter parameter = command.CreateParameter();
                    parameter.ParameterName = "name";
                    parameter.DbType = DbType.String;
                    parameter.Value = name;
                    command.CommandText = s;
                    command.Parameters.Add(parameter);
                    return command;
                }));
            }
        }

        /// <summary>
        /// 获取身份查询的sql语句，默认值从principaltable中查询所有。
        /// 子类可以根据需要重写，但重写时，sql语句中必须具有@name参数
        /// </summary>
        protected virtual string GetPrincipalSelectCause()
        {
            return "select * from " + principaltable + " where " + userIndentity + "=@name"; 
        }

        /// <summary>
        /// 使用数据集的数据创建身份令牌实现，默认返回UserPasswordPrincipalToken,
        /// 且他默认使用数据表的password列设置令牌密码，子类可以根据需要重写
        /// </summary>
        protected virtual IPrincipalToken CreatePrincipalToken(DataSet principalSet)
        {
            DataTable ptable = principalSet.Tables[0];
            if (ptable.Rows.Count == 0)
                return FactoryServices.ANONY_PRINCIPAL_TOKEN;//返回匿名用户
            return new UserPasswordPrincipalToken((string)ptable.Rows[0][userIndentity], (string)ptable.Rows[0]["password"], this);
        }

        public virtual void InitData(XmlNode element, string attribute, object data)
        {
            foreach (XmlNode node in element.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Comment)
                    continue;
                switch(node.Name)
                {
                    case ELE_CONNECTION:
                        connectionString = node.InnerText;
                        break;
                    case ELE_PRINCIPAL_TABLE:
                        principaltable = node.InnerText;
                        break;
                    case ELE_PERMISSION_TABLE:
                        permissiontable = node.InnerText;
                        break;
                    case ELE_CONN_PROVIDER:
                        connProvider = node.InnerText;
                        break;
                    case ELE_USER_INDENTITY_COLUMN:
                        userIndentity = node.InnerText;
                        break;
                    case ELE_FOREIGN_TABLE:
                        foreigntable = node.Attributes["name"].Value;
                        for (int i = 0; i < node.ChildNodes.Count; i++)
                        {
                            switch (node.ChildNodes[i].Name)
                            {
                                case ELE_FOREIGN_USER:
                                    foreignuser = node.ChildNodes[i].InnerText;
                                    break;
                                case ELE_FOREIGN_PERMISSION:
                                    foreignpermission = node.ChildNodes[i].InnerText;
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            if (connectionString == null || principaltable == null || permissiontable == null)
                throw new ConfigurationException(element.Name, "基于数据库的身份提供者的配置错误，你必须配置connection/principal-table/permission-table元素。");
            if (foreigntable == null)
            {
                foreigntable = principaltable + "_" + permissiontable;//默认中间表
                foreignuser = principaltable + "_id";//默认身份表id外键
                foreignpermission = permissiontable + "_id";//默认权限表外键id
            }
            else if (foreignuser == null)
            {
                foreignuser = principaltable + "_id";//默认身份表id外键
            }
            else if (foreignpermission == null)
            {
                foreignpermission = permissiontable + "_id";//默认权限表外键id
            }
        }

        public virtual PermissionInfoCollection GetPermissions(string name)
        {
            string sql = GetPermissionSelectCause();
            initConnection();
            DataSet ds = ExecuteQuery(sql, s =>
            {
                DbCommand command = connection.CreateCommand();
                DbParameter parameter = command.CreateParameter();
                parameter.ParameterName = "name";
                parameter.DbType = DbType.String;
                parameter.Value = name;
                command.CommandText = s;
                command.Parameters.Add(parameter);
                return command;
            });
            if (ds.Tables[0].Rows.Count == 0)
                return PermissionInfoCollection.EMPTY_PERMISSIONINFO_COLLECTION;
            try
            {
                PermissionInfoCollection pcoll = new PermissionInfoCollection();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    ConstructorInfo constructor = Type.GetType((string)dr[PERMISSION_TYPE_COLUMN]).GetConstructor(new Type[] { typeof(string), typeof(string) });
                    PermissionInfo permission = (PermissionInfo)constructor.Invoke(new object[] { dr[PERMISSION_NAME_COLUMN], dr[PERMISSION_ACTION_COLUMN] });
                    pcoll.Add(permission);
                }
                return pcoll;
            }
            catch (Exception e)
            {
                ServiceManager.LoggingService.Fatal("权限信息无法构造无法使用name和action参数构造，无法获取指定身份" + name + "的授权集合", e);
                throw e;
            }
        }

        /// <summary>
        /// 获取查询permission权限信息的select语句子类可以根据需要重写，但重写时，sql语句中必须具有@name参数
        /// </summary>
        protected virtual string GetPermissionSelectCause()
        {
            return "select * from " + permissiontable + " as ppermission left join " + foreigntable + " as pprincipal_permission on "
                       + "pprincipal_permission." + foreignpermission + "=ppermission.id"
                       + " left join " + principaltable + " as pprincipal on pprincipal.id=pprincipal_permission." + foreignuser
                       + " where pprincipal." + userIndentity + "=@name"; 
        }
    }
}
