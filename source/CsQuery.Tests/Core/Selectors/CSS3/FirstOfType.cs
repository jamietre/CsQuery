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


        /// <summary>
        /// First of Type selector returns elements which are the first child matching the type of the specific element type.
        /// Differs from first-child in that elements that aren't the same type are not consisdered when evaluating "first"
        /// </summary>
        [Test, TestMethod]
        public void FirstOfType()
        {
            var res = Dom["#hlinks-user a:first-of-type"];
            Assert.AreEqual(1, res.Length);
            Assert.IsTrue(res.Is(".profile-link"));

            res = Dom["#hlinks-user a:first-child"];
            Assert.AreEqual(0, res.Length);

            // with no tag type, should return anything that is the first of its type
            res = Dom["#hlinks-user :first-of-type"];
            Assert.AreEqual(7, res.Length);
            Assert.AreEqual("profile-triangle", res[0].ClassName);
            Assert.AreEqual("profile-link", res[1].ClassName);
            Assert.AreEqual("reputation-score", res[2].ClassName);
            Assert.AreEqual("badge2", res[3].ClassName);
            Assert.AreEqual("badge3", res[4].ClassName);
            Assert.AreEqual("INPUT", res[5].NodeName);
            Assert.AreEqual("TEXTAREA", res[6].NodeName);
        }
    }

}