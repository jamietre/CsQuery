using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Reflection;
using Jtc.CsQuery;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = Drintl.Testing.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;

namespace CsqueryTests.Csharp
{
    
    [TestFixture, TestClass,Description("CsQuery Tests (Not from Jquery test suite)")]
    public class Clone: CsQueryTest
    {

        [SetUp]
        public override void Init()
        {
            string html = Support.GetFile("CsQueryTests\\Resources\\TestHtml.htm");
            Dom = CsQuery.Create(html);
        }
        [Test,TestMethod]
        public void SimpleClone()
        {
            CsQuery hlinks = Dom.Select("#hlinks-user");
            int spanCount = hlinks.Find("span").Length;
            CsQuery clone = hlinks.Clone();

            Assert.AreEqual(hlinks.Find("*").Length+1, clone.Select("*").Length,"Clone has same total elements as original");

            CsQuery newHome = Dom.Select("#hidden-div");
            
            spanCount = newHome.Find("span").Length;
            int cloneSpanCount = clone.Select("span").Length;

            Assert.AreEqual(1, newHome.Children().Length, "Sanity check - target has 1 child");
            newHome.Append(clone);
            Assert.AreEqual(2, newHome.Children().Length, "Target has 2 children after cloning");
            Assert.AreEqual(spanCount+cloneSpanCount, newHome.Find("span").Length, "Same # of spans in the clone");


        }

    }
}