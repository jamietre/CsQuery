using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;
using CsQuery.Utility.StringScanner;

namespace CsqueryTests.Csharp
{
    [TestClass, TestFixture, Category("Attributes")]
    public class Css : CsQueryTest
    {
        [SetUp]
        public override void FixtureSetUp()
        {
            ResetQunit();
        }
        protected void ResetQunit()
        {
            Dom = CQ.Create(Support.GetFile("csquery\\csquery.tests\\resources\\jquery-unit-index.htm"));
        }

        [Test, TestMethod]
        public void FromObjects()
        {
            var div = CQ.Create("<div></div>");
            div.CssSet(new
            {
                width="10px",
                height=20
            });
            Assert.AreEqual("10px", div.Css("width"));
            Assert.AreEqual("20px", div.Css("height"));
        }
        [Test, TestMethod]
        public void CamelCase()
        {
            var div = CQ.Create("<div></div>");
            div.CssSet(new
            {
                fontSize = "10px",
                overflowX = "scroll"
            });
            Assert.AreEqual("scroll", div.Css("overflow-x"));
            Assert.AreEqual("10px", div.Css("font-size"));
        }
        [Test, TestMethod]
        public void FromDynamic()
        {
            var div = CQ.Create("<div></div>");
            dynamic dict = new JsObject();
            dict.width = "10px";
            dict.height = 20;

            div.CssSet(dict);
            Assert.AreEqual("10px", div.Css("width"));
            Assert.AreEqual("20px", div.Css("height"));
        }
        [Test, TestMethod]
        public void FromDictionary()
        {
            var div = CQ.Create("<div></div>");

            var dict = new Dictionary<string, object>();
            dict["width"]= "10px";
            dict["height"]= 20;

            div.CssSet(dict);
            Assert.AreEqual("10px", div.Css("width"));
            Assert.AreEqual("20px", div.Css("height"));
        }

        [Test, TestMethod]
        public void FromPoco()
        {
            
            var div = CQ.Create("<div></div>");
            div.CssSet(new StylesClass
            {
                width = "10px",
                height = 20
            });
            Assert.AreEqual("10px", div.Css("width"));
            Assert.AreEqual("20px", div.Css("height"));
        }

        protected class StylesClass
        {
            public string width;
            public int height;
        }
    }


}
