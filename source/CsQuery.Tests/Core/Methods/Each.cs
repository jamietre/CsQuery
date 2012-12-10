using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;

namespace CsQuery.Tests.Core
{
    public partial class Methods: CsQueryTest
    {

        [Test, TestMethod]
        public void EachUntil()
        {

            var dom = TestDom("TestHtml");

            dom["span"].EachUntil(CheckSpan);

            Assert.AreEqual("13 bronze badges", dom["[special]:last"][0]["title"]);
            Assert.AreEqual(7, dom["[special]"].Length);

        }


        [Test, TestMethod]
        public void Each()
        {
            var dom = TestDom("TestHtml");
            
            dom["div"].Each(CheckSpanAction);
            Assert.AreEqual(dom["div"].Length, dom["[special]"].Length);
        }

        private void CheckSpanAction(int index, IDomObject el)
        {
            el["special"] = "added";
        }
        private bool CheckSpan(int index, IDomObject el)
        {
            el["special"] = "added";
            return el["title"] != "13 bronze badges";
        }
    }
}