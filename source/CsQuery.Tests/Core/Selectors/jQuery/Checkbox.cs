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
    public partial class jQuery: PseudoSelector
    {
        [Test, TestMethod]
        public void Checkbox()
        {
            CQ res = Dom.Find("input:checkbox");
            Assert.AreEqual(2, res.Length, "Expected to find 2 checkbox elements");
        }


    }

}