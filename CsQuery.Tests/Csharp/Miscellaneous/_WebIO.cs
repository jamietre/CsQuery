﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery;
using CsQuery.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using System.Diagnostics;

namespace CsQuery.Tests.Csharp.Miscellaneous
{
    /// <summary>
    /// This test is disabled by default because it accesses public web sites, activate it just to test this feature
    /// </summary>
    [TestFixture, TestClass, Description("CsQuery Tests (Not from Jquery test suite)")]
    public class _WebIO : CsQueryTest
    {
        public override void FixtureSetUp()
        {
            base.FixtureSetUp();
            CQ.DefaultServerConfig.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
        }
        //[Test,TestMethod]
        public void GetHtml()
        {
            
            Dom = CQ.CreateFromUrl("http://www.microsoft.com/en/us/default.aspx?redir=true");

            Assert.IsTrue(Dom.Document != null, "Dom was created");
            var csq = Dom.Find(".hpMst_Stage");
            Assert.IsTrue(csq.Length == 1, "I found an expected content container - if MS changed their web site this could fail.");
        }

        private static int AsyncStep = 0;
        [Test, TestMethod]
        public void GetHtmlAsync()
        {
            CQ.CreateFromUrlAsync("http://www.microsoft.com/en/us/default.aspx?redir=true",1, FinishRequest);
            Debug.WriteLine("Started Async Request 1 @" + DateTime.Now);
            AsyncStep |= 1;
            CQ.CreateFromUrlAsync("http://www.cnn.com/", 2,FinishRequest);
            Debug.WriteLine("Started Async Request 2 @" + DateTime.Now);
            AsyncStep |= 2;

            // Time out after 10 seconds
            CQ.WaitForAsyncEvents(10000);
            AsyncStep |=4;

            Debug.WriteLine("Finished Test @" + DateTime.Now);

            Assert.AreEqual(31, AsyncStep, "All async steps finished before exiting.");

        }

        [Test, TestMethod]
        public void GetHtmlAsyncPromises()
        {
            bool p1resolved = false;
            bool p2resolved = false;

            var promise1 = CQ.CreateFromUrlAsync("http://www.microsoft.com/en/us/default.aspx?redir=true")
                .Then(new Action<ICsqWebResponse>((resp)  => {
                    Assert.IsTrue(resp.Dom.Find(".hpMst_Stage").Length == 1, "I found an expected content container - if MS changed their web site this could fail.");
                    p1resolved = true;
                }));

            var promise2 = CQ.CreateFromUrlAsync("http://www.cnn.com/").Then(new Action<ICsqWebResponse>((resp) =>
            {
                Assert.IsTrue(resp.Dom.Find("#cnn_hdr").Length == 1, "I found an expected content container - if CNN changed their web site this could fail.");
                p2resolved = true;
            }));

            bool complete = false;
            CQ.WhenAll(promise1,promise2).Then(new Action<ICsqWebResponse>((response) =>
            {
                Assert.IsTrue(p1resolved, "Promise 1 is resolved");
                Assert.IsTrue(p2resolved, "Promise 1 is resolved");
                complete = true;
            }), new Action(() =>
            {
                Assert.Fail("The web requests were rejected.");
            }));

            // if we don't do this the test will exit before finishing
            CQ.WaitForAsyncEvents(10000);

            Assert.IsTrue(complete);


        }

        [Test, TestMethod]
        public void GetHtmlAsyncPromisesFailed()
        {
            bool p1resolved = false;
            bool p2rejected = false;

            var promise1 = CQ.CreateFromUrlAsync("http://www.microsoft.com/en/us/default.aspx?redir=true")
                .Then(resp =>
                {
                    Assert.IsTrue(resp.Dom.Find(".hpMst_Stage").Length == 1, "I found an expected content container - if MS changed their web site this could fail.");
                    p1resolved = true;
                });

            var promise2 = CQ.CreateFromUrlAsync("http://www.bad-domain.zzyzx/").Then(null,
                resp =>
                {
                    p2rejected = true;
                });


            bool complete = false;
            CQ.WhenAll(promise1, promise2).Then(response =>
            {
                Assert.Fail("This should have been rejected");
            }, response =>
            {
                Assert.IsTrue(p1resolved, "Promise 1 is resolved");
                Assert.IsTrue(p2rejected, "Promise 2 is rejected");
                complete = true;

            });

            // if we don't do this the test will exit before finishing
            CQ.WaitForAsyncEvents(10000);

            Assert.IsTrue(complete, "Complete flag was set properly.");

        }
        private void FinishRequest(ICsqWebResponse response)
        {
            var csq= response.Dom;
            if ((int)response.Id == 1)
            {
                AsyncStep |= 8;
                Assert.IsTrue(csq.Find(".hpMst_Stage").Length == 1, "I found an expected content container - if MS changed their web site this could fail.");

            }
            if ((int)response.Id == 2)
            {
                AsyncStep |= 16;
                Assert.IsTrue(csq.Find("#cnn_hdr").Length == 1, "I found an expected content container - if CNN changed their web site this could fail.");
            }
        
            Debug.WriteLine(String.Format("Received Async Response {0} @{1}", response.Id.ToString(),DateTime.Now));

        }

    }
}
