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
    [TestClass]
    public class _DomCreationPerformance: CsQueryTest
    {
        protected int iterationsLoad = 3;
        protected int iterationsSelect = 10;
        protected int iterationsClone = 2;
        
        [TestMethod,Test]
        public void ParseHTMLSpec()
        {
            string html;

            DateTime start = DateTime.Now;

            System.GC.Collect();
            var GC_MemoryStart = System.GC.GetTotalMemory(true);
            html = Support.GetFile("csquery\\csquery.tests\\resources\\HTML Standard.htm");
            for (int i = 0; i < iterationsLoad; i++)
            {

                Dom = CQ.Create(html);
            }
            var GC_MemoryEnd = System.GC.GetTotalMemory(true);

            DateTime loaded = DateTime.Now;

            int divSpan = 0;

            int randomLength = Dom.Select("p").Eq(22).RenderSelection().Length;

            var equation = Equations.CreateEquation<int>("2x+2");

            for (int i = 0; i < iterationsSelect; i++)
            {
                // todo: cache equations once parsed - should speed this up immenseley
                CQ sel = Dom.Select("div:nth-child(2n+1)");
                //CsQuery sel = Dom.Select("div span");
                
                // get length to force it to iterate through everything
                int len = sel.Length;

                //equation.SetVariable("x", 12);
               // var y = equation.Value;
            }

            CQ selFinal = Dom.Select("div span");
            divSpan = selFinal.Length;

            DateTime selected = DateTime.Now;

          
            for (int i = 0; i < iterationsClone; i++)
            {
                CQ c = Dom.Clone();
                Assert.AreEqual(divSpan, Dom.Select("div span").Length, "Clone wasn't equal in # of elements");
                string cloneContents = c.Select("p").Eq(22).RenderSelection();
                Assert.AreEqual(randomLength, cloneContents.Length, "Some random text was right");
            }
            DateTime done = DateTime.Now;
            TimeSpan loadTime = loaded - start;

            string result = Dom.Select("div").Length + " div elements";
            result += ", " + Dom.Select("*").Length + " total elements";
            result += ", " + loadTime.TotalSeconds / iterationsLoad + " seconds to load domain";
            result += ", " + (selected - loaded).TotalSeconds / iterationsSelect + " seconds to perform select 'div span' - " + Dom.Select("div span").Length + " elements";
            result += ", " + (done - selected).TotalSeconds / iterationsClone + " seconds to cloning";
            result += ", " + (GC_MemoryEnd - GC_MemoryStart) + " bytes used by object";
            //Debug.WriteLine(result);
            Assert.Pass(result);
        }
        [TestMethod, Test]
        public void ParseHTMLSpecHAP()
        {
            string html;

            DateTime start = DateTime.Now;
            HtmlDocument doc = new HtmlDocument();
            var GC_MemoryStart = System.GC.GetTotalMemory(true);
            html = Support.GetFile("csquery\\csquery.tests\\resources\\HTML Standard.htm");
            for (int i = 0; i < iterationsLoad; i++)
            {
                doc = new HtmlDocument();
                doc.LoadHtml(html);

            }
            var GC_MemoryEnd = System.GC.GetTotalMemory(true);

            DateTime loaded = DateTime.Now;

            int divSpan = 0;

            int randomLength = doc.DocumentNode.QuerySelectorAll("p").ElementAt(22).InnerHtml.Length;

            iterationsSelect = (int)Math.Floor((double)iterationsSelect / 10);

            for (int i = 0; i < iterationsSelect ; i++)
            {
                //HtmlNodeCollection sel = doc.DocumentNode.SelectNodes("//div");
                HtmlNode node = doc.DocumentNode;
                IEnumerable<HtmlNode> sel = node.QuerySelectorAll("div span");
                divSpan = sel.Count();
            }
            DateTime selected = DateTime.Now;

            // This test is way too slow with HAP to be meaningful. If there is another way to 
            // create subselectors I don't know it

            int count=0;
            for (int i = 0; i < iterationsClone ; i++)
            {
                HtmlNode n = doc.DocumentNode.Clone();
                //var divs = n.SelectNodes("//div");
                var divs = n.QuerySelectorAll("div span");
                count = divs.Count();
                //foreach (var item in divs)
                //{
                //    var spans = item.SelectNodes("//span");
                //    count += spans.Count;
                // }

                Assert.AreEqual(divSpan, count, "Clone was equal in # of elements");

                int cloneContentsLen = n.SelectNodes("//p")[22].InnerHtml.Length; ;
                Assert.AreEqual(randomLength, cloneContentsLen, "Some random text was right");
            }
            DateTime done = DateTime.Now;
            TimeSpan loadTime = loaded - start;

            string result = divSpan + " div elements";
            result += ", unknown total elements";
            result += ", " + loadTime.TotalSeconds / iterationsLoad + " seconds to load domain";
            result += ", " + (selected - loaded).TotalSeconds / iterationsSelect + " seconds to perform select 'div span' - " + count + " elements";
            result += ", " + (done - selected).TotalSeconds / iterationsClone + " seconds to cloning";
            result += ", " + (GC_MemoryEnd - GC_MemoryStart) + " bytes used by object";
            //Debug.WriteLine(result);
            Assert.Pass(result);
        }
    }
}
