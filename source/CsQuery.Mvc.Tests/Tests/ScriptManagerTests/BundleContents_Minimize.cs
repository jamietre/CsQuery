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
    public class BundleContents_Minimize: AppHostBase
    {
        public BundleContents_Minimize()
            : base()
        {

        }

        [TestMethod]
        public void BundleContents()
        {
            var doc = RenderView<TestController>("index");
            var bundleUrl=  doc["script[src^=/cqbundle]"][0]["src"];

            string script = Host.BundlesContentsForUrl(bundleUrl);

            Assert.AreEqual("var dep3,dep1,dep2", script);

        }

         
    }
}
