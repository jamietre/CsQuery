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
        public void LastOfType()
        {
            var res = Dom["#hlinks-user > span:last-of-type"];
            Assert.AreEqual(1, res.Length);
            Assert.IsTrue(res.Is(".lsep"));

            res = Dom["#hlinks-user span:last-of-type"];
            Assert.AreEqual(4, res.Length);

            // with no tag type, should return all children
            res = Dom["#hlinks-user :last-of-type"];
            Assert.AreEqual(7, res.Length);
        }

    }

}