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
using CsQuery.StringScanner;

namespace CsQuery.Tests.Core.Css
{
    [TestClass, TestFixture]
    public class Css : CsQueryTest
    {
        [SetUp]
        public override void FixtureSetUp()
        {
            base.FixtureSetUp();
            ResetQunit();
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

        [Test, TestMethod]
        public void RemoveStyle()
        {
            CQ dom = "<div style='height: 10; width: 10; display: none;'>";

            Assert.AreEqual("height: 10; width: 10; display: none;", dom["div"][0]["style"]);

            dom["div"].Css("width", null);
            Assert.AreEqual("height: 10; display: none;", dom["div"][0]["style"]);

            dom["div"].Css("width",20);
            Assert.AreEqual("height: 10; width: 20px; display: none;", dom["div"][0]["style"]);

            dom["div"].Css("width", "");
            Assert.AreEqual("height: 10; display: none;", dom["div"][0]["style"]);

            dom["div"].CssSet(new { display = null as object });
            Assert.AreEqual("height: 10;", dom["div"][0]["style"]);
        }

        [Test, TestMethod]
        public void SetHeightWithImportant() {
            CQ dom = "<div style='height: 10; width: 10; display: none;'>";

            Assert.AreEqual("height: 10; width: 10; display: none;", dom["div"][0]["style"]);

            dom["div"].Css("height", "10px !important");
            Assert.AreEqual("height: 10px !important; width: 10; display: none;", dom["div"][0]["style"]);
        }

        protected class StylesClass
        {
            public string width;
            public int height;
        }
    }


}
