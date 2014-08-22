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

    [Flags]
    public enum TestMethods
    {
        CsQuery_NoIndex = 1,
        CsQuery_SimpleIndex = 2,
        CsQuery_RangedIndex = 4,
        HAP = 8,
        Fizzler=16,
        AngleSharp=32
    }

    public class Program
    {
        public static TestMethods IncludeTests
        {
            get
            {
                return TestMethods.CsQuery_RangedIndex | 
                    TestMethods.CsQuery_NoIndex |
                    TestMethods.CsQuery_SimpleIndex |
                    TestMethods.Fizzler |
                    TestMethods.HAP |
                    TestMethods.AngleSharp;
            }
        }
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
                typeof(_Performance_SmallDom),
                typeof(_Performance_MediumDom)
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
