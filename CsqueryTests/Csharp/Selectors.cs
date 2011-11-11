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

        [SetUp]
        public override void Init()
        {
            string html = Support.GetFile("CsQueryTests\\Resources\\TestHtml.htm");
            Dom = CsQuery.Create(html);
        }
        [Test,TestMethod]
        public void GetElementById()
        {
            IDomElement el = document.GetElementById("reputation_link");

        }

        [Test,TestMethod]
        public void Find()
        {
            int spanCount = 0;
            CsQuery res = Dom.Find("span");
            foreach (DomElement obj in res)
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
            Assert.AreEqual("13",res[0].InnerHtml, "InnerHtml of element id=badge_span_bronze did not match");

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
            List<DomElement> foundDetails = new List<DomElement>();
            CsQuery res = Dom.Find("input:checkbox");
            foreach (DomElement obj in res)
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
                    inner = e.InnerHtml;
                });
                Assert.AreEqual("3,215", inner, "Found '" + inner + "' in span inside #reputation_link");
        }
        [Test,TestMethod]
        public void TextArea()
        {
            var res = jQuery("textarea");
            Assert.AreEqual("Test textarea <div><span></div>",res.Text(),"Textare did not parse inner HTML");
        }

        [Test, TestMethod]
        public void Unwrap()
        {
            Init();
            int count = jQuery("#hlinks-user").Find("span").Length;
            var unwrapItem = jQuery("span[title='2 silver badges']");
            unwrapItem.Unwrap();

            Assert.AreEqual(count - 1, jQuery("#hlinks-user").Find("span").Length, "There's one less span now");
            Assert.IsTrue(jQuery(".badge2").Parent()[0] == jQuery("#hlinks-user")[0], "It's moved up");

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