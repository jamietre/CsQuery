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

namespace CsQuery.Tests._Performance
{

    public class PerfCompare
    {
        public string Context { get; set; }
        public CQ CsqueryDocument { get; set; }
        public HtmlDocument HapDocument { get; set; }
        public TimeSpan MaxTestTime { get; set; }
        
        /// <summary>
        /// Loads both CSQ and HAP documents
        /// </summary>
        /// <param name="doc"></param>
        public void LoadBoth(string doc)
        {
            var html = Support.GetFile(CsQueryTest.TestDomPath(doc));
            CsqueryDocument = CQ.Create(html);

            HapDocument = new HtmlDocument();
            HapDocument.LoadHtml(html);
        }

        public PerfComparison Compare(string selector)
        {
            int cqCount = CsqueryDocument[selector].Length;
            int hapCount=0;
            try
            {
                hapCount = HapDocument.DocumentNode.QuerySelectorAll(selector).Count();
            }
            catch { }

            string description = String.Format("Selector returning {0} elements.", cqCount);

            bool same = true;
            if (hapCount != cqCount)
            {
                description += String.Format(" NOTE: HAP returned {0} elements", hapCount);
                same = false;
            }

            // use Count() for both to ensure that all results are retrieved (e.g. if the engine is lazy)

            string testName = "CSS selector: { " + selector + " }";
            Action csq = new Action(() =>
            {
                int csqLength = CsqueryDocument[selector].Count();
            });

            Action hap = new Action(() =>
            {
                int hapLength = HapDocument.DocumentNode.QuerySelectorAll(selector).Count();
            });

            var results = Compare(csq, hap, testName,description);
            results.SameResults = same;
            results.Context = Context;
            return results;
        }

        public PerfComparison Compare(Action action1, Action action2, 
            string testName,
            string description="")
        {

            var comparison = NewComparison();

            var maxTime = MaxTestTime;
            DateTime start = DateTime.Now;

            var csqPerf = Test(action1, maxTime, "CsQuery");
            var hapPerf = Test(action2, maxTime, "Fizzler");

            comparison.TestName = testName;
            comparison.Description = description;

            comparison.Data.Add(csqPerf);
            comparison.Data.Add(hapPerf);
            comparison.SameResults = true;

            return comparison;
        }

        public PerfData Test(Action action, TimeSpan maxTime, string testSource) 
        {
            
            PerfData perfData = new PerfData { Source = testSource };
            double diffMs = 0;

            DateTime start = DateTime.Now;
            // first test 1 iteration to make sure it's not really slow
            try
            {
                action();
            }
            catch (Exception e)
            {
                perfData.Iterations = 0;
                perfData.Time = TimeSpan.FromSeconds(0);
                perfData.ErrorMessage = e.Message;
                return perfData;
            }

            TimeSpan diff = DateTime.Now - start;
            if (diff > maxTime) {
                perfData.Iterations=1;
                perfData.Time = diff;
                return perfData;
            }
            diffMs = diff.TotalMilliseconds;

            // now do a slightly better time estimate
            if (diffMs < maxTime.TotalMilliseconds/100 || diffMs == 5)
            {
                start = DateTime.Now;
                int testIterations = 10;
                for (int i = 0; i < testIterations; i++)
                {
                    action();
                }
                diff = DateTime.Now - start;
                diffMs = diff.TotalMilliseconds / testIterations;
            }

            int estimatedIterations = (int)Math.Floor(maxTime.TotalMilliseconds / diffMs);
            
            // Try to check the time only 10 times per second, e.g. if we expect 500 iterations 
            // and time is 5 seconds than check every 10 iterations

            int blockSize = Math.Max(1,(int)Math.Floor(estimatedIterations/maxTime.TotalSeconds/10));

            DateTime endTime = DateTime.Now + maxTime;

            int iterations=0;
            start = DateTime.Now;

            while (DateTime.Now < endTime) {
                for (int i=0;i<blockSize;i++) {
                    action();
                }
                iterations+=blockSize;
            }

            TimeSpan totalTime = DateTime.Now - start;

            perfData.Iterations = iterations;
            perfData.Time =totalTime;
 
            return perfData;
        }

        private PerfComparison NewComparison() {
            return new PerfComparison
            {
                Context = Context
            };
        }
        
    }
}
