using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;

namespace CsQuery.Tests.Core.Dom
{
    [TestFixture, TestClass]
    public class InnerText: CsQueryTest
    {

        string testHtml = @"<div><span><!-- a comment --></span>This text has <a href>a link</a></div><div>a new block</div>";

        [Test, TestMethod]
        public void InnerTextGet()
        {
            

            var doc = TestDom("TestHtml");

            // Note: this is what Chrome actually produces. The specific logic that decides how many spaces & line feeds to add seems
            // a bit obtuse and not part of any spec anyway so we can't exactly match it yet.
            
//            var expected = @"Hello there
//
//▾ jamietre  3,215 2 13     |  
//Nested non-hidden span
//Nested non=hidden header";

            var expected = @"Hello there
▾ jamietre 3,215 2 13 |
Nested hidden span
Nested hidden header
Nested non-hidden span
Nested non=hidden header
";

            Assert.AreEqual(expected, doc["body"][0].InnerText);

        }

        [Test, TestMethod]
        public void InnerTextGet2()
        {


            CQ doc = CQ.CreateDocument(testHtml);

            var text = doc["body"][0].InnerText;

            Assert.AreEqual("This text has a link" + Environment.NewLine + "a new block", text);


        }
        [Test, TestMethod]
        public void InnerTextSet()
        {
            var doc = TestDom("TestHtml");

            var res = doc["#reputation_link"][0];
            res.InnerText = "change the text";
            Assert.AreEqual(1,res.ChildNodes.Length);
            Assert.AreEqual("change the text", res.InnerText);

        }

    }
}

