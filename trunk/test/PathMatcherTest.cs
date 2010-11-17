using CrystalWall.Permissions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Crystalwall.Test
{
    
    
    /// <summary>
    ///这是 PathMatcherTest 的测试类，旨在
    ///包含所有 PathMatcherTest 单元测试
    ///</summary>
    [TestClass()]
    public class PathMatcherTest
    {


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
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
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
        ///Match成功的测试
        ///</summary>
        [TestMethod()]
        public void MatchSuccessTest()
        {
            PathMatcher target = new PathMatcher();
            string regex = "/xyz/mn/**/pq/*.exe";
            string path = "/xyz/mn/abc/d/pq/x.exe";
            bool actual = target.Match(regex, path);
            Assert.IsTrue(actual, "全英文字符应该匹配");

            regex = "/xyz/中文/**/我国/abc*我的.exe";
            path = "/xyz/中文/**/我国/abc我们的就是我的.exe";
            actual = target.Match(regex, path);
            Assert.IsTrue(actual, "中英文混合字符应该匹配");
        }

        /// <summary>
        ///Match失败的测试
        ///</summary>
        [TestMethod()]
        public void MatchFailueTest()
        {
            //TODO:添加匹配失败的测试
        }
    }
}
