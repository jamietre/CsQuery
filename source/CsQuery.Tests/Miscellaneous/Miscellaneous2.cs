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
using System.Net;

namespace CsQuery.Tests.Miscellaneous
{
    /// <summary>
    /// Trying to reproduce a problem during dom manipulation that comes up when removing text nodes.
    /// </summary>

    [TestFixture, TestClass]
    public class Reindex: CsQueryTest
    {
        [Test, TestMethod]
        public void InnerText()
        {
            var dom = CQ.Create("<p><b>inner text</b>innertext2</p>");

            Assert.AreEqual(dom["p"].Text(), dom["p"][0].InnerText);
        }
//        {
//            string content1 = null;
//            CQ dom;
//            bool done = false;

//            CQ.CreateFromUrlAsync("http://www.ahram.org.eg/Stars-Arts/News/194972.aspx")
//                .Then(response =>
//                {
//                    dom = response.Dom;
//                    content1 = string.Empty;
//                    content1 = dom["#txtBody  > p"].Text();
//                    done = true;
//                }, response =>
//                {
//                    done = true;
//                });
 
//            while (done == false) ;
 
//            // Now content 1 comes incorrect like this "ظپظ…ظ†ط° ظ‚ط¯ظ…طھ ط£ظ„ط¨ظˆظ…ظ‡ط§ ط§ظ„ط£ظˆظ„' ط³ط§ظ"
 
//            // ** Start Using Web Client 

            
//            WebClient client = new WebClient();
//            client.Encoding = System.Text.Encoding
//.UTF8;
//            string downloadedString = client.DownloadString("http://www.ahram.org.eg/Stars-Arts/News/194972.aspx");
 
//            // At this point the downloaded string is correct. it get all html document and the inner test like this "ا وحاولت باستمرار أن تنحاز بحسها الفطري إلي نصوص لها جمالها وقيمتها التي تناسب دقة أدائها وشدة وضوح المعاني والحروف"
 
//            var dom2 = CQ.Create(downloadedString);
//            var content2 = dom2["#txtBody > p"].Text();
 
//            // The downlowaded string is corrupted again after using CQ.Create. it looks like this "ظپظ…ظ†ط° ظ‚ط¯ظ…طھ ط£ظ„ط¨ظˆظ…ظ‡ط§ ط§ظ„ط£ظˆظ„' ط³ط§ظ"
//            // So content 1 = content 2 
 
 
//           // try change the charset in downloaded string to utf8
//            //downloadedString = downloadedString.Replace("charset=windows-1256", "charset=utf8");
           
//            var dom3 = CQ.Create(downloadedString);
//            var content3 = dom3["#txtBody > p"].Text();
 
//            // Content3 is correct now and the text looks like this "فمنذ قدمت ألبومها الأول' ساكن' وهي تسكن وجدان وأذهان الجماهير العربية في منطقة لم تبرحها حتي الآن, ورغم أنها فنانة تنتمي لجيل الأغنية الجديدة إلا إننا نحس من وراء صوتها بشجن تراثنا العربي كله في"
 
        
//        }

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

        [Test, TestMethod]
        public void Issue80()
        {
            var dom = CQ.Create(@"
<div>bar match</div>
<div id='123'>
</div>
</div>");

            var query = dom["div#123"];
            Assert.AreEqual(1, query.Length);
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