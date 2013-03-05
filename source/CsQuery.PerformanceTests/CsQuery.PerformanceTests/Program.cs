using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using CsQuery.Utility;
using CsQuery.PerformanceTests.Tests;

namespace CsQuery.PerformanceTests
{
    /// <summary>
    /// Run performance tests comparing certain operations in CsQuery against HTMLAgilityPack +
    /// Fizzler. Will produce results in txt and csv files in the "Output" subfolder.
    /// </summary>

    public class Program
    {
        public static string OutputDirectory;
        public static string ResourceDirectory;

        static void Main()
        {

            string exeLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resources";

            if (!Support.TryGetFilePath("CsQuery.PerformanceTests\\Output", out OutputDirectory))
            {
                OutputDirectory = exeLocation + "\\Output";
            }
            if (!Directory.Exists(OutputDirectory))
            {
                Directory.CreateDirectory(OutputDirectory);
            }
            

            ResourceDirectory = exeLocation;

            Type[] TestTypes = new Type[] { 
                typeof(_Performance_SmallDom)
                , typeof(_Performance_MediumDom)
                ,typeof(_Performance_BigDom) 
            };

            foreach (var type in TestTypes)
            {
                PerformanceShared test = (PerformanceShared)Objects.CreateInstance(type);
                test.LoadingDom();
                test.Miscellanous();
                test.IDSelectors();
                test.AttributeSelectors();
                test.SubSelectors();
                test.NthChild();
            }
            
            PerformanceTest.CleanupTestRun();
        }
    }
}
