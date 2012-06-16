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
    public class PerfComparison
    {
        public PerfComparison()
        {
            Data = new List<PerfData>();
        }
        public string TestName { get; set; }
        public string Description { get; set; }

        public List<PerfData> Data { get; protected set; }
        public override string ToString()
        {
            string divider="-----------------------" ;
            string newline = System.Environment.NewLine;
            string results = "";
            results += ">>> Comparison for \"" + TestName + "\"" + newline;
            if (!String.IsNullOrEmpty(Description))
            {
                results += ">>> " + Description + newline;
            }
            results += divider + newline;

            PerfData best=null;
            PerfData worst=null;
            PerfData nextBest = null;

            foreach (var item in Data)
            {
                results += "\"" + item.Source + "\": ";
                if (item.Iterations == 0)
                {
                    results += String.Format("Failed, error: \"{0}\"",item.ErrorMessage) + newline;
                }
                else
                {
                    results += String.Format("{0} iterations per second ({1} in {2} seconds)",
                    Math.Round(item.IterationsPerSecond, 1),
                    item.Iterations,
                    Math.Round((double)item.Time.TotalMilliseconds / 1000d, 1))
                    + newline;
                }

                if (best == null || item.IterationsPerSecond > best.IterationsPerSecond)
                {
                    nextBest = best;
                    best = item;
                }
                else if (nextBest == null || item.IterationsPerSecond > nextBest.IterationsPerSecond)
                {
                    nextBest = item;
                }

                if (worst == null || item.IterationsPerSecond < worst.IterationsPerSecond)
                {
                    worst = item;
                }

            }
            results += divider + newline;

            string howMuchFaster = nextBest.Iterations > 0 ?
                Math.Round(best.IterationsPerSecond / nextBest.IterationsPerSecond, 2).ToString() :
                "--";
            results += String.Format(">>> WINNER: \"{0}\" -- {1} times faster than next best performer ({2})",
                best.Source, 
                howMuchFaster,
                nextBest.Source);

            return results;
        }

    }
}
