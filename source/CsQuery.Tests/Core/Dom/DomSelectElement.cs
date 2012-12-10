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
using CsQuery.ExtensionMethods;

namespace CsQuery.Tests.Core.Dom
{
    [TestFixture, TestClass]
    public class DomSelectElement:CsQueryTest
    {
        /// <summary>
        /// Issue #27: implementaton of option properties
        /// </summary>
        [Test, TestMethod]
        public void SelectedIndex()
        {
            var dom = TestDom("jquery-unit-index");
            var res = dom["#select4"];
            var el = res[0] as IHTMLSelectElement;

            res.RemoveAttr("multiple");

            // In HTML5 browser this would be 1. However, there would be no way to represent that state
            // (e.g the effect of removing the multiple attribute) -- if the DOM were rendered this way
            // initially, the LAST selected item would be the one chosen. 
            
            Assert.AreEqual(3,el.SelectedIndex);
            
            res.Find("#option4b").RemoveAttr("selected");
            res.Find("#option4c").RemoveAttr("selected");
            res.Find("#option4d").RemoveAttr("selected");
            Assert.AreEqual(0, res.Find(":selected").Length);
            Assert.AreEqual(4,el.SelectedIndex);

            res.Find("optgroup[disabled]").RemoveAttr("disabled");
            Assert.AreEqual(0,el.SelectedIndex);

            res.Prop("multiple",true);
            Assert.AreEqual(-1, el.SelectedIndex);

        }

        /// <summary>
        /// 
        /// </summary>
        //[Test, TestMethod]
        //public void SelectedOption()
        //{
        //    var dom = TestDom("jquery-unit-index");
        //    var res = dom["#select4"];
        //    var el = res[0] as IHTMLSelectElement;

        //    res.RemoveAttr("multiple");
        //    res.Find(":selected").ForEach(item =>
        //    {
        //        item.Selected = false;
        //    });

        //    var newSelected = res.Find(":selected");
        //    Assert.AreEqual(1, newSelected.Length);
        //    Assert.AreEqual("#option4e", newSelected[0].Id);

        //    res.Find("optgroup[disabled]").RemoveAttr("disabled");
        //    Assert.AreEqual("#option4a", res.Find(":selected")[0].Id);

        //}
    }
}

