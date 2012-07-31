using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery.Tests._Performance;
using CsQuery.Tests;
using CsQuery.Utility;
using System.Reflection;
using System.Diagnostics;

namespace CsQuery.Tests
{
    [SetUpFixture,TestClass]
    public class TestAssemblyConfig
    {
        [SetUp]
        public static void AssemblySetup()
        {


            var solutionFolderTry = Support.GetFilePath("./TestResults/");
            if (solutionFolderTry == "")
            {
                solutionFolderTry = Support.GetFilePath("./CsQuery.Tests/");
            }

            CsQueryTest.SolutionDirectory = Support.CleanFilePath(solutionFolderTry+"/../");
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
