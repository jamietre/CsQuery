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

namespace CsQuery.Tests.Csharp
{
    /// <summary>
    /// This method is largely covered by the jQuery tests
    /// </summary>

    public partial class Methods: CsQueryTest
    {

        [Test, TestMethod]
        public void Before_MissingTarget()
        {
            
            var dom = TestDom("TestHtml");
            var len = dom["*"].Length;

            var target = dom["does-not-exist"];
            var content = dom["<div id='content' />"];

            target.Before(content);

            // nothing was added
            Assert.AreEqual(dom["*"].Length, len);
        }

    
   }
}