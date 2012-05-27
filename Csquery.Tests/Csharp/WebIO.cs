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

namespace CsqueryTests.Csharp
{
    /// <summary>
    /// This test is disabled by default because it accesses public web sites, activate it just to test this feature
    /// </summary>
    [TestFixture, TestClass, Description("CsQuery Tests (Not from Jquery test suite)")]
    public class WebIO : CsQueryTest
    {
        //[Test,TestMethod]
        public void GetHtml()
        {
            Server.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
            Dom = Server.CreateFromUrl("http://www.microsoft.com/en/us/default.aspx?redir=true");

            Assert.IsTrue(Dom.Document != null, "Dom was created");
            var csq = Dom.Find(".hpMst_Stage");
            Assert.IsTrue(csq.Length == 1, "I found an expected content container - if MS changed their web site this could fail.");
        }

        private static int AsyncStep = 0;
        [Test, TestMethod]
        public void GetHtmlAsync()
        {
            Server.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
            Server.StartAsyncWebRequest("http://www.microsoft.com/en/us/default.aspx?redir=true", FinishRequest, 1);
            Debug.WriteLine("Started Async Request 1 @" + DateTime.Now);
            AsyncStep |= 1;
            Server.StartAsyncWebRequest("http://www.cnn.com/", FinishRequest,2);
            Debug.WriteLine("Started Async Request 2 @" + DateTime.Now);
            AsyncStep |= 2;

            // Time out after 10 seconds
            Server.WaitForAsyncEvents(10000);
            AsyncStep |=4;

            Debug.WriteLine("Finished Test @" + DateTime.Now);

            Assert.AreEqual(31, AsyncStep, "All async steps finished before exiting.");

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
