using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Jtc.CsQuery;
using NUnit.Framework;

namespace CsqueryTests
{
    
    [TestFixture,Description("CsQuery Tests (Not from Jquery test suite)")]
    public class TCsQuery1: CsQueryTest
    {
 
        [SetUp]
        public void Init()
        {
            string html = Support.GetFile("Resources\\TestHtml.htm");
            Dom = CsQuery.Create(html);
        }
        [Test]
        public void GetElementById()
        {
            IDomElement el = document.GetElementById("reputation_link");

        }
        [Test]
        public void Find()
        {
            int spanCount = 0;
            CsQuery res = Dom.Find("span");
            foreach (DomElement obj in res)
            {
                spanCount++;
            }
            Assert.AreEqual(spanCount, 10, "Expected 10 spans");
            Assert.AreEqual(spanCount,res.Length,"Expected Length property to match element count");

            res = Dom.Find("hr");
            Assert.AreEqual(res.Length,1, "Expected one <hr> element");
        }
        [Test]
        public void AttributeEqualsSelector()
        {
            CsQuery res = Dom.Find("span[name=badge_span_bronze]");
            Assert.AreEqual(res[0].InnerHtml,"13", "InnerHtml of element id=badge_span_bronze did not match");

        }
        [Test]
        public void AttributeStartsWithSelector()
        {
            CsQuery res = Dom.Find("span[name^=badge_span]");
            Assert.AreEqual(res.Length, 2, "Expected two elements starting with badge_span");
        }
        
        [Test]
        public void CheckboxSelector()
        {
            List<DomElement> foundDetails = new List<DomElement>();
            CsQuery res = Dom.Find("input:checkbox");
            foreach (DomElement obj in res)
            {
                foundDetails.Add(obj);
            }
            Assert.AreEqual(res.Length,2, "Expected to find 2 checkbox elements");
        }
        [Test]
        public void CssSelector()
        {

            CsQuery res = Dom.Find(".badgecount");

            Assert.AreEqual(res.Length,2, "Expected 2 .badgecount items");
        }

        [Test]
        public void IDSelector()
        {
            var res = Dom.Find("#reputation_link");

            Assert.AreEqual(1, res.Length, "Found " + res.Length + " #reputation_link items");

                string inner = "";
                Dom.Find("#reputation_link").Find("span").Each((IDomElement e) =>
                {
                    inner = e.InnerHtml;
                });
                Assert.AreEqual("3,215", inner, "Found '" + inner + "' in span inside #reputation_link");
        }
        [Test]
        public void TextArea()
        {
            var res = jQuery("textarea");
            Assert.AreEqual(res.Text(), "Test textarea <div><span></div>");
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
    }
}