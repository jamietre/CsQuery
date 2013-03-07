using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using CsQuery;
using CsQuery.Utility;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using System.Diagnostics;
using CsQuery.EquationParser;

// namespace: CsQuery.Tests._Performance
//
// summary:	Underscored to keep this test (which you won't want to run very often) at the top of lists..

namespace CsQuery.PerformanceTests
{
    public abstract class PerformanceTest
    {
        private static bool IsPerformanceTest = false;
        private static bool isHeaderWritten = false;

        private static string OutputPrefix = "perftest_";
        private static string OutputPrefixCsv = "perftest_";
        private static string OutputFileName;
        private static string OutputFileNameCsv;
        private static string OutputFolder;


        public PerformanceTest()
        {
            PerfCompare = new PerfCompare();
            PerfCompare.MaxTestTime = TimeSpan.FromSeconds(5);
        }

        static PerformanceTest()
        {

            //CsQuery.HtmlParser.HtmlData.Touch();

            OutputFolder = Program.OutputDirectory;
            if (!Directory.Exists(OutputFolder))
            {
                Directory.CreateDirectory(OutputFolder);

            }

            DateTime dt = DateTime.Now;

            string dateStamp = String.Format("{0}_{1}_{2}_{3}",
                    dt.Year,
                    dt.Month,
                    dt.Day,
                    dt.Hour * 3600 + dt.Minute * 60 + dt.Second
                    );

            OutputFileName = OutputFolder + OutputPrefix + dateStamp + ".txt";
            OutputFileNameCsv = OutputFolder + OutputPrefixCsv + dateStamp + ".csv";

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
            IsPerformanceTest = false;
        }

        protected PerfCompare PerfCompare;

        protected void Compare(string selector, string xpath)
        {
            var comp = PerfCompare.Compare(selector,xpath);
            Output(comp);
        }
        protected void Compare(IEnumerable<KeyValuePair<string,Action>> actions,
            string testName,
            string description = "")
        {
            GC.Collect();
            var comp = PerfCompare.Compare(actions, testName, description);
            Output(comp);
        }


        #region static file output methods

        private static string qu= "\"";

        public static void Output(PerfComparison comp)
        {

            if (!isHeaderWritten)
            {
                OutputHeaders();
                OutputHeadersCsv(comp.Data.Count);
                isHeaderWritten = true;
            }

            OutputToFile(OutputFileName, comp.ToString());
            OutputLineToFile(OutputFileName);
            OutputLineToFile(OutputFileName);

            string line = qu + comp.TestName + qu + ","
                + qu + comp.Context + qu + ","
                + qu + comp.Description + qu + ","
                + (comp.SameResults ? 1 : 0).ToString() + ","
                + qu + comp.Best.Source + qu + ","
                + qu + comp.HowMuchWorse().ToString() + qu;


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
