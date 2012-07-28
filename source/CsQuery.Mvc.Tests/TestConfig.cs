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
using System.Web.Hosting;
using System.IO;
using CsQuery.Mvc;

namespace CsQuery.Mvc.Tests
{
    [SetUpFixture, TestClass]
    public class TestConfig
    {
        public static MvcAppHost Host;
        private static DirectoryInfo TempFiles;

        [SetUp]
        public static void AssemblySetup()
        {
            string appPath = Support.FindPathTo("CsQuery.Mvc.Tests");
            string binPath = AppDomain.CurrentDomain.BaseDirectory;
            string destPath = appPath+"\\bin";
            
            // in order for CreateApplicationHost to work, all the assemblies must be in the 
            // bin folder of the root. Copy everything there.

            DirectoryInfo bin = new DirectoryInfo(binPath);
            TempFiles = new DirectoryInfo(destPath);

            Support.CopyFiles(bin, TempFiles, "*.dll", "*.pdb");

            Host = (MvcAppHost)ApplicationHost.CreateApplicationHost(
                typeof(MvcAppHost), 
                "/",
                appPath);

            Host.InitializeApplication<MvcTestApp>();
        }

        [TearDown]
        public static void AssemblyTeardown()
        {
            Support.DeleteFiles(TempFiles,"*.pdb","*.dll");
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
