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
using CsQuery.Implementation;
using CsQuery.HtmlParser;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.PerformanceTests.Tests
{
    
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
        public PerformanceShared(): base() 
        {

            PerfCompare.LoadBoth(DocName);
            PerfCompare.MaxTestTime = TimeSpan.FromSeconds(TestTimeSeconds);
            PerfCompare.Context = DocDescription;
            

            OutputLine(String.Format("Beginning tests using document \"{0}\"", DocDescription));
            OutputLine();
        }

        
        public void LoadingDom()
        {

            var html = Support.GetFile(Program.ResourceDirectory+"\\"+DocName+".htm");
            
            Action csq1 = new Action(() =>
            {
                var factory = new ElementFactory(DomIndexProviders.None);
                
                using (var stream = html.ToStream()) {
                    var document = factory.Parse(stream,Encoding.UTF8);
                    var csqDoc = CQ.Create(document);
                }
                
                //var test = csqDoc["*"].Count();
            });

            Action csq2 = new Action(() =>
            {
                var factory = new ElementFactory(DomIndexProviders.Simple);

                using (var stream = html.ToStream())
                {
                    var document = factory.Parse(stream, Encoding.UTF8);
                    var csqDoc = CQ.Create(document);
                }

                //var test = csqDoc["*"].Count();
            });
            Action csq3 = new Action(() =>
            {
                var factory = new ElementFactory(DomIndexProviders.Ranged);

                using (var stream = html.ToStream())
                {
                    var document = factory.Parse(stream, Encoding.UTF8);
                    var csqDoc = CQ.Create(document);
                }

                //var test = csqDoc["*"].Count();
            });
            Action hap = new Action(() =>
            {
                var hapDoc = new HtmlDocument();
                hapDoc.LoadHtml(html);
                //var test = hapDoc.DocumentNode.QuerySelectorAll("*").Count();
            });

            IDictionary<string,Action> tests = new Dictionary<string,Action>();

            tests.Add("No Index (CsQuery)", csq1);
            tests.Add("Simple Index (CsQuery)",csq2);
            tests.Add("Ranged Index (CsQuery)",csq3);
            tests.Add("HAP",hap);

            Compare(tests, "Create DOM from html");

        }

        public void IDSelectors()
        {
            Compare("#body");
            Compare("#button");
            Compare("#missing");

            //Assert.IsTrue(true,"Performance tests completed.");
        }


        public void SubSelectors()
        {
            Compare("div > span");
            Compare("div span:first-child");
            Compare("div span:last-child");
            Compare("div > span:last-child");
            Compare("div span:only-child");
        }


        public void AttributeSelectors()
        {
            Compare("[type]");
            Compare("input[type]");
            Compare("input[type][checked]");
        }


        public void NthChild()
        {


            Compare("div:nth-child(3)");
            Compare("div:nth-child(2n+1)");
            Compare("div:nth-last-child(3)");
            Compare("div:nth-last-child(2n+1)");
        }



        public  void Miscellanous()
        {
            Compare("*");

        }

    }
}
