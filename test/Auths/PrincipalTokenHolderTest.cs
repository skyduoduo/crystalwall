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
using CrystalWall;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CrystalWall.Auths;
using Crystalwall.Test.Auths;
using System.Collections.Generic;
using System.Reflection;
using CrystalWall.Web;

namespace Crystalwall.Test
{
    
    
    /// <summary>
    ///这是 PrincipalTokenHolderTest 的测试类，旨在
    ///包含所有 PrincipalTokenHolderTest 单元测试
    ///</summary>
    ///<author>vincent valenlee</author>
    [TestClass()]
    public class PrincipalTokenHolderTest
    {
        private static IPrincipalTokenStorage tokenStorage;

        private static DBPrincipalProvider_Accessor target;

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
            tokenStorage = new TestPrincipalTokenStorage();
            target = new DBPrincipalProvider_Accessor();
            target.connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=Test;User ID=sa;Password=123456;";
            target.principaltable = "principal";
            target.permissiontable = "permission";
            target.foreigntable = "principal_permission";
            target.foreignuser = "principal_id";
            target.foreignpermission = "permission_id";
            PrincipalTokenHolder.Storage = tokenStorage;
            PrincipalTokenHolder.PrincipalProviders.Add((DBPrincipalProvider)target.Target);
        }
        //
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            PrincipalTokenHolder.Storage = null;
            PrincipalTokenHolder.PrincipalProviders.Clear();
            PrincipalTokenHolder.ClearCurrentToken();
        }
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
        ///ClearCurrentToken 的测试
        ///</summary>
        [TestMethod()]
        public void ClearCurrentTokenTest()
        {
            PrincipalTokenHolder.CurrentPrincipal = PrincipalTokenHolder.GetPrincipal("admin");
            PrincipalTokenHolder.ClearCurrentToken();
            Assert.IsNull(PrincipalTokenHolder.CurrentPrincipal, "当前用户不能清空");
        }

        /// <summary>
        ///GetPrincipal 的测试
        ///</summary>
        [TestMethod()]
        public void GetPrincipalTest()
        {
            IPrincipalToken actual  = PrincipalTokenHolder.GetPrincipal("admin");
            Assert.AreEqual("111111", actual.Certificate, "admin用户的密码应该为111111");

            actual = PrincipalTokenHolder.GetPrincipal("admin2");
            Assert.IsNull(actual, "admin2用户应该不存在");
        }

        /// <summary>
        ///CurrentPrincipal 的测试
        ///</summary>
        [TestMethod()]
        public void CurrentPrincipalTest()
        {
            PrincipalTokenHolder.CurrentPrincipal = PrincipalTokenHolder.GetPrincipal("admin");
            Assert.AreSame(PrincipalTokenHolder.CurrentPrincipal, PrincipalTokenHolder.Storage.GetCurrentToken());

            PrincipalTokenHolder.ClearCurrentToken();
            Assert.IsNull(PrincipalTokenHolder.Storage.GetCurrentToken(), "存储中当前的用户应该不存在");
        }

        /// <summary>
        ///测试PermissionInfo的！！操作符重载
        ///</summary>
        [TestMethod()]
        public void PermissionInfoOperatorTest()
        {
            PrincipalTokenHolder.CurrentPrincipal = PrincipalTokenHolder.GetPrincipal("admin");
            PermissionInfo p = new TestPermissionInfo("test", "test");
            try
            {
                p++;
                Assert.IsTrue(true, "代码必须运行到这里表示当前用户具有test权限");
            }
            catch (Exception e)
            {
                Assert.Fail("测试权限信息的操作符++失败，当前用户应该具有test权限，但执行++操作却抛出权限异常");
            }

            p = new TestPermissionInfo("testX", "testX");
            try
            {
                p++;
                Assert.Fail("测试权限信息的操作符++失败，当前用户不应该具有testX权限");
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(AccessException), "当前抛出的异常不是AccessException");
            }
            PrincipalTokenHolder.ClearCurrentToken();//清除当前用户
        }

        /// <summary>
        /// 测试Init初始化方法
        /// </summary>
        [TestMethod()]
        public void InitTest()
        {
            IPrincipalTokenStorage old_storage = PrincipalTokenHolder.Storage;
            IList<IPrincipalProvider> old_provider = PrincipalTokenHolder.PrincipalProviders;
            IPrincipalToken old_token = PrincipalTokenHolder.CurrentPrincipal;

            PrincipalTokenHolder.Init(Assembly.GetAssembly(typeof(IPrincipalProvider)).Location);
            Assert.IsInstanceOfType(PrincipalTokenHolder.Storage, typeof(WebPrincipalTokenStorage));
            Assert.AreEqual(2, PrincipalTokenHolder.PrincipalProviders.Count);

            PrincipalTokenHolder.Storage = old_storage;
            PrincipalTokenHolder.PrincipalProviders = old_provider;
            PrincipalTokenHolder.CurrentPrincipal = old_token;

        }
    }
}
