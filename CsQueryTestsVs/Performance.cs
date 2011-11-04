using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jtc.CsQuery;
using Jtc.CsQuery.Utility;
using Drintl.Testing;

namespace Jtc.CsQuery.Tests
{
    [TestClass]
    public class _Performance: CsQueryTest
    {
        [TestInitialize]
        public void SetUp()
        {
        }
        [TestMethod]
        public void ParseHTMLSpec()
        {
            string html;

            DateTime start = DateTime.Now;
            int iterations = 1;

            var GC_MemoryStart = System.GC.GetTotalMemory(true);
            for (int i = 0; i < iterations; i++)
            {
                html = Support.GetFile("Resources\\HTML Standard.htm");
                Dom = CsQuery.Create(html);
            }
            var GC_MemoryEnd = System.GC.GetTotalMemory(true);

            DateTime loaded = DateTime.Now;

            int selIterations = 100;
            int divSpan=0;

            for (int i = 0; i < selIterations; i++)
            {
                CsQuery sel = Dom.Select("div span");
                divSpan = sel.Length;
            }
            DateTime selected = DateTime.Now;
            
            int cloneIterations = 5;
            for (int i = 0; i < cloneIterations; i++)
            {
                CsQuery c = Dom.Clone();
                Assert.AreEqual(divSpan, Dom.Select("div span").Length, "Clone wasn't equal in # of elements");
                Assert.AreEqual(786,Dom.Select("p").Eq(22).RenderSelection().Length,"Some random text was right");
            }
            DateTime done = DateTime.Now;
            TimeSpan loadTime = loaded-start;

            string result =Dom.Select("div").Length + " div elements";
           result += ", " +Dom.Select("*").Length + " total elements";
           result += ", " + loadTime.TotalSeconds / iterations + " seconds to load domain";
           result += ", " + (selected - loaded).TotalSeconds / selIterations + " seconds to perform select 'div span' - " + Dom.Select("div span").Length + " elements";
           result += ", " + (done - selected).TotalSeconds / cloneIterations + " seconds to cloning";
           result += ", " + (GC_MemoryEnd - GC_MemoryStart) + " bytes used by object";
            assert.Inconclusive(result);
        }
    }
}
