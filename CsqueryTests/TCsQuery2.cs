using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Jtc.CsQuery;
using NUnit.Framework;

namespace CsqueryTests
{

    [TestFixture, Description("CsQuery Tests (Not from Jquery test suite)")]
    public class TCsQuery2
    {
        protected CsQuery csq;
        [SetUp]
        public void Init()
        {
            Initialize();
        }

        protected void Initialize()
        {
            string html = Support.GetFile("Resources\\TestHtml2.htm");
            csq = CsQuery.Create(html);
        }
        [Test]
        public void InnerParsingRules()
        {
            CsQuery res = csq.Select("script");
            // bug found 10/31/11
            Assert.AreEqual(0, res.Children().Length, "Script cannot have children");
        }
        [Test]
        public void BasicDomCreation()
        {
            string tags = String.Empty;
            csq.Each(delegate(IDomElement e)
            {
                tags += (tags == "" ? "" : ",") + e.NodeName;
            });
            Assert.AreEqual(12, csq.Length, "Found correct number of elements in the DOM");
        }
        [Test]
        public void InputCheckbox()
        {
            string ids = String.Empty;
            var res = csq.Select("input:checkbox").Each(delegate(IDomElement e)
            {
                ids += (ids == "" ? "" : ",") + e.ID;
            });
            Assert.AreEqual(4, res.Length, "Test input:checkbox");
        }
        [Test]
        public void DomManipulation()
        {
            string ids = String.Empty;

            var res = csq.Select("li").Each((IDomElement e) =>
            {
                ids += (ids == "" ? "" : ",") + "'" + e.InnerHtml.Trim() + "'";
            });
            Assert.AreEqual(3, res.Length, "Find('li')");

            ids = String.Empty;

            res = csq.Select("li");
            res.Eq(1).Remove();

            Assert.AreEqual(1, csq.Select("li").Length, "Removed a list item (one was inner, should be one) list items");

            csq.Select("ul").Each((int index, IDomElement e) =>
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
        [Test]
        public void ComplexSelectors()
        {
            Initialize();
            var res = csq.Select("input:checkbox,li");
            Assert.AreEqual(7,res.Length,"Multiple select (two queries appended)");

        }
        [Test]
        public void DomManipulation2()
        {
            csq.Select("#last_div").Html("<span>Test</span>");
            var res = csq.Select("#last_div");
            Assert.AreEqual(res[0].InnerHtml, "<span>Test</span>", "Replace inner HTML");

            res.Append("<p>This is some more content</p>");
            res = csq.Select("#last_div");
            Assert.AreEqual(2, res[0].ChildNodes.Count(), "Test results of appending content (verify node length)");
            Assert.AreEqual("<span>Test</span><p>This is some more content</p>", res[0].InnerHtml, "Test results of appending content (verify actual content)");

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


            Assert.AreEqual("Test--Iteration 1", res[0].InnerHtml,
                "Modifying contents during a delegate in Append (index 00");
            Assert.AreEqual("This is some more content <b>Iteration 2</b>", res[1].InnerHtml,
                "Modifying contents during a delegate in Append (index 1)");
        }

        [Test]
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
        protected string GetChildTags(CsQuery csq)
        {
            string tags = "";
            csq.Each(delegate(IDomElement e)
            {
                tags += (tags == "" ? "" : ",") + "'" + e.NodeName + "'";
            });
            return tags;
        }
        public void Test_CSQuery_Dom()
        {
            //SetTestInfo("CsQuery.DomObject","Parse");

            DomElementFactory factory = new DomElementFactory();
            string html = " <b>Iteration 2</b>";
            DomRoot container = new DomRoot(factory.CreateObjects(html));
            //AddTestResult("Simple create", container.Html==html, container.Html);


            html = "<div><input type=\"checkbox\" checked=\"checked\" customtag /><br /><span>more inner text</span></div>";


            IDomElement obj = factory.CreateElement(html);
            string reParsed = WriteDOMObkect(obj);
            string result = "tag=" + obj.NodeName + ", " + reParsed;

            ///AddTestResult("SimpleParseTest", html== reParsed , result);

        }
        protected string WriteDOMObkect(IDomElement obj)
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
            result += "InnerHtml=" + obj.InnerHtml;
            return result;
        }
        //public override void RunAll()
        //{
        //    Test_CsQuery();
        //    Test_CSQuery_Dom();
        //}
    }
}