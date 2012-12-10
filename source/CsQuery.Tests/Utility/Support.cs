using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.HtmlParser;
using CsQuery.Utility;

namespace CsQuery.Tests.Utility
{

    /// <summary>
    /// Tests for the "Support" utility library
    /// </summary>
    [TestFixture, TestClass]
    public class Support_ : CsQueryTest
    {

        [Test, TestMethod]
        public void ToCamelCase()
        {
            Assert.AreEqual("one",Support.ToCamelCase("one"));
            Assert.AreEqual("One", Support.ToCamelCase("one",true));
            Assert.AreEqual("oneTwo", Support.ToCamelCase("one-two"));
            Assert.AreEqual("oneTwo-", Support.ToCamelCase("one-two-"));
            Assert.AreEqual("oneTwo--", Support.ToCamelCase("one-two--"));
            Assert.AreEqual("-OneTwo--", Support.ToCamelCase("-one-two--",true));

        }

        [Test, TestMethod]
        public void FromCamelCase()
        {
            Assert.AreEqual("one", Support.FromCamelCase("One"));
            Assert.AreEqual("one-two", Support.FromCamelCase("OneTwo"));
            Assert.AreEqual("-one-two", Support.FromCamelCase("-OneTwo"));
            Assert.AreEqual("one-two--", Support.FromCamelCase("OneTwo--"));

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


        [Test, TestMethod]
        public void IsImmutable()
        {
            int x = 1;
            string a = "hello";
            object test = null;
            object test2 = new List<string>();
            object exp = new ExpandoObject();
            KeyValuePair<string, string> kvp = new KeyValuePair<string, string>("somekey", "somevalue");

            Assert.IsTrue(Objects.IsImmutable(x), "integer is immutable type");
            Assert.IsTrue(Objects.IsImmutable(a), "string is immutable type");
            Assert.IsTrue(Objects.IsImmutable(test), "null is immutable type");
            Assert.IsTrue(!Objects.IsImmutable(test2), "List is not immutable type");
            Assert.IsTrue(!Objects.IsImmutable(kvp), "KVP is not immutable type");
            Assert.IsTrue(!Objects.IsExpando(kvp), "KVP is not expando");
            Assert.IsTrue(Objects.IsExpando(exp), "expando object is expando");

            Assert.IsTrue(!Objects.IsExtendableType(x), "int is not extendable");
            Assert.IsTrue(!Objects.IsExtendableType(a), "string is not extendable");
            Assert.IsTrue(!Objects.IsExtendableType(test), "null is not extendable");
            Assert.IsTrue(Objects.IsExtendableType(exp), "expando object is extendable");
            Assert.IsTrue(Objects.IsExtendableType(kvp), "kvp is extendable");

        }


        protected long TestUseFunc(string key, object value)
        {
            return System.Convert.ToInt64(value);
        }
        
    }
}