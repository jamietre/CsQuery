using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute;
using MsTestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using TestContext = NUnit.Framework.TestContext;
using CsQuery;
using CsQuery.Utility;

namespace CsQuery.Tests.jsdom
{
    
    [TestClass,TestFixture]
    public class Attributes: CsQueryTest 
    {
        /// <summary>
        /// Some tests migrated from tmpvar/jsdom
        /// </summary>

        [Test,TestMethod]
        public void option_set_selected()
        {
            var window = CQ.Create();

            IHTMLSelectElement select = (IHTMLSelectElement)window.Document.CreateElement("select");

            var option0 = window.Document.CreateElement("option");
            select.AppendChild(option0);
            option0.SetAttribute("selected", "selected");

            var optgroup = window.Document.CreateElement("optgroup");
            select.AppendChild(optgroup);
            var option1 = window.Document.CreateElement("option");
            optgroup.AppendChild(option1);

            Assert.AreEqual(true, option0.Selected, "initially selected");
            Assert.AreEqual(false, option1.Selected, "initially not selected");
            Assert.AreEqual(option1, select.Options[1], "options should include options inside optgroup");

            //option1.DefaultSelected = true;
            option1.Selected = true;
            
            Assert.AreEqual(false, option0.Selected, "selecting other option should deselect this");
            //Assert.AreEqual(true, option0.defaultSelected, "default should not change");
            Assert.AreEqual(true, option1.Selected, "selected changes when defaultSelected changes");
            //Assert.AreEqual(true, option1.defaultSelected, "I just set this");

            //option0.defaultSelected = false;
            option0.Selected = true;
            Assert.AreEqual(true, option0.Selected, "I just set this");
            //Assert.AreEqual(false, option0.defaultSelected, "selected does not set default");
            Assert.AreEqual(false, option1.Selected, "should deselect others");
            //Assert.AreEqual(true, option1.defaultSelected, "unchanged");
          }

    }
}
