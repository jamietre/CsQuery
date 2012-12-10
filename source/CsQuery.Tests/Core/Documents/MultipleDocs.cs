using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using CollectionAssert = NUnit.Framework.CollectionAssert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.HtmlParser;
using CsQuery.Utility;

namespace CsQuery.Tests.Core
{

    [TestFixture, TestClass]
    public class MultipleDocs : CsQueryTest
    {

        [Test, TestMethod]
        public void AddFragment()
        {
            var dom1 = CQ.CreateFragment("<div id='doc1-div1'><div id='doc1-div2'>innertext</div></div>");
            var dom2 = CQ.CreateFragment("<div></div>");

            var els = dom2["<div id='doc2-div1'><div id='doc2-div2'>innertext</div></div>"];

            //var el = dom1["#doc1-div2"][0];

            dom1["#doc1-div1"].Append(els);

            //Assert.AreEqual(0, dom1["#doc1-div2"].Length);

            Assert.AreEqual(2, dom1["div:first"].Children().Length);
            Assert.AreEqual(1, dom1["#doc2-div2"].Length);
        }

        /// <summary>
        /// Ensure that when you create a new fragment from elements, then add them somewhere else, they
        /// don't get removed from the other document.
        /// </summary>

        [Test, TestMethod]
        public void CreateFromElements()
        {
            var dom1 = CQ.CreateFragment("<div id='doc1-div1'><div id='doc1-div2'>innertext</div></div>");
            var dom2 = CQ.CreateFragment("<div id='doc2-div1'><div id='doc2-div2'>innertext</div></div>");

            var el = dom1["#doc1-div2"][0];

            dom2["#doc2-div1"].Append(el);

            // should be moved when done this way
            Assert.AreEqual(0, dom1["#doc1-div2"].Length);
            Assert.AreEqual(2, dom2["#doc2-div1"].Children().Length);

            // move it back
            dom1["#doc1-div1"].Append(dom2["#doc1-div2"]);
            Assert.AreEqual(1, dom1["#doc1-div2"].Length);
            Assert.AreEqual(1, dom2["#doc2-div1"].Children().Length);

            // This time it should be added to the target, and not removed from the source.
            var els = CQ.CreateFragment(dom1["#doc1-div2"]);
            dom2["#doc2-div1"].Append(els);

            Assert.AreEqual(1, dom1["#doc1-div2"].Length);
            Assert.AreEqual(2, dom2["#doc2-div1"].Children().Length);
        }
    }
}