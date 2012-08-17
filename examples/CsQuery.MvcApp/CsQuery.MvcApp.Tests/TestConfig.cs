using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using CsQuery;
using CsQuery.Utility;

namespace CsQuery.MvcApp.Tests
{
    [TestClass]
    public class TestConfig
    {
        public static MvcAppHost Host;


        /// <summary>
        /// Set up this test run - configuration of the file name is done in the static constructor so
        /// it's not starting a new file for each test fixture.
        /// </summary>

        [AssemblyInitialize]
        public static void SetupTestRun(TestContext context)
        {
            string testAppPath = Path.GetDirectoryName(new System.Diagnostics.StackFrame(true).GetFileName());

            string mvcAppPath = Support.CleanFilePath(testAppPath+"/../CsQuery.MvcApp");

            Host = MvcAppHost.CreateApplicationHost<CsQuery.MvcApp.MvcApplication>(mvcAppPath);

        }

        [AssemblyCleanup]
        public static void CleanupTestRun()
        {
            Host.Dispose();
        }

        /// <summary>
        /// Returns a CQ object from a view's HTML output
        /// </summary>
        ///
        /// <typeparam name="T">
        /// Controller type
        /// </typeparam>
        /// <param name="action">
        /// The action to invoke on the view
        /// </param>
        ///
        /// <returns>
        /// A CQ object
        /// </returns>

        public static CQ RenderViewCQ<T>(string action) where T : Controller, new()
        {
            // Use "CreateFragment" when testing HTML output because we don't want CsQuery to do any 
            // infilling of optional tags, etc. - we want to see what actually was produced.
            
            return CQ.CreateFragment(TestConfig.Host.RenderView<T>(action));
        }

    }
}


