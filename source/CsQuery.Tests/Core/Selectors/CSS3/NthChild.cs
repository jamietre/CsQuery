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
        public void NthChild()
        {
            var res = Dom["body > :nth-child(2)"];
            Assert.AreEqual(jQuery("body").Children().Eq(1)[0], res[0], "nth-child(x) works");

            // odd & even are reversed for the jQuery pseudoselectors from nth-child version.

            var even = Dom["body > :nth-child(2n)"];
            Assert.AreEqual(jQuery("body").Children(":odd").Elements, even.Elements, "Simple math nth child workd");

            var odd = Dom["body > :nth-child(2n+1)"];
            Assert.AreEqual(jQuery("body").Children(":even").Elements, odd.Elements, "Simple math nth child workd");

            res = Dom["#hlinks-user > :nth-child(2(n+1))"];
            Assert.AreEqual(jQuery("#hlinks-user").Children(":odd").Not("#profile-triangle").Elements, res.Elements, "Simple math nth child workd");

            // odd & even parms

            res = Dom["body > :nth-child(even)"];
            Assert.AreEqual(res.Elements, even.Elements);

            res = Dom["body > :nth-child(odd)"];
            Assert.AreEqual(res.Elements, odd.Elements);
        }


    }

}