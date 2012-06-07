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

        [Test, TestMethod, Description("ID with space - issue #5")]
        public void TestInvalidID()
        {
            string html = @"<img alt="""" id=""Picture 7"" src=""Image.aspx?imageId=26381""
                style=""border-top-width: 1px; border-right-width: 1px; border-bottom-width:
                1px; border-left-width: 1px; border-top-style: solid; border-right-style:
                solid; border-bottom-style: solid; border-left-style: solid; margin-left:
                1px; margin-right: 1px; width: 950px; height: 451px; "" />";

            var dom = CQ.Create(html);
            var res = dom["img"];
            Assert.AreEqual(1, res.Length);
            Assert.AreEqual("Picture 7", res[0].Id);

            var img = dom.Document.GetElementById("Picture 7");
            Assert.IsNotNull(img);
            Assert.AreEqual("Picture 7", img.Id);

            img = dom.Document.GetElementById("Picture");
            Assert.IsNull(img);

            dom = CQ.Create("<body><div></div><img id=\"image test\" src=\"url\"/>content</div></body>");
            res = dom["img"];
            Assert.AreEqual(1, res.Length);
            Assert.AreEqual("image test", res[0].Id);

        }

        /// <summary>
        /// Issue #5 revealed that during DOM creation duplicate IDs were being stripped. We want this
        /// when an end-user adds things to an existing DOM (e.g. for clones) but not when building from HTML.
        /// Added "AddAlways" method to NodeList to eliminate this check during DOM creation. This actually made
        /// a significant performance improvement for DOM creation too.
        /// </summary>
        [Test, TestMethod, Description("Issue #5 side effects - make sure dup ids can be added")]
        public void TestInvalidID2()
        {
            string html = @"<div id=""test""></div><div id=""test""></div>";
            var dom = CQ.Create(html);
            
            var res = dom["#test"];
            Assert.AreEqual(2, res.Length);
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