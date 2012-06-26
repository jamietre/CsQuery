using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery.Tests.Performance;
using CsQuery.Tests;
using CsQuery.Utility;
using System.Reflection;
using System.Diagnostics;

namespace CsQuery.Tests
{
    [SetUpFixture,TestClass]
    public class TestAssemblyConfig
    {
        /// <summary>
        /// If you keep CsQuery.Tests in a folder named other than CsQuery, change this. The tests need to access things from the Resources folder in the
        /// test project. We can't be sure where this will be relative to the executing folder, so the root of the project is used as a
        /// reference. Copying the files on each build is time consuming because we test against a really big file.
        /// </summary>
        private static string SolutionFolderName= "CsQuery";

        [SetUp]
        public static void AssemblySetup()
        {
           
            CsQueryTest.SolutionDirectory = Support.GetFilePath("./" + SolutionFolderName + "/");
            PerformanceTest.SetupTestRun();

        }
        
        [TearDown]
        public static void AssemblyTeardown()
        {
            PerformanceTest.CleanupTestRun();
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
