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
    public class Selectors: CsQueryTest
    {
        
        public override void FixtureSetUp()
        {
            string html = Support.GetFile("CsQueryTests\\Resources\\TestHtml.htm");
            Dom = CsQuery.Create(html);
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

        [Test,TestMethod]
        public void Find()
        {
            int spanCount = 0;
            CsQuery res = Dom.Find("span");
            foreach (IDomElement obj in res)
            {
                spanCount++;
            }
            Assert.AreEqual(12,spanCount, "Expected 10 spans");
            Assert.AreEqual(spanCount,res.Length,"Expected Length property to match element count");

            res = Dom.Find("hr");
            Assert.AreEqual(1,res.Length, "Expected one <hr> element");
        }
        [Test,TestMethod]
        public void AttributeEqualsSelector()
        {
            CsQuery res = Dom.Find("span[name=badge_span_bronze]");
            Assert.AreEqual("13",res[0].InnerHTML, "InnerHtml of element id=badge_span_bronze did not match");

        }
        [Test,TestMethod]
        public void AttributeStartsWithSelector()
        {
            CsQuery res = Dom.Find("span[name^=badge_span]");
            Assert.AreEqual(2,res.Length,  "Expected two elements starting with badge_span");
        }
        
        [Test,TestMethod]
        public void CheckboxSelector()
        {
            List<IDomElement> foundDetails = new List<IDomElement>();
            CsQuery res = Dom.Find("input:checkbox");
            foreach (IDomElement obj in res)
            {
                foundDetails.Add(obj);
            }
            Assert.AreEqual(2, res.Length, "Expected to find 2 checkbox elements");
        }
        [Test,TestMethod]
        public void CssSelector()
        {

            CsQuery res = Dom.Find(".badgecount");

            Assert.AreEqual(2, res.Length, "Expected 2 .badgecount items");
        }

        [Test,TestMethod]
        public void IDSelector()
        {
            var res = Dom.Find("#reputation_link");

            Assert.AreEqual(1, res.Length, "Found " + res.Length + " #reputation_link items");

                string inner = "";
                Dom.Find("#reputation_link").Find("span").Each((IDomElement e) =>
                {
                    inner = e.InnerHTML;
                });
                Assert.AreEqual("3,215", inner, "Found '" + inner + "' in span inside #reputation_link");
        }
        [Test,TestMethod]
        public void TextArea()
        {
            var res = jQuery("textarea");
            Assert.AreEqual("Test textarea <div><span></div>",res.Text(),"Textarea did not parse inner HTML");
        }

        [Test, TestMethod]
        public void Siblings()
        {
            var res = jQuery("span[title='13 bronze badges']");
            var childCount = res.Parent().Children().Length;
            Assert.AreEqual(childCount-1, res.Siblings().Length, "Sibling count correct");

            res= res.Add(jQuery("span[title='2 silver badges']"));
            Assert.AreEqual(2, res.Length, "Result set has 2 members");
            Assert.AreEqual(res[0].ParentNode ,res[1].ParentNode, "The two members are in fact siblings");
            Assert.AreEqual(childCount, res.Siblings().Length, "Siblings includes both members of the set when more than one sibling included");

            List<IDomObject> correctList = new List<IDomObject>(jQuery("#hlinks-user").Children());
             
            Assert.AreEqual(correctList,new List<IDomObject>(res.Siblings()),"The child list is identical to the sibling list");
        }
        [Test, TestMethod]
        public void PseudoSelectors()
        {
            var res = jQuery("body > [id] > :last-child"); // two elements with IDs
            Assert.AreEqual(q("textarea", "test-show-last"), res, "last-child with child works");

            int count = res["[id]"].Length;
            int bodyChildIDCount = res["body [id] > :last-child[id]"].Length;

            res = res["#textarea"].Attr("id", null);
            Assert.AreEqual(res["[id]"].Length + 1, count, "One less ID");
            Assert.AreEqual(bodyChildIDCount-1, res["body > [id]"].Length, "Setting ID to null removed it from the index too");

            res[0].Id = "";
            Assert.AreEqual(bodyChildIDCount, res["body [id] > :last-child[id]"].Length, "Setting ID to empty string added it back");
            res.Attr("id", null);
            res.Attr("id", "newid");
            Assert.AreEqual(res["body [id='newid']"][0],res[0], "Further messing with ID and everything is fine");

            var para = jQuery("p:first");
            para[0].ClassName = "some classes";
            Assert.AreEqual(jQuery("p[class]")[0],para[0],"Selected by class attribute");
        }
        [Test, TestMethod]
        public void NthChild()
        {
            var res = jQuery("body > :nth-child(2)");
            Assert.AreEqual(jQuery("body").Children().Eq(1)[0], res[0], "nth-child(x) works");

            res = jQuery("body > :nth-child(2n)");
            Assert.AreEqual(jQuery("body").Children(":odd").Elements,res.Elements,"Simple math nth child workd");

            res = jQuery("body > :nth-child(2n+1)");
            Assert.AreEqual(jQuery("body").Children(":even").Elements, res.Elements, "Simple math nth child workd");

        }
    }
}