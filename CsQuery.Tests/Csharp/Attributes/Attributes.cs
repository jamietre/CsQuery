﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using CollectionAssert = NUnit.Framework.CollectionAssert;
using StringAssert = NUnit.Framework.StringAssert;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;

namespace CsQuery.Tests.Csharp.Attributes
{
    [TestClass,TestFixture]
    public class Attribute : CsQueryTest 
    {
        Func<object,object> bareObj = (input) => {return input; };

        [TestMethod,Test]
        public void Show()
        {
            // Show method should figure out if an ancestor has "display" and either remove the display
            // property, or set it to "inline" or "block" depending on element type

            Assert.AreEqual("none", jQuery("#hidden-div").Css("display"), "Container is hidden");

            jQuery("#hidden-span").Show();
            Assert.AreEqual("inline", jQuery("#hidden-span").Css("display"), "Span has default display attribute after Show");

            jQuery("#hidden-span").Hide();
            Assert.AreEqual("none", jQuery("#hidden-span").Css("display"), "Span has display=none attribute after Show");

            jQuery("#hidden-span").Toggle();
            Assert.AreEqual("inline", jQuery("#hidden-span").Css("display"), "Span has default display attribute after Toggle");

            jQuery("#hidden-span").Toggle();
            Assert.AreEqual("none", jQuery("#hidden-span").Css("display"), "Span has display=none attribute after 2nd Toggle");

            jQuery("#hidden-span").Toggle(true);
            Assert.AreEqual("inline", jQuery("#hidden-span").Css("display"), "Span has default display attribute after Toggle(true)");

            jQuery("#hidden-span").Toggle(false);
            Assert.AreEqual("none", jQuery("#hidden-span").Css("display"), "Span has display=none attribute after Toggle(false)");
        }

        [Test, TestMethod]
        public void FromObjects()
        {
            var div = CQ.Create("<div></div>");
            div.AttrSet(new
            {
                width = "10px",
                style="display: none;"
            });
            Assert.AreEqual("10px", div[0]["width"]);
            Assert.AreEqual("display: none", div[0].Style.ToString());
        }

        [Test, TestMethod]
        public void QuickSetter()
        {
            var div = CQ.Create("<div></div>");
            div.AttrSet(new
            {
                css= new {
                    width="10px",
                    overflowX="scroll"
                },
                height= "10",
                id="test"
            },true);
            Assert.AreEqual("10px", div.Css("width"));
            Assert.AreEqual("scroll", div.Css("overflow-x"));
            Assert.AreEqual("10px", div.Css("height"));
            Assert.AreEqual("test", div.Elements.First().Id);
        }

        [Test, TestMethod]
        public void JSONQuickSet()
        {
            var div = CQ.Create("<div></div>");
            div.AttrSet(@"{ css: {
                    width: '10px',
                    'overflow-x': 'scroll'
                },
                height: '10',
                id: ""test""
            }",true);
            Assert.AreEqual("10px", div.Css("width"));
            Assert.AreEqual("scroll", div.Css("overflow-x"));
            Assert.AreEqual("10px", div.Css("height"));
            Assert.AreEqual("test", div.Elements.First().Id);
        }
        [Test, TestMethod]
        public void FromDynamic()
        {
            var div = CQ.Create("<div></div>");
            dynamic dict = new JsObject();
            dict.width = "10px";
            dict.style = "display: none;";

            div.AttrSet(dict);
            Assert.AreEqual("10px", div[0]["width"]);
            Assert.AreEqual("display: none", div[0].Style.ToString());
        }
        [Test, TestMethod]
        public void FromDictionary()
        {
            var div = CQ.Create("<div></div>");

            var dict = new Dictionary<string, object>();
            dict["width"] = "10px";
            dict["height"] = 20;

            div.AttrSet(dict);
            Assert.AreEqual("10px", div[0]["width"]);
            Assert.AreEqual("20", div[0]["height"]);
        }

        [Test, TestMethod]
        public void FromPoco()
        {

            var div = CQ.Create("<div></div>");
            div.AttrSet(new StylesClass
            {
                width = "10px",
                height = 20
            });
            Assert.AreEqual("10px", div[0]["width"]);
            Assert.AreEqual("20", div[0]["height"]);
        }

        /// <summary>
        /// Make sure that the IAttributesInterface works, and that class & style are enumerated properly
        /// </summary>
        [Test, TestMethod]
        public void AttributesInterface()
        {
            var el = Dom["a:first"].Single();

            Assert.AreEqual(3, el.Attributes.Length);
            Assert.AreEqual("profile-link", el.Attributes["class"]);
            Assert.AreEqual("profile-link", el["class"]);

            IList<KeyValuePair<string, string>> attributes = new List<KeyValuePair<string, string>>(el.Attributes);
            Assert.AreEqual(new KeyValuePair<string, string>("class", "profile-link"), attributes[0]);
            Assert.AreEqual(new KeyValuePair<string, string>("style", "color: #ff0000"), attributes[1]);
            Assert.AreEqual(new KeyValuePair<string, string>("href", "/users/480527/jamietre"), attributes[2]);

            el = Dom["span.badgecount"].FirstOrDefault();

            Assert.AreEqual(2, el.Attributes.Length);
            Assert.AreEqual("badgecount", el.Attributes["class"]);
            Assert.AreEqual(null,el.Attributes["test"]);
        }


        [SetUp]
        public override void FixtureSetUp()
        {
            base.FixtureSetUp();
            ResetDom();
        }
        protected void ResetDom()
        {
            Dom = TestDom("TestHtml");
        }


        protected class StylesClass
        {
            public string width;
            public int height;
        }
    }
}
