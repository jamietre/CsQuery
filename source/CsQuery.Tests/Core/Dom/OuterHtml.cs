using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;

namespace CsQuery.Tests.Core.Dom
{
    [TestFixture, TestClass]
    public class OuterHtml: CsQueryTest
    {

        [Test, TestMethod]
        public void OuterHtmlGet()
        {
            var doc = TestDom("TestHtml");

            var res = doc[".reputation-score"];
            Assert.AreEqual(@"<span class=""reputation-score"" title=""your reputation; view reputation changes"">3,215</span>",
                res[0].OuterHTML);

            res = doc[".badge2"];
            Assert.AreEqual("<span class=\"badge2\"></span>", res[0].OuterHTML);
        }

        [Test, TestMethod]
        public void OuterHtmlSet()
        {
            var doc = TestDom("TestHtml");

            var res = doc["#reputation_link"][0];
            var replaceWith = "<h2 class=test-outer-html>New Content</h2><div class=test-outer-html><span>Some content<br>Some more content</span></div>";

            var parent = res.ParentNode;
            var currentLength = parent.ChildNodes.Length;
            var currentLeft = res.PreviousSibling;
            var currentRight = res.NextSibling;

            res.OuterHTML = replaceWith;

            var result = doc[".test-outer-html"];

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(currentLength + 1, parent.ChildNodes.Length);
            Assert.AreEqual(currentLeft, result[0].PreviousSibling);
            Assert.AreEqual(currentRight, result[1].NextSibling);



        }

        private CQ TestDocument()
        {
            return CQ.CreateDocument("<div><span></span></div>");
        }
    }
}

