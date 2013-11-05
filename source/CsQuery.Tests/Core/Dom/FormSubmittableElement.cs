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
    public class FormSubmittableElement : CsQueryTest
    {
        [Test, TestMethod]
        public void Enabled()
        {
            CQ dom = "<input>";
            var e = dom["input"].FirstElement();

            Assert.IsFalse(e.Disabled);
        }

        [Test, TestMethod]
        public void Disabled()
        {
            CQ dom = "<input disabled>";
            var e = dom["input"].FirstElement();

            Assert.IsTrue(e.Disabled);
        }

        [Test, TestMethod]
        public void Enabled_InEnabledFieldset()
        {
            CQ dom = "<fieldset><input></fieldset>";
            var e = dom["input"].FirstElement();

            Assert.IsFalse(e.Disabled);
        }

        [Test, TestMethod]
        public void Disabled_InDisabledFieldset()
        {
            CQ dom = "<fieldset disabled><input></fieldset>";
            var e = dom["input"].FirstElement();

            Assert.IsTrue(e.Disabled);
        }

        [Test, TestMethod]
        public void Enabled_InEnabledFieldset_FirstLegend()
        {
            CQ dom = "<fieldset><legend><input></legend></fieldset>";
            var e = dom["input"].FirstElement();

            Assert.IsFalse(e.Disabled);
        }

        [Test, TestMethod]
        public void Enabled_InDisabledFieldset_FirstLegend()
        {
            CQ dom = "<fieldset disabled><legend><input></legend></fieldset>";
            var e = dom["input"].FirstElement();

            Assert.IsFalse(e.Disabled);
        }

        [Test, TestMethod]
        public void Enabled_InEnabledFieldset_SecondLegend()
        {
            CQ dom = "<fieldset><legend></legend><legend><input></legend></fieldset>";
            var e = dom["input"].FirstElement();

            Assert.IsFalse(e.Disabled);
        }

        [Test, TestMethod]
        public void Disabled_InDisabledFieldset_SecondLegend()
        {
            CQ dom = "<fieldset disabled><legend></legend><legend><input></legend></fieldset>";
            var e = dom["input"].FirstElement();

            Assert.IsTrue(e.Disabled);
        }
    }

}
