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
    public class PerfComparison
    {
        public PerfComparison()
        {
            Data = new List<PerfData>();
        }

        private bool statisticsCalculated = false;
        private PerfData _Best;
        private PerfData _Worst;
        private PerfData _NextBest;

        /// <summary>
        /// Specific name for this test
        /// </summary>
        public string TestName { get; set; }
        /// <summary>
        /// Any detailed description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The source document or context for this comparison
        /// </summary>
        public string Context { get; set; }
        public bool SameResults { get; set; }
        public List<PerfData> Data { get; protected set; }


        public PerfData Best
        {
            get
            {
                EnsureStatisticsCalculated();
                return _Best;
            }
        }
        public PerfData NextBest
        {
            get
            {
                EnsureStatisticsCalculated();
                return _NextBest;
            }
        }
        public PerfData Worst
        {
            get
            {
                EnsureStatisticsCalculated();
                return _Worst;
            }
        }
        /// <summary>
        /// The number x which the winner beat the next best
        /// </summary>
        /// <returns></returns>
        public double HowMuchFaster()
        {
            EnsureStatisticsCalculated();
            return Math.Round(_Best.IterationsPerSecond / _NextBest.IterationsPerSecond, 2);
        }
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

                
            }
            results += divider + newline;

            string howMuchFaster = NextBest.Iterations > 0 ?
                HowMuchFaster().ToString() :
                "--";
            results += String.Format(">>> WINNER: \"{0}\" -- {1} times faster than next best performer ({2})",
                Best.Source, 
                howMuchFaster,
                NextBest.Source);

            return results;
        }

        private void EnsureStatisticsCalculated()
        {
            if (!statisticsCalculated)
            {
                _Best = null;
                _Worst = null;
                _NextBest = null;

                foreach (var item in Data)
                {
                    if (_Best == null || item.IterationsPerSecond > _Best.IterationsPerSecond)
                    {
                        _NextBest = _Best;
                        _Best = item;
                    }
                    else if (_NextBest == null || item.IterationsPerSecond > _NextBest.IterationsPerSecond)
                    {
                        _NextBest = item;
                    }

                    if (_Worst == null || item.IterationsPerSecond < _Worst.IterationsPerSecond)
                    {
                        _Worst = item;
                    }
                }
            }
        }

    }
}
