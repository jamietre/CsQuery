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
    /// <summary>
    /// Tests to ensure that overridden property handling works for certain elements
    /// </summary>

    [TestFixture, TestClass]
    public class DomText: CsQueryTest
    {
        [Test, TestMethod]
        public void TextNullValue()
        {
            var textNode = Objects.CreateTextNode("");
            Assert.AreEqual("",textNode.NodeValue);
            textNode.NodeValue = null;
            Assert.AreEqual("", textNode.NodeValue);

        }

       
    }

}
 