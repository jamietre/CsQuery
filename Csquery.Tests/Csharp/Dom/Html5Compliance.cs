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
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.HtmlParser;
using CsQuery.Utility;

namespace CsqueryTests.Csharp
{

    [TestFixture, TestClass]
    public class HTML5Compliance : CsQueryTest
    {

        [Test, TestMethod]
        public void TabsInClassNames()
        {
            string html = "<html><body><div class=\"class1\tclass2\"></div></body></html>";
            var dom = CQ.Create(html);

            var div = dom["div"].FirstElement();
            Assert.AreEqual(2, div.Classes.Count());
            Assert.IsTrue(div.HasClass("class1"));
            Assert.IsTrue(div.HasClass("class2"));
            
 
        }

        [Test, TestMethod]
        public void NewLinesInClassNames()
        {
            var html = "<html><body><div class=\"class1" + System.Environment.NewLine + "class2  class3\r\n\t class4\"></div></body></html>";
            var dom = CQ.Create(html);

            var div = dom["div"].FirstElement();
            Assert.AreEqual(4, div.Classes.Count());
            Assert.IsTrue(div.HasClass("class1"));
            Assert.IsTrue(div.HasClass("class4"));
        }
    }

    
}