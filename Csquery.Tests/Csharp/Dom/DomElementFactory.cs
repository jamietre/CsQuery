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

namespace CsqueryTests.Csharp
{
    
    [TestFixture, TestClass,Description("HTML parser (DomElementFactory)")]
    public class DomElementFactory_ : CsQueryTest
    {
        DomElementFactory factory;
        const string testFile="csquery\\CsQuery.Tests\\Resources\\TestHtml.htm";
        [SetUp]
        public override void FixtureSetUp()
        {
            base.FixtureSetUp();
            string html = Support.GetFile(testFile);
            Dom = CQ.Create(html);
            factory = new DomElementFactory(Dom.Document);
        }
        [Test,TestMethod]
        public void BasicCreate()
        {

            var output = CreateFromHtml("<div/>");
            Assert.AreEqual("<div></div>", output.Render(), "Basic element creation");

            output = CreateFromHtml("<div><span></div>");
            Assert.AreEqual("<div><span></span></div>", output.Render(), "broken closing tag");
            
            output = CreateFromHtml("<div><input type=text><ul><li>item1<li>item2</div>");
            Assert.AreEqual("<div><input type=\"text\" /><ul><li>item1</li><li>item2</li></ul></div>", output.Render(), "broken closing tag");

            output = CreateFromHtml("<div><span>yyy</div>xxx");
            Assert.AreEqual("<div><span>yyy</span></div>xxx", output.Render(), "broken closing tag + trailing text");

            output = CreateFromHtml("  <div><span>yyy</div>xxx  ");
            Assert.AreEqual("  <div><span>yyy</span></div>xxx  ", output.Render(), "broken closing tag + leading + trailing text");
            
            output = CreateFromHtml("<div>a&#32;space&amp;&lt;div&gt;--that was text</div>");
            Assert.AreEqual("<div>a space&amp;&lt;div&gt;--that was text</div>", output.Render(), "special characters");

            output = CreateFromHtml("<div>&nbsp;spacer&nbsp;</div>");
            Assert.AreEqual("<div>&#160;spacer&#160;</div>", output.Render(), "special characters 2");

            output = CreateFromHtml("<div>xxxxx");
            Assert.AreEqual("<div>xxxxx</div>", output.Render(), "trailing text with broken tag");

            output = CreateFromHtml("<input type=\"text>text<div> value=\"x\"><div>xxx");
            Assert.AreEqual("<input type=\"text>text<div> value=\" x /><div>xxx</div>", output.Render(), "some messed up attributes -- keeps open til closing quote, tosses unexpected quote");

            // when an unexpected close tag is found:
            // 1) Try to match to an open tag above. If found, close everything currently open.
            // 2) If not matched, treat as text.
            
            output = CreateFromHtml("<div><span>xxxxx</div><ul><li>before</span>after<crap type=\"x\">");
            Assert.AreEqual("<div><span>xxxxx</span></div><ul><li>before&lt;/span&gt;after<crap type=\"x\"></crap></li></ul>", output.Render(), "broken tag rules");

            // TODO: Need to be able to enforce child rules for elements like LI, TD, etc. This should force a close of the open LI tag.
            //output = CreateFromHtml("<div><span>xxxxx</div><ul><li>before</span>after<crap type=\"x\"><li>implicit close");
        }


        [Test, TestMethod]
        public void CopyEntireDom()
        {
            string html = Support.GetFile("csquery\\csquery.tests\\resources\\HTML Standard.htm");

            var dom = CQ.Create(html);
            string output = dom.Render();
            var dom2 = CQ.Create(output);
            string output2 = dom2.Render();
            Assert.AreEqual(output, output2, "There's no entropy in reparsing a rendered domain.");
            Debug.Write("HTML5 spec parsing: original was " + html.Length + " bytes. CSQ output was " + output.Length);

        }
        protected CQ CreateFromHtml(string html)
        {
            return CQ.Create(factory.CreateObjects(html));
        }
    }
}