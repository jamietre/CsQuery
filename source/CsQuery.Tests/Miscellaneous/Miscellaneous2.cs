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

        /// <summary>
        /// Ensure we can have really deep nesting. Things start to get really slow after about 10,000.
        /// </summary>

        [Test, TestMethod]
        public void DeepNesting()
        {
            string html = "";
            string htmlEnd = "";
#if SLOW_TESTS
            int depth = 15000;
#else 
            int depth = 4000;
#endif

            for (int i = 0; i < depth; i++)
            {
                html += "<div>text";
                htmlEnd += "</div>";
            }
            html = html + htmlEnd;

            var dom = CQ.Create(html);

            Assert.AreEqual(depth, dom["div"].Length);


            var htmlOut = dom.Render();
            Assert.AreEqual(html, htmlOut);
        }

        [Test, TestMethod]
        public void Issue66()
        {
            var dom = CQ.Create(@"
<div>bar match</div>
<div name='size'>
	<div class='matched'>no match</div>
	<div class='matched'>no match 2</div>
	<div>foo<div>bar match</div>
	</div>
	<div>bar another match</div>
</div>");
            
            var query = dom["div[name~=size] :not(*:contains('bar'))"];
            Assert.AreEqual(2, query.Length);
            CollectionAssert.AreEqual(dom[".matched"], query);

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