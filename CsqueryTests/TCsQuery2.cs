using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Jtc.CsQuery;
using NUnit.Framework;

namespace Jtc.CsQuery.Tests
{

    [TestFixture, Description("CsQuery Tests (Not from Jquery test suite)")]
    public class CsQueryTest2
    {
        protected CsQuery csq;
        [SetUp]
        public void Init()
        {
            string html = Support.GetFile("Resources\\TestHtml2.htm");
            csq = CsQuery.Create(html);
        }


        [Test]
        public void BasicDomCreation()
        {
            string tags = String.Empty;
            csq.Each(delegate(IDomElement e)
            {
                tags += (tags == "" ? "" : ",") + e.NodeName;
            });
            Assert.AreEqual(11, csq.Length, "Found correct number of elements in the DOM");
        }
        [Test]
        public void InputCheckbox()
        {
            string ids = String.Empty;
            var res = csq.Find("input:checkbox").Each(delegate(IDomElement e)
            {
                ids += (ids == "" ? "" : ",") + e.ID;
            });
            Assert.AreEqual(4, res.Length, "Test input:checkbox");
        }
        [Test]
        public void DomManipulation()
        {
            string ids = String.Empty;

            var res = csq.Find("li").Each((IDomElement e) =>
            {
                ids += (ids == "" ? "" : ",") + "'" + e.InnerHtml.Trim() + "'";
            });
            Assert.AreEqual(3,res.Length, "Find('li')");

            ids = String.Empty;

            res = csq.Find("li");
            res.Remove(res[1]);
            res = csq.Find("li").Each(delegate(IDomElement e)
            {
                ids += (ids == "" ? "" : ",") + "'" + e.InnerHtml.Trim() + "'";
            });
            Assert.AreEqual(1,res.Length, "Removed a list item (one was inner, should be one) list items");

            csq.Find("ul").Each((int index,IDomElement e) =>
            {
                if (index == 1)
                {
                    csq.Remove(e);
                }
            });
            res = csq.Find("ul");
            Assert.AreEqual(1, res.Length, "Removed a ul");

        }

        //    ids = String.Empty;

        //    res = csq.Find("li").Each(e=> {
        //        ids += (ids == "" ? "" : ",") + "'" + e.InnerHtml.Trim() + "'";
        //    });
        //    AddTestResult("test broken tags", res.Length == 3, "list items: " + ids);

        //    ids = String.Empty;

        //    res = csq.Find("li");
        //    res.Remove(res[1]);
        //    res = csq.Find("li").Each(delegate(DomElement e)
        //    {
        //        ids += (ids == "" ? "" : ",") + "'" + e.InnerHtml.Trim() + "'";
        //    });
        //    AddTestResult("Remove 2nd item", res.Length == 1, "(one was inner, should be one) list items: " + ids);

        //    csq.Find("ul").Each((index,e) => {
        //       if (index==1) {
        //           csq.Remove(e);
        //       }
        //    });
        //    res = csq.Find("ul");
        //    AddTestResult("Remove 2nd ul in each", res.Length == 1, "remaining UL:" + csq.Html() );

        //    ids="";
        //    res=csq.Find("input:checkbox,li").Each(
        //        delegate(DomElement e)
        //    {
        //        ids += (ids == "" ? "" : ",") + "'" + e.Tag + "'";
        //    });
        //    AddTestResult("Multiple select", res.Length == 5, "Found: " + ids);

        //    csq.Find("#last_div").Html("<span>Test</span>");
        //    res = csq.Find("#last_div");
        //    AddTestResult("Replace inner HTML",res[0].InnerHtml=="<span>Test</span>", res[0].InnerHtml);

        //    res.Append("<p>This is some more content</p>");
        //    res = csq.Find("#last_div");
        //    AddTestResult("Apppend", res[0].Children.Count() == 2 && res[0].InnerHtml == "<span>Test</span><p>This is some more content</p>", res[0].InnerHtml);


        //    res = csq.Find("#last_div").Children();

        //    AddTestResult("Children", res.Length == 2, "Children: " + GetChildTags(res));

        //    res.Append(delegate(int index, string oldHtml)
        //    {
        //        if (index == 0)
        //        {
        //            return "--Iteration 1";
        //        }
        //        else
        //        {
        //            return " <b>Iteration 2</b>";
        //        }
        //    });

        //    AddTestResult("Append (func)",
        //        res[0].InnerHtml == "Test--Iteration 1" && res[1].InnerHtml == "This is some more content <b>Iteration 2</b>",
        //        res[0].InnerHtml + "," + res[1].InnerHtml);

        //    // Start over now, the dom is so messed up it's impossible to know if things are working
        //    csq = CsQuery.Create(html);

        //    res = csq.Find("div:contains('Product')");

        //    AddTestResult(":contains", res.Length == 2, "Contains: found " + GetChildTags(res));


        //    res = csq.Find("#chk_utility_products_qualified").Attr("checked", String.Empty);

        //    AddTestResult("Set checkbox value", csq.Find("#chk_utility_products_qualified").Is(":checked")==true, "InnerHtml: " + res[0].Html );

        //    AddTestResult("Is to test checkbox", res.Is(":checked")==true, "Test with is");

        //    res.Attr("checked", false);
        //    AddTestResult("Set checkbox value false", res.Is(":checked") == false, "InnerHtml: " + res[0].Html);


        //    // Test Eq method

        //    res = csq.Find("div");
        //    AddTestResult("Eq pulled 2nd div", res.Eq(1).Attr("id") == "Div1", res.Eq(1).Render() );
        //    AddTestResult("First pulled 1st div", res.First().Attr("id") == "first_div", res[0].Html);


        //    AddTestResult("Next worked", res.Eq(0).Next().Attr("id") == "para", res[0].Html);
        //    AddTestResult("Prev worked", res.Eq(-2).Prev().Attr("id") == "para", res[0].Html);

        //}
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