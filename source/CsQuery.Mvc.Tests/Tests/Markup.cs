using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CsQuery.Mvc.Tests.Controllers;
using CsQuery.Mvc;

namespace CsQuery.Mvc.Tests
{
    [TestClass]
    public class Markup: AppHostBase
    {
        [TestMethod]
        public void Index()
        {
            var doc = RenderView<TestController>("index");

            Common(doc);

            Assert.AreEqual(1, doc["#index-content"].Length);
            Assert.AreEqual(0, doc["#action1-content"].Length);

            Assert.AreEqual("cq-index", doc["header"][0].ClassName);

        }
        [TestMethod]
        public void Action1()
        {
            var doc = RenderView<TestController>("action1");

            Common(doc);

            Assert.AreEqual(0, doc["#index-content"].Length);
            Assert.AreEqual(1, doc["#action1-content"].Length);

            Assert.AreEqual("cq-action1", doc["header"][0].ClassName);
          
        }

        /// <summary>
        /// These tests ensure that the code run for any action (Cq_Start and Cq_End) are applied for either action we test.
        /// </summary>
        ///
        /// <param name="doc">
        /// The document.
        /// </param>

        protected void Common(CQ doc)
        {
            Assert.IsTrue(doc["#cq-footer-1"].HasClass("cq-start"), "the 1st footer should get classes applied by controller-specific startup code");
            Assert.IsTrue(doc["#cq-footer-1"].HasClass("cq-end"), "the 1st footer should get classes applied by controller-specific end code");

            Assert.IsFalse(doc["#cq-footer-2"][0].HasClasses, "The footer added should have no classes from the controller-specific code");
        }
    }
}
