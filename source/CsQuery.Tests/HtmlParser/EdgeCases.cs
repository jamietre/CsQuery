using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using CollectionAssert = NUnit.Framework.CollectionAssert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.HtmlParser;
using CsQuery.Utility;

namespace CsQuery.Tests.HtmlParser
{

    [TestFixture, TestClass]
    public class EdgeCases : CsQueryTest
    {

        [Test, TestMethod]
        public void UnquotedAttributeHandling()
        {

            CQ doc = new CQ("<div custattribute=10/23/2012 id=\"tableSample\"><span>sample text</span></div>");
            IDomElement obj = doc["#tableSample"].FirstElement();

            Assert.AreEqual("10/23/2012", obj["custattribute"]);
        }

        /// <summary>
        /// Random JSDom issue
        /// https://github.com/tmpvar/jsdom/issues/494
        /// </summary>

        [Test, TestMethod]
        public void CaretsInAttributes()
        {
            CQ doc = "<div><img src=\"test.png\" alt=\">\" /></div>";
            Assert.AreEqual("<div><img src=\"test.png\" alt=\">\"></div>",doc.Render());
        }

        [Test,TestMethod]
        public void UnwrapWithoutParent() {
            string s = "This is <b> a big</b> text";
            var f = CQ.Create(s);

            Assert.DoesNotThrow(() =>
            {
                f["b"].Unwrap().Render();
            });
        }

        [Test, TestMethod]
        public void DisconnectedBefore()
        {
            string s = "This is <b> a big</b> text";
            var dom = CQ.Create(s);
            var res = dom[dom.Document];
            var el = dom.Document.CreateElement("code");

            res =  res.Before(el);
            CollectionAssert.AreEqual(Objects.Enumerate(el,dom.Document),res.ToList());
        }

        /// <summary>
        /// Issue #48
        /// </summary>

        [Test, TestMethod]
        public void PossibleMemLeak()
        {

            //string url = "http://www.ebay.com/itm/SAMSUNG-GALAXY-S-II-GT-I9100-UNLOCKED-UNBRANDED-BLACK-2-9100-NEW-BOX-/280958752199?pt=Cell_Phones&hash=item416a7255c7";
            //var data = CQ.CreateFromUrlAsync(url).Then(success=>{
            //    File.WriteAllText("c:\\temp\\samsung.html",success.Html);
            //});

            var timer = DateTime.Now;
            var dom = TestDom("samsung-ebay.htm");

            Assert.AreEqual(1,dom["#TopPanelDF"].Length);
            Assert.AreEqual(1, dom["#JSDF"].Length);
            Assert.IsTrue(DateTime.Now - timer < TimeSpan.FromSeconds(1));

        }

        [Test, TestMethod]
        public void RountripEncoding()
        {

            string html = "<span>Test &nbsp; nbsp</span>";
            var dom = CQ.Create(html);
            var output = dom.Render(OutputFormatters.Create(HtmlEncoders.Minimum)).Replace(""+(char)160, "&nbsp;");
            Assert.AreEqual(html, output);

        }

        /// <summary>
        /// Issue #50.
        /// 
        /// Right now an empty class attribute will be removed. This is actually a bug (but shouldn't
        /// matter for most purposes), but for full compliance, an empty class attribute should be
        /// allowed and kept.
        /// </summary>

        [Test, TestMethod]
        public void ClassAndStyleAsBoolean()
        {

            string html = @"<span class="""" style="""">Test </span><div class style><br /></div>";
            var dom = CQ.Create(html);
            var output = dom.Render(OutputFormatters.Create(HtmlEncoders.Minimum)).Replace("" + (char)160, "&nbsp;");
            Assert.AreEqual(@"<span style>Test </span><div style><br></div>", output);

        }

        [Test, TestMethod]
        public void Utf8_HighValues()
        {

            string html = @"<span>&#55449;&#56580;</span>";
            var dom = CQ.Create(html);
            var output = dom.Render();

            Assert.AreEqual(@"<span>&#65533;&#65533;</span>", output);

        }

    }

}
