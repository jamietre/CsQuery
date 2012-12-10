using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using CollectionAssert = NUnit.Framework.CollectionAssert;
using StringAssert = NUnit.Framework.StringAssert;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.StringScanner;

namespace CsQuery.Tests.Utility
{
    [TestClass,TestFixture]
    public class FastActivator_
    {

        [Test,TestMethod]
        public void CreateInstance_Object()
        {
            var test = FastActivator.CreateInstance<TestClass>();

            Assert.AreEqual(1, test.integer);

        }

        [TestMethod,Test]
        public void CreateInstance_Type() {
            var integer = FastActivator.CreateInstance(typeof(int));
            Assert.AreEqual(integer, 0);

        }


        class TestClass
        {
            public TestClass()
            {
                integer = 1;
            }
            public int integer;

        }
      
    }
}



