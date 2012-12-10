using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;

namespace CsQuery.Tests.HtmlParser
{
    [TestClass]
    public class Rendering: CsQueryTest 
    {

        protected string node = "<div class='a b c c' attr1='{\"somejson\": \"someval\"}'>";

        [TestMethod, Test]
        public void HtmlCleanup()
        {
            var dom = CQ.CreateFragment(node);
            var expected =  "<div class=\"a b c\" attr1='{\"somejson\": \"someval\"}'></div>";
            Assert.AreEqual(expected, dom.Render(), "Basic cleanup - no duplicated class - missing end tag");


            // TODO
            // test attribute rendering options
            // Doctype options

        }
        [TestMethod,Test]
        public void AttributeQuoting()
        {


        }
        [TestMethod,Test]
        public void AttributeHandling()
        {
            string test1html = "<input type=\"text\" id=\"\" checked custom=\"sometext\">";
            var dom = CQ.CreateFragment(test1html);
            Assert.AreEqual("<input type=\"text\" id checked custom=\"sometext\">", dom.Render(), "Missing & boolean attributes are parsed & render correctly");

            // remove "quote all attributes"

            Assert.AreEqual("<input type=text id checked custom=sometext>", dom.Render(DomRenderingOptions.None), "Missing & boolean attributes are parsed & render correctly");

            dom = CQ.CreateFragment("<div id='test' quotethis=\"must've\" class=\"one two\" data='\"hello\"' noquote=\"regulartext\">");

            var expected = "<div class=\"one two\" id=test quotethis=\"must've\" data='\"hello\"' noquote=regulartext></div>";
            Assert.AreEqual(expected, dom.Render(DomRenderingOptions.None), "Handle various quoting situations");

            // go back to test 1
            dom = CQ.CreateFragment(test1html);

            var jq = dom["input"];
            var el = jq[0];

            Assert.AreEqual("", el["id"], "Empty attribute is empty");
            Assert.AreEqual("",el["checked"], "Boolean attribute is the same as empty");
            Assert.AreEqual(null, el["missing"], "Missing attribute is null");

            Assert.AreEqual("", jq.Attr("id"), "Empty attribute is empty");
            Assert.AreEqual("checked", jq.Attr("checked"), "Boolean attribute is true");
            Assert.AreEqual(true, jq.Prop("checked"), "Boolean attribute is true");
            Assert.AreEqual(null, jq.Attr("selected"), "Boolean attribute is true");
            // TODO - actually jquery would return "undefined" b/c selected doesn't apply to input. Need to do this mapping somewhere.
            Assert.AreEqual(false, jq.Prop("selected"), "Boolean attribute is true");
            Assert.AreEqual(null, jq.Attr("missing"), "Missing attribute is null");


        }

    
        /// <summary>
        /// Allow self closing tags - added feature to ITokenHandler implemenation in HtmlParserSharp to
        /// permit self-closing tags, if desired.
        /// </summary>

        [Test, TestMethod]
        public void AllowSelfClosing()
        {

            string html = @"<input /><span /><g />";
            var dom = CQ.Create(html, HtmlParsingMode.Fragment, HtmlParsingOptions.AllowSelfClosingTags);
            var output = dom.Render();

            Assert.AreEqual(@"<input><span></span><g></g>", output);

            dom = CQ.Create(html, HtmlParsingMode.Fragment, HtmlParsingOptions.None);
            output = dom.Render();

            // should nest the self-closed tags (default html5 behavior)
            Assert.AreEqual(@"<input><span><g></g></span>", output);

        }
    }
}
