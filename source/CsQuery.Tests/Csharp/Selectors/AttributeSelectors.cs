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


    }
}