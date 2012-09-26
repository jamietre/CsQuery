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

namespace CsQuery.Tests.Csharp.HtmlParser
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

            dom = CQ.CreateDocument("<!doctype html PUBLIC \"-//W3C//DTD XHTML 1.0 >");
            Assert.AreEqual(DocType.XHTML, dom.Document.DocType);

            dom.Document.DocTypeNode = dom.Document.CreateDocumentType("html", "", "");
            Assert.AreEqual("<!DOCTYPE html><html><head></head><body></body></html>", dom.First().Render());

        }
    }
}
