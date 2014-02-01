using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using CollectionAssert = NUnit.Framework.CollectionAssert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.Tests.Miscellaneous
{
    /// <summary>
    /// Trying to reproduce a problem during dom manipulation that comes up when removing text nodes.
    /// </summary>

    [TestFixture, TestClass]
    public class Reindex: CsQueryTest
    {



        [Test, TestMethod]
        public void Issue105()
        {
            //var dom = TestDom("jquery-unit-index");
            CQ dom = @"<h2>Téxto</h2>";
            

            Assert.AreEqual("Téxto", dom["h2"][0].InnerText);
            Assert.AreEqual("Téxto", dom["h2"].Text());



        }

        [Test, TestMethod]
        public void Issue104()
        {
            //var dom = TestDom("jquery-unit-index");
            CQ dom = @"<div data-citationprefix='see'></div>";
            var res = dom["div"].Data("citationprefix");
            Assert.AreEqual("see", res);



        }
        [Test, TestMethod]
        public void Issue99()
        {
            //var dom = TestDom("jquery-unit-index");
            CQ dom = @"<html><body><span style=''>1lb14.4oz</span></body></html>";
            var res = dom["span:visible"];
            Assert.IsTrue(true);



        }
        [Test, TestMethod]
        public void Issue97()
        {
            var dom = CQ.Create("<?xml version=\"1.0\"?><rss version=\"2.0\" xmlns:atom=\"http://www.w3.org/2005/Atom\"><channel><atom:link href=\"http://www.cnbc.com/id/100003241/device/rss/rss.html\" rel=\"self\" type=\"application/rss+xml\" /><title>Markets Headlines</title><description><![CDATA[]]></description><fakeLink>http://www.cnbc.com/id/100003241</fakeLink><pubDate>Fri, 22 Mar 2013 06:29 GMT</pubDate><lastBuildDate>Fri, 22 Mar 2013 06:29 GMT</lastBuildDate><language>en-us</language><ttl>60</ttl><item><title>Euro Rises on Hopes for Last-Minute Cyprus Deal</title><description><![CDATA[The euro rose Friday on optimism Cyprus will be able to cobble together a last-minute deal.]]></description><fakeLink>http://www.cnbc.com/id/100579967</fakeLink><pubDate>Fri, 22 Mar 2013 21:02 GMT</pubDate><guid isPermaLink=\"false\">guid_100579967</guid> </item> <item><title>US Oil Gains Over $1 as Equities Rise</title><description><![CDATA[Brent crude edged higher on Friday while U.S. crude futures jumped more than $1 per barrel, sending the spread between the two contracts to the narrowest level since July. \r\n ]]></description><fakeLink>http://www.cnbc.com/id/100580094</fakeLink><pubDate>Fri, 22 Mar 2013 20:23 GMT</pubDate><guid isPermaLink=\"false\">guid_100580094</guid> </item> <item><title>US Bonds Near Flat on Cyprus Uncertainty</title><description><![CDATA[Prices for U.S. Treasurys traded flat on Friday as a bailout for Cyprus remained in doubt going into the weekend. ]]></description><fakeLink>http://www.cnbc.com/id/100580561</fakeLink><pubDate>Fri, 22 Mar 2013 20:17 GMT</pubDate><guid isPermaLink=\"false\">guid_100580561</guid> </item> <item><title>Gold Down as Cyprus Fears Ease, Notches Weekly Gain</title><description><![CDATA[Gold fell as investors took profits a day after the precious metal hit a one-month high.]]></description><fakeLink>http://www.cnbc.com/id/100580130</fakeLink><pubDate>Fri, 22 Mar 2013 19:55 GMT</pubDate><guid isPermaLink=\"false\">guid_100580130</guid> </item> <item><title>Asia Falls 2% This Week on Cyprus; China Rallies</title><description><![CDATA[Festering concerns of a banking collapse in Cyprus led to a sell-off in most Asian markets on Friday, resulting in a 2 percent weekly loss. However, mainland shares outshone to post a weekly gain of 2 percent after Thursday&#039;s Flash PMI data.\r\n ]]></description><fakeLink>http://www.cnbc.com/id/100579945</fakeLink><pubDate>Fri, 22 Mar 2013 08:00 GMT</pubDate><guid isPermaLink=\"false\">guid_100579945</guid> </item> <item><title>Yen Surges as Investors Worry Over Cyprus Turmoil</title><description><![CDATA[The yen surged against the dollar and euro on Thursday amid fears of a financial meltdown in Cyprus.]]></description><fakeLink>http://www.cnbc.com/id/100575330</fakeLink><pubDate>Thu, 21 Mar 2013 20:58 GMT</pubDate><guid isPermaLink=\"false\">guid_100575330</guid> </item> <item><title>Treasurys Climb as Cyprus Scrambles for Aid</title><description><![CDATA[Prices for U.S. Treasurys rose on Thursday as bailout plans for Cyprus remained in disarray.]]></description><fakeLink>http://www.cnbc.com/id/100575882</fakeLink><pubDate>Thu, 21 Mar 2013 20:23 GMT</pubDate><guid isPermaLink=\"false\">guid_100575882</guid> </item> <item><title>Oil Battered by Cyprus Angst, Dour German Figures</title><description><![CDATA[Crude oil was pushed lower by fears of further turmoil in the euro zone, as Cyprus scrambled to avoid bankruptcy.]]></description><fakeLink>http://www.cnbc.com/id/100575400</fakeLink><pubDate>Thu, 21 Mar 2013 19:45 GMT</pubDate><guid isPermaLink=\"false\">guid_100575400</guid> </item> <item><title>Gold Hits Near 1-Month High on Cyprus Debt Fears</title><description><![CDATA[Gold rose to a near one-month high, as safe-haven buying emerged after the European Union gave Cyprus an ultimatum.]]></description><fakeLink>http://www.cnbc.com/id/100575458</fakeLink><pubDate>Thu, 21 Mar 2013 18:44 GMT</pubDate><guid isPermaLink=\"false\">guid_100575458</guid> </item> <item><title>European Shares Close Lower on Cyprus Fears</title><description><![CDATA[European shares closed lower.]]></description><fakeLink>http://www.cnbc.com/id/100575843</fakeLink><pubDate>Thu, 21 Mar 2013 16:35 GMT</pubDate><guid isPermaLink=\"false\">guid_100575843</guid> </item> <item><title>Futures Narrowly Mixed After Jobless Claims</title><description><![CDATA[Stock index futures hovered around the flatline Thursday following the weekly jobless claims report, while uncertainty in Cyprus kept investors on edge. ]]></description><fakeLink>http://www.cnbc.com/id/100576605</fakeLink><pubDate>Thu, 21 Mar 2013 13:16 GMT</pubDate><guid isPermaLink=\"false\">guid_100576605</guid> </item> <item><title>Asia Turns Mixed; Nikkei Scales 4 1/2-Year Peak</title><description><![CDATA[A pick-up in Chinese manufacturing and expectations of monetary easing drove North Asian shares higher but political tensions in Seoul and Sydney put a cap on further regional gains.\r\n ]]></description><fakeLink>http://www.cnbc.com/id/100574983</fakeLink><pubDate>Thu, 21 Mar 2013 08:06 GMT</pubDate><guid isPermaLink=\"false\">guid_100574983</guid> </item> <item><title>Can Investors Take Comfort in Latest China PMI?</title><description><![CDATA[With recent Chinese economic data offering mixed signals over the health of the world&#039;s second largest economy, should investors take comfort from the HSBC&#039;s flash manufacturing PMI for March?]]></description><fakeLink>http://www.cnbc.com/id/100575757</fakeLink><pubDate>Thu, 21 Mar 2013 04:07 GMT</pubDate><guid isPermaLink=\"false\">guid_100575757</guid> </item> <item><title>The UK Budget Did No Favor to the Pound</title><description><![CDATA[Chancellor George Osborne disappointed those who wanted to see a big change in the Bank of England&#039;s mandate. The pound strengthened a bit as a result, but those who are waiting to see it weaken will not be disappointed.]]></description><fakeLink>http://www.cnbc.com/id/100575565</fakeLink><pubDate>Thu, 21 Mar 2013 03:55 GMT</pubDate><guid isPermaLink=\"false\">guid_100575565</guid> </item> <item><title>Traders to Pick Over Data, With Fed in Mind</title><description><![CDATA[Housing and jobs data will get a close look from traders Thursday, as they continue to dissect the comments from the Fed.]]></description><fakeLink>http://www.cnbc.com/id/100574651</fakeLink><pubDate>Thu, 21 Mar 2013 00:25 GMT</pubDate><guid isPermaLink=\"false\">guid_100574651</guid> </item> <item><title>Dollar Rallies as Fed Says No Change to Easy Money</title><description><![CDATA[The US dollar rallied after a decision by the Federal Reserve to continue its aggressive monetary easing.]]></description><fakeLink>http://www.cnbc.com/id/100570564</fakeLink><pubDate>Wed, 20 Mar 2013 21:16 GMT</pubDate><guid isPermaLink=\"false\">guid_100570564</guid> </item> <item><title>Gold Holds Losses After Fed Decision</title><description><![CDATA[Gold eased from the previous session&#039;s three-week high on Wednesday, reflecting some investor optimism that the crisis in Cyprus may not spread further in the euro zone.\r\n ]]></description><fakeLink>http://www.cnbc.com/id/100570568</fakeLink><pubDate>Wed, 20 Mar 2013 20:36 GMT</pubDate><guid isPermaLink=\"false\">guid_100570568</guid> </item> <item><title>Stocks End Higher After Fed Decision, Dow Hits New Intraday High</title><description><![CDATA[Stocks finished higher Wednesday, wiping out most of the past week&#039;s losses and with the Dow touching a new intraday high. ]]></description><fakeLink>http://www.cnbc.com/id/100572871</fakeLink><pubDate>Wed, 20 Mar 2013 20:23 GMT</pubDate><guid isPermaLink=\"false\">guid_100572871</guid> </item> <item><title>Brent Crude Oil Rises to $108 From 3-Month Low</title><description><![CDATA[Brent crude oil rose above $108 a barrel, recovering from a three-month low/]]></description><fakeLink>http://www.cnbc.com/id/100570567</fakeLink><pubDate>Wed, 20 Mar 2013 20:05 GMT</pubDate><guid isPermaLink=\"false\">guid_100570567</guid> </item> <item><title>US Bonds Stay Heavy After Fed Decision</title><description><![CDATA[U.S. bonds were mired in the red after the Federal Reserve held firm on its monetary policy.]]></description><fakeLink>http://www.cnbc.com/id/100572535</fakeLink><pubDate>Wed, 20 Mar 2013 19:21 GMT</pubDate><guid isPermaLink=\"false\">guid_100572535</guid> </item> <item><title>Market Shakes Off Cyprus Jitters</title><description><![CDATA[Even as the euro zone stands firm, markets have embraced the view that Cyprus will strike a deal.\r\n ]]></description><fakeLink>http://www.cnbc.com/id/100573847</fakeLink><pubDate>Wed, 20 Mar 2013 17:12 GMT</pubDate><guid isPermaLink=\"false\">guid_100573847</guid> </item> <item><title>European Shares End Mixed as Investors Continue to Watch Cyprus</title><description><![CDATA[European shares pared their earlier gains to close narrowly mixed Wednesday. ]]></description><fakeLink>http://www.cnbc.com/id/100571158</fakeLink><pubDate>Wed, 20 Mar 2013 16:35 GMT</pubDate><guid isPermaLink=\"false\">guid_100571158</guid> </item> <item><title>More Bond Investors Bet on US Rate Rise</title><description><![CDATA[U.S. bond investors are seeking new ways to hedge against the risk of a sharp rise in interest rates, the FT reports.]]></description><fakeLink>http://www.cnbc.com/id/100573190</fakeLink><pubDate>Wed, 20 Mar 2013 15:05 GMT</pubDate><guid isPermaLink=\"false\">guid_100573190</guid> </item> <item><title>Cyprus's Plan B Spooks Asia; China Shrugs It Off</title><description><![CDATA[Asian stocks were under pressure on Wednesday as concerns rose if a bailout deal was still possible for Cyprus while Greater Chinese shares ignored the news to outperform the market as attention turned to domestic issues.\r\n ]]></description><fakeLink>http://www.cnbc.com/id/100570537</fakeLink><pubDate>Wed, 20 Mar 2013 08:10 GMT</pubDate><guid isPermaLink=\"false\">guid_100570537</guid> </item> <item><title>Warning! China Stocks May See Double-Digit Drop </title><description><![CDATA[Chinese stocks are down over 6 percent in the past month. But be warned, the worst is far from over.]]></description><fakeLink>http://www.cnbc.com/id/100570888</fakeLink><pubDate>Wed, 20 Mar 2013 03:46 GMT</pubDate><guid isPermaLink=\"false\">guid_100570888</guid> </item> <item><title>Dovish Fed Seen Keeping Lid on Market Worry</title><description><![CDATA[With Cyprus creating a new wave of worry, markets will be looking to the Fed Wednesday to keep a steady hand on the tiller.]]></description><fakeLink>http://www.cnbc.com/id/100569902</fakeLink><pubDate>Wed, 20 Mar 2013 00:12 GMT</pubDate><guid isPermaLink=\"false\">guid_100569902</guid> </item> <item><title>Euro Nears Four-Month Low on Turmoil in Cyprus </title><description><![CDATA[The euro dropped near a four-month low vs. the US dollar as uncertainty about Cyprus stoked fears about the currency. ]]></description><fakeLink>http://www.cnbc.com/id/100565748</fakeLink><pubDate>Tue, 19 Mar 2013 21:02 GMT</pubDate><guid isPermaLink=\"false\">guid_100565748</guid> </item> <item><title>US Treasurys Climb on Cyprus Worries</title><description><![CDATA[U.S. Treasurys prices climbed on Tuesday as a plan in Cyprus to tax bank accounts to help pay for a bailout unraveled, creating uncertainty about the island country&#039;s financial future and reviving fears about the stability of the euro zone. \r\n ]]></description><fakeLink>http://www.cnbc.com/id/100566307</fakeLink><pubDate>Tue, 19 Mar 2013 20:26 GMT</pubDate><guid isPermaLink=\"false\">guid_100566307</guid> </item> <item><title>Don't Count Cyprus Out of the Euro Just Yet: Traders</title><description><![CDATA[Uncertainty about Cyprus set markets on edge, but they are far from pricing in the tiny country&#039;s exit from the euro.]]></description><fakeLink>http://www.cnbc.com/id/100569669</fakeLink><pubDate>Tue, 19 Mar 2013 20:22 GMT</pubDate><guid isPermaLink=\"false\">guid_100569669</guid> </item> <item><title>Gold Extends Gains as Safety Investment Amid Cyprus Vote</title><description><![CDATA[Gold reversed earlier losses, hitting a 2-1/2 week high above $1,615 an ounce on renewed flight-to-safety investment.]]></description><fakeLink>http://www.cnbc.com/id/100565956</fakeLink><pubDate>Tue, 19 Mar 2013 19:44 GMT</pubDate><guid isPermaLink=\"false\">guid_100565956</guid> </item> </channel> </rss>");
            Assert.IsTrue(true);

        }

        //[Test, TestMethod]
        //public void XmlProb()
        //{

        //    var dom = CsQuery.CQ.CreateFromUrl("http://feeds.benzinga.com/benzinga");

        //    dom = "<p style='height:10; width:10'>";
        //    dom.Each((i,e) =>
        //    {
        //        e.RemoveStyle("height");
        //    });
        //    Console.WriteLine("Got items: " + dom["item"].Count());
        //    Console.WriteLine("Got item HTML" + dom["item"].First().Html());
        //    Console.WriteLine("Link is: " + dom["item"].First()["link"].Html());
        //    Console.Read();
        //}

        [Test, TestMethod]
        public void Issue87()
        {
            CQ dom = "<div>Hello world! <b>I am feeling bold!</b> What about <b>you?</b></div>";


            CQ bold = dom["b"];               /// find all "b" nodes (there are two in this example)

            string boldText = bold.Text();        /// jQuery text method; == "I am feeling bold! you?"

            bold.Remove();                        /// jQuery Remove method

            string html = dom.Render();           /// =="<div>Hello world!  What about </div>"
        }
        [Test, TestMethod]
        public void InnerTextFixBroken()
        {
            var dom = TestDom("wiki-cheese");

            dom["span,p,div"].Each(
                    obj =>
                    {
                        if (string.IsNullOrEmpty(obj.TextContent.Trim()) && string.IsNullOrEmpty(obj.InnerHTML))
                            obj.Remove();
                    });
           


        }

        [Test, TestMethod]
        public void Issue85_CleanElementAdding()
        {
            CQ dom = "<table><tr></tr></table>";
            dom["tr"].Append("<td />");
            Assert.AreEqual("<table><tbody><tr><td></td></tr></tbody></table>", dom.Render());
        }

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

        [Test, TestMethod]
        public void Issue145()
        {
            var dom= @"<html xmlns=""http://www.w3.org/1999/xhtml"" xmlns:xi=""http://www.w3.org/2001/XInclude""><body></html>";

            var cq = CQ.CreateDocument(dom);
            Assert.AreEqual(cq["html"].Attr("xmlns:xi"),"http://www.w3.org/2001/XInclude");
        }

        [Test, TestMethod]
        public void Issue134()
        {
            var dom = @"<html xmlns=""http://www.w3.org/1999/xhtml"" xmlns:xi=""http://www.w3.org/2001/XInclude""><body></html>";

            var str = dom.ToStream(); 

            var cq = CQ.CreateDocument(str);

            str.Position = 0;
            
            var reader = new StreamReader(str);
            var textOut = reader.ReadToEnd();
            Assert.AreEqual(dom, textOut);
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