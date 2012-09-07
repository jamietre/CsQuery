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
    public class ScriptDependencies2: AppHostBase
    {
        public ScriptDependencies2()
            : base()
        {
            Doc = RenderView<TestController>("index2", false);
        }
        
        CQ Doc;

        [TestMethod]
        public void BundlingDetails2()
        {
            var bundleScripts = Doc["script[src^=/cqbundle]"];
            Assert.AreEqual(1, bundleScripts.Length);
            
            var bundleUrl = bundleScripts.Attr("src");
            var bundles = Host.BundleFilesForUrl(bundleUrl);
            Assert.AreEqual(4, bundles.Count());

            var files = bundles.Select(item => item.AfterLast("\\"));
            CollectionAssert.AreEqual(new String[] { "dep3.js", "dep1.js", "dep2.js", "dep4.js" }, files.ToList());

        }
         
    }
}
