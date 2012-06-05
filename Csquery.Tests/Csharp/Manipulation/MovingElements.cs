using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;

namespace CsqueryTests.CSharp
{

    [TestFixture, TestClass]
    public class MovingElements
    {
        private static CQ csq;
        [TestFixtureSetUp,ClassInitialize]
        public static void Init(TestContext context)
        {
            Initialize();
        }

        private static void Initialize()
        {
            string html = Support.GetFile("csquery\\CsQuery.Tests\\Resources\\TestHtml.htm");
            csq = CQ.Create(html);
        }
        [TestInitialize, SetUp]
        public void TestInitialize()
        {
            Initialize();
        }
        [Test,TestMethod]
        public void Wrap()
        {
            var el = csq["#reputation_link"];
            var wrapper = CQ.Create("<div id=\"wrapper\"><span><div id=\"innerdiv\"></div></span></div><span><ul><li></li></ul></span>");
            int wrapperElementCount = wrapper.Select("*").Length-3;
            int domElementCount = csq.Select("*").Length;
            el.Wrap(wrapper);
            // get a current ref to wrapper - it gets cloned when added to the DOM so the previous object is obsolete
            wrapper = csq["#wrapper"];
            Assert.AreEqual(wrapperElementCount + domElementCount, csq.Select("*").Length, "Wrapper appears to have the right number of kids");
            Assert.AreEqual(wrapper.Parent()[0], csq["#hlinks-user"][0], "Wrapper is in correct place in DOM");
            Assert.AreEqual(1, csq["#wrapper"].Length, "Wrapper only appears once in DOM");
            Assert.AreEqual(csq["#innerdiv"].Children()[0], csq["#reputation_link"][0], "Wrapped element is in correct place in DOM");
        }
        [Test, TestMethod]
        public void UnWrap()
        {
            var oldLen = csq.Select("*").Length;
            var el = csq["#reputation_link, .badgecount:first"];
            Assert.AreEqual(2, el.Length, "Sanity check");
            Assert.AreEqual(1,csq["[title='13 bronze badges']"].Length,"Badgecount parent was found");
            el.Unwrap();
            Assert.AreEqual("body", csq[".badgecount:first"].Parent()[0].NodeName, "Badgecount appears to be unwrapped");
            Assert.AreEqual(0, csq["[title='2 silver badges']"].Length, "Badgecount parent is gone");
            Assert.AreEqual("body", csq["#reputation_link"].Parent()[0].NodeName, "reputation_link is now a direct child of body");
            Assert.AreEqual(oldLen - 2, csq.Select("*").Length, "Correct # of elements after unwrapping");
        }
        [Test, TestMethod]
        public void UnWrap2()
        {
            int count = csq.Select("body span").Length;
            var unwrapItem = csq["span[title='2 silver badges']"];
            Assert.IsTrue(unwrapItem.Parent()[0]==csq["#hlinks-user"][0],"Original parent is correct");
            unwrapItem.Unwrap();

            var stillThere = csq["span[title='2 silver badges']"];
            Assert.AreEqual(1, stillThere.Length, "Unwrap target still exists");
            Assert.AreEqual(count - 1, csq["body span"].Length, "There's one less span now");
            Assert.IsTrue(csq["span[title='2 silver badges']"].Parent()[0] == csq["body"][0], "It's moved up");

        
        }
    }
}