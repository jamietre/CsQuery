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
using HttpWebAdapters;

namespace CsQuery.Tests.Core.WebIO
{
    /// <summary>
    /// This test is disabled by default because it accesses public web sites, activate it just to test this feature
    /// </summary>
    [TestFixture, TestClass]
    public class _WebIO_BasicWeb : CsQueryTest
    {

        [Test, TestMethod]
        public void GetHtmlTimeout()
        {

            var creator = new Mocks.MockWebRequestCreator();
            creator.ResponseTime = 1000;
            creator.ResponseStream = GetMemoryStream("<div>Test</div>");
            
            var request = new CsqWebRequest("http://test.com", creator);
            request.Options = new ServerConfig
            {
                Timeout = TimeSpan.FromMilliseconds(500),
                UserAgent = "test"
            };

            var httpRequest = request.GetWebRequest();

            IHttpWebResponse response;

            Assert.Throws<System.Net.WebException>(() =>
            {
                response = httpRequest.GetResponse();
            });

            request.Timeout = 1500;
            httpRequest = request.GetWebRequest();
            response = httpRequest.GetResponse();

            var responseStream = response.GetResponseStream();
            var encoding = CsqWebRequest.GetEncoding(response);

            var dom = CQ.CreateDocument(responseStream, encoding);


            Assert.AreEqual(1, dom["div"].Length);
        }

        [Test, TestMethod]
        public void GetHtmlAsyncTimeout()
        {

            var creator = new Mocks.MockWebRequestCreator();
            creator.ResponseTime = 1000;
            creator.ResponseStream = GetMemoryStream("<div>Test</div>");

            var request = new CsqWebRequest("http://test.com", creator);
            request.Options = new ServerConfig
            {
                Timeout = TimeSpan.FromMilliseconds(500),
                UserAgent = "test"
            };

            bool? done = null;

            request.GetAsync((r) =>
            {
                done = true;
            }, (r) =>
            {
                done = false;
            });

            while (done == null) ;
            Assert.IsFalse((bool)done);

            creator.ResponseTime = 300;
            request = new CsqWebRequest("http://test.com", creator);

            done = null;
            request.GetAsync((r) =>
            {
                done = true;
            }, (r) =>
            {
                done = false;
            });

            while (done == null) ;
            Assert.IsTrue((bool)done);


        }
    }
}
