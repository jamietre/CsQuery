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
    public class MovingElements: CsQueryTest
    {

        public override void FixtureSetUp()
        {
            base.FixtureSetUp();
            Dom = TestDom("TestHtml");
        }

        [Test,TestMethod]
        public void Wrap()
        {
            var el = Dom["#reputation_link"];
            var wrapper = CQ.CreateFragment("<div id=\"wrapper\"><span><div id=\"innerdiv\"></div></span></div><span><ul><li></li></ul></span>");
            int wrapperElementCount = wrapper.Select("*").Length-3;
            int domElementCount = Dom.Select("*").Length;
            el.Wrap(wrapper);
            // get a current ref to wrapper - it gets cloned when added to the DOM so the previous object is obsolete
            wrapper = Dom["#wrapper"];
            Assert.AreEqual(wrapperElementCount + domElementCount, Dom.Select("*").Length, "Wrapper appears to have the right number of kids");
            Assert.AreEqual(wrapper.Parent()[0], Dom["#hlinks-user"][0], "Wrapper is in correct place in DOM");
            Assert.AreEqual(1, Dom["#wrapper"].Length, "Wrapper only appears once in DOM");
            Assert.AreEqual(Dom["#innerdiv"].Children()[0], Dom["#reputation_link"][0], "Wrapped element is in correct place in DOM");
        }
        [Test, TestMethod]
        public void UnWrap()
        {
            var oldLen = Dom.Select("*").Length;
            var el = Dom["#reputation_link, .badgecount:first"];
            Assert.AreEqual(2, el.Length, "Sanity check");
            Assert.AreEqual(1, Dom["[title='13 bronze badges']"].Length, "Badgecount parent was found");
            el.Unwrap();
            Assert.AreEqual("BODY", Dom[".badgecount:first"].Parent()[0].NodeName, "Badgecount appears to be unwrapped");
            Assert.AreEqual(0, Dom["[title='2 silver badges']"].Length, "Badgecount parent is gone");
            Assert.AreEqual("BODY", Dom["#reputation_link"].Parent()[0].NodeName, "reputation_link is now a direct child of body");
            Assert.AreEqual(oldLen - 2, Dom.Select("*").Length, "Correct # of elements after unwrapping");
        }
        [Test, TestMethod]
        public void UnWrap2()
        {
            int count = Dom.Select("body span").Length;
            var unwrapItem = Dom["span[title='2 silver badges']"];
            Assert.IsTrue(unwrapItem.Parent()[0] == Dom["#hlinks-user"][0], "Original parent is correct");
            unwrapItem.Unwrap();

            var stillThere = Dom["span[title='2 silver badges']"];
            Assert.AreEqual(1, stillThere.Length, "Unwrap target still exists");
            Assert.AreEqual(count - 1, Dom["body span"].Length, "There's one less span now");
            Assert.IsTrue(Dom["span[title='2 silver badges']"].Parent()[0] == Dom["body"][0], "It's moved up");

        
        }
    }
}