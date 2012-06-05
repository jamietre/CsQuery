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

namespace CsqueryTests.Csharp
{
    
    [TestFixture, TestClass,Description("Styles objects")]
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
    }
}