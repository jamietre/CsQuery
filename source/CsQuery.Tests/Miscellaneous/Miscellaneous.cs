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

namespace CsQuery.Tests.Miscellaneous
{

    [TestFixture, TestClass,Description("Misc. tests; from support and bug reports")]
    public class Miscellaneous: CsQueryTest
    {

        /// <summary>
        /// Resolve a bug in nth-last type operations
        /// </summary>
        [Test, TestMethod]
        public void BigDomProblems()
        {
            var dom = TestDom("HTML Standard");

            var test = dom["div:nth-last-child(3)"];
            foreach (var item in test) 
            {
                Debug.WriteLine("Starting: " + item.ToString());

                var next = item.NextElementSibling;
                int after = 0;
                while (next!= null)
                {
                    Debug.WriteLine("  "+after+":"+next.ToString());
                    next = next.NextElementSibling;
                    after++;
                }
                Assert.AreEqual(2, after,"Every result should be followed by 2 elements.");
                //Debug.WriteLine("");

            }
        }
        

        [Test, TestMethod]
        public void CharEncoding()
        {
            var res = CQ.Create("<div><span>x…</span></div>");
            Assert.AreEqual(res["div span"].Text(), "x"+(char)8230);

        }



        [Test, TestMethod, Description("ID with space - issue #5")]
        public void TestInvalidID()
        {
            string html = @"<img alt="""" id=""Picture 7"" src=""Image.aspx?imageId=26381""
                style=""border-top-width: 1px; border-right-width: 1px; border-bottom-width:
                1px; border-left-width: 1px; border-top-style: solid; border-right-style:
                solid; border-bottom-style: solid; border-left-style: solid; margin-left:
                1px; margin-right: 1px; width: 950px; height: 451px; "" />";

            var dom = CQ.Create(html);
            var res = dom["img"];
            Assert.AreEqual(1, res.Length);
            Assert.AreEqual("Picture 7", res[0].Id);

            var img = dom.Document.GetElementById("Picture 7");
            Assert.IsNotNull(img);
            Assert.AreEqual("Picture 7", img.Id);

            img = dom.Document.GetElementById("Picture");
            Assert.IsNull(img);

            dom = CQ.Create("<body><div></div><img id=\"image test\" src=\"url\"/>content</div></body>");
            res = dom["img"];
            Assert.AreEqual(1, res.Length);
            Assert.AreEqual("image test", res[0].Id);

        }

        /// <summary>
        /// Issue #5 revealed that during DOM creation duplicate IDs were being stripped. We want this
        /// when an end-user adds things to an existing DOM (e.g. for clones) but not when building from HTML.
        /// Added "AddAlways" method to NodeList to eliminate this check during DOM creation. This actually made
        /// a significant performance improvement for DOM creation too.
        /// </summary>
        [Test, TestMethod, Description("Issue #5 side effects - make sure dup ids can be added")]
        public void TestInvalidID2()
        {
            string html = @"<div id=""test""></div><div id=""test""></div>";
            var dom = CQ.Create(html);
            
            var res = dom["#test"];
            Assert.AreEqual(2, res.Length);
        }

        [Test, TestMethod]
        public void GetText()
        {
            string html = @"<table>
                <tr>
            <td>
            <div class='background_picture'></div>
            <a href='my/link/to/page2'>Page 2 Title</a> :second part of title</td>
            <td>09/09/2011</td>
            </tr>
            </table>";

            var dom = CQ.Create(html);
            string text = dom["tr td:first-child"].Text();
            Assert.AreEqual("Page 2 Title :second part of title", text.Trim());

            var target = dom["tr td:first-child"].Clone();
            target.Children().Remove();
            text= target.Text();
            Assert.AreEqual(":second part of title", text.Trim());

            var textNodeAfterLink = dom["tr td:first-child a"][0].NextSibling.NodeValue;
            Assert.AreEqual(":second part of title", textNodeAfterLink.Trim());
        }

        [Test, TestMethod]
        public void LastWithHTMLNode()
        {
            var dom = TestDom("jquery-unit-index");
            //var parents =dom["#groups"].Parents();
            var res = dom["html"].Not(":last");
            Assert.AreEqual(0,res.Length);
            
        }

        /// <summary>
        /// Bug - Classes collection failing when no classes.
        /// </summary>

        [Test,TestMethod]
        public void ClassStartsWith() {
            // filter elements based on having any class starting with "entry" using LINQ
            
            var html = Dom["TestHtml"];
            var elements = html["span"].Where(item=>
                    item.Classes.Where(cls=>cls.StartsWith("badge")).Any()
                );

            // wrap the sequence of elements in a CQ object again and use 
            // Text() to get just the text

            var text = new CQ(elements).Text();

       }

        /// <summary>
        /// Index Out of Range parsing - unverified
        /// </summary>

        [Test, TestMethod]
        public void Issue28()
        {
            var strFilePath = Support.GetFilePath(SolutionDirectory + "\\CsQuery.Tests\\Resources\\pupillogin.htm");

            var objStreamReader = new StreamReader(strFilePath, Encoding.UTF8);
            string str = objStreamReader.ReadToEnd();
            var dom = CQ.Create(str);
        }

        /// <summary>
        /// Issue 53: bug in Text method
        /// </summary>
        
        [Test, TestMethod]
        public void Issue53()
        {
            CQ dom = TestDom("samsung-ebay-2");
            var cur = DateTime.Now;
            for (int i = 0; i < 100; i++)
            {
                var text = dom[".prodDetailSec table"].Text();
            }

            Assert.IsTrue(DateTime.Now < cur.AddSeconds(2));
        }

        [Test, TestMethod]
        public void Issue55()
        {
            var testDom = @"<div id=""test"">
            <script>scriptText</script>  
            realText
            </div>";

            var dom = CQ.Create(testDom);

            Assert.AreEqual("\n            scriptText  \n            realText\n            ", dom["#test"].Text());
        }

        [Test, TestMethod]
        public void Issue57()
        {
            var document = CQ.Create(@"<div xmlns=""http://www.w3.org/1999/xhtml"" id=""content"" class=""results""><table><thead><tr><th><a href=""/uksi/2007-*?sort=title"" class=""sortAsc"" title=""Sort ascending by Title""><span class=""accessibleText"">Sort ascending by </span>Title</a></th><th><span>Years and Numbers</span></th><th><span>Legislation type</span></th></tr></thead><tbody><tr class=""oddRow""><td><a href=""/uksi/2012/2652/contents/made"">The Motor Vehicles (Tests) (Amendment) (No. 2) Regulations 2012</a></td><td><a href=""/uksi/2012/2652/contents/made"">2012 No. 2652</a></td><td>UK Statutory Instruments</td></tr><tr><td><a href=""/uksi/2012/2651/contents/made"">The A259 Trunk Road (Various Roads, Rye) (Temporary Restriction and Prohibition of Traffic) Order 2012</a></td><td><a href=""/uksi/2012/2651/contents/made"">2012 No. 2651</a></td><td>UK Statutory Instruments</td></tr><tr class=""oddRow""><td><a href=""/uksi/2012/2650/contents/made"">The A21 Trunk Road (Northbridge Street Roundabout) (Temporary Prohibition of Traffic) Order 2012</a></td><td><a href=""/uksi/2012/2650/contents/made"">2012 No. 2650</a></td><td>UK Statutory Instruments</td></tr><tr><td><a href=""/uksi/2012/2649/contents/made"">The M4 Motorway (Junction 10, Link Roads) (Temporary Prohibition of Traffic) (No.3) Order 2012</a></td><td><a href=""/uksi/2012/2649/contents/made"">2012 No. 2649</a></td><td>UK Statutory Instruments</td></tr><tr class=""oddRow""><td><a href=""/uksi/2012/2648/contents/made"">The M4 Motorway (Junction 4, Eastbound Exit Slip Road) (Temporary Prohibition of Traffic) Order 2012</a></td><td><a href=""/uksi/2012/2648/contents/made"">2012 No. 2648</a></td><td>UK Statutory Instruments</td></tr><tr><td><a href=""/uksi/2012/2647/contents/made"">The Health Act 2009 (Commencement No. 6) Order 2012</a></td><td><a href=""/uksi/2012/2647/contents/made"">2012 No. 2647 (C. 105)</a></td><td>UK Statutory Instruments</td></tr><tr class=""oddRow""><td><a href=""/uksi/2012/2646/contents/made"">The A26 Trunk Road (Beddingham Roundabout - South of The Lay) (Temporary 40 Miles Per Hour Speed Restriction) Order 2012</a></td><td><a href=""/uksi/2012/2646/contents/made"">2012 No. 2646</a></td><td>UK Statutory Instruments</td></tr><tr><td><a href=""/uksi/2012/2645/contents/made"">The M6 Motorway (Junction 20-22 Southbound Carriageway and Slip Road) (Temporary Prohibition and Restriction of Traffic) Order 2012</a></td><td><a href=""/uksi/2012/2645/contents/made"">2012 No. 2645</a></td><td>UK Statutory Instruments</td></tr><tr class=""oddRow""><td><a href=""/uksi/2012/2644/contents/made"">The M62 Motorway (Junction 32 to Junction 33) (Temporary Prohibition of Traffic) Order 2012</a></td><td><a href=""/uksi/2012/2644/contents/made"">2012 No. 2644</a></td><td>UK Statutory Instruments</td></tr><tr><td><a href=""/uksi/2012/2643/contents/made"">The M18 Motorway (Junction 2, Wadworth) (Temporary Restriction and Prohibition of Traffic) Order 2012</a></td><td><a href=""/uksi/2012/2643/contents/made"">2012 No. 2643</a></td><td>UK Statutory Instruments</td></tr><tr class=""oddRow""><td><a href=""/uksi/2012/2642/contents/made"">The M1 Motorway and the M18 Motorway (Thurcroft Interchange to Bramley Interchange) (Temporary Restriction and Prohibition of Traffic) Order 2012</a></td><td><a href=""/uksi/2012/2642/contents/made"">2012 No. 2642</a></td><td>UK Statutory Instruments</td></tr><tr><td><a href=""/uksi/2012/2641/contents/made"">The M1 Motorway (Junction 32 to Junction 33) and the M18 Motorway (Thurcroft Interchange to Junction 1) (Temporary Restriction and Prohibition of Traffic) Order 2012</a></td><td><a href=""/uksi/2012/2641/contents/made"">2012 No. 2641</a></td><td>UK Statutory Instruments</td></tr><tr class=""oddRow""><td><a href=""/uksi/2012/2640/contents/made"">The A1 Trunk Road (Darrington Interchange) (Temporary Prohibition of Traffic) (No.2) Order 2012</a></td><td><a href=""/uksi/2012/2640/contents/made"">2012 No. 2640</a></td><td>UK Statutory Instruments</td></tr><tr><td><a href=""/uksi/2012/2639/contents/made"">The A1 Trunk Road (Catterick South Interchange) (Temporary Restriction and Prohibition of Traffic) Order 2012</a></td><td><a href=""/uksi/2012/2639/contents/made"">2012 No. 2639</a></td><td>UK Statutory Instruments</td></tr><tr class=""oddRow""><td><a href=""/uksi/2012/2638/contents/made"">The A1 Trunk Road (Gateshead Quays Interchange) (Temporary Prohibition of Traffic) Order 2012</a></td><td><a href=""/uksi/2012/2638/contents/made"">2012 No. 2638</a></td><td>UK Statutory Instruments</td></tr><tr><td><a href=""/uksi/2012/2637/contents/made"">The A1 Trunk Road (West Mains to Haggerston) (Temporary Restriction and Prohibition of Traffic) Order 2012</a></td><td><a href=""/uksi/2012/2637/contents/made"">2012 No. 2637</a></td><td>UK Statutory Instruments</td></tr><tr class=""oddRow""><td><a href=""/uksi/2012/2636/contents/made"">The Merchant Shipping (Passenger Ships on Domestic Voyages)(Amendment) Regulations 2012</a></td><td><a href=""/uksi/2012/2636/contents/made"">2012 No. 2636</a></td><td>UK Statutory Instruments</td></tr><tr><td><a href=""/uksi/2012/2635/contents/made"">The Network Rail (North Doncaster Chord) Order 2012</a></td><td><a href=""/uksi/2012/2635/contents/made"">2012 No. 2635</a></td><td>UK Statutory Instruments</td></tr><tr class=""oddRow""><td class=""bilingual en""><a href=""/wsi/2012/2634/contents/made"">The A470 Trunk Road (Llyswen, Powys) (Temporary Prohibition of Vehicles) Order 2012</a></td><td rowspan=""2""><a href=""/wsi/2012/2634/contents/made"">2012 No. 2634</a></td><td rowspan=""2"">Wales Statutory Instruments</td></tr><tr class=""oddRow""><td class=""bilingual cy""><a href=""/wsi/2012/2634/contents/made/welsh"" lang=""cy"" xml:lang=""cy"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"">Gorchymyn Cefnffordd yr A470 (Llys-wen, Powys) (Gwahardd Cerbydau Dros Dro) 2012</a></td></tr><tr><td class=""bilingual en""><a href=""/wsi/2012/2633/contents/made"">The A40 Trunk Road (Glangwili Roundabout to Broad Oak, Carmarthenshire) (Temporary Traffic Restrictions and Prohibitions) Order 2012</a></td><td rowspan=""2""><a href=""/wsi/2012/2633/contents/made"">2012 No. 2633</a></td><td rowspan=""2"">Wales Statutory Instruments</td></tr><tr><td class=""bilingual cy""><a href=""/wsi/2012/2633/contents/made/welsh"" lang=""cy"" xml:lang=""cy"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"">Gorchymyn Cefnffordd yr A40 (Cylchfan Glangwili i Dderwen-fawr, Sir Gaerfyrddin) (Cyfyngiadau a Gwaharddiadau Traffig Dros Dro) 2012</a></td></tr></tbody></table></div>");

            var elem = document["#content.results > table > tbody > tr > td:nth-child(1) > a:nth-child(1)"].Eq(19).Parent().Next("td").Children("a").First();
            // Do something with elem
            var link = elem.Parent().Next("td").Children("a").First();
            // NullReferenceException thrown.

        }


        [Test, TestMethod]
        public void Issue56_withDataAttr()
        {
            var dom = CQ.CreateFragment(@"<div data-test=""display:block;
width:10px;"">");

            var div = dom["div[data-test='display:block;\\a width:10px;']"];
            Assert.AreEqual(1,div.Length);
        }

        /// <summary>
        /// Style attribute has special handling - however it should still reflect the input formatting unless altered by a CsQuery method
        /// </summary>

        [Test, TestMethod]
        public void Issue56_withStyle()
        {
            var dom = CQ.CreateFragment(@"<div style=""display:block;
width:10px;"">");

            var div = dom["div[style='display:block;\\a width:10px;']"];
            Assert.AreEqual(1, div.Length);

            div.Css("width", "20px");

            div = dom["div[style='display: block; width: 20px;']"];
            Assert.AreEqual(1, div.Length);

        }


        [Test, TestMethod]
        public void Issue59_Comments()
        {
            var intext = "<!-- Head Comment --><!-- Body Comment -->";
            var doc = CsQuery.CQ.Create(intext, HtmlParsingMode.Fragment, HtmlParsingOptions.None);
            var outtext = doc.Render(DomRenderingOptions.None);
            Assert.AreEqual(intext, outtext);

        }

        /// <summary>
        /// I am trying to apply the Closest selector method to a set of matched elements and expecting
        /// to get one closest ancestor per matched element (in cases where such an ancestor exists), but
        /// only the first one is getting matched.
        /// </summary>

        [Test, TestMethod]
        public void Issue61_Closest()
        {
            var html = "<div id=outer><div id=div1><span></span></div><div id=div2><span></span></div></div>";
            var doc = CQ.CreateFragment(html);
            var spans = doc["span"];
            Assert.AreEqual(2,spans.Length);
            
            var closestDivs = spans.Closest("div");
            Assert.AreEqual(2, closestDivs.Length);
            CollectionAssert.AreEqual(Arrays.String("div1", "div2"), closestDivs.Select(item => item.Id));
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