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

    [TestFixture, TestClass]
    public partial class Css3 
    {
     

        [Test, TestMethod]
        public void NthLastChildOfType()
        {
            var res = Dom["span:nth-last-of-type(2)"];
            Assert.AreEqual(Dom[".badge2, [title='13 bronze badges'], .badge3"], res);

            // odd & even are reversed for the jQuery pseudoselectors from nth-child version.

            res = Dom["span:nth-last-of-type(2n)"];
            Assert.AreEqual(Dom[".profile-triangle, .badge2, [title='13 bronze badges'], .badge3"], res);

            res = Dom["span:nth-last-of-type(2n+1)"];
            Assert.AreEqual(8, res.Length); ;

        }

        [Test, TestMethod]
        public void NthLastChildOfType_Children()
        {

            var res = Dom["span > :nth-last-of-type(2)"];
            CollectionAssert.AreEqual(
                Dom[".profile-link, .badge2, [title='13 bronze badges'], .badge3, input[name=stuff]"], 
                res);

            // odd & even are reversed for the jQuery pseudoselectors from nth-child version.

            res = Dom["span > :nth-last-of-type(2n)"];
            CollectionAssert.AreEqual(
                Dom[".profile-triangle, .profile-link, .badge2, [title='13 bronze badges'], .badge3, input[name=stuff]"],
                res);

            res = Dom["span > :nth-last-of-type(2n+1)"];
            Assert.AreEqual(7, res.Length); ;

        }
    }

}