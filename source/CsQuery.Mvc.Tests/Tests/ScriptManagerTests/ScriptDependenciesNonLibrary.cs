using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Hosting;
using System.Web.Optimization;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using CollectionAssert = NUnit.Framework.CollectionAssert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery.Mvc.Tests.Controllers;
using CsQuery.Mvc;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.Mvc.Tests
{
    [TestClass,TestFixture]
    public class ScriptDependenciesNonLibrary: AppHostBase
    {
        /// <summary>
        /// The partial view InvalidDependencies is unresolvable and should throw an error.
        /// </summary>

        [Test,TestMethod]
        public void All()
        {
            
            var doc = RenderView<TestController>("depsoutsidelibrarypath", false);
            

            var bundleScripts = doc["script[src^=/cqbundle]"];
            var otherScripts = doc["script"].Not(bundleScripts);


            Assert.AreEqual(1, bundleScripts.Length);
            
            var bundleUrl = bundleScripts.Attr("src");
            var bundles = Host.BundleFilesForUrl(bundleUrl);
            Assert.AreEqual(3, bundles.Count());

            var files = bundles.Select(item => item.AfterLast("\\"));
            CollectionAssert.AreEqual(new String[] { "dep8.js", "dep9.js", "dep10.js" }, files.ToList());
            
        }

         
    }
}
