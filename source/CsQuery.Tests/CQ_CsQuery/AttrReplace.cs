using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using CsQuery;


namespace CsQuery.Tests.CQ_CsQuery 
{
    [TestClass,TestFixture]
    public class AttrReplace : CsQueryTest 
    {

        [TestMethod, Test]
        public void Substring() 
        {
            var expected = CQ.Create("<p class=\"two\"></p>");
            var cq = CQ.Create("<p class=\"one\"></p>");

            Assert.AreEqual(expected, cq.AttrReplace("class", "one", "two"));
        }

        [TestMethod, Test]
        public void Regex() {
            var expected = CQ.Create("<p style=\"\"></p>");
            var cq = CQ.Create("<p style=\"font-size: 10px;\"></p>");

            Assert.AreEqual(expected, cq.AttrRegexReplace("style", @"font-size:\s+\d+px;?", ""));
        }

        [TestMethod, Test]
        public void Regex_Evaluated() {
            var expected = CQ.Create("<p style=\"font-size: 20px;\"></p>");
            var cq = CQ.Create("<p style=\"font-size: 10px;\"></p>");

            Assert.AreEqual(expected, cq.AttrRegexReplace("style", @"font-size:\s+(\d+)px;?", (m) => {
                return m.Captures[0].Value.Replace(m.Groups[1].Value, "20");
            }));
        }
    }
}
