using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.HtmlParser;
using CsQuery.Implementation;
using CsQuery.Utility;

namespace CsqueryTests.CSharp
{

    [TestFixture, TestClass, Description("CsQuery Tests (Not from Jquery test suite)")]
    public class DomManipulation
    {
        private static CQ csq;
        [TestFixtureSetUp,ClassInitialize]
        public static void Init(TestContext context)
        {
            Initialize();
        }

        private static void Initialize()
        {
            string html = Support.GetFile("CsQuery\\CsQuery.Tests\\Resources\\TestHtml2.htm");
            csq = CQ.Create(html);
            //RangeSortedDictionary<IDomObject> test = ((IDomRoot)csq.Document).SelectorXref;
            //foreach (var item in test)
            //{
            //    ;
            //}
        }
        [TestInitialize, SetUp]
        public void TestSetUp()
        {
            Initialize();
        }
        [Test, TestMethod]
        public void UnWrap()
        {

        }
        [Test,TestMethod]
        public void InnerParsingRules()
        {
            CQ res = csq.Select("script");
            // bug found 10/31/11
            Assert.AreEqual(0, res.Children().Length, "Script cannot have children");
        }
        [Test,TestMethod]
        public void BasicDomCreation()
        {
            string tags = String.Empty;
            csq.Each(delegate(IDomObject e)
            {
                tags += (tags == "" ? "" : ",") + e.NodeName;
            });
            Assert.AreEqual(12, csq.Length, "Found correct number of elements in the DOM");
        }
        [Test,TestMethod]
        public void InputCheckbox()
        {
            string ids = String.Empty;
            var res = csq.Select("input:checkbox").Each(delegate(IDomObject e)
            {
                ids += (ids == "" ? "" : ",") + e.Id;
            });
            Assert.AreEqual(4, res.Length, "Test input:checkbox");
        }
        [Test,TestMethod]
        public void DomManipulationTests()
        {
            string ids = String.Empty;

            var res = csq.Select("li").Each((IDomObject e) =>
            {
                ids += (ids == "" ? "" : ",") + "'" + e.InnerHTML.Trim() + "'";
            });
            Assert.AreEqual(3, res.Length, "Find('li')");

            ids = String.Empty;

            res = csq.Select("li");
            res.Eq(1).Remove();

            Assert.AreEqual(1, csq.Select("li").Length, "Removed a list item (one was inner, should be one) list items");

            csq.Select("ul").Each((int index, IDomObject e) =>
            {
                if (index == 1)
                {

                    csq.Remove(":eq(1)");
                }
            });
            res = csq.Select("ul");
            Assert.AreEqual(1, res.Length, "Removed a ul");

            
            ids = String.Empty;

            res = csq.Select("li");
            res.Remove();
            Assert.AreEqual(1, res.Length, "Removing an item leaves it in the selection set");
            Assert.AreEqual(0, csq.Select("li").Length, "Test removing last list item");


        }
        [Test,TestMethod]
        public void ComplexSelectors()
        {
            Initialize();
            var res = csq.Select("input:checkbox,li");
            Assert.AreEqual(7,res.Length,"Multiple select (two queries appended)");

        }
        [Test,TestMethod]
        public void DomManipulation2()
        {
            csq.Select("#last_div").Html("<span>Test</span>");
            var res = csq.Select("#last_div");
            Assert.AreEqual(res[0].InnerHTML, "<span>Test</span>", "Replace inner HTML");

            res.Append("<p>This is some more content</p>");
            res = csq.Select("#last_div");
            Assert.AreEqual(2, res[0].ChildNodes.Count(), "Test results of appending content (verify node length)");
            Assert.AreEqual("<span>Test</span><p>This is some more content</p>", res[0].InnerHTML, "Test results of appending content (verify actual content)");

            res = csq.Select("#last_div").Children();

            Assert.AreEqual(2,res.Length,"Inspect children of last div");

            res.Append(delegate(int index, string oldHtml)
            {
                if (index == 0)
                {
                    return "--Iteration 1";
                }
                else
                {
                    return " <b>Iteration 2</b>";
                }
            });


            Assert.AreEqual("Test--Iteration 1", res[0].InnerHTML,
                "Modifying contents during a delegate in Append (index 00");
            Assert.AreEqual("This is some more content <b>Iteration 2</b>", res[1].InnerHTML,
                "Modifying contents during a delegate in Append (index 1)");
        }

        [Test,TestMethod]
        public void MoreDomManipulation()
        {
            // Start over now, the dom is so messed up it's impossible to know if things are working
            Initialize();

            var res = csq.Select("div:contains('Product')");

            
            Assert.AreEqual(2, res.Length, "Contains: found " + GetChildTags(res));


            res = csq.Select("#chk_utility_products_qualified").Attr("checked", String.Empty);

            Assert.AreEqual(true,csq.Find("#chk_utility_products_qualified").Is(":checked"), "Checking checkbox using attr");

            
            res.Attr("checked", false);
            Assert.AreEqual(false,res.Is(":checked") ,"Set checkbox false");


            // Test Eq method

            res = csq.Select("div");
            Assert.AreEqual("Div1",res.Eq(1).Attr("id"), "Test eq method");
            Assert.AreEqual("first_div",res.First().Attr("id"), "test first method");


            Assert.AreEqual("para",res.Eq(0).Next().Attr("id"), "test next method");
            Assert.AreEqual("para",res.Eq(-1).Prev().Attr("id"),"Test eq with negative parm, and prev");
        }

        /// <summary>
        /// Added 12/15/11 - ensure that appending to a table element attds to a tbody tag (or creates one)
        /// </summary>
        [Test, TestMethod]
        public void AppendToTable()
        {
            var csq = CQ.Create("<div><table></table></div>").Find("table");

            csq.Append("<tr />");

            Assert.AreEqual("tbody", csq.Select("table :first-child")[0].NodeName, "Adding a span to an empty table created tbody");
            Assert.AreEqual(2, csq.Select("table *").Length, "Two total elements in the table");

            csq.Append("<b>yo</b>");

            var nodeNames =  csq.Select("table tbody > *").Elements.Select(item => item.NodeName);
            Assert.AreEqual(Objects.Enumerate("tr","b"),nodeNames, 
                "Adding another element was within the now-existing tbody");

            Assert.AreEqual(3, csq.Select("table *").Length, "3 total elements in the table");

            // now get rid of the tbody
            csq.Find("tr").Unwrap();

            Assert.AreEqual(2, csq.Select("table *").Length, "2 total elements in the table after unwrapping (sanity check)");

            csq.Append("<p>Text</p>");

            nodeNames = csq.Select("table > *").Elements.Select(item => item.NodeName);
            Assert.AreEqual(Objects.Enumerate("tr", "b", "p"),
               nodeNames,
               "Adding another element was within the now-existing tbody");
            Assert.AreEqual(0, csq.Find("tbody").Length, "No tbody after another append.");
        }


        #region private methods
        protected string GetChildTags(CQ csq)
        {
            string tags = "";
            csq.Each(delegate(IDomObject e)
            {
                tags += (tags == "" ? "" : ",") + "'" + e.NodeName + "'";
            });
            return tags;
        }
        protected string WriteDOMObject(IDomElement obj)
        {
            string result = "";
            foreach (var kvp in obj.Attributes)
            {
                if (kvp.Value != null)
                {
                    result += kvp.Key + "=" + kvp.Value + ",";
                }
                else
                {
                    result += kvp.Value + ",";
                }
            }
            result += "InnerHtml=" + obj.InnerHTML;
            return result;
        }
        #endregion
    }
}