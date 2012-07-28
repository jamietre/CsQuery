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
    public class Basic
    {
        [TestMethod]
        public void TestMethod1()
        {
            var doc = TestUtil.RenderViewCQ<TestController>("index");
            

            Assert.AreEqual("cq-index ran",doc["#index-content"].Text());
        }
    }
}
