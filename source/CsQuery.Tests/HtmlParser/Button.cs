using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.HtmlParser;
using CsQuery.Utility;

namespace CsQuery.Tests.HtmlParser
{

    [TestFixture, TestClass]
    public class Button : CsQueryTest
    {
        /// <summary>
        /// Ensure a parsed button implements the button interface.
        /// </summary>
        [Test, TestMethod]
        public void ImplementsInterface()
        {
            CQ cq = CQ.Create("<button>Boo!</button>");
            IHTMLButtonElement buttonElement = cq["button"].FirstElement() as IHTMLButtonElement;

            Assert.IsNotNull(buttonElement);
        }

        /// <summary>
        /// Ensure the button type is always lowercase.
        /// </summary>
        [Test, TestMethod]
        public void LowercaseType()
        {
            CQ cq = CQ.Create("<button type=SEARCH>Boo!</button>");
            IHTMLButtonElement buttonElement = cq["button"].FirstElement() as IHTMLButtonElement;

            Assert.IsNotNull(buttonElement);
            Assert.AreEqual("search", buttonElement.Type);
            Assert.AreEqual("SEARCH", buttonElement.GetAttribute("type"));
        }

        /// <summary>
        /// Ensure the button type defaults to submit.
        /// </summary>
        [Test, TestMethod]
        public void DefaultSubmitType()
        {
            CQ cq = CQ.Create("<button>Boo!</button>");
            IHTMLButtonElement buttonElement = cq["button"].FirstElement() as IHTMLButtonElement;

            Assert.IsNotNull(buttonElement);
            Assert.AreEqual("submit", buttonElement.Type);
            Assert.IsFalse(buttonElement.HasAttribute("type"));
        }
    }
}