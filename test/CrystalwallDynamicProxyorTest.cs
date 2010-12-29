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
using CrystalWall.Aop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CrystalWall;
using CrystalWall.Auths;
using System.Reflection;
using CrystalWall.Attr;
using CrystalWall.Permissions;

namespace Crystalwall.Test
{
    
    
    /// <summary>
    ///这是 CrystalwallDynamicProxyorTest 的测试类，旨在
    ///包含所有 CrystalwallDynamicProxyorTest 单元测试
    ///</summary>
    ///<author>vincent valenlee</author>
    [TestClass()]
    public class CrystalwallDynamicProxyorTest
    {

        private static IPrincipalTokenStorage tokenStorage;

        private TestContext testContextInstance;

        public const string TEST_NO_POINT_RESULT = "执行没有使用point元注释的方法!";

        public const string TEST_HAS_PERMISSION_RESULT = "执行具有元注释但具有权限的方法！";

        public const string TEST_NO_PERMISSION_RESULT = "执行具有元注释但不具有权限的方法！";

        public const string TEST_AND_PERMISSION_RESULT = "执行AND权限的方法！";

        public const string TEST_AND_NO_PERMISSION_RESULT = "执行AND无权限的方法！";

        public const string TEST_OR_PERMISSION_RESULT = "执行OR权限的方法！";

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

        }
        //
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            PrincipalTokenHolder.Clear();
        }
        
        //使用 TestInitialize 在运行每个测试前先运行代码
        [TestInitialize()]
        public void MyTestInitialize()
        {
            PrincipalTokenHolder.Clear();//首先清空
            PrincipalTokenHolder.Init(Assembly.GetAssembly(typeof(IPrincipalProvider)).Location);
            tokenStorage = new TestPrincipalTokenStorage();
            PrincipalTokenHolder.Storage = tokenStorage;
            PrincipalTokenHolder.CurrentPrincipal = PrincipalTokenHolder.GetPrincipal("admin");
        }
        
        //使用 TestCleanup 在运行完每个测试后运行代码
        [TestCleanup()]
        public void MyTestCleanup()
        {
            PrincipalTokenHolder.Clear();
        }
        
        #endregion


        /// <summary>
        ///ProxyClass 的测试
        ///</summary>
        public void ProxyClassTestHelper<T>()
            where T : class
        {
            var actual = CrystalwallDynamicProxyor.ProxyClass<ProxyTestClass>();
            string result = actual.MyTestNoPointMethod();
            Assert.AreEqual(TEST_NO_POINT_RESULT, result, "没有使用permissionpoint元标注的方法不应该被拦截");
        }

        [TestMethod()]
        public void ProxyClassTest()
        {
            ProxyClassTestHelper<ProxyTestClass>();
        }


        /// <summary>
        /// 测试具有权限的方法
        /// </summary>
        [TestMethod()]
        public void ProxyClassHasPermissionTest()
        {
            var actual = CrystalwallDynamicProxyor.ProxyClass<ProxyTestClass>();
            try
            {
                string result = actual.MyTestHasPermissionMethod();
                Assert.AreEqual(TEST_HAS_PERMISSION_RESULT, result, "admin用户具有test权限，测试成功！");
            }
            catch (AccessException e)
            {
                Assert.Fail("admin用户具有test权限，但却抛出了权限异常！但代码执行到这里，测试失败！");
            }
           
        }

        /// <summary>
        /// 测试具有权限的方法
        /// </summary>
        [TestMethod()]
        public void ProxyClassNoPermissionTest()
        {
            var actual = CrystalwallDynamicProxyor.ProxyClass<ProxyTestClass>();
            try
            {
                string result = actual.MyTestNoPermissionMethod();
                Assert.Fail("admin用户不具有test3权限，应该抛出权限异常，但代码执行到这里，测试失败！");
            }
            catch (AccessException e)
            {
                Assert.IsTrue(true);
            }
        }

        /// <summary>
        /// 测试逻辑权限点方法
        /// </summary>
        [TestMethod]
        public void ProxyClassAndNoPermissionTest()
        {
            var actual = CrystalwallDynamicProxyor.ProxyClass<ProxyTestClass>();
            try
            {
                string result = actual.MyTestAndNoPermissionMethod();
                Assert.Fail("admin用户不同时具有test3与test1权限，应该抛出权限异常，但代码执行到这里，测试失败！");
            }
            catch (AccessException e)
            {
                Assert.IsTrue(true);
            }
        }

        /// <summary>
        /// 测试具有逻辑或权限的方法
        /// </summary>
        [TestMethod()]
        public void ProxyClassOrPermissionTest()
        {
            var actual = CrystalwallDynamicProxyor.ProxyClass<ProxyTestClass>();
            try
            {
                string result = actual.MyTestOrPermissionMethod();
                Assert.AreEqual(TEST_OR_PERMISSION_RESULT, result, "admin用户具有test或者test3权限之一，测试成功！");
            }
            catch (AccessException e)
            {
                Assert.Fail("admin用户应该具有test与test3权限之一，但却抛出了权限异常！但代码执行到这里，测试失败！");
            }

        }

        /// <summary>
        /// 测试具有逻辑或权限的方法
        /// </summary>
        [TestMethod()]
        public void ProxyClassAndPermissionTest()
        {
            var actual = CrystalwallDynamicProxyor.ProxyClass<ProxyTestClass>();
            try
            {
                string result = actual.MyTestAndPermissionMethod();
                Assert.AreEqual(TEST_AND_PERMISSION_RESULT, result, "admin用户具有test与test2权限，测试成功！");
            }
            catch (AccessException e)
            {
                Assert.Fail("admin用户应该具有test与test2权限，但却抛出了权限异常！但代码执行到这里，测试失败！");
            }

        }



    }

    public class ProxyTestClass
    {
        public virtual string MyTestNoPointMethod()
        {
            return CrystalwallDynamicProxyorTest.TEST_NO_POINT_RESULT;
        }

        /// <summary>
        /// 测试数据库中admin用户具有test1权限
        /// </summary>
        [PermissionPoint(Name = "test", Action = "test", Type = "Crystalwall.Test.Auths.TestPermissionInfoPoint, Crystalwall.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
        public virtual string MyTestHasPermissionMethod()
        {
            return CrystalwallDynamicProxyorTest.TEST_HAS_PERMISSION_RESULT;
        }

        /// <summary>
        /// 测试数据库中admin用户不具有test3权限
        /// </summary>
        [PermissionPoint(Name = "test3", Action = "test3", Type = "Crystalwall.Test.Auths.TestPermissionInfoPoint, Crystalwall.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
        public virtual string MyTestNoPermissionMethod()
        {
            return CrystalwallDynamicProxyorTest.TEST_NO_PERMISSION_RESULT;
        }

        /// <summary>
        /// 测试数据库中admin用户必须同时具有test与test3权限
        /// </summary>
        /// <returns></returns>
        [LogicPermissionPoint(Name = "test", Action = "test", LeftType = "Crystalwall.Test.Auths.TestPermissionInfo, Crystalwall.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                                           RightName = "test3", RightAction = "test3", RightType = "Crystalwall.Test.Auths.TestPermissionInfo, Crystalwall.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", Logic = LogicPoint.AND)]
        public virtual string MyTestAndNoPermissionMethod()
        {
            return CrystalwallDynamicProxyorTest.TEST_AND_NO_PERMISSION_RESULT;
        }

        /// <summary>
        /// 测试数据库中admin用户必须同时具有test与test3权限
        /// </summary>
        /// <returns></returns>
        [LogicPermissionPoint(Name = "test", Action = "test", LeftType = "Crystalwall.Test.Auths.TestPermissionInfo, Crystalwall.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                                           RightName = "test2", RightAction = "test2", RightType = "Crystalwall.Test.Auths.TestPermissionInfo, Crystalwall.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", Logic = LogicPoint.AND)]
        public virtual string MyTestAndPermissionMethod()
        {
            return CrystalwallDynamicProxyorTest.TEST_AND_PERMISSION_RESULT;
        }

        /// <summary>
        /// 测试数据库中admin用户必须具有test与test3权限之一
        /// </summary>
        /// <returns></returns>
        [LogicPermissionPoint(Name = "test", Action = "test", LeftType = "Crystalwall.Test.Auths.TestPermissionInfo, Crystalwall.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                                           RightName = "test3", RightAction = "test3", RightType = "Crystalwall.Test.Auths.TestPermissionInfo, Crystalwall.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", Logic = LogicPoint.OR)]
        public virtual string MyTestOrPermissionMethod()
        {
            return CrystalwallDynamicProxyorTest.TEST_OR_PERMISSION_RESULT;
        }
    }

}
