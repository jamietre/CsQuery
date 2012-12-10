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
    public class HTMLSelectElement
    {
        [Test, TestMethod]
        public void SingleSelect()
        {
            var dom = CQ.CreateFragment(@"<select><option value='1'><option>second<option value='3'>third</option></select>");

            var el = (IHTMLSelectElement)dom["select"][0];

            // default is 1st item selected
            Assert.AreEqual("1",el.Value);
            
            dom["option:eq(1)"].Prop("selected", true);
            Assert.AreEqual("second",el.Value);

            // should change to the last selected option
            
            dom["option:last-child"].Prop("selected", true);
            Assert.AreEqual("3", el.Value);

            // nothing should happen here
            dom["option:eq(2)"].Prop("selected", true);
            Assert.AreEqual("3", el.Value);

            // ... but this should result in only
            dom["option:eq(1)"][0].Selected = true;
            Assert.AreEqual("second", el.Value);

            el.SelectedIndex = 0;
            Assert.AreEqual("1", el.Value);

            // In a real browsers, you could set a non-multiple select to be nothing, but there's no way to
            // represent that state in HTML so ours still falls back on the first option. 

            el.SelectedIndex = -1;
            Assert.AreEqual("1", el.Value);
        }
    }
}

