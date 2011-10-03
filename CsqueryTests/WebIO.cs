using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Jtc.CsQuery;
using Jtc.CsQuery.Server;
namespace CsqueryTests
{
    [TestFixture, Description("CsQuery Tests (Not from Jquery test suite)")]
    public class WebIO : CsQueryTest
    {
        [Test]
        public void GetHtml()
        {
            Dom = new CsQuery();
            //Dom.Server().CreateFromUrl("http://www.outsharked.com");


           // Assert.IsTrue(Dom.Dom != null, "Dom was created");
        }
    }
}
