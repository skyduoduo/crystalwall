using CrystalWall.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using CrystalWall.Auths;
using CrystalWall;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;

namespace Crystalwall.Test
{
    /// <summary>
    ///这是 PrincipalProviderSectionHandlerTest 的测试类，旨在
    ///包含所有 PrincipalProviderSectionHandlerTest 单元测试
    ///</summary>
    [TestClass()]
    public class PrincipalProviderSectionHandlerTest
    {

        private static string xml;

        private TestContext testContextInstance;

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
            xml = "<principal-providers>"
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


        ///// <summary>
        /////PrincipalProviderSectionHandler 构造函数 的测试
        /////</summary>
        //[TestMethod()]
        //public void PrincipalProviderSectionHandlerConstructorTest()
        //{
        //    PrincipalProviderSectionHandler target = new PrincipalProviderSectionHandler();
        //    Assert.Inconclusive("TODO: 实现用来验证目标的代码");
        //}

        /// <summary>
        ///Create 的测试(正常情况下）
        ///</summary>
        [TestMethod()]
        public void CreateTest()
        {

            PrincipalProviderSectionHandler target = new PrincipalProviderSectionHandler();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNode section = doc.DocumentElement.ChildNodes[0];
            //Assert.AreEqual("provider", section.Name, "节点名称不为provider");
            //Assert.AreEqual(1, section.Attributes.Count, "节点属性数目超过1");
            //Assert.AreEqual("class", section.Attributes[0].Name, "节点属性名称不为class");
            //Assert.AreEqual("CrystalWall.Auths.DBPrincipalProvider", section.Attributes[0].Value, "节点class属性的值不为CrystalWall.Auths.DBPrincipalProvider");

            object actual = target.Create(null, null, section);
            Assert.IsNotNull(actual, "提供者不为null");
            Assert.IsInstanceOfType(actual, typeof(DBPrincipalProvider), "提供者不为DBPrincipalProvider类型");

            DBPrincipalProvider provider = (DBPrincipalProvider)actual;
            Assert.IsNotNull(provider.ConnectionString, "连接字符串未获取到");
            Assert.AreEqual("Data Source=**;Initial Catalog=***;User ID=sa;Password=***;", provider.ConnectionString, "连接字符串不是期望的");

            Assert.IsNotNull(provider.Principaltable, "身份表未获取");
            Assert.AreEqual("user", provider.Principaltable, "身份字符不是期望的");

            Assert.IsNotNull(provider.ConnProvider, "默认连接提供者为null");
            Assert.AreEqual("System.Data.SqlClient", provider.ConnProvider, "默认连接提供者不是默认的System.Data.SqlClient");

            Assert.IsNotNull(provider.Foreignpermission, "默认身份权限中间表权限外键为null");
            Assert.AreEqual("permission_id", provider.Foreignpermission, "默认身份权限中间表权限外键名不是默认的permission_id");

            Assert.IsNotNull(provider.Foreignuser, "默认身份权限中间表身份外键为null");
            Assert.AreEqual("user_id", provider.Foreignuser, "默认身份权限中间表身份外键名不是默认的user_id");

            Assert.IsNotNull(provider.Foreigntable, "默认身份权限中间表为null");
            Assert.AreEqual("user_permission", provider.Foreigntable, "默认身份权限中间表名不是默认的user_permission");

            Assert.IsNotNull(provider.Permissiontable, "权限表名未获取到");
            Assert.AreEqual("permission", provider.Permissiontable, "权限表名不是permission");

            Assert.IsNotNull(provider.UserIndentity, "默认身份表的标识列未获取到");
            Assert.AreEqual("name", provider.UserIndentity, "默认身份表的标识列不是默认的name");
        }

        [TestMethod]
        public void GetProviderTest()
        {
            string path = Assembly.GetAssembly(typeof(IPrincipalProvider)).Location;
            ConfigurationFile configuration = new ConfigurationFile(path);
            Assert.IsNotNull(configuration, "获取配置文件出错");

            IPrincipalProvider iprovider = (IPrincipalProvider)configuration.GetSection("principal-providers/provider");
            Assert.IsInstanceOfType(iprovider, typeof(DBPrincipalProvider), "提供者不为DBPrincipalProvider类型");

            DBPrincipalProvider provider = (DBPrincipalProvider)iprovider;
            Assert.IsNotNull(provider.ConnectionString, "连接字符串未获取到");
            Assert.AreEqual("Data Source=**;Initial Catalog=***;User ID=sa;Password=***;", provider.ConnectionString, "连接字符串不是期望的");

            Assert.IsNotNull(provider.Principaltable, "身份表未获取");
            Assert.AreEqual("user", provider.Principaltable, "身份字符不是期望的");

            Assert.IsNotNull(provider.ConnProvider, "默认连接提供者为null");
            Assert.AreEqual("System.Data.SqlClient", provider.ConnProvider, "默认连接提供者不是默认的System.Data.SqlClient");

            Assert.IsNotNull(provider.Foreignpermission, "默认身份权限中间表权限外键为null");
            Assert.AreEqual("permission_id", provider.Foreignpermission, "默认身份权限中间表权限外键名不是默认的permission_id");

            Assert.IsNotNull(provider.Foreignuser, "默认身份权限中间表身份外键为null");
            Assert.AreEqual("user_id", provider.Foreignuser, "默认身份权限中间表身份外键名不是默认的user_id");

            Assert.IsNotNull(provider.Foreigntable, "默认身份权限中间表为null");
            Assert.AreEqual("user_permission", provider.Foreigntable, "默认身份权限中间表名不是默认的user_permission");

            Assert.IsNotNull(provider.Permissiontable, "权限表名未获取到");
            Assert.AreEqual("permission", provider.Permissiontable, "权限表名不是permission");

            Assert.IsNotNull(provider.UserIndentity, "默认身份表的标识列未获取到");
            Assert.AreEqual("name", provider.UserIndentity, "默认身份表的标识列不是默认的name");
        }

    }
}
