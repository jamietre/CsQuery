using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery.EquationParser;
using CsQuery.EquationParser.Implementation;
using CsQuery.EquationParser.Implementation.Functions;

namespace CsQuery.Tests.Core.Dom
{
    [TestFixture, TestClass]
    public class DomElement
    {
        /// <summary>
        /// Issue #15
        /// </summary>
        [Test, TestMethod]
        public void Type()
        {
            var dom = CQ.CreateFragment(@"<input name='domContent' value='' />");

            // text is default
            Assert.AreEqual("text", dom[0].Type);
            Assert.IsFalse(dom.Is(":hidden"));
            
            // just try out no closing tag, for fun.

            var newDom = dom["<div>"].Css("display", "none").Append(dom);

            Assert.IsTrue(newDom.Is(":hidden"));

            dom = CQ.CreateFragment(@"<input name='domContent' type='HIDDEN' value='' />");
            Assert.AreEqual("hidden", dom[0].Type);
            Assert.IsTrue(dom.Is(":hidden"));
        }
    }
}

