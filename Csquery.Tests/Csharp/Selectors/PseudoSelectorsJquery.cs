using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using CollectionAssert = NUnit.Framework.CollectionAssert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;

namespace CsqueryTests.Csharp.Selectors
{

    [TestFixture, TestClass]
    public class PseudoSelectorsJquery : CsQueryTest
    {

        
        private CQ VisibilityTestDom()
        {
            return CQ.Create(@"<div id='wrapper'>
                    <div id='outer' style='display:none;'>
                    <span id='inner'>should be hidden</span></div>
                    <div id='outer2' width='10'><span id='inner2'>should not be hidden</span></div>
                    <div id='outer3' height='0'><span id='inner3'>hidden</span></div>
                    <div id='outer4' style='width:0px;'><span id='inner4'>hidden</span></div>
                    <div id='outer5' style='display:block'><span id='inner5'>visible</span></div>
                    <div id='outer6' style='opacity: 0;'>Hidden</div>
                    <input type='hidden' value='nothing'>
                </div>
            ");

        }

        [Test, TestMethod]
        public void Visible()
        {
            var dom = VisibilityTestDom();

            var res = dom.Select("div:visible");
            Assert.AreEqual(dom.Select("#wrapper, #outer2, #outer5"), res, "Correct divs are visible");

            res = dom.Select("span:visible");
            Assert.AreEqual(dom.Select("#inner2,#inner5"), res, "Correct spans are visible");
        }
        

        /// <summary>
        /// Issue#11
        /// </summary>
        [Test, TestMethod]
        public void Visible_InputTypeHidden()
        {
            var dom = VisibilityTestDom();

            var res = dom["input[type=hidden]"];
            Assert.IsTrue(dom.Is(":visible"));

            Assert.AreEqual(1, dom["input:hidden"].Length);
            Assert.AreEqual(0, dom["input:visible"].Length);
        }

        [Test, TestMethod]
        public void Hidden()
        {
            var dom = VisibilityTestDom();

            var res = dom.Select("div:hidden");
            Assert.AreEqual(dom.Select("#outer,#outer3,#outer4,#outer6"), res, "Correct divs are visible");

            res = dom.Select("span:hidden");
            Assert.AreEqual(dom.Select("#inner, #inner3, #inner4"), res, "Correct spans are visible");
        }



        [Test, TestMethod]
        public void Odd()
        {
            var res = Dom["#hlinks-user span:odd"];
            Assert.AreEqual(4, res.Length);
            Assert.AreEqual("reputation-score", res[0].ClassName);
            Assert.AreEqual("badge2", res[1].ClassName);
        }


        [Test, TestMethod]
        public void Even()
        {
            var res = Dom["#hlinks-user span:even"];
            Assert.AreEqual(5, res.Length);
            Assert.AreEqual("profile-triangle", res[0].ClassName);
            Assert.AreEqual("badgecount", res[2].ClassName);
        }


        [Test, TestMethod]
        public void Checkbox()
        {
            CQ res = Dom.Find("input:checkbox");
            Assert.AreEqual(2, res.Length, "Expected to find 2 checkbox elements");
        }

        [Test, TestMethod]
        public void Header()
        {
            var dom = TestDom("Wiki-Cheese");

            CQ res = dom.Select(":header");
            Assert.AreEqual(39, res.Length);
        }

        #region Setup
        public override void FixtureSetUp()
        {
            Dom = TestDom("TestHtml");
        }
       
        #endregion
    }
}