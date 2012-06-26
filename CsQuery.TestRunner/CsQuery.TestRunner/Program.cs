using System;
using System.Collections.Generic;
using System.Text;
using CsqueryTests;
using CsqueryTests.Performance;

namespace CsQuerySpeedTest
{
    public class Program
    {
        static void Main()
        {
            CsQueryTest.SolutionDirectory= "C:\\projects\\csharp\\csquery\\";

            CsqueryTests.Performance.PerformanceTest.SetupTestRun();

            PerformanceShared test = new _Performance_SmallDom();

            test.FixtureSetUp();
            test.LoadingDom();

            test = new _Performance_MediumDom();

            test.FixtureSetUp();
            test.LoadingDom();

            test = new _Performance_BigDom();

            test.FixtureSetUp();
            test.LoadingDom();

            CsqueryTests.Performance.PerformanceTest.CleanupTestRun();
        }
    }
}
