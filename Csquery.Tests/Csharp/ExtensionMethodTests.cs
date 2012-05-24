using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using NUnit.Framework;
using CsQuery;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;

namespace CsqueryTests.ExtensionMethodTests
{

    [TestFixture, TestClass, Description("CsQuery Core Tests ")]
    public class External
    {

        [Test,TestMethod]
        public void IsImmutable()
        {
            int x = 1;
            string a = "hello";
            object test = null;
            object test2 = new List<string>();
            object exp = new ExpandoObject();
            KeyValuePair<string, string> kvp = new KeyValuePair<string, string>("somekey", "somevalue");

            Assert.IsTrue(x.IsImmutable(), "integer is immutable type");
            Assert.IsTrue(a.IsImmutable(), "string is immutable type");
            Assert.IsTrue(test.IsImmutable(), "null is immutable type");
            Assert.IsTrue(!test2.IsImmutable(), "List is not immutable type");
            Assert.IsTrue(!kvp.IsImmutable(), "KVP is not immutable type");
            Assert.IsTrue(!Objects.IsExpando(kvp), "KVP is not expando");
            Assert.IsTrue(Objects.IsExpando(exp), "expando object is expando");

            Assert.IsTrue(!x.IsExtendableType(), "int is not extendable");
            Assert.IsTrue(!a.IsExtendableType(), "string is not extendable");
            Assert.IsTrue(!test.IsExtendableType(), "null is not extendable");
            Assert.IsTrue(exp.IsExtendableType(), "expando object is extendable");
            Assert.IsTrue(kvp.IsExtendableType(), "kvp is extendable");

        }
        [Test, TestMethod]
        public void Seek()
        {
            string testString = "The lazy quick brown fox jumps over the lazy dogs";
            char[] testArr = testString.ToCharArray();

            int pos = testArr.Seek("fox", 0);
            Assert.AreEqual(testString.IndexOf("fox"), pos, "Seek from zero works");

            pos = testArr.Seek("lazy", 10);
            Assert.AreEqual(testString.IndexOf("lazy", 10), pos, "Seek from index works");

            pos = testArr.Seek("lazy", 4);
            Assert.AreEqual(4, pos, "Seek from starting position works");

            pos = testArr.Seek("blah");
            Assert.AreEqual(-1, pos, "Seek fail works");

        }
      
    }
}