using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Dynamic;
using CsQuery;
using CsQuery.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;

namespace CsqueryTests.Csharp
{

    [TestFixture, TestClass, Description("CsQuery Tests (Not from Jquery test suite)")]
    public class Objects_ : CsQueryTest
    {
        [TestFixtureSetUp,ClassInitialize]
        public static void Setup(TestContext context) {

        }

        [Test, TestMethod]
        public void Coerce()
        {
            DateTime curDate = DateTime.Now;

            Assert.AreEqual(Objects.Coerce(null), null);
            Assert.AreEqual(Objects.Coerce(true), true);
            Assert.AreEqual(Objects.Coerce(false), false);
            Assert.AreEqual(Objects.Coerce(1), (int)1);
            Assert.AreEqual(Objects.Coerce(1.2), (double)1.2);
            Assert.AreEqual(Objects.Coerce((decimal)1.23), (double)1.23);
            Assert.AreEqual(Objects.Coerce((float)1.23), (double)(float)1.23);
            Assert.AreEqual(Objects.Coerce((Single)1), (int)1);
            Assert.AreEqual(Objects.Coerce(curDate), curDate);

            Assert.AreEqual(Objects.Coerce("null"),null);
            Assert.AreEqual(Objects.Coerce("undefined"), null);
            Assert.AreEqual(Objects.Coerce("false"), false);
            Assert.AreEqual(Objects.Coerce("true"), true);
            Assert.AreEqual(Objects.Coerce("1"), (int)1);
            Assert.AreEqual(Objects.Coerce("3.14"), (double)3.14);
            Assert.AreEqual(Objects.Coerce("1"), (int)1);
            Assert.AreEqual(Objects.Coerce(curDate.ToString()).ToString(), curDate.ToString());
            Assert.AreEqual(Objects.Coerce("1/1/2010"), DateTime.Parse("1/1/2010"));

        }
        [Test,TestMethod]
        public void AttributeEncode()
        {
            string onlyQuotes = "{\"someprop\": 1, \"someprop2\", \"someval\"}";
            string onlyApos = "{'someprop': 1, 'someprop2', 'someval'}";
            string both = "{\"someprop\": 1, \"someprop2\", \"o'brien\"}";
            string neither= "plain old text";

            string quoteChar;

            string result = Objects.AttributeEncode(onlyQuotes,true,out quoteChar);
            Assert.AreEqual(onlyQuotes,result, "With only quotes, nothing changed");
            Assert.AreEqual("'", quoteChar, "Quote char was an apostrophe with only quotes");

            result = Objects.AttributeEncode(onlyApos, true,out quoteChar);
            Assert.AreEqual(onlyApos, result, "With only apostrophes, nothing changed");
            Assert.AreEqual("\"", quoteChar, "Quote char was a quote with only apos");

            result = Objects.AttributeEncode(both,true, out quoteChar);
            string expected = "{\"someprop\": 1, \"someprop2\", \"o&#39;brien\"}";
            Assert.AreEqual(expected, result, "With both, only apostrophes changed");
            Assert.AreEqual("'", quoteChar, "Quote char was an apos with both");

            result = Objects.AttributeEncode(neither, true, out quoteChar);
            Assert.AreEqual(neither, result, "With neither, nothing changeed");
            Assert.AreEqual("\"", quoteChar, "Quote char was a quote with both");


        }
        [Test, TestMethod]
        public void ParseJson()
        {
            string value = "null";
            Assert.AreEqual(null,CQ.ParseJSON(value));
            value="undefined";
            Assert.AreEqual(null,CQ.ParseJSON(value));
            value="\"test\"";
            Assert.AreEqual("test",CQ.ParseJSON(value));
            value="12";
            Assert.AreEqual((int)12,CQ.ParseJSON(value));
            value="12.2";
            Assert.AreEqual((double)12.2,CQ.ParseJSON(value));
            value="2";
            Assert.AreEqual(CsQuery.Engine.SelectorType.Tag,CQ.ParseJSON<CsQuery.Engine.SelectorType>(value));


        }
        protected long TestUseFunc(string key, object value)
        {
            return System.Convert.ToInt64(value);
        }
        
    }
}