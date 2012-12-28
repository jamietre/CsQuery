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
    public class BundleContents_NoMinimize: AppHostBase
    {
        public BundleContents_NoMinimize()
            : base()
        {
            Host.ViewEngineOptions |= ViewEngineOptions.NoMinifyScripts;

        }

        [TestMethod]
        public void BundleContents()
        {
            var doc = RenderView<TestController>("index");
            var bundleUrl=  doc["script[src^=/cqbundle]"][0]["src"];

            string script = Host.BundlesContentsForUrl(bundleUrl);

            script = script.Replace("\r\n", "\n");
            Assert.AreEqual("var dep3;\n;//using dep3\nvar dep1;\n;var dep2;;", script);

            //TestConfig.Host.ViewEngineOptions |= CsQueryViewEngineOptions.NoMinifyScripts;

            //script = TestConfig.Host.BundlesContentsForUrl(bundleUrl);

            //Assert.AreEqual("somethin", script);


        }

         
    }
}
