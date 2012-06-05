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
using CsQuery.Utility.EquationParser;

namespace CsqueryTests.Performance
{
    /// <summary>
    /// This is a quick and dirty test to compare CsQuery to HTML Agility Pack + fizzler. This is by no means comprehensive and
    /// doesn't test things like complex subselectors, or do much to compare dom creation vs. selector time. 
    /// </summary>
    [TestClass]
    public class _DomCreationPerformance: CsQueryTest
    {
        protected int iterationsLoad = 3;
        protected int iterationsSelect = 10;
        protected int iterationsClone = 2;
        string selector = "div span";

        [TestMethod,Test]
        public void CompareCsQueryToHAP()
        {
            CQ Dom=null;
            Action CsQueryLoad = new Action(()=> {
                var html = Support.GetFile(TestDomPath("HTML Standard"));
                Dom = CQ.Create(html);

            });
            Func<string,int> CsQuerySelect =  new Func<string,int>(selector =>{
                // accessing Length will force iterating through all results
                return Dom.Select(selector).Count();
            });
            Func<int> CsQueryClone = new Func<int>(()=> {
                return Dom.Clone().Length;
            });
            HtmlDocument doc = null;

            Action HapLoad = new Action(()=> {
                var html = Support.GetFile(TestDomPath("HTML Standard"));
                doc = new HtmlDocument();
                doc.LoadHtml(html);

            });
            Func<string, int> HapSelect = new Func<string, int>(selector =>
            {
                return doc.DocumentNode.QuerySelectorAll(selector).Count();

            });
            Func<int> HapClone = new Func<int>( ()=> {
                return doc.DocumentNode.Clone().ChildNodes.Count;
            });

            string result = "";
            result += RunPerfTest("CsQuery", CsQueryLoad, CsQuerySelect, CsQueryClone);
            result += RunPerfTest("HTMLAgilityPack+Fizzler",HapLoad, HapSelect,HapClone);
            
            Assert.Pass(result);
        }


        public string RunPerfTest(string title, Action loadFunc, Func<string,int> selectionFunc, Func<int> cloneFunc)
        {

            DateTime start = DateTime.Now;

            System.GC.Collect();
            var GC_MemoryStart = System.GC.GetTotalMemory(true);


            for (int i = 0; i < iterationsLoad; i++)
            {

                loadFunc();
            }
            var GC_MemoryEnd = System.GC.GetTotalMemory(true);

            DateTime loaded = DateTime.Now;

            int divSpan = 0;

            for (int i = 0; i < iterationsSelect; i++)
            {
               divSpan = selectionFunc(selector);
            }

            DateTime selected = DateTime.Now;
          
            for (int i = 0; i < iterationsClone; i++)
            {
                int cloneLen = cloneFunc();
            }
            DateTime done = DateTime.Now;
            TimeSpan loadTime = loaded - start;

            string result = title+ ":" +  selectionFunc("*") + " total elements";
            result += ", " + loadTime.TotalSeconds / iterationsLoad + " seconds to load domain";
            result += ", " + (selected - loaded).TotalSeconds / iterationsSelect + " seconds to select - " 
                + divSpan + " elements";
            result += ", " + (done - selected).TotalSeconds / iterationsClone + " seconds to cloning";
            result += ", " + (GC_MemoryEnd - GC_MemoryStart) + " bytes used";
            result += System.Environment.NewLine;

            return result;

        }
    }
}
