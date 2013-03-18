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
    public class DomContainer
    {
        [Test, TestMethod]
        public void ChildNodeIndexer()
        {
            CQ dom = "<div>Test</div>";

            Assert.AreEqual("DIV", dom.Document[0].NodeName);

            Assert.AreEqual("Test", dom.Document[0][0].NodeValue);
        }
    }
}

