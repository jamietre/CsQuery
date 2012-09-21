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

namespace CsQuery.Tests.Csharp.HtmlParser.HTML5
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
            Assert.AreEqual("<div><img src=\"test.png\" alt=\">\" /></div>",doc.Render());
        }
    }
}
