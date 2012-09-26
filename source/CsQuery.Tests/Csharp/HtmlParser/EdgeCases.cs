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

namespace CsQuery.Tests.Csharp.HtmlParser
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
    }

}
