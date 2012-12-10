using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;

namespace CsQuery.Tests.Core
{

    [TestFixture, TestClass]
    public partial class Methods : CsQueryTest
    {

        [Test,TestMethod]
        public void HasAttr()
        {
            var test = CQ.CreateFragment("<div id='wrapper' class='someclass' style='width: 100px;' data-test='mydata'></div>");

            Assert.IsTrue(test.HasAttr("id"));
            Assert.IsTrue(test.HasAttr("class"));
            Assert.IsTrue(test.HasAttr("style"));
            Assert.IsTrue(test.HasAttr("data-test"));
            Assert.IsFalse(test.HasAttr("missing"));
            Assert.IsFalse(test.HasAttr(null));

            test = CQ.CreateFragment("<div></div>");
            Assert.IsFalse(test.HasAttr("id"));
            Assert.IsFalse(test.HasAttr("class"));
            Assert.IsFalse(test.HasAttr("style"));
            
        }
        
    }
}