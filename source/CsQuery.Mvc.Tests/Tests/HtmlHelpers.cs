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
    public class HtmlHelpers : AppHostBase
    {
        /// <summary>
        /// Ensure that HTML helper produces correct output. 
        /// </summary>

        [TestMethod]
        public void HtmlTag()
        {
            var doc = RenderView<TestController>("index");

            Assert.IsTrue(doc["#test-cq-helper"].HasClass("testclass"));

            // Check the inner text to be sure the correct rendering method is being used
            Assert.AreEqual("This is the inner text",doc["#test-cq-helper"].Text());
            Assert.IsTrue(doc["#test-cq-helper2"].HasClass("testclass2"));

        }

    }
}
