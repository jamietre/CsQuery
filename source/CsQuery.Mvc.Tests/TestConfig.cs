using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery.Utility;
using System.Reflection;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Routing;
using System.IO;
using CsQuery.Mvc;

namespace CsQuery.Mvc.Tests
{
    [SetUpFixture, TestClass]
    public class TestConfig
    {
        public static string AppPath {get; private set;}
        //public static MvcAppHost Host;

#if DEBUG 
        public static string Build = "debug";
#else
        public static string Build="release";
#endif
        
        [SetUp]
        public static void AssemblySetup()
        {
            AppPath = Path.GetDirectoryName(new System.Diagnostics.StackFrame(true).GetFileName());

            MvcAppHost.SetupApplicationHost(AppPath, AppPath + "\\bin\\" + Build);
        }

        [TearDown]
        public static void AssemblyTeardown()
        {
            MvcAppHost.CleanupApplicationHost();
        }

        /// <summary>
        /// Set up this test run - configuration of the file name is done in the static constructor so
        /// it's not starting a new file for each test fixture.
        /// </summary>

        [AssemblyInitialize]
        public static void SetupTestRun(TestContext context)
        {
            AssemblySetup();

        }

        [AssemblyCleanup]
        public static void CleanupTestRun()
        {
            AssemblyTeardown();
        }

    }
}
