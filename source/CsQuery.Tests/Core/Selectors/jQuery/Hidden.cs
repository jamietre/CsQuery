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
    public partial class jQuery: PseudoSelector
    {
        [Test, TestMethod]
        public void Hidden()
        {
            var dom = VisibilityTestDom();

            var res = dom.Select("div:hidden");
            Assert.AreEqual(dom.Select("#outer,#outer3,#outer4,#outer6"), res, "Correct divs are visible");

            res = dom.Select("span:hidden");
            Assert.AreEqual(dom.Select("#inner, #inner3, #inner4"), res, "Correct spans are visible");
        }



    }

}