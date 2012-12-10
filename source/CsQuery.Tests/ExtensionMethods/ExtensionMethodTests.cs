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

namespace CsQuery.Tests.ExtensionMethods
{

    [TestFixture, TestClass]
    public class External
    {

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