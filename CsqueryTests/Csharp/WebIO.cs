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
        [Test]
        public void GetHtml()
        {
            Dom = new CsQuery();
            //Dom.Server().CreateFromUrl("http://www.outsharked.com");


           // Assert.IsTrue(Dom.Dom != null, "Dom was created");
        }
    }
}
