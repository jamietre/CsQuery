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
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery.Mvc.Tests.Controllers;
using CsQuery.Mvc;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.Mvc.Tests
{
    [TestClass,TestFixture]
    public class ScriptDependenciesInvalid: AppHostBase
    {
        /// <summary>
        /// The partial view InvalidDependencies is unresolvable and should throw an error.
        /// </summary>

        [Test,TestMethod]
        public void GetViewWithErrors()
        {
            Assert.Throws<FileNotFoundException>(() =>
            {
                var doc = RenderView<TestController>("invalidscripts", false);
            });

            
        }

        /// <summary>
        /// Ensure that root-level scripts that can't be resolved don't throw errors
        /// </summary>

        [Test, TestMethod]
        public void GetViewWithUnresolvedScripts()
        {
            var doc = RenderView<TestController>("unresolvedscripts", false);

            // there should be 3 scripts generated, dep5 & dep6 which is "nocombine" plus the others

            Assert.AreEqual(3, doc["script.csquery-generated"].Length);

            // .. plus the existing 5
            Assert.AreEqual(8, doc["script"].Length);
            
        }
         
    }
}
