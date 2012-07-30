using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;

namespace CsQuery.Tests.Csharp.Selectors
{
    
    [TestFixture, TestClass]
    public class AttributeSelectors: CsQueryTest
    {
        
        public override void FixtureSetUp()
        {
            base.FixtureSetUp();
            Dom = TestDom("TestHtml2");
        }

        [Test,TestMethod]
        public void StartsWithOrHyphen()
        {
            
            CQ res = Dom.Select("[lang|=en]");
            Assert.AreEqual(2, res.Length);

            res = Dom.Select("[lang|=en-uk]");
            Assert.AreEqual(1, res.Length);
        }

        [Test, TestMethod]
        public void ContainsWord()
        {
            var dom = CQ.Create(@"<div data-val='quick brown' id='1'></div>
                <div data-val='quick brown fox jumps' id='2'></div>
                <div data-val id='3'></div>");

            Assert.AreEqual(Arrays.String("1", "2"), dom["[data-val~=brown]"].Select(item=>item.Id));
            Assert.AreEqual(Arrays.String("2"), dom["[data-val~=fox]"].Select(item => item.Id));
            Assert.AreEqual(0, dom["[data-val~=lazy]"].Length);

        }

    }
}