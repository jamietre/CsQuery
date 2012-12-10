using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using CollectionAssert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;

namespace CsQuery.Tests.Core
{

    public partial class Methods
    {

        CQ testDom = CQ.Create("<el1></el1><el2></el2><el3></el3><el4></el4><el5></el5>");
        [Test, TestMethod]
        public void Slice()
        {


            Assert.AreEqual(5, testDom.Length);
            Assert.AreEqual(testDom, testDom.Slice(0));

            var allButFirst = testDom.Slice(1);
            CollectionAssert.AreEqual(testDom.Skip(1).ToList(), testDom.Slice(1));
            CollectionAssert.AreEqual(testDom.Skip(1).Take(3), testDom.Slice(1, 4));
         
        }


        [Test, TestMethod]
        public void SliceNegative()
        {

            Assert.AreEqual(testDom,testDom.Slice(-5));
            CollectionAssert.AreEqual(testDom.Last(), testDom.Slice(-1));

            Assert.AreEqual(testDom.Skip(1).ToList(), testDom.Slice(-4));
            Assert.AreEqual(testDom.Skip(1).Take(3), testDom.Slice(-4, -1));




        }

    }
}