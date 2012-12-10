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
    /// <summary>
    /// This method is largely covered by the jQuery tests
    /// </summary>

    public partial class Methods: CsQueryTest
    {

        [Test, TestMethod]
        public void First()
        {
            
            var dom = TestDom("TestHtml");

            Assert.AreEqual(1, dom["span"].First().Length);
            Assert.AreEqual("hlinks-user", dom["span"].First().Attr("id"));

            Assert.AreEqual(0, dom["not-here"].First().Length);
        }


    
   }
}