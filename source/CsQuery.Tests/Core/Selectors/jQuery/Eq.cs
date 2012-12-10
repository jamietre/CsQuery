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
    public partial class jQuery : PseudoSelector
    {

        /// <summary>
        /// Issue #19: error when no children
        /// </summary>

        [Test, TestMethod]
        public void Eq()
        {
            var res = CQ.CreateFragment(@"<div class='header'>
                Billing Address
                </div>");

            var k = res.Find("> label:visible:eq(0)");

            Assert.AreEqual(0, k.Length);
        }


    }

}