using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using CollectionAssert = NUnit.Framework.CollectionAssert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;

namespace CsQuery.Tests.Miscellaneous
{
    /// <summary>
    /// Trying to reproduce a problem during dom manipulation that comes up when removing text nodes.
    /// </summary>

    [TestFixture, TestClass]
    public class Reindex: CsQueryTest
    {

        /// <summary>
        /// 
        /// </summary>
        [Test, TestMethod]
        public void ReindexTest()
        {
            var test = CQ.CreateDocument(@"
<div>abcde<a href='#'>def</a>ghi<b>kjm<div>fgh</div></b>opq</div>
");
            var toRemove = test["body>div"][0].ChildNodes.ToList();
            foreach (var el in toRemove)
            {
                el.Remove();
            }
            Assert.AreEqual(0, test["body>div"][0].ChildNodes.Count);
        }

    
        #region setup
        public override void FixtureSetUp()
        {
            base.FixtureSetUp();
            Dom = TestDom("TestHtml");
        }
        #endregion

    }

}