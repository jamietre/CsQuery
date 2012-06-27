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
using CsQuery.Tests;

namespace CsQuery.Tests.Performance
{
    [TestClass]
    public class PerformanceTest : CsQueryTest
    {
        public static bool IsPerformanceTest = false;

        private static string OutputPrefix = "perftest_";
        private static string OutputPrefixCsv = "perftest_";
        private static string OutputFileName;
        private static string OutputFileNameCsv;
        private static string OutputFolder;

        private static bool isHeaderWritten = false;

 
        public static void SetupTestRun()
        {
            CsQuery.HtmlParser.HtmlData.Touch();
            OutputFolder = CsQueryTest.TestProjectDirectory + "\\performance\\output\\";

            DateTime dt = DateTime.Now;

            string dateStamp = String.Format("{0}_{1}_{2}_{3}_{4}_{5}",
                    dt.Year,
                    dt.Month,
                    dt.Day,
                    dt.Hour,
                    dt.Minute,
                    dt.Second
                    );

            OutputFileName = OutputFolder + OutputPrefix + dateStamp + ".txt";
            OutputFileNameCsv = OutputFolder + OutputPrefixCsv + dateStamp + ".csv";


            OutputHeaders();
            Debug.WriteLine("");
        }

        public static void CleanupTestRun()
        {
            if (!IsPerformanceTest)
            {
                return;
            }

            Debug.WriteLine("");
            OutputLineToFile(OutputFileName);
            OutputLineToFile(OutputFileName, "Completed tests " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
            OutputLineToFile(OutputFileName);
          

            File.Copy(OutputFileName, OutputFolder + OutputPrefix + "latest.txt", true);
            File.Copy(OutputFileNameCsv, OutputFolder + OutputPrefix + "latest.csv", true);

        }

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

            IsPerformanceTest = true;

            PerfCompare = new PerfCompare();
            PerfCompare.MaxTestTime = TimeSpan.FromSeconds(5);
        }

        #region static file output methods

        private static string qu= "\"";

        public static void Output(PerfComparison comp)
        {
            OutputToFile(OutputFileName, comp.ToString());
            OutputLineToFile(OutputFileName);
            OutputLineToFile(OutputFileName);

            // csv

            if (!isHeaderWritten)
            {
                isHeaderWritten = true;
                OutputHeaders();
                OutputHeadersCsv(comp.Data.Count);
            }

            string line = qu + comp.TestName + qu + ","
                + qu + comp.Context + qu + ","
                + qu + comp.Description + qu + ","
                + (comp.SameResults ? 1 : 0).ToString() + ","
                + qu + comp.Best.Source + qu + ","
                + qu + comp.HowMuchFaster().ToString() + qu;


            foreach (var item in comp.Data) {
                if (String.IsNullOrEmpty(item.ErrorMessage))
                {

                    line += ",,";
                    line += qu + item.Source + qu + ","
                        + Math.Round(item.IterationsPerSecond,2) + ","
                        + item.Iterations + ","
                        + item.TimeSeconds;
                }
                else
                {
                    line += ",,,,";

                }
            }
            OutputLineToFile(OutputFileNameCsv,line);

        }


        private static void OutputHeaders()
        {
            OutputLineToFile(OutputFileName, "Beginning tests " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
            OutputLineToFile(OutputFileName);

          
        }
        private static void OutputHeadersCsv(int numToCompare)
        {
            string line = qu + "Test Name" + qu + ","
                + qu + "Test Document" + qu + ","
                + qu + "Description" + qu + ","
                + qu + "Same" + qu + ","
                + qu + "Winner" + qu + ","
                + qu + "FasterRatio" + qu;

            // extra comma for an empty col before the output & between each result

            for (int i = 0; i < numToCompare; i++)
            {
                line += ",,"+ qu + "Source" + qu;
                line += "," + qu + "Iterations/Sec" + qu + ","
                + qu + "Iterations" + qu + ","
                + qu + "Seconds" + qu;
            }

            OutputLineToFile(OutputFileNameCsv, line);
        }
        public static void OutputLine(string text = "")
        {
            OutputLineToFile(OutputFileName, text);
        }

        public static void OutputLineToFile(string file,string text = "")
        {
            OutputToFile(file,text + System.Environment.NewLine);
        }

        private static void OutputToFile(string file, string text)
        {
            Debug.WriteLine(text);

            File.AppendAllText(file, text);
        }

        #endregion

    }
}
