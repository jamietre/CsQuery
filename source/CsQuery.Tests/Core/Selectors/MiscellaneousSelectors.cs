using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using CollectionAssert = NUnit.Framework.CollectionAssert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;

namespace CsQuery.Tests.Core.Selectors
{
    
    [TestFixture, TestClass]
    public class MiscellanousSelectors: CsQueryTest
    {
        
      
        [Test,TestMethod]
        public void EverythingButFistLast()
         {
             var res = Dom["#hlinks-user > :not(:first-child,:last-child)"];
             IEnumerable<IDomElement> res2 = Dom["#hlinks-user"].Children().Not(":first-child,:last-child").Elements;
             
            CollectionAssert.AreEqual(res, res2);

            var hlinks = Dom["#hlinks-user"][0];
            var lastEl = hlinks.LastElementChild;

           res2 = hlinks.ChildElements.Skip(1).TakeWhile(item => item != lastEl);

           CollectionAssert.AreEqual(res, res2);

         }
       
        [Test,TestMethod]
        public void Find()
        {
            int spanCount = 0;
            CQ res = Dom.Find("span");
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
            CQ res = Dom.Find("span[name=badge_span_bronze]");
            Assert.AreEqual("13",res[0].InnerHTML, "InnerHtml of element id=badge_span_bronze did not match");
        }

        [Test,TestMethod]
        public void AttributeStartsWithSelector()
        {
            CQ res = Dom.Find("span[name^=badge_span]");
            Assert.AreEqual(2,res.Length,  "Expected two elements starting with badge_span");
        }
        
       
        [Test,TestMethod]
        public void CssSelector()
        {

            CQ res = Dom.Find(".badgecount");

            Assert.AreEqual(2, res.Length, "Expected 2 .badgecount items");
        }

        [Test,TestMethod]
        public void IDSelector()
        {
            var res = Dom["#reputation_link"];

            Assert.AreEqual(1, res.Length, "Found " + res.Length + " #reputation_link items");

                string inner = "";
                Dom.Find("#reputation_link").Find("span").Each((IDomObject e) =>
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
        public void ClassAttribute()
        {

            var para = Dom["p:first"];
            para[0].ClassName = "some classes";
            Assert.AreEqual(jQuery("p[class]")[0], para[0], "Selected by class attribute");
        }

        public override void FixtureSetUp()
        {
            base.FixtureSetUp();
            Dom = TestDom("TestHtml");
        }

		[Test, TestMethod]
		public void HexWith6DigitsEscapedSelector()
		{
			string html = "<div id='B&W?'><p>test</p></div>";
			var dom = CsQuery.CQ.CreateFragment(html);
			var result = dom.Select("#B\\000026W\\3F");
			Assert.Greater(result.Length, 0);
		}
		
		[Test, TestMethod]
		public void HexWith2DigitsEscapedSelector()
		{
			string html = "<div id=\"B&W?\"><p>test</p></div>";
			var dom = CsQuery.CQ.CreateFragment(html);
			var result = dom.Select("#B\\26 W\\3F");
			Assert.Greater(result.Length, 0);
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
    }
}