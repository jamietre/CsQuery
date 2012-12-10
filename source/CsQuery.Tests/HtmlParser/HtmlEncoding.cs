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

namespace CsQuery.Tests.HtmlParser
{

    [TestFixture, TestClass]
    public class HtmlEncoding : CsQueryTest
    {

        string spanish = "Romney dice que Chávez expande la tiranía";
        [TestMethod, Test]
        public void Utf8Handling()
        {
            
            var dom = CQ.CreateFragment("<div>" + spanish + "</div>");

            // the "render" method should turn UTF8 characters to HTML. Accessing the node value directly should not.

            Assert.AreEqual(spanish, dom["div"][0].ChildNodes[0].NodeValue);
            Assert.AreEqual(spanish, dom.Text());
            Assert.AreEqual("<div>Romney dice que Ch&#225;vez expande la tiran&#237;a</div>", dom.Render());

        }


        [TestMethod, Test]
        public void FullHtmlEncoding()
        {
            var dom = CQ.CreateFragment("<div>" + spanish + "</div>");

            Assert.AreEqual(spanish, dom.Text());
            var output = dom.Render(OutputFormatters.HtmlEncodingFull);
            Assert.AreEqual("<div>Romney dice que Ch&aacute;vez expande la tiran&iacute;a</div>", output);
        }


        [TestMethod, Test]
        public void RoundTrip()
        {
            var dom = CQ.CreateFragment("<div>&#8482;&#160;&#219;&#8225;&#8664;&#8665;</div>");

            var output = dom.Render(OutputFormatters.HtmlEncodingFull);
            Assert.AreEqual("<div>&trade;&nbsp;&Ucirc;&Dagger;&#8664;&#8665;</div>", output);
        }


    }
}
