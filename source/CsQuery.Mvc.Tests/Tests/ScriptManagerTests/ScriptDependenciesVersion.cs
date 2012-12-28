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
    public class ScriptDependenciesVersion: AppHostBase
    {
        public ScriptDependenciesVersion()
            : base()
        {
            Doc = RenderView<TestController>("index5", false);
        }
        CQ Doc;

        
        // Test that using the {version} directive in a dependency path works.
        // 

        [TestMethod]
        public void VersioningWorks()
        {
            var scripts = Doc["script"];
            Assert.AreEqual(2, scripts.Length);

            var bundleScripts = Doc["script[src^=/cqbundle]"];
            Assert.AreEqual(1, bundleScripts.Length);
            
            var bundleUrl = bundleScripts.Attr("src");
            var bundles = Host.BundleFilesForUrl(bundleUrl);
            Assert.AreEqual(2, bundles.Count());

            var files = bundles.Select(item=>item.AfterLast("\\"));
            CollectionAssert.AreEqual(new String[] { "dep6-1.2.3.js","dep7-1.4-beta.js"},files.ToList());

        }
 
    }
}
