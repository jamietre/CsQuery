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

namespace CsQuery.Tests.Core.Manipulation
{

    [TestFixture, TestClass]
    public class DomManipulation: CsQueryTest
    {
        

        public override void FixtureSetUp()
        {
            Dom = TestDom("TestHtml2");
        }
        [Test,TestMethod]
        public void InnerParsingRules()
        {
            CQ res = Dom.Select("script");
            // bug found 10/31/11
            Assert.AreEqual(0, res.Children().Length, "Script cannot have children");
        }

        [Test,TestMethod]
        public void BasicDomCreation()
        {
            // TODO this test doesn't exactly match Chrome. The mismatched closing <p> tag before "first_div" is treated as an empty <p></p> block
            // by chrome. This is probably a part of the spec we don't yet deal with.

            // changed from 30 to 31 when using validator.nu parser (chrome=31, not sure what missing el was before)
            Assert.AreEqual(31, Dom["*"].Length, "Found correct number of elements in the DOM");
            Assert.AreEqual(27, Dom["body *"].Length, "Found correct number of elements in the DOM");
            Assert.AreEqual(9, Dom["body > *"].Length, "Found correct number of elements in the DOM");
        }
        [Test,TestMethod]
        public void InputCheckbox()
        {
            string ids = String.Empty;
            var res = Dom.Select("input:checkbox").Each(delegate(IDomObject e)
            {
                ids += (ids == "" ? "" : ",") + e.Id;
            });
            Assert.AreEqual(4, res.Length, "Test input:checkbox");
        }

        [Test,TestMethod]
        public void DomManipulationTests()
        {
            var dom = TestDom("TestHtml2");

            string ids = String.Empty;

            var res = dom.Select("li").Each((IDomObject e) =>
            {
                ids += (ids == "" ? "" : ",") + "'" + e.InnerHTML.Trim() + "'";
            });
            Assert.AreEqual(3, res.Length, "Find('li')");

            ids = String.Empty;

            res = dom.Select("li");
            res.Eq(1).Remove();

            Assert.AreEqual(1, dom.Select("li").Length, "Removed a list item (one was inner, should be one) list items");

            dom.Select("ul").Each((int index, IDomObject e) =>
            {
                if (index == 1)
                {

                    dom.Remove(":eq(1)");
                }
            });
            res = dom.Select("ul");
            Assert.AreEqual(1, res.Length, "Removed a ul");

            
            ids = String.Empty;

            res = dom.Select("li");
            res.Remove();
            Assert.AreEqual(1, res.Length, "Removing an item leaves it in the selection set");
            Assert.AreEqual(0, dom.Select("li").Length, "Test removing last list item");


        }
        [Test,TestMethod]
        public void ComplexSelectors()
        {
            
            var res = Dom.Select("input:checkbox,li");
            Assert.AreEqual(7,res.Length,"Multiple select (two queries appended)");

        }
        [Test,TestMethod]
        public void DomManipulation2()
        {
            var dom = TestDom("TestHtml2");
            dom.Select("#last_div").Html("<span>Test</span>");
            var res = dom.Select("#last_div");
            Assert.AreEqual(res[0].InnerHTML, "<span>Test</span>", "Replace inner HTML");

            res.Append("<p>This is some more content</p>");
            res = dom.Select("#last_div");
            Assert.AreEqual(2, res[0].ChildNodes.Count(), "Test results of appending content (verify node length)");
            Assert.AreEqual("<span>Test</span><p>This is some more content</p>", res[0].InnerHTML, "Test results of appending content (verify actual content)");

            res = dom.Select("#last_div").Children();

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
            var dom = TestDom("TestHtml2");
            var res = dom.Select("div:contains('Product')");

            
            Assert.AreEqual(2, res.Length, "Contains: found " + GetChildTags(res));


            res = dom.Select("#chk_utility_products_qualified").Attr("checked", String.Empty);

            Assert.AreEqual(true, dom.Find("#chk_utility_products_qualified").Is(":checked"), "Checking checkbox using attr");

            
            res.Attr("checked", false);
            Assert.AreEqual(false,res.Is(":checked") ,"Set checkbox false");


            // Test Eq method

            res = dom.Select("div");
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

            Assert.AreEqual("TBODY", csq.Select("table :first-child")[0].NodeName, "Adding a span to an empty table created tbody");
            Assert.AreEqual(2, csq.Select("table *").Length, "Two total elements in the table");

            csq.Append("<b>yo</b>");

            var nodeNames =  csq.Select("table tbody > *").Elements.Select(item => item.NodeName);
            Assert.AreEqual(Objects.Enumerate("TR","B"),nodeNames, 
                "Adding another element was within the now-existing tbody");

            Assert.AreEqual(3, csq.Select("table *").Length, "3 total elements in the table");

            // now get rid of the tbody
            csq.Find("tr").Unwrap();

            Assert.AreEqual(2, csq.Select("table *").Length, "2 total elements in the table after unwrapping (sanity check)");

            csq.Append("<p>Text</p>");

            nodeNames = csq.Select("table > *").Elements.Select(item => item.NodeName);
            Assert.AreEqual(Objects.Enumerate("TR", "B", "P"),
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