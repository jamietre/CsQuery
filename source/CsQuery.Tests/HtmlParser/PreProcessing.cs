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

namespace CsQuery.Tests.HtmlParser.HTML5
{

    [TestFixture, TestClass]
    public class PreProcessing : CsQueryTest
    {
        [Test, TestMethod]
        public void ExpandingSelfClosingTags()
        {
            string testHtml = @"<div/><span/><bad />";
            var dom = CQ.Create(testHtml,HtmlParsingMode.Fragment);
            Assert.AreEqual("<div><span><bad></bad></span></div>", dom.Render());

            dom = CQ.Create(testHtml, HtmlParsingMode.Fragment,HtmlParsingOptions.AllowSelfClosingTags);
            Assert.AreEqual("<div></div><span></span><bad></bad>", dom.Render());
        }

        [Test, TestMethod]
        public void ExpandingSelfClosingTags_Complex()
        {
            string testHtml = @"<div id=""some stuff""/><span/><bad />";

            var dom = CQ.Create(testHtml);
            Assert.AreEqual("<div id=\"some stuff\"></div><span></span><bad></bad>", dom.Render());


            // jQuery gets this wrong -- probably uses a regex similar to the one CsQuery used to use 
            // to blow up self-closed tags before parsing. CsQuery now handles them in the parser itself
            // so this works right
            
            testHtml = @"<fake id=""some stuff"" val="">"" /><b></b><span/><bad />";

            dom = CQ.Create(testHtml);
            Assert.AreEqual(@"<fake id=""some stuff"" val="">""></fake><b></b><span></span><bad></bad>", dom.Render());


            // Removing the caret, it works
            testHtml = @"<fake id=""some stuff"" val=""caret"" tricky=/ /><b></b><span/><bad />";

            dom = CQ.Create(testHtml);
            Assert.AreEqual(@"<fake id=""some stuff"" val=""caret"" tricky=""/""></fake><b></b><span></span><bad></bad>", dom.Render());
        }

        [Test, TestMethod]
        public void DocumentTypeCreation()
        {
            var dom = CQ.Create("<div></div>");
            Assert.AreEqual(1, dom.Document.ChildNodes.Length);
            Assert.AreEqual(1, dom["*"].Length);
            Assert.AreEqual(DocType.HTML5, dom.Document.DocType);

            dom = CQ.Create("<div></div>",HtmlParsingMode.Document,HtmlParsingOptions.None,DocType.HTML4);
            Assert.AreEqual(1, dom.Document.ChildNodes.Length);
            Assert.AreEqual(4, dom["*"].Length);

            // Parsing with a DocType does NOT set it unless you manually add a DOCTYPE node.
            Assert.AreEqual(DocType.HTML5, dom.Document.DocType);

        }
        

    }
}
