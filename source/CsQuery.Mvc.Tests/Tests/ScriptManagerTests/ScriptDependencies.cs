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
    public class ScriptDependencies: AppHostBase
    {
        public ScriptDependencies()
            : base()
        {
            Doc = RenderView<TestController>("index", false);
        }
        CQ Doc;

        
        // the referenece to "scripts/test-script" should result in all 3 dependencies being loaded. 
        // it loads dep1 & dep2; dep1 loads dep3. The should be ordered in revers: dep3, dep2, dep1

        [TestMethod]
        public void BundlingBasic()
        {
            var scripts = Doc["script"];
            Assert.AreEqual(2, scripts.Length);

            var bundleUrl = scripts[0]["src"];
            
            string startsWith = "/cqbundle";
            Assert.IsTrue(bundleUrl.StartsWith(startsWith));

            // ensure it has a valid "v=" string
            Assert.IsTrue(bundleUrl.Length > 10);
        }

        /// <summary>
        /// Ensure that the dependencies are resolved in the correct order, and without duplicates
        /// </summary>

        [TestMethod]
        public void BundlingDetails() 
        {
            var bundleScripts = Doc["script[src^=/cqbundle]"];
            Assert.AreEqual(1, bundleScripts.Length);
            
            var bundleUrl = bundleScripts.Attr("src");
            var bundles = Host.BundleFilesForUrl(bundleUrl);
            Assert.AreEqual(3, bundles.Count());

            var files = bundles.Select(item=>item.AfterLast("\\"));
            CollectionAssert.AreEqual(new String[] { "dep3.js","dep1.js","dep2.js"},files.ToList());

        }
 
    }
}
