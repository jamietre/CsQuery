using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jtc.CsQuery;
using Jtc.CsQuery.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;

namespace CsqueryTests.Csharp
{
    [TestFixture, TestClass, Description("CsQuery Tests (Not from Jquery test suite)")]
    public class WebIO : CsQueryTest
    {
        [Test,TestMethod]
        public void GetHtml()
        {
            Dom = CsQuery.Create();
            Dom.Server().UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
            Dom.Server().CreateFromUrl("http://www.microsoft.com/en/us/default.aspx?redir=true");
            Assert.IsTrue(Dom.Document != null, "Dom was created");
            var csq = Dom.Find(".hpMst_Stage");
            Assert.IsTrue(csq.Length == 1, "I found an expected content container - if MS changed their web site this could fail.");
        }
    }
}
