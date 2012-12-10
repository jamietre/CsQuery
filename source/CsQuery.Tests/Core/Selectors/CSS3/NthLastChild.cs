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
        public void NthLastChild()
        {
            var res = Dom["body > :nth-last-child(2)"];
            Assert.AreEqual(Dom["#hlinks-user"], res);

            // odd & even are reversed for the jQuery pseudoselectors from nth-child version.

            res = Dom["body > :nth-last-child(2n)"];
            Assert.AreEqual(Dom["#first-p, #hlinks-user"], res);

            res = Dom["body > :nth-last-child(2n+1)"];
            Assert.AreEqual(Dom["hr:first, #test-show"], res);

        }


    }

}