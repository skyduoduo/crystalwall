using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CrystalWall.Config;
using System.Data;
using System.Data.Common;

namespace CrystalWall.Auths
{
    /// <summary>
    /// 基于数据库的身份提供者实现，他指定存储身份令牌的表、权限表、外键、身份名称字段。
    /// 子类可以根据自己的数据库设计重写。DB身份提供者配置必须按照如下方式配置：
    /// <code>
    /// <principal-providers>
    ///       <provider class="DBPrincipalProvider" assembly="程序集文件名">
    ///         <connection>Data Source=**;Initial Catalog=***;User ID=sa;Password=***;</connection>
    ///         <conn-provider>数据提供者名称</conn-provider>（可选，默认为sql server提供者）
    ///         <principal-table>User</principal-table>
    ///         <user-indentity>name</user-indentity>（可选，默认为name）
    ///         <permission-table>Permission</permission-table>
    ///         <foreign-key>user_per_id</foreign-key>
    ///       </privider>
    /// </principal-providers>
    /// </code>
    /// 其中表的主键必须id字段，且主键必须为36位的唯一字符串。User表必须具有name字段，permission表必须具有name、
    /// action字段
    /// </summary>
    public class DBPrincipalProviders : IPrincipalProvider
    {
        public  const string ELE_CONNECTION = "connection";

        public  const string ELE_CONN_PROVIDER = "conn-provider";

        public  const string ELE_PRINCIPAL_TABLE = "principal-table";

        public  const string ELE_PERMISSION_TABLE = "permission-table";

        public  const string ELE_FOREIGN_KEY = "foreign-key";

        public  const string ELE_USER_INDENTITY_COLUMN = "user-indentity";//唯一标识用户的列

        private string connectionString;

        private string principaltable;

        private string userIndentity = "name";//唯一标识用户的列

        private string permissiontable;

        private string foreignkey;

        private string connProvider = "System.Data.SqlClient"; //数据提供者名称

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
            string sql = "select  count(*) from " + principaltable + " where " +  ELE_USER_INDENTITY_COLUMN+ "=@name";
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
                adapter.Fill(ds, ELE_PRINCIPAL_TABLE);
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
            return "select * from " + principaltable + " where " + ELE_USER_INDENTITY_COLUMN + "=@name"; 
        }

        /// <summary>
        /// 使用数据集的数据创建身份令牌实现，默认返回UserPasswordPrincipalToken,
        /// 且他默认使用数据表的password列设置令牌密码，子类可以根据需要重写
        /// </summary>
        protected virtual IPrincipalToken CreatePrincipalToken(DataSet principalSet)
        {
            DataTable ptable = principalSet.Tables[ELE_PRINCIPAL_TABLE];
            return new UserPasswordPrincipalToken((string)ptable.Rows[0][ELE_USER_INDENTITY_COLUMN], (string)ptable.Rows[0]["password"], this);
        }

        public virtual void InitData(XmlNode element, string attribute, object data)
        {
            foreach (XmlNode node in element.ChildNodes)
            {
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
                    case ELE_FOREIGN_KEY:
                        foreignkey = node.InnerText;
                        break;
                    case ELE_CONN_PROVIDER:
                        connProvider = node.InnerText;
                        break;
                    case ELE_USER_INDENTITY_COLUMN:
                        userIndentity = node.InnerText;
                        break;
                    default:
                        break;
                }
            }
            if (connectionString == null || principaltable == null || permissiontable == null || foreignkey == null)
                throw new ConfigurationException(element.Name, "基于数据库的身份提供者的配置错误，你必须配置connection/principal-table/permission-table/foreign-key元素。");
        }

        public virtual PermissionInfoCollection GetPermissions(string name)
        {
            //TODO:编写获取指定身份的权限集的代码
            throw new NotImplementedException();
        }
    }
}
