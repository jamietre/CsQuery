using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using CollectionAssert = NUnit.Framework.CollectionAssert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;

namespace CsQuery.Tests.Core.Selectors
{
    public partial class Css3 : PseudoSelector
    {
        [Test, TestMethod]
        public void Empty()
        {
            var res = Dom["span:empty"];
            Assert.AreEqual(2, res.Length);
            CollectionAssert.AreEquivalent(res, Dom[".badge2,.badge3"]);


            res = Dom["body :empty"];
            Assert.AreEqual(5, res.Length);

            res = Dom[":empty"];
            Assert.AreEqual(6, res.Length, "6 total element seleced (5 in body, + title & html nopde ");
        }

    }

}