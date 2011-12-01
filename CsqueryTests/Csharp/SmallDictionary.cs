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
    public class SmallDictionary_ : CsQueryTest
    {
     

        [TestFixtureSetUp,ClassInitialize]
        public static void Setup(TestContext context) {

        }
        [Test,TestMethod]
        public void SmallDictionary()
        {
            IDictionary<string, int> dict = new SmallDictionary<string, int>();

            dict.Add("test1", 10);
            dict.Add("test2", 20);
            dict.Add("test3",30);

            Assert.AreEqual(3, dict.Count, "Correct # of elements");
            Assert.IsTrue(dict.ContainsKey("test2"), "Contains");
            Assert.IsFalse(dict.ContainsKey("xxx"), "!Contains");

            Assert.AreEqual(20, dict["test2"], "Got a value");
            dict["test2"] = 45;
            Assert.AreEqual(45, dict["test2"], "Updated a value");
            Assert.AreEqual(3, dict.Count, "Correct # of elements still");

            dict.Remove("test2");
            Assert.AreEqual(2, dict.Count, "Correct # of elements still");
            Assert.IsFalse(dict.ContainsKey("test2"), "Contains");
        }

        
    }
}