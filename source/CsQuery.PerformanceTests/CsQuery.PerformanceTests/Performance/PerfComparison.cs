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

namespace CsQuery.PerformanceTests
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
        public double HowMuchWorse()
        {
            EnsureStatisticsCalculated();
            return HowMuchWorse(_Best,_NextBest);
        }

        public override string ToString()
        {
            string divider="-----------------------" ;
            string newline = System.Environment.NewLine;
            var results = new List<string>();

            results.Add(">>> Comparison for " + TestName );

            if (!String.IsNullOrEmpty(Description))
            {
                results.Add(">>> " + Description + newline);
            }

            results.Add("Target".PadRight(40)
                +"iterations/s".PadRight(16)
                +"%".PadRight(16)
                +"winner xFaster");

            results.Add(divider);

            var ordered = Data.OrderByDescending(item => item.Iterations);
            var best = ordered.First();

            int pos = 1;
            foreach (var item in ordered)
            {
                string result = String.Format("#{0}. {1}", pos, item.Source).PadRight(40);

                    if (item.Iterations == 0)
                    {
                        result += String.Format("Failed, error: \"{0}\"", item.ErrorMessage) + newline;
                    }
                    else
                    {
                        result += String.Format("{0}", Math.Round(item.IterationsPerSecond, 2)).PadRight(16);

                        if (pos > 1)
                        {

                            result += String.Format("{0}%", PercentWorse(best, item)).PadRight(16)
                                    + String.Format("{0}x", HowMuchBetter(best, item));
                        }

                    }
                results.Add(result);
                pos++;
            }
            results.Add(divider);


            return String.Join(Environment.NewLine,results);
        }

        private double PercentWorse(PerfData result1, PerfData result2)
        {
            return (HowMuchWorse(result1, result2)) * 100;
        }
        private double HowMuchWorse(PerfData result1, PerfData result2)
        {
            return PctDiff(result1, result2, true);
        }
        private double HowMuchBetter(PerfData result1, PerfData result2)
        {
            return PctDiff(result1, result2, false);
        }

        private double PctDiff(PerfData result1, PerfData result2, bool showPctWorse)
        {
            double faster = result1.IterationsPerSecond > result2.IterationsPerSecond ?
                result1.IterationsPerSecond : result2.IterationsPerSecond;

            double slower = result1.IterationsPerSecond > result2.IterationsPerSecond ?
                result2.IterationsPerSecond :result1.IterationsPerSecond;

            if (showPctWorse)
            {
                return faster != 0 ?
                    Math.Round(slower / faster, 2) :
                    0;
            }
            else
            {
                return slower != 0 ?
                Math.Round(faster / slower, 2) :
                0;
            }
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
