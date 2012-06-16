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
    [TestClass]
    public class _Performance_BigDom : PerformanceTest
    {
        public override void FixtureSetUp()
        {
            base.FixtureSetUp();

            PerfCompare.LoadBoth("sizzle");
            PerfCompare.MaxTestTime = TimeSpan.FromSeconds(2);
        }
        [TestMethod, Test]
        public void IDSelectors()
        {
            Compare("#body");
            Compare("#button");

            //Assert.IsTrue(true,"Performance tests completed.");
        }

        [TestMethod, Test]
        public void SubSelectors()
        {
            Compare("div > span");
            Compare("div span:first-child");
            Compare("div span:last-child");
            Compare("div > span:last-child");
        }

        [TestMethod, Test]
        public void NthChild()
        {

            Compare("div:nth-child(2n+1)");
        }



    }
}
