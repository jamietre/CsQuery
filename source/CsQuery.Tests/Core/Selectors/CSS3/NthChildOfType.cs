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
        public void NthChildOfType()
        {
            var res = Dom["body > span:nth-of-type(1)"];
            Assert.AreEqual(1, res.Length);
            Assert.AreEqual("hlinks-user", res[0].Id);

            res = Dom["body span:nth-of-type(2)"];
            Assert.AreEqual(3, res.Length);
            Assert.AreEqual("badgecount", res[1].ClassName);
            Assert.AreEqual("badgecount", res[2].ClassName);

            // odd & even are reversed for the jQuery pseudoselectors from nth-child version.

            var even = Dom["body span:nth-of-type(2n)"];
            Assert.AreEqual(4, even.Length);
            Assert.AreEqual("lsep", even[3].ClassName);

        }

        [Test, TestMethod]
        public void NthChildOfType_Children()
        {
            var res = Dom["body > span > :nth-of-type(1)"];

            var expected = Dom[".profile-triangle, .profile-link, input[name=stuff], #textarea"];

            CollectionAssert.AreEqual(
                expected, 
                res);

        }
    }

}