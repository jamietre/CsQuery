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

namespace CsQuery.Tests.Core.Manipulation
{

    [TestFixture, TestClass]
    public class MovingElements: CsQueryTest
    {

        [Test,TestMethod]
        public void Wrap()
        {
            var dom = TestDom("TestHtml");
            var el = dom["#reputation_link"];
            var wrapper = CQ.CreateFragment("<div id=\"wrapper\"><span><div id=\"innerdiv\"></div></span></div><span><ul><li></li></ul></span>");
            int wrapperElementCount = wrapper.Select("*").Length-3;
            int domElementCount = dom.Select("*").Length;
            el.Wrap(wrapper);
            // get a current ref to wrapper - it gets cloned when added to the DOM so the previous object is obsolete
            wrapper = dom["#wrapper"];
            Assert.AreEqual(wrapperElementCount + domElementCount, dom.Select("*").Length, "Wrapper appears to have the right number of kids");
            Assert.AreEqual(wrapper.Parent()[0], dom["#hlinks-user"][0], "Wrapper is in correct place in DOM");
            Assert.AreEqual(1, dom["#wrapper"].Length, "Wrapper only appears once in DOM");
            Assert.AreEqual(dom["#innerdiv"].Children()[0], dom["#reputation_link"][0], "Wrapped element is in correct place in DOM");
        }
        [Test, TestMethod]
        public void UnWrap()
        {
            var dom = TestDom("TestHtml");
            var oldLen = dom.Select("*").Length;
            var el = dom["#reputation_link, .badgecount:first"];
            Assert.AreEqual(2, el.Length, "Sanity check");
            Assert.AreEqual(1, dom["[title='13 bronze badges']"].Length, "Badgecount parent was found");
            el.Unwrap();
            Assert.AreEqual("BODY", dom[".badgecount:first"].Parent()[0].NodeName, "Badgecount appears to be unwrapped");
            Assert.AreEqual(0, dom["[title='2 silver badges']"].Length, "Badgecount parent is gone");
            Assert.AreEqual("BODY", dom["#reputation_link"].Parent()[0].NodeName, "reputation_link is now a direct child of body");
            Assert.AreEqual(oldLen - 2, dom.Select("*").Length, "Correct # of elements after unwrapping");
        }
        [Test, TestMethod]
        public void UnWrap2()
        {
            var dom = TestDom("TestHtml");
            int count = dom.Select("body span").Length;
            var unwrapItem = dom["span[title='2 silver badges']"];
            Assert.IsTrue(unwrapItem.Parent()[0] == dom["#hlinks-user"][0], "Original parent is correct");
            unwrapItem.Unwrap();

            var stillThere = dom["span[title='2 silver badges']"];
            Assert.AreEqual(1, stillThere.Length, "Unwrap target still exists");
            Assert.AreEqual(count - 1, dom["body span"].Length, "There's one less span now");
            Assert.IsTrue(dom["span[title='2 silver badges']"].Parent()[0] == dom["body"][0], "It's moved up");

        
        }
    }
}