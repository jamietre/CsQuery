using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CsQuery;
using CsQuery.Web;
using CsQuery.Utility;
using CsQuery.ExtensionMethods;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;


namespace CsqueryTests.Csharp
{
    [TestFixture, TestClass, Description("CsQuery Tests (Not from Jquery test suite)")]
    public class Server_ : CsQueryTest
    {

        protected void ResetQunit()
        {
            Dom = CQ.Create(Support.GetFile("csquery\\csquery.tests\\resources\\jquery-unit-index.htm"));
        }
        [Test, TestMethod]
        public void RestorePost_()
        {
            ResetQunit();


        }
        

    }
}
