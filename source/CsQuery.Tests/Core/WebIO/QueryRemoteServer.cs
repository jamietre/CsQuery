using System;
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

namespace CsQuery.Tests.Core.WebIO
{
    /// <summary>
    /// This test is disabled by default because it accesses public web sites, activate it just to test this feature
    /// </summary>
    [TestFixture, TestClass]
    public class _WebIO_QueryRemoteServer : CsQueryTest
    {
        private KeyValuePair<string,string>[] urls = new KeyValuePair<string,string>[] {
            new KeyValuePair<string,string>("https://github.com/jamietre/csquery","div.repository-description"),
            new KeyValuePair<string,string>("http://www.cnn.com/","#cnn_hdr")
        };
        
        public override void FixtureSetUp()
        {
            base.FixtureSetUp();
            ServerConfig.Default.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
        }

        [Test, TestMethod]
        public void GetHtmlWhoseContentTypeHasNoCharset()
        {
            var url = "http://www.pixnet.net";

            Dom = CQ.CreateFromUrl(url);

            Assert.IsTrue(Dom.Document != null, "Dom was created");
            var csq = Dom.Find("title:contains('痞客邦 PIXNET')");
            Assert.IsTrue(csq.Length > 0, "I found an expected content container - if Pixnet changed their web site this could fail.");
        }

        [Test, TestMethod]
        public void GetHtml()
        {
            var url = urls[0];

            Dom = CQ.CreateFromUrl(url.Key);

            Assert.IsTrue(Dom.Document != null, "Dom was created");
            var csq = Dom.Find(url.Value);
            Assert.IsTrue(csq.Length>0, "I found an expected content container - if Github changed their web site this could fail.");
        }

        private static int AsyncStep = 0;
        [Test, TestMethod]
        public void GetHtmlAsync()
        {
            CQ.CreateFromUrlAsync(urls[0].Key, 1, FinishRequest);
            Debug.WriteLine("Started Async Request 1 @" + DateTime.Now);
            AsyncStep |= 1;
            
            CQ.CreateFromUrlAsync(urls[1].Key, 2,FinishRequest);
            Debug.WriteLine("Started Async Request 2 @" + DateTime.Now);
            AsyncStep |= 2;

            // Time out after 15 seconds
            CQ.WaitForAsyncEvents(15000);
            AsyncStep |=4;

            Debug.WriteLine("Finished Test @" + DateTime.Now);

            Assert.AreEqual(31, AsyncStep, "All async steps finished before exiting.");

        }

        [Test, TestMethod]
        public void GetHtmlAsyncPromises()
        {
            bool p1resolved = false;
            bool p2resolved = false;

            var promise1 = CQ.CreateFromUrlAsync(urls[0].Key)
                .Then(new Action<ICsqWebResponse>((resp)  => {
                    Assert.IsTrue(resp.Dom.Find(urls[0].Value).Length >0, "I found an expected content container - if MS changed their web site this could fail.");
                    p1resolved = true;
                }));

            var promise2 = CQ.CreateFromUrlAsync(urls[1].Key).Then(new Action<ICsqWebResponse>((resp) =>
            {
                Assert.IsTrue(resp.Dom.Find(urls[1].Value).Length >0, "I found an expected content container - if CNN changed their web site this could fail.");
                p2resolved = true;
            }));

            bool? complete = null;
            CQ.WhenAll(promise1,promise2).Then(new Action<ICsqWebResponse>((response) =>
            {
                complete = true;
                Assert.IsTrue(p1resolved, "Promise 1 is resolved");
                Assert.IsTrue(p2resolved, "Promise 1 is resolved");
                
            }), new Action(() =>
            {
                complete = false;
                Assert.Fail("The web requests were rejected.");
            }));

            // if we don't do this the test will exit before finishing
            CQ.WaitForAsyncEvents(10000);
            while (complete == null) ;
            Assert.IsTrue(complete==true);
        }

        [Test, TestMethod]
        public void GetHtmlAsyncPromisesFailed()
        {
            bool p1resolved = false;
            bool p2rejected = false;

            var promise1 = CQ.CreateFromUrlAsync(urls[0].Key)
                .Then(resp =>
                {
                    Assert.IsTrue(resp.Dom.Find(urls[0].Value).Length > 0, "I found an expected content container - if MS changed their web site this could fail.");
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

        //[Test, TestMethod]
        //public void GetCompressedResponseTest()
        //{
        //    CQ mCQ = CQ.CreateFromUrl("http://api.jquery.com/jQuery.contains/");
        //    Assert.IsTrue(mCQ.Select("#jq-footerNavigation").Length == 1, "I found an expected content container - if jQuery changed their web site or compressing method this could fail.");
        //}

        private void FinishRequest(ICsqWebResponse response)
        {
            var csq= response.Dom;
            if ((int)response.Id == 1)
            {
                AsyncStep |= 8;
                Assert.IsTrue(csq.Find(urls[0].Value).Length>0, "I found an expected content container - if Github changed their web site this could fail.");

            }
            if ((int)response.Id == 2)
            {
                AsyncStep |= 16;
                Assert.IsTrue(csq.Find(urls[1].Value).Length == 1, "I found an expected content container - if CNN changed their web site this could fail.");
            }
        
            Debug.WriteLine(String.Format("Received Async Response {0} @{1}", response.Id.ToString(),DateTime.Now));

        }

    }
}
