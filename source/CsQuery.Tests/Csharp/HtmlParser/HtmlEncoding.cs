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

namespace CsQuery.Tests.Csharp.HtmlParser
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
            var dom = CQ.CreateFragment("<div>&#8482;&#160;&#219;&#8225;&#8664;&#8665;&#123445;</div>");

            var output = dom.Render(OutputFormatters.HtmlEncodingFull);
            Assert.AreEqual("<div>&TRADE;&nbsp;&Ucirc;&Dagger;&seArr;&swArr;&#123445;</div>", output);
        }

        /// <summary>
        /// This test ensures the full encoder (that maps entities to names) works, and that the parser
        /// correctly interprets all the encodednames. It renders everything first using the names; then
        /// parses that and re-renders so entites over ascii 160 are just numbers, then finally full-
        /// encodes again and compares the two full-encoded versions. Since the HTML spec document uses
        /// all these codes, and this sources the encoder data, we expect them to be rendered correctly --
        /// so this really tests the HTML5 parser's ability to decode them.
        /// </summary>

        [TestMethod, Test]
        public void EncoderBigDomRoundTrip()
        {

            string html = Support.GetFile(TestDomPath("HTML Standard"));
            
            var dom = CQ.Create(html);
            // render it using the full markup
            string outputFullFirst = dom.Render(OutputFormatters.HtmlEncodingFull);

            // pull it back in
            dom = CQ.Create(outputFullFirst);

            // output it wihtout using the full scheme
            string outputFullSecond = dom.Render(OutputFormatters.HtmlEncodingBasic);

            // recycle again...
            dom = CQ.Create(outputFullSecond);
            outputFullSecond = dom.Render(OutputFormatters.HtmlEncodingFull);

            // and make sure they still match!
            Assert.AreEqual(outputFullFirst, outputFullSecond, "There's no entropy in reparsing a rendered domain.");

        }       
    }
}
