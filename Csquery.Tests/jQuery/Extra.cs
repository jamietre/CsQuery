using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute;
using MsTestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using TestContext = NUnit.Framework.TestContext;
using CsQuery;
using CsQuery.Utility;

namespace CsQuery.Tests.jQuery
{
    /// <summary>
    /// Tests added to address specific bug reports
    /// </summary>
    [TestClass,TestFixture, Category("Core")]
    public class Extra: CsQueryTest 
    {

        [SetUp]
        public override void FixtureSetUp()
        {
            base.FixtureSetUp();
            Dom = TestDom("wiki-cheese");            
        }


       /// <summary>
       /// Bug #1 5/7/2012
       /// </summary>
       [Test,TestMethod]
       public void Header()
       {
           CQ headers = Dom["h1"];

           Assert.AreEqual(1, headers.Length,"Could select headers");

       }
        
    }
}
