using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using System.Diagnostics;
using CsQuery;
using CsQuery.Web;
using CsQuery.Promises;

namespace CsQuery.Tests.Core.WebIO
{
    /// <summary>
    /// This test is disabled by default because it accesses public web sites.
    /// 
    /// The purpose of this is to just test CsQuery's ability to parse random HTML. Because the sites are random we
    /// don't have any way to verify that it's returning the correct results.
    /// </summary>
    [TestFixture, TestClass]
    public class _WebIO_StressTests: CsQueryTest
    {
        private const int simultaneousThreads = 5;
        private const int totalTests = 100;

        public override void FixtureSetUp()
        {
            base.FixtureSetUp();
            ServerConfig.Default.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
        }

        private List<string> GetRandomUrls(int howMany)
        {

            List<string> urls = new List<string>();

            while (urls.Count < howMany)
            {
                var randomPage = CQ.CreateFromUrl("http://www.uroulette.com/");
                var links = randomPage["blockquote a"];
                foreach (var item in links)
                {
                    if (urls.Count < howMany)
                    {
                        string href = item["href"];
                        if (href.ToLower().StartsWith("http"))
                        {
                            urls.Add(item["href"]);
                        }
                    }
                }
            }
            return urls;
        }

#if SLOW_TESTS
        [Test,TestMethod]
        public void RandomUrls()
        {
            ServerConfig options = new ServerConfig {
                TimeoutSeconds=5
            };

            int outer = totalTests/simultaneousThreads;
            var list = GetRandomUrls(totalTests);
            List<string> successList = new List<string>();
            List<string> failList = new List<string>();

            int cur=0;
            for (int i = 0; i < outer; i++)
            {
                IPromise[] promises = new IPromise[simultaneousThreads];
                List<string> active = new List<string>();

                for (int j = 0; j < simultaneousThreads; j++)
                {
                    string url = list[cur++];
                    active.Add(url);

                    promises[j] = CQ.CreateFromUrlAsync(url, options).Then((success) =>
                    {
                        successList.Add(FormatResult(success));
                        active.Remove(success.Url);
    
                    },(fail) => {

                        failList.Add(FormatResult(fail));
                        active.Remove(fail.Url);
                    });

                }

                
                if (!AsyncWebRequestManager.WaitForAsyncEvents(10000))
                {
                    AsyncWebRequestManager.CancelAsyncEvents();
                    foreach (var item in active)
                    {
                        failList.Add(item + ": aborted");
                    }
                }
            }

           Debug.WriteLine(FormatTestOutput(successList,failList));
        }
#endif
        /// <summary>
        /// Format all the test output
        /// </summary>
        /// <param name="successList"></param>
        /// <param name="failList"></param>
        /// <returns></returns>
        private string FormatTestOutput(List<string> successList, List<string> failList)
        {
            
            return successList.Count.ToString() + " urls read, " + failList.Count + " failed. Success:"
                + nl + nl
                + String.Join(System.Environment.NewLine, successList)
                + nl + nl + "Failed:" + nl + nl
                + String.Join(System.Environment.NewLine, failList);

        }

        /// <summary>
        /// Format a single web response
        /// </summary>
        /// <param name="resp"></param>
        /// <returns></returns>
        private string FormatResult(ICsqWebResponse resp)
        {
            string url = resp.Url;
            
            if (resp.Success)
            {
                CQ dom = resp.Dom;
                return url + ": " + SelectorTests(dom);
            }
            else
            {
                string httpStatus = resp.HttpStatus > 0 ?
                    String.Format(" ({0} - {1})",
                        resp.HttpStatus,
                        resp.HttpStatusDescription) :
                        "";

                return String.Format("{0}: failed with error \"{1}\"{2}",
                    url,
                    resp.Error,
                     httpStatus);
            }
        }
        /// <summary>
        /// Run some random selectors to see if anything breaks
        /// </summary>
        /// <param name="dom"></param>
        private string SelectorTests(CQ dom)
        {
            string result = "";

            // verify tag generation works

            Assert.AreEqual(1, dom["html"].Length);
            Assert.AreEqual(1, dom["head"].Length);
            Assert.AreEqual(1, dom["body"].Length);

            Assert.AreEqual(dom["table"].Length, dom["tbody"].Length);

            var res = dom["div > span"];
            result += res.Length + ",";

            res = dom["div:nth-child(2n+1) > *"];
            result += res.Length + ",";

            res = dom["p:first-child"];
            result += res.Length + ",";

            result += dom["*"].Length + " elements";
            return result;
        }

        /// <summary>
        /// Newline character
        /// </summary>
        string nl = System.Environment.NewLine;

    }
}
