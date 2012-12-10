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
        public void Visible()
        {
            var dom = VisibilityTestDom();

            var res = dom.Select("div:visible");
            Assert.AreEqual(dom.Select("#wrapper, #outer2, #outer5"), res, "Correct divs are visible");

            res = dom.Select("span:visible");
            Assert.AreEqual(dom.Select("#inner2,#inner5"), res, "Correct spans are visible");
        }

        /// <summary>
        /// Issue#11
        /// </summary>
        [Test, TestMethod]
        public void Visible_InputTypeHidden()
        {
            var dom = VisibilityTestDom();

            var res = dom["input[type=hidden]"];
            Assert.IsTrue(dom.Is(":visible"));

            Assert.AreEqual(1, dom["input:hidden"].Length);
            Assert.AreEqual(0, dom["input:visible"].Length);
        }
    }

}