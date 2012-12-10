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
        public void OnlyOfType()
        {
            var res = Dom["span:only-of-type"];
            Assert.AreEqual(4, res.Length);
            CollectionAssert.AreEquivalent(res, Dom["#hlinks-user,.reputation-score,#hidden-span,#non-hidden-span"]);


            res = Dom["body :only-of-type"];
            Assert.AreEqual(11, res.Length);

            res = Dom["a:only-of-type"];
            Assert.AreEqual(0, res.Length);

        }

    }

}