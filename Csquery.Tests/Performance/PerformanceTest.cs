using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using CsQuery;
using CsQuery.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using MsAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using System.Diagnostics;
using CsQuery.EquationParser;

namespace CsqueryTests.Performance
{
    [TestClass]
    public class PerformanceTest : CsQueryTest
    {
        

        /// <summary>
        /// Set up this test run - configuration of the file name is done in the static constructor so
        /// it's not starting a new file for each test fixture.
        /// </summary>

        [AssemblyInitialize]
        public static void SetupTestRun(TestContext context)
        {

            OutputFolder = CsQueryTest.TestProjectDirectory + "\\performance\\output\\";

            DateTime dt = DateTime.Now;
            OutputFileName = OutputFolder + outputPrefix +
                String.Format("{0}_{1}_{2}_{3}_{4}_{5}",
                    dt.Year,
                    dt.Month,
                    dt.Day,
                    dt.Hour,
                    dt.Minute,
                    dt.Second
                    ) +
                    ".txt";



            OutputLine("Beginning tests " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
            OutputLine("");
            Debug.WriteLine("");
        }


        [AssemblyCleanup]
        public static void CleanupTestRun()
        {
            Debug.WriteLine("");
            OutputLine();
            OutputLine("Completed tests " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
            OutputLine();
          
            File.Copy(OutputFileName, OutputFolder + outputPrefix + "latest.txt", true);

        }

        private static string outputPrefix = "perftest_";
        private static string OutputFileName;
        private static string OutputFolder;

        protected PerfCompare PerfCompare;



        protected void Compare(string selector)
        {
            var comp = PerfCompare.Compare(selector);
            Output(comp);
        }
        protected void Compare(Action action1, Action action2,
            string testName,
            string description = "")
        {
            var comp = PerfCompare.Compare(action1,action2,testName,description);
            Output(comp);
        }



        public override void FixtureSetUp()
        {
            base.FixtureSetUp();
            PerfCompare = new PerfCompare();
            PerfCompare.MaxTestTime = TimeSpan.FromSeconds(5);
        }

        #region static file output methods

        public static void Output(PerfComparison comp)
        {
            Output(comp.ToString());
            OutputLine();
            OutputLine();
        }
        public static void OutputLine(string text = "")
        {
            Output(text + System.Environment.NewLine);
        }

        public static void Output(string text)
        {
            Debug.WriteLine(text);

            File.AppendAllText(OutputFileName, text);
        }

        #endregion

    }
}
