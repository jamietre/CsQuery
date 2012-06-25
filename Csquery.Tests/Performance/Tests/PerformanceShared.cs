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
    public abstract class PerformanceShared : PerformanceTest
    {
        protected abstract string DocName {get;}
       
        protected abstract string DocDescription { get; }

        protected virtual int TestTimeSeconds
        {
            get
            {
                return 2;
            }
        }

        public override void FixtureSetUp()
        {
            base.FixtureSetUp();

            PerfCompare.LoadBoth(DocName);
            PerfCompare.MaxTestTime = TimeSpan.FromSeconds(TestTimeSeconds);
            PerfCompare.Context = DocDescription;
            

            OutputLine(String.Format("Beginning tests using document \"{0}\"", DocDescription));
            OutputLine();
        }

        [TestMethod, Test]
        public void LoadingDom()
        {
            var html = Support.GetFile(CsQueryTest.TestDomPath(DocName));
            
            Action csq = new Action(() =>
            {
                var csqDoc = CQ.Create(html);
                //var test = csqDoc["*"].Count();
            });

            Action hap = new Action(() =>
            {
                var hapDoc = new HtmlDocument();
                hapDoc.LoadHtml(html);
                //var test = hapDoc.DocumentNode.QuerySelectorAll("*").Count();
            });
            Compare(csq, hap, "Create DOM from html");

        }
        [TestMethod, Test]
        public void IDSelectors()
        {
            Compare("#body");
            Compare("#button");
            Compare("#missing");

            //Assert.IsTrue(true,"Performance tests completed.");
        }

        [TestMethod, Test]
        public void SubSelectors()
        {
            Compare("div > span");
            Compare("div span:first-child");
            Compare("div span:last-child");
            Compare("div > span:last-child");
            Compare("div span:only-child");
        }

        [TestMethod, Test]
        public void AttributeSelectors()
        {
            Compare("[type]");
            Compare("input[type]");
            Compare("input[type][checked]");
        }


        [TestMethod, Test]
        public void NthChild()
        {


            Compare("div:nth-child(3)");
            Compare("div:nth-child(2n+1)");
            Compare("div:nth-last-child(3)");
            Compare("div:nth-last-child(2n+1)");
        }


        [TestMethod, Test]
        public  void Miscellanous()
        {
            Compare("*");

        }

    }
}
