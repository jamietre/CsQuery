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
    public class DocType_: CsQueryTest 
     {

        [Test, TestMethod]
        public void TestDocType()
        {
            var dom = CQ.CreateDocument("<!doctype html >");

            Assert.AreEqual(DocType.HTML5, dom.Document.DocType);
            Assert.AreEqual("<!DOCTYPE html>", dom[0].Render());

            string xhtmlType = "<!DOCTYPE html PUBLIC \"-//w3c//dtd xhtml 1.0\">";
            dom = CQ.CreateDocument(xhtmlType);
            Assert.AreEqual(DocType.XHTML, dom.Document.DocType);
            Assert.AreEqual(xhtmlType, dom.Document.DocTypeNode.Render());




            dom.Document.DocTypeNode = dom.Document.CreateDocumentType("html", "", "","");
            Assert.AreEqual("<!DOCTYPE html><html><head></head><body></body></html>", dom.First().Render());

        }

        public void TestDocTypeXHTML()
        {
            var dom = CQ.CreateDocument("<!doctype html >");
            dom.Document.DocTypeNode = dom.Document.CreateDocumentType(DocType.XHTMLStrict);

            var xhtmlStrict = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">";
            Assert.AreEqual(xhtmlStrict, dom.First().Render());


        }
    }
}
