using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Reflection;
using Jtc.CsQuery;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;

namespace CsqueryTests.Csharp
{
    
    [TestFixture, TestClass,Description("CsQuery Tests (Not from Jquery test suite)")]
    public class Clone: CsQueryTest
    {
        const string testFile="CsQueryTests\\Resources\\TestHtml.htm";
        [SetUp]
        public override void Init()
        {
            string html = Support.GetFile(testFile);
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

        [Test, TestMethod]
        public void ChangingClones()
        {
            Init();
            var hlinks = Dom.Select("#hlinks-user");
            var cloneRoot = Dom["#test-show"].Append(hlinks.Clone());
            
            Assert.AreEqual("jamietre", hlinks.Find(".profile-link").Text(), "Sanity check: got the correct text");
            var profileLink =  cloneRoot.Find(".profile-link");
            Assert.AreEqual("jamietre",profileLink.Text(), "Clone had the correct text");
            profileLink.Text("ChangedMyName");
            Assert.AreEqual("ChangedMyName", profileLink.Text(), "Clone had the correct text after change");
            Assert.AreEqual("jamietre", hlinks.Find(".profile-link").Text(), "Original still had the correct text");

            profileLink.Append("<h2>A Header</h2><div class=\"newstuff\"><span>I'm new</span></div>");
            Assert.AreEqual(2, profileLink.Children().Length, "Added to my clone");
            Assert.AreEqual("ChangedMyName A Header I'm new", profileLink.Text(), "Text was obtained correctly from the clone");
            Assert.AreEqual("jamietre", hlinks.Find(".profile-link").Text(), "Original still had the correct text");

        }
        [Test, TestMethod]
        public void InsertingContent()
        {
            Init();
            // The total # of ele
            var totalCount = Dom["*"].Length;

            string newHtml = Support.GetFile(testFile);
            //change it slightly to force the string refs to be offset

            newHtml = newHtml.Insert(newHtml.IndexOf("<body>")+6,"<div id=\"new-div\">Johnny Come Lately</div>");
            var dom2 = CsQuery.Create(newHtml);

            Assert.AreEqual(totalCount+1, dom2["*"].Length, "The new copy was the same as the original");

            var addingCount = dom2["body *"].Length;
            dom2["body"].Children().InsertBefore(Dom["#hlinks-user"].Children().First());
            Assert.AreEqual(totalCount + addingCount, Dom["*"].Length, "The combined DOM is the right length");

            Dom["#hlinks-user .reputation-score"].Clone().AppendTo("body");
            Assert.AreEqual("3,215", Dom[".reputation-score"].First().Text(), "Text didn't get mangled on multiple moves (bug 11/11/11)");
            Assert.AreEqual("3,215", Dom[".reputation-score"].Last().Text(), "Text didn't get mangled on multiple moves (bug 11/11/11)");


        }


    }
}