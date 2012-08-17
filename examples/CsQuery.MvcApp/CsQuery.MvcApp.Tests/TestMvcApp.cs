using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CsQuery.MvcApp.Controllers;

namespace CsQuery.MvcApp.Tests
{
    /// <summary>
    /// A very simple test showing how to test an external MVC app's output
    /// </summary>

    [TestClass]
    public class TestMvcApp
    {
        [TestMethod]
        public void VerifyStyle()
        {
            var cq = TestConfig.RenderViewCQ<HomeController>("index");
            Assert.AreEqual("1px solid red", cq[".page"].Css("border"));


        }
    }
}
