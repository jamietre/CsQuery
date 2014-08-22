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
using AngleSharp.DOM;

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

            });

            Action csq2 = new Action(() =>
            {
                var factory = new ElementFactory(DomIndexProviders.Simple);

                using (var stream = html.ToStream())
                {
                    var document = factory.Parse(stream, Encoding.UTF8);
                    var csqDoc = CQ.Create(document);
                }

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
            });

            Action angleSharp = new Action(() =>
            {
                var angleSharpDocument = AngleSharp.DocumentBuilder.Html(html);
            });


            IDictionary<string,Action> tests = new Dictionary<string,Action>();

            if (Program.IncludeTests.HasFlag(TestMethods.CsQuery_NoIndex)) {
                tests.Add("No Index (CsQuery)", csq1);
            }
            if (Program.IncludeTests.HasFlag(TestMethods.CsQuery_SimpleIndex)) {
                tests.Add("Simple Index (CsQuery)",csq2);
            }
            if (Program.IncludeTests.HasFlag(TestMethods.CsQuery_RangedIndex))
            {
                tests.Add("Ranged Index (CsQuery)", csq3);
            }
            if (Program.IncludeTests.HasFlag(TestMethods.HAP) || Program.IncludeTests.HasFlag(TestMethods.Fizzler))
            {
                tests.Add("HAP", hap);
            }

            if (Program.IncludeTests.HasFlag(TestMethods.AngleSharp))
            {
                tests.Add("AngleSharp", angleSharp);
            }

            Compare(tests, "Create DOM from html");

        }

        public void IDSelectors()
        {
            Compare("#body","//*[@id='body']");
            Compare("#button","//*[@id='button']");
            Compare("#missing","//*[@id='missing']");

            //Assert.IsTrue(true,"Performance tests completed.");
        }


        public void SubSelectors()
        {
            Compare("div > span","//div/span");
            Compare("div span:first-child","//div//span[1]");
            Compare("div span:last-child","//div//span[last()]");
            Compare("div > span:last-child","//div/span[last()]");
            Compare("div span:only-child","//div//span[last() = 1]");
        }


        public void AttributeSelectors()
        {
            Compare("[type]","//*[@type]");
            Compare("[type]", "descendant-or-self::*[@type]");
            Compare("input[type]","//input[@type]");
            Compare("input[type][checked]","//input[@type and (@checked)]");
        }


        public void NthChild()
        {


            Compare("div:nth-child(3)","//div[(position() = 3)]");
            Compare("div:nth-child(2n+1)","//div[(position() -1) mod 2 = 0 and position() >= 1]");
            Compare("div:nth-last-child(3)","//div[position() = last() - 3]");
            Compare("div:nth-last-child(2n+1)", "//div[(position() +1) mod -2 = 0 and position() < (last() -1)]");
        }



        public  void Miscellanous()
        {
            Compare("*","//*");
            Compare("div.thumb", "//div[contains(concat(' ', normalize-space(@class), ' '), ' thumb ')]");
            Compare(".note", "//*[contains(concat(' ', normalize-space(@class), ' '), ' note ')]");
            Compare("table tr:nth-child(3) td:nth-child(2)", "//table//tr[3]//td[2]");
            Compare("div + div","//div/following-sibling::*[name() = 'div' and (position() = 1)]");


        }

    }
}
