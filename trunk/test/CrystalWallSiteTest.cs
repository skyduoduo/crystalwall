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
using Castle.DynamicProxy;
using System.Collections.Generic;
using System.Reflection;

namespace Crystalwall.Test
{
    
    
    /// <summary>
    ///这是 CrystalWallSiteTest 的测试类，旨在
    ///包含所有 CrystalWallSiteTest 单元测试
    ///</summary>
    ///<author>vincent valenlee</author>
    [TestClass()]
    public class CrystalWallSiteTest
    {

        private static IPrincipalTokenStorage tokenStorage;

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
            PrincipalTokenHolder.Clear();//首先清空
            PrincipalTokenHolder.Init(Assembly.GetAssembly(typeof(IPrincipalProvider)).Location);
            tokenStorage = new TestPrincipalTokenStorage();
            PrincipalTokenHolder.Storage = tokenStorage;
            PrincipalTokenHolder.CurrentPrincipal = PrincipalTokenHolder.GetPrincipal("admin");
        }
        
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            PrincipalTokenHolder.Clear();
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
        #endregion


        /// <summary>
        ///Find 的测试
        ///</summary>
        [TestMethod()]
        public void FindTest()
        {
            CrystalWallSite actual = CrystalWallSite.Find(new TestInvocationClass());
            Assert.IsNotNull(actual, "对应上下文invocation类的site不应该为null");
            Assert.AreEqual(1, actual.Sites.Count, "配置的site数目为1");
            IEnumerator<CrystalWallSite> enumerator = actual.Sites.GetEnumerator();
            enumerator.MoveNext();
            Assert.AreSame(actual, enumerator.Current, "缓存中获取的对应invocation上下文的site与实际得到的不相等");
            Assert.AreSame(typeof(DefaultDecider), actual.Decider.GetType(), "对应上下文invocation的site中的decider应该为默认的decider");
        }

        ///// <summary>
        /////InitSite 的测试
        /////</summary>
        //[TestMethod()]
        //public void InitSiteTest()
        //{
        //    CrystalWallSite target = new CrystalWallSite(); // TODO: 初始化为适当的值
        //    target.InitSite();
        //    Assert.Inconclusive("无法验证不返回值的方法。");
        //}
    }

    /// <summary>
    /// 仅用于测试sites的上下文对象实现
    /// </summary>
    public class TestInvocationClass : IInvocation
    {
        public object[] Arguments
        {
            get { throw new NotImplementedException(); }
        }

        public Type[] GenericArguments
        {
            get { throw new NotImplementedException(); }
        }

        public object GetArgumentValue(int index)
        {
            throw new NotImplementedException();
        }

        public System.Reflection.MethodInfo GetConcreteMethod()
        {
            throw new NotImplementedException();
        }

        public System.Reflection.MethodInfo GetConcreteMethodInvocationTarget()
        {
            throw new NotImplementedException();
        }

        public object InvocationTarget
        {
            get { throw new NotImplementedException(); }
        }

        public System.Reflection.MethodInfo Method
        {
            get { throw new NotImplementedException(); }
        }

        public System.Reflection.MethodInfo MethodInvocationTarget
        {
            get { throw new NotImplementedException(); }
        }

        public void Proceed()
        {
            throw new NotImplementedException();
        }

        public object Proxy
        {
            get { throw new NotImplementedException(); }
        }

        public object ReturnValue
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void SetArgumentValue(int index, object value)
        {
            throw new NotImplementedException();
        }

        public Type TargetType
        {
            get { throw new NotImplementedException(); }
        }
    }
}
