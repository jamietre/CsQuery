using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Hosting;
using System.Web.Optimization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CsQuery.Mvc.Tests.Controllers;
using CsQuery.Mvc;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.Mvc.Tests
{
    [TestClass]
    public class BundleContents_NoBundle : AppHostBase
    {
        public BundleContents_NoBundle()
            : base()
        {
            Host.ViewEngineOptions |= ViewEngineOptions.NoBundle;
        }

        [TestMethod]
        public void BundleContents()
        {
           

            var doc = RenderView<TestController>("index");
            var scripts = doc["script"];

            // dependencies should be inserted in order before the original script
            Assert.AreEqual(4, scripts.Length);
            Assert.IsTrue(scripts[0]["src"].Contains("dep3"));
            Assert.IsTrue(scripts[1]["src"].Contains("dep1"));
            Assert.IsTrue(scripts[2]["src"].Contains("dep2"));

        }


    }
}
