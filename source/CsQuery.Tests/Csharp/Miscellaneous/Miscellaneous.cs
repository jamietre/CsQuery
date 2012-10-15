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
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;

namespace CsQuery.Tests.Csharp.Miscellaneous
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
        /// Issue 51. A bug concerning subselectors  that arises when the selector matches the element,
        /// but only when run from the root context. That is a compound selector like ".class1 .class2"
        /// which would result in an element ".class2" being returned, but the ".class1" element was
        /// above the context in the dom.
        /// 
        /// The test DOM involves a target class that appears both inside and outside a parent container.
        /// The intial selector gets all of them, then a subselector exlcudes some based on the parent
        /// container.
        /// </summary>
        
        [Test, TestMethod]
        public void Issue51()
        {
            CQ dom = TestDom("ABS - Auto Brake Service.htm");
            CQ items = dom.Select(".items:not(.relatedListingsContainer .items)");

            Assert.AreEqual(1, items.Length);
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