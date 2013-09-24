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
    public class HTMLOptionElement
    {
        [Test, TestMethod]
        public void Enabled()
        {
            var dom = CQ.CreateFragment(@"<select><option>a</option></select>");
            var e = dom["option"].FirstElement();

            Assert.IsFalse(e.Disabled);
        }

        [Test, TestMethod]
        public void Disabled()
        {
            var dom = CQ.CreateFragment(@"<select><option disabled>a</option></select>");
            var e = dom["option"].FirstElement();

            Assert.IsTrue(e.Disabled);
        }

        [Test, TestMethod]
        public void DisabledInDisabledOptGroup()
        {
            var dom = CQ.CreateFragment(@"<select><optgroup disabled><option>a</option></optgroup></select>");
            var e = dom["option"].FirstElement();

            Assert.IsTrue(e.Disabled);
        }

        [Test, TestMethod]
        public void EnabledInEnabledOptGroup()
        {
            var dom = CQ.CreateFragment(@"<select><optgroup><option>a</option></optgroup></select>");
            var e = dom["option"].FirstElement();

            Assert.IsFalse(e.Disabled);
        }
    }
}

