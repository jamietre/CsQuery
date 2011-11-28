using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Jtc.CsQuery;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using CollectionAssert = NUnit.Framework.CollectionAssert;
using StringAssert = NUnit.Framework.StringAssert;
//using Description = Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;

namespace CsqueryTests.Csharp
{
    [TestClass,TestFixture, Category("Attributes")]
    public class Attribute : CsQueryTest 
    {
        Func<object,object> bareObj = (input) => {return input; };

        [SetUp]
        public override void FixtureSetUp()
        {
            ResetDom();
        }
        protected void ResetDom()
        {
            Dom = CsQuery.Create(Support.GetFile("csquerytests\\resources\\TestHtml.htm"));
        }
        [TestMethod,Test]
        public void Show()
        {
            // Show method should figure out if an ancestor has "display" and either remove the display
            // property, or set it to "inline" or "block" depending on element type

            Assert.AreEqual("none", jQuery("#hidden-div").Css("display"), "Container is hidden");
            jQuery("#hidden-span").Show();
            Assert.AreEqual(null, jQuery("#hidden-span").Css("display"), "Span has no display attribute after Show");
            jQuery("#hidden-span").Hide();
            Assert.AreEqual("none", jQuery("#hidden-span").Css("display"), "Span has display=none attribute after Show");
            jQuery("#hidden-span").Toggle();
            Assert.AreEqual(null, jQuery("#hidden-span").Css("display"), "Span has no display attribute after Toggle");
            jQuery("#hidden-span").Toggle();
            Assert.AreEqual("none", jQuery("#hidden-span").Css("display"), "Span has display=none attribute after 2nd Toggle");
            jQuery("#hidden-span").Toggle(true);
            Assert.AreEqual(null, jQuery("#hidden-span").Css("display"), "Span has no display attribute after Toggle(true)");
            jQuery("#hidden-span").Toggle(false);
            Assert.AreEqual("none", jQuery("#hidden-span").Css("display"), "Span has display=none attribute after Toggle(false)");
        }


    }
}
