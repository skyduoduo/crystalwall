using CrystalWall.Auths;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using CrystalWall;

namespace Crystalwall.Test.Auths
{

    /// <summary>
    ///这是 DBPrincipalProviderTest 的测试类，旨在
    ///包含所有 DBPrincipalProviderTest 单元测试
    ///</summary>
    [TestClass()]
    public class DBPrincipalProviderTest
    {

        private TestContext testContextInstance;

        private static DBPrincipalProvider_Accessor target;

        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        // 
        //编写测试时，还可使用以下特性:
        //
        //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            target = new DBPrincipalProvider_Accessor();
            //初始化数据
            /*
             * "<principal-providers>"
                                        + "<provider class=\"CrystalWall.Auths.DBPrincipalProvider\">"
                                        + "<connection>Data Source=**;Initial Catalog=***;User ID=sa;Password=***;</connection>"
                                        + "<!--<conn-provider>数据提供者名称</conn-provider>（可选，默认为sql server提供者）-->"
                                        + "<principal-table>user</principal-table>"
                                        + "<!--<user-indentity>name</user-indentity>（可选，默认为name）-->"
                                        + "<permission-table>permission</permission-table>"
                                        + "<!--以下可选，关联表默认为身份表_权限表"
                                        + "<foreign-table name=\"user_permission\">"
                                        + "  <foreign-user>user_id</foreign-user>"
                                        + "  <foreign-permission>permission_id</foreign-permission>"
                                        + "</foreign-table>-->"
                                        + "</provider>"
                                        + "</principal-providers>";
             */
            target.connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=Test;User ID=sa;Password=123456;";
            target.principaltable = "principal";
            target.permissiontable = "permission";
            target.foreigntable = "principal_permission";
            target.foreignuser = "principal_id";
            target.foreignpermission = "permission_id";
        }
        //
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //使用 TestInitialize 在运行每个测试前先运行代码
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //使用 TestCleanup 在运行完每个测试后运行代码
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///GetPermissions 的测试
        ///</summary>
        [TestMethod()]
        [DeploymentItem("CrystalWall.dll")]
        public void GetPermissionsTest()
        {
            PermissionInfoCollection collections = target.GetPermissions("admin");
            Assert.IsNotNull(collections, "admin的权限集不为空");
            Assert.AreEqual(2, collections.Count, "admin应该具有两个权限信息");
            Assert.IsInstanceOfType(collections[1], typeof(TestPermissionInfo), "admin第二个权限的类型应该为TestPermissionInfo");

            Assert.AreSame(PermissionInfoCollection.EMPTY_PERMISSIONINFO_COLLECTION, target.GetPermissions("admin2"), "admin2的权限集应该为空集");
        }

        [TestMethod()]
        [DeploymentItem("CrystalWall.dll")]
        public void GetPrincipalTokenTest()
        {
            IPrincipalToken token = target["admin"];
            Assert.IsInstanceOfType(token, typeof(UserPasswordPrincipalToken), "admin身份的类型应该为UserPasswordPrincipalToken");

            token = target["admin2"];
            Assert.AreEqual(FactoryServices.ANONY_PRINCIPAL_TOKEN, token, "admin2的身份不存在，通过provider索引应该返回匿名身份");
        }

        /// <summary>
        ///HasPrincipal 的测试
        ///</summary>
        [TestMethod()]
        [DeploymentItem("CrystalWall.dll")]
        public void HasPrincipalTest()
        {
            bool actual = target.HasPrincipal("admin");
            Assert.IsTrue(actual, "db中应该具有admin身份");

            bool actual2 = target.HasPrincipal("admin2");
            Assert.IsFalse(actual2, "db中应该不具有admin2身份");
        }
    }
}
