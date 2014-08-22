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
using CsQuery.Engine;
using CsQuery.HtmlParser;
using CsQuery.ExtensionMethods.Internal;
using AngleSharp.DOM;

namespace CsQuery.PerformanceTests
{

    public class PerfCompare
    {
        public string Context { get; set; }
        public CQ CsqueryDocument_Simple { get; set; }
        public CQ CsqueryDocument_Ranged { get; set; }
        public CQ CsqueryDocument_NoIndex { get; set; }
        public IDocument AngleSharpDocument { get; set; }
        public HtmlDocument HapDocument { get; set; }
        public TimeSpan MaxTestTime { get; set; }
        
        /// <summary>
        /// Loads both CSQ and HAP documents
        /// </summary>
        /// <param name="doc"></param>
        public void LoadBoth(string doc)
        {
            var html = Support.GetFile(Program.ResourceDirectory+"\\"+doc+".htm");

            var factory = new ElementFactory(DomIndexProviders.Simple);

            using (var stream = html.ToStream())
            {
                var document = factory.Parse(stream, Encoding.UTF8);
                CsqueryDocument_Simple = CQ.Create(document);
            }

            factory = new ElementFactory(DomIndexProviders.Ranged);

            using (var stream = html.ToStream())
            {
                var document = factory.Parse(stream, Encoding.UTF8);
                CsqueryDocument_Ranged= CQ.Create(document);
            }

            factory = new ElementFactory(DomIndexProviders.None);

            using (var stream = html.ToStream())
            {
                var document = factory.Parse(stream, Encoding.UTF8);
                CsqueryDocument_NoIndex = CQ.Create(document);
            }

            HapDocument = new HtmlDocument();
            HapDocument.LoadHtml(html);

            AngleSharpDocument = AngleSharp.DocumentBuilder.Html(html);
        }

        public PerfComparison Compare(string selector, string xpath)
        {
            int cqCount = CsqueryDocument_Simple[selector].Length;
            
            int hapCount=0;
            try
            {
                hapCount = HapDocument.DocumentNode.SelectNodes(xpath).OrDefault().Count();
            }
            catch { }

            int angleSharpCount = 0;
            try
            {
                angleSharpCount = AngleSharpDocument.QuerySelectorAll(selector).Count();
            }
            catch { }

            string description = String.Format("Selector returning {0} elements.", cqCount);

            bool same = true;
            if (hapCount != cqCount)
            {
                description += String.Format(" NOTE: HAP returned {0} elements", hapCount);
                same = false;
            }
            if (angleSharpCount != cqCount)
            {
                description += String.Format(" NOTE: AngleSharp returned {0} elements", angleSharpCount);
                same = false;
            }



            // use Count() for both to ensure that all results are retrieved (e.g. if the engine is lazy)

            string testName = String.Format("css '{0}', xpath '{1}'", selector, xpath);
            Action csq1 = new Action(() =>
            {
                var csqLength = CsqueryDocument_NoIndex[selector].ToList(); 
            });

            Action csq2 = new Action(() =>
            {
                var csqLength = CsqueryDocument_Simple[selector].ToList(); 
            });

            Action csq3 = new Action(() =>
            {
                var csqLength = CsqueryDocument_Ranged[selector].ToList();
            });

            Action fiz = new Action(() =>
            {
                var hapLength = HapDocument.DocumentNode.QuerySelectorAll(selector).ToList();
                
            });

            Action hap = new Action(() =>
            {
                var hapLength = HapDocument.DocumentNode.SelectNodes(xpath).OrDefault().ToList();
            });

            Action angle = new Action(() =>
            {
                var angleSharpLength = AngleSharpDocument.QuerySelectorAll(selector).ToList();
            });

            IDictionary<string, Action> actions = new Dictionary<string, Action>();
            if (Program.IncludeTests.HasFlag(TestMethods.CsQuery_NoIndex))
            {
                actions.Add("No Index (CsQuery)", csq1);
            }
            if (Program.IncludeTests.HasFlag(TestMethods.CsQuery_SimpleIndex))
            {
                actions.Add("Simple Index (CsQuery)", csq2);
            }
            if (Program.IncludeTests.HasFlag(TestMethods.CsQuery_RangedIndex))
            {
                actions.Add("Ranged Index (CsQuery)", csq3);
            }
            if (Program.IncludeTests.HasFlag(TestMethods.HAP))
            {
                actions.Add("HAP", hap);
            }
            if (Program.IncludeTests.HasFlag(TestMethods.Fizzler))
            {
                actions.Add("Fizzler", fiz);
            }
            if (Program.IncludeTests.HasFlag(TestMethods.AngleSharp))
            {
                actions.Add("AngleSharp", angle);
            }
            var results = Compare(actions, testName, description);
            results.SameResults = same;
            results.Context = Context;
            return results;
        }

        public PerfComparison Compare(IEnumerable<KeyValuePair<string,Action>> actions, 
            string testName,
            string description="")
        {

            var comparison = NewComparison();

            var maxTime = MaxTestTime;
            DateTime start = DateTime.Now;

            comparison.TestName = testName;
            comparison.Description = description;

            foreach (var item in actions)
            {
                var perf = Test(item.Value, maxTime, item.Key);
                comparison.Data.Add(perf);
            }

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
