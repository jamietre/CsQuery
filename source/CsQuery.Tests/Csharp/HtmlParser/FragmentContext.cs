using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using CollectionAssert = NUnit.Framework.CollectionAssert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.HtmlParser;
using CsQuery.Utility;

namespace CsQuery.Tests.Csharp.HtmlParser
{

    [TestFixture, TestClass]
    public class FragmentContext : CsQueryTest
    {
        [Test, TestMethod]
        public void Table()
        {
            var dom = CQ.CreateFragment(@"<tr><td>test</td></tr>");
            Assert.AreEqual("<tr><td>test</td></tr>", dom.Render());

        }

        [Test, TestMethod]
        public void TableCell()
        {
            var dom = CQ.CreateFragment(@"<td>test</td>");
            Assert.AreEqual("<td>test</td>", dom.Render());

        }
    }
}
