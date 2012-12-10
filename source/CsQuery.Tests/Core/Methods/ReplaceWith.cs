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
        /// <summary>
        /// Replace with: ensure that when content is moved between domains, a CsQuery object that was
        /// the replacement content remains connected to the content in its new home.
        /// </summary>

        [Test, TestMethod]
        public void ReplaceWith()
        {

            var dom = TestDom("TestHtml");

            var target = dom["#test-show-last"];
            var source = CQ.Create("<div id='new-content'><span /></div>");
            
            Assert.AreEqual(2, target.Children().Length);
            target.ReplaceWith(source);

            Assert.AreEqual(1, source["#hlinks-user"].Length);
            Assert.AreEqual(dom["#new-content"][0], source[0]);
            Assert.IsTrue(target[0].IsDisconnected);
        }

    
   }
}