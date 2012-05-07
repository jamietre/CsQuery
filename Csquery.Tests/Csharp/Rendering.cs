using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CsQuery;

namespace CsqueryTests.Csharp
{
    [TestClass]
    public class Rendering:CsQueryTest 
     {

        protected string node = "<div class='a b c c' attr1='{\"somejson\": \"someval\"}'";



        [TestMethod]
        public void HtmlCleanup()
        {
            var dom = CQ.Create(node);
            var expected =  "<div class=\"a b c\" attr1='{\"somejson\": \"someval\"}'></div>";
            Assert.AreEqual(expected, dom.Render(), "Basic cleanup - no duplicated class - missing end tag");


            // TODO
            // test attribute rendering options
            // Doctype options

        }
        [TestMethod]
        public void AttributeQuoting()
        {


        }
        [TestMethod]
        public void AttributeHandling()
        {
            string test1html = "<input type=\"text\" id=\"\" checked custom=\"sometext\">";
            var dom = CQ.Create(test1html);
            Assert.AreEqual("<input id=\"\" type=\"text\" checked custom=\"sometext\" />", dom.Render(), "Missing & boolean attributes are parsed & render correctly");

            // remove "quote all attributes"
            dom.Document.DomRenderingOptions = 0;

            Assert.AreEqual("<input id=\"\" type=text checked custom=sometext />", dom.Render(), "Missing & boolean attributes are parsed & render correctly");

            dom = CQ.Create("<div id='test' quotethis=\"must've\" class=\"one two\" data='\"hello\"' noquote=\"regulartext\"");
            dom.Document.DomRenderingOptions = 0;

            var expected = "<div id=test class=\"one two\" quotethis=\"must've\" data='\"hello\"' noquote=regulartext></div>";
            Assert.AreEqual(expected, dom.Render(), "Handle various quoting situations");

            // go back to test 1
            dom = CQ.Create(test1html);

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
    }
}
