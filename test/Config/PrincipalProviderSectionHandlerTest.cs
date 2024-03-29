﻿/*
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
    ///<author>vincent valenlee</author>
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
                                        + "<principal-table>principal</principal-table>"
                                        + "<!--<user-indentity>pname</user-indentity>（可选，默认为pname）-->"
                                        + "<permission-table>permission</permission-table>"
                                        + "<!--以下可选，关联表默认为身份表_权限表"
                                        + "<foreign-table name=\"principal_permission\">"
                                        + "  <foreign-user>principal_id</foreign-user>"
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
            XmlNode section = doc.DocumentElement;
            IList<IPrincipalProvider> providers = target.Create(null, null, section) as IList<IPrincipalProvider>;
            if (providers == null || providers.Count ==0)
                Assert.Fail("提供者不能为空");
            object actual = providers[0];
            Assert.IsNotNull(actual, "提供者不为null");
            Assert.IsInstanceOfType(actual, typeof(DBPrincipalProvider), "提供者不为DBPrincipalProvider类型");

            DBPrincipalProvider provider = (DBPrincipalProvider)actual;
            Assert.IsNotNull(provider.ConnectionString, "连接字符串未获取到");
            Assert.AreEqual("Data Source=**;Initial Catalog=***;User ID=sa;Password=***;", provider.ConnectionString, "连接字符串不是期望的");

            Assert.IsNotNull(provider.Principaltable, "身份表未获取");
            Assert.AreEqual("principal", provider.Principaltable, "身份字符不是期望的");

            Assert.IsNotNull(provider.ConnProvider, "默认连接提供者为null");
            Assert.AreEqual("System.Data.SqlClient", provider.ConnProvider, "默认连接提供者不是默认的System.Data.SqlClient");

            Assert.IsNotNull(provider.Foreignpermission, "默认身份权限中间表权限外键为null");
            Assert.AreEqual("permission_id", provider.Foreignpermission, "默认身份权限中间表权限外键名不是默认的permission_id");

            Assert.IsNotNull(provider.Foreignuser, "默认身份权限中间表身份外键为null");
            Assert.AreEqual("principal_id", provider.Foreignuser, "默认身份权限中间表身份外键名不是默认的user_id");

            Assert.IsNotNull(provider.Foreigntable, "默认身份权限中间表为null");
            Assert.AreEqual("principal_permission", provider.Foreigntable, "默认身份权限中间表名不是默认的user_permission");

            Assert.IsNotNull(provider.Permissiontable, "权限表名未获取到");
            Assert.AreEqual("permission", provider.Permissiontable, "权限表名不是permission");

            Assert.IsNotNull(provider.UserIndentity, "默认身份表的标识列未获取到");
            Assert.AreEqual("pname", provider.UserIndentity, "默认身份表的标识列不是默认的pname");
        }

        [TestMethod]
        public void GetProviderTest()
        {
            string path = Assembly.GetAssembly(typeof(IPrincipalProvider)).Location;
            ConfigurationFile configuration = new ConfigurationFile(path);
            Assert.IsNotNull(configuration, "获取配置文件出错");

            IPrincipalProvider iprovider = (IPrincipalProvider)((IList<IPrincipalProvider>)configuration.GetSection("principal-providers"))[0];
            Assert.IsInstanceOfType(iprovider, typeof(DBPrincipalProvider), "提供者不为DBPrincipalProvider类型");

            DBPrincipalProvider provider = (DBPrincipalProvider)iprovider;
            Assert.IsNotNull(provider.ConnectionString, "连接字符串未获取到");
            Assert.AreEqual(@"Data Source=.;Initial Catalog=CrystalwallTest;User ID=sa;Password=123456;", provider.ConnectionString.Trim(), "连接字符串不是期望的");

            Assert.IsNotNull(provider.Principaltable, "身份表未获取");
            Assert.AreEqual("principal", provider.Principaltable, "身份字符不是期望的");

            Assert.IsNotNull(provider.ConnProvider, "默认连接提供者为null");
            Assert.AreEqual("System.Data.SqlClient", provider.ConnProvider, "默认连接提供者不是默认的System.Data.SqlClient");

            Assert.IsNotNull(provider.Foreignpermission, "默认身份权限中间表权限外键为null");
            Assert.AreEqual("permission_id", provider.Foreignpermission, "默认身份权限中间表权限外键名不是默认的permission_id");

            Assert.IsNotNull(provider.Foreignuser, "默认身份权限中间表身份外键为null");
            Assert.AreEqual("principal_id", provider.Foreignuser, "默认身份权限中间表身份外键名不是默认的principal_id");

            Assert.IsNotNull(provider.Foreigntable, "默认身份权限中间表为null");
            Assert.AreEqual("principal_permission", provider.Foreigntable, "默认身份权限中间表名不是默认的principal_permission");

            Assert.IsNotNull(provider.Permissiontable, "权限表名未获取到");
            Assert.AreEqual("permission", provider.Permissiontable, "权限表名不是permission");

            Assert.IsNotNull(provider.UserIndentity, "默认身份表的标识列未获取到");
            Assert.AreEqual("pname", provider.UserIndentity, "默认身份表的标识列不是默认的pname");
        }

    }
}
