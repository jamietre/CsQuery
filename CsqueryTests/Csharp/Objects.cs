using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Dynamic;
using Jtc.CsQuery;
using Jtc.CsQuery.Utility;
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
        private static IDictionary<string, object> testDict = null;

        [TestFixtureSetUp,ClassInitialize]
        public static void Setup(TestContext context) {

        }
        
        [Test,TestMethod]
        public void AttributeEncode()
        {
            string onlyQuotes = "{\"someprop\": 1, \"someprop2\", \"someval\"}";
            string onlyApos = "{'someprop': 1, 'someprop2', 'someval'}";
            string both = "{\"someprop\": 1, \"someprop2\", \"o'brien\"}";
            string neither= "plain old text";

            char quoteChar;
            string result = Objects.AttributeEncode(onlyQuotes, out quoteChar);
            Assert.AreEqual(onlyQuotes,result, "With only quotes, nothing changed");
            Assert.AreEqual('\'', quoteChar, "Quote char was an apostrophe with only quotes");

            result = Objects.AttributeEncode(onlyApos, out quoteChar);
            Assert.AreEqual(onlyApos, result, "With only apostrophes, nothing changed");
            Assert.AreEqual('"', quoteChar, "Quote char was a quote with only apos");

            result = Objects.AttributeEncode(both, out quoteChar);
            string expected = "{\"someprop\": 1, \"someprop2\", \"o&#39;brien\"}";
            Assert.AreEqual(expected, result, "With both, only apostrophes changed");
            Assert.AreEqual('\'', quoteChar, "Quote char was an apos with both");

            result = Objects.AttributeEncode(neither, out quoteChar);
            Assert.AreEqual(neither, result, "With neither, nothing changeed");
            Assert.AreEqual('"', quoteChar, "Quote char was a quote with both");


        }
        protected long TestUseFunc(string key, object value)
        {
            return System.Convert.ToInt64(value);
        }
        
    }
}