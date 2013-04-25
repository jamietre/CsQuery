using System;
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

namespace CsQuery.Tests.Core.Attributes
{
    [TestClass,TestFixture]
    public class Attribute : CsQueryTest 
    {
     

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
            Assert.AreEqual("display: none;", div[0].Style.ToString());
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
            Assert.AreEqual("display: none;", div[0].Style.ToString());
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
            Assert.AreEqual(new KeyValuePair<string, string>("style", "color: #ff0000;"), attributes[1]);
            Assert.AreEqual(new KeyValuePair<string, string>("href", "/users/480527/jamietre"), attributes[2]);

            el = Dom["span.badgecount"].FirstOrDefault();

            Assert.AreEqual(2, el.Attributes.Length);
            Assert.AreEqual("badgecount", el.Attributes["class"]);
            Assert.AreEqual(null,el.Attributes["test"]);
        }

        /// <summary>
        /// Attributes can be: boolean and render just as 'checked';
        /// Have an empty string value and render as 'checked=""'
        /// Be missing (and not render) but return null from GetAttribute
        /// 
        /// When an attribute is set to "" or is set using "SetAttribute" it will always return "", but could
        /// render as just a property or as an empty string.
        /// 
        /// </summary>

        [Test, TestMethod]
        public void Boolean()
        {
            var el = CQ.CreateFragment("<div>")[0];
            el.SetAttribute("test");
            Assert.AreEqual("<div test></div>",el.Render());
            Assert.AreEqual("",el.GetAttribute("test"));

            el.SetAttribute("test","someValue");
            Assert.AreEqual("<div test=\"someValue\"></div>",el.Render());

            el.SetAttribute("test", "");
            Assert.AreEqual("<div test></div>", el.Render());
            
            el.SetAttribute("test", null);
            Assert.AreEqual("<div></div>", el.Render());
            Assert.AreEqual(null,el.GetAttribute("test"));

            el.SetAttribute("test");
            el.RemoveAttribute("test");
            Assert.AreEqual(null, el.GetAttribute("test"));

        }

        /// <summary>
        /// Since class and style are not tracked as attributes internally, make sure they work
        /// </summary>

        [Test, TestMethod]
        public void ClassStyleAttributes()
        {
            var dom = TestDom("TestHtml");

            var el = dom["[class=profile-link]"][0];

            Assert.IsTrue(el.HasAttribute("class"));
            Assert.IsTrue(el.HasAttribute("style"));
            Assert.AreEqual("profile-link",el["class"]);
            Assert.AreEqual("color: #ff0000;",el["style"]);

            Assert.IsTrue(dom["[class]"].Length > 0);
            Assert.IsTrue(dom["[style]"].Length > 0);


            var badge2 = dom[".badge2"][0];

            Assert.IsTrue(badge2.HasAttributes);
            Assert.IsTrue(badge2.HasClasses);
            Assert.IsFalse(badge2.HasStyles);
            Assert.AreEqual("badge2", badge2.Attributes["class"]);
            Assert.AreEqual("badge2", badge2["class"]);
            Assert.AreEqual("badge2", badge2.GetAttribute("class"));
            Assert.AreEqual(1,badge2.Classes.Count());
            Assert.AreEqual("badge2",badge2.Classes.First());

            var styleOnly = dom["#hidden-div > :first-child"][0];
            var style= "width: 100; height: 200;";

            Assert.IsTrue(styleOnly.HasAttributes);
            Assert.IsFalse(styleOnly.HasClasses);
            Assert.IsTrue(styleOnly.HasStyles);
            Assert.AreEqual(style, styleOnly.Attributes["style"]);
            Assert.AreEqual(style, styleOnly["style"]);
            Assert.AreEqual(style, styleOnly.GetAttribute("style"));
            Assert.AreEqual(2, styleOnly.Style.Count);

            // accessing Count causes the styles to be enumerated, and thus formatted
            style = "width: 100; height: 200;" ;
            Assert.AreEqual(style, styleOnly.Style.ToString());
        }

        [Test, TestMethod]
        public void CaseInsensitive()
        {
            var dom = CQ.Create(@"<input type='checkbox' checked='checked' name='stuff' />
        <input type='Checkbox' checked='Checked' name='Stuff' />");
            
            Assert.AreEqual(2,dom["input[type='checkbox']"].Length);
            Assert.AreEqual(1, dom["input[name='stuff']"].Length);
        }


        #region test configuration

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
        #endregion

        protected class StylesClass
        {
            public string width;
            public int height;
        }
    }
}
