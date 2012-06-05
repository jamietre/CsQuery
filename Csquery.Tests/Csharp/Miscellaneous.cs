using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;

namespace CsqueryTests.Csharp
{
    
    [TestFixture, TestClass,Description("CsQuery Tests (Not from Jquery test suite)")]
    public class Miscellaneous: CsQueryTest
    {
        

        [Test, TestMethod]
        public void Encoding()
        {
            var res = CQ.Create("<div><span>x…</span></div>");
            Assert.AreEqual(res["div span"].Text(), "x"+(char)8230);

        }

        [Test, TestMethod]
        public void TestDocType()
        {
            var dom = CQ.Create("<!doctype html >");

            Assert.AreEqual(DocType.HTML5, dom.Document.DocType);
            Assert.AreEqual("<!DOCTYPE html>", dom[0].Render() );

            dom = CQ.Create("<!doctype html PUBLIC \"-//W3C//DTD XHTML 1.0 >");
            Assert.AreEqual(DocType.XHTML, dom.Document.DocType);

            dom.Document.DocType = DocType.HTML5;
            Assert.AreEqual("<!DOCTYPE html>", dom.First().Render());

        }

        #region setup
        public override void FixtureSetUp()
        {
            base.FixtureSetUp();
            Dom = TestDom("TestHtml");
        }
        #endregion

    }
}