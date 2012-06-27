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
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.HtmlParser;
using CsQuery.Utility;

namespace CsQuery.Tests.Csharp.HtmlParser
{
    
    [TestFixture, TestClass]
    public class Styles_ : CsQueryTest
    {


        /// <summary>
        /// Test the Style
        /// </summary>
        [Test, TestMethod]
        public void Styles()
        {
            var styleDefs = DomStyles.StyleDefs;
            ICssStyle style = styleDefs["padding-left"];
            Assert.AreEqual(CssStyleType.Unit | CssStyleType.Option, style.Type, "Padding style is correct type");

            style = styleDefs["word-wrap"];
            HashSet<string> expectedOpts = new HashSet<string>(new string[] { "normal", "break-word" });
            Assert.AreEqual(expectedOpts, style.Options, "word-wrap has correct options");
        }


        /// <summary>
        /// Test the Style
        /// </summary>
        [Test, TestMethod]
        public void RenderStyles()
        {
            var dom = TestDom("TestHtml");
            
            var res = dom["#hidden-div"];
            Assert.AreEqual("none", res.FirstElement().Style["display"]);
            Assert.AreEqual("display: none", res.FirstElement().Style.ToString());

            res = dom["#hidden-div > div:first-child"];
            Assert.AreEqual("100", res.FirstElement().Style["width"]);
            Assert.AreEqual("200", res.FirstElement().Style["height"]);
            Assert.AreEqual("width: 100; height: 200;", res.FirstElement().Style.ToString());

            res[0].AddStyle("width: 125");
            
            //the 1st will be formatted; the 2nd will be as it was parsed.
            Assert.AreEqual("width: 125px; height: 200;", res.FirstElement().Style.ToString());

        }

    }
}