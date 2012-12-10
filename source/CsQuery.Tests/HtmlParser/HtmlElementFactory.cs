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
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.HtmlParser;
using CsQuery.Utility;

namespace CsQuery.Tests.HtmlParser
{
    
    [TestFixture, TestClass]
    public class HtmlElementFactory : CsQueryTest
    {
      

        [SetUp]
        public override void FixtureSetUp()
        {
            base.FixtureSetUp();

            Dom = TestDom("TestHtml");
        }

        [Test,TestMethod]
        public void BasicCreate()
        {

            var output = CreateFromHtml("<div></div>");
            Assert.AreEqual("<div></div>", output.Render(), "Basic element creation");

            output = CreateFromHtml("<div/>");
            Assert.AreEqual("<div></div>", output.Render(), "Basic element creation");

            output = CreateFromHtml("<div><span></div>");
            Assert.AreEqual("<div><span></span></div>", output.Render(), "broken closing tag");
            
            output = CreateFromHtml("<div><input type=text><ul><li>item1<li>item2</div>");
            Assert.AreEqual("<div><input type=\"text\"><ul><li>item1</li><li>item2</li></ul></div>", output.Render(), "broken closing tag");

            output = CreateFromHtml("<div><span>yyy</div>xxx");
            Assert.AreEqual("<div><span>yyy</span></div>xxx", output.Render(), "broken closing tag + trailing text");

            output = CreateFromHtml("  <div><span>yyy</div>xxx  ");
            Assert.AreEqual("  <div><span>yyy</span></div>xxx  ", output.Render(), "broken closing tag + leading + trailing text");
            
            output = CreateFromHtml("<div>a&#32;space&amp;&lt;div&gt;--that was text</div>");
            Assert.AreEqual("<div>a space&amp;&lt;div&gt;--that was text</div>", output.Render(), "special characters");

            output = CreateFromHtml("<div>&nbsp;spacer&nbsp;</div>");
            Assert.AreEqual("<div>&nbsp;spacer&nbsp;</div>", output.Render(), "special characters 2");

            output = CreateFromHtml("<div>xxxxx");
            Assert.AreEqual("<div>xxxxx</div>", output.Render(), "trailing text with broken tag");

            output = CreateFromHtml("<input type=\"text>text<div> value=\"x\"><div>xxx");
            
            //Assert.AreEqual("<input type=\"text>text<div> value=\" x /><div>xxx</div>", output.Render(), "some messed up attributes -- keeps open til closing quote, tosses unexpected quote");
            // test changed for validator.nu parser - this is correct now! The attribute name should actually be 'x"'
            Assert.AreEqual("<input type=\"text>text<div> value=\" x\"><div>xxx</div>", output.Render(), "some messed up attributes -- keeps open til closing quote, tosses unexpected quote");

            // when an unexpected close tag is found:
            // 1) Try to match to an open tag above. If found, close everything currently open.
            // 2) If not matched, treat as text.
            
            output = CreateFromHtml("<div><span>xxxxx</div><ul><li>before</span>after<crap type=\"x\">");
            //Assert.AreEqual("<div><span>xxxxx</span></div><ul><li>before&lt;/span&gt;after<crap type=\"x\"></crap></li></ul>", output.Render(), "broken tag rules");
            //  another test correct now - matches chrome output
            Assert.AreEqual("<div><span>xxxxx</span></div><ul><li>beforeafter<crap type=\"x\"></crap></li></ul>", output.Render(), "broken tag rules");

            // In Chrome, the final text "implicit close" is thrown out. I don't know why but validator.nu seems to be doing it right
            
            output = CreateFromHtml("<div><span>xxxxx</div><ul><li>before</span>after<crap type=\"x\"><li>implicit close");
            Assert.AreEqual("<div><span>xxxxx</span></div><ul><li>beforeafter<crap type=\"x\"></crap></li><li>implicit close</li></ul>", output.Render(), "broken tag rules");
        }


        [Test, TestMethod]
        public void CopyEntireDom()
        {

            string html = Support.GetFile(TestDomPath("HTML Standard"));
            
            var dom = CQ.Create(html);
            string output = dom.Render();
            var dom2 = CQ.Create(output);
            string output2 = dom2.Render();
            Assert.AreEqual(output, output2, "There's no entropy in reparsing a rendered domain.");
            Debug.Write("HTML5 spec parsing: original was " + html.Length + " bytes. CSQ output was " + output.Length);

        }
        protected CQ CreateFromHtml(string html)
        {
            return CQ.CreateFragment(html);
        }

        [Test, TestMethod]
        public void CreationState()
        {
            // bug 6/18 -- doctype isn't really supposed to be selectable.

            var dom = TestDom("wiki-cheese");
            

            Assert.AreEqual(0,dom["\\!doctype"].Length);
            Assert.IsFalse(dom["*"].Any(item => item.NodeName == "!DOCTYPE"));
            Assert.IsFalse(dom["*"].Any(item=>item.NodeType == NodeType.DOCUMENT_TYPE_NODE));

            Assert.AreEqual(2,dom.Document.ChildNodes.Count);
            Assert.AreEqual(NodeType.DOCUMENT_TYPE_NODE,dom.Document.ChildNodes.First().NodeType);
            Assert.AreEqual("HTML",dom.Document.ChildNodes.Last().NodeName);
        }

        [Test, TestMethod]
        public void ComplicatedComments()
        {
            string html = @"<!--
    Where would document.write() insert?
    Consider: data:text/xml,<script xmlns=""http://www.w3.org/1999/xhtml""><![CDATA[ document.write('<foo>Test</foo>'); ]]></script>
    -->";
            html = "<header><div>" + html + "</div></header>";
            var dom = CQ.CreateFragment(html);

            Assert.AreEqual(1, dom["div"].Contents().Where(item => item.NodeType == NodeType.COMMENT_NODE).Count());
            Assert.AreEqual(2, dom["*"].Length);

        }

        [Test, TestMethod]
        public void AttributeEncode()
        {
            string onlyQuotes = "{\"someprop\": 1, \"someprop2\", \"someval\"}";
            string onlyApos = "{'someprop': 1, 'someprop2', 'someval'}";
            string both = "{\"someprop\": 1, \"someprop2\", \"o'brien\"}";
            string neither = "plain old text";

            string quoteChar;

            string result = HtmlData.AttributeEncode(onlyQuotes, true, out quoteChar);
            Assert.AreEqual(onlyQuotes, result, "With only quotes, nothing changed");
            Assert.AreEqual("'", quoteChar, "Quote char was an apostrophe with only quotes");

            result = HtmlData.AttributeEncode(onlyApos, true, out quoteChar);
            Assert.AreEqual(onlyApos, result, "With only apostrophes, nothing changed");
            Assert.AreEqual("\"", quoteChar, "Quote char was a quote with only apos");

            result = HtmlData.AttributeEncode(both, true, out quoteChar);
            string expected = "{\"someprop\": 1, \"someprop2\", \"o&#39;brien\"}";
            Assert.AreEqual(expected, result, "With both, only apostrophes changed");
            Assert.AreEqual("'", quoteChar, "Quote char was an apos with both");

            result = HtmlData.AttributeEncode(neither, true, out quoteChar);
            Assert.AreEqual(neither, result, "With neither, nothing changeed");
            Assert.AreEqual("\"", quoteChar, "Quote char was a quote with both");


        }

    }

    

}