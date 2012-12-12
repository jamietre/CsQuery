using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Reflection;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;
using CsQuery.Implementation;

namespace CsQuery.Tests.Core
{
    [TestFixture, TestClass]
    public class Methods_Create: CsQueryTest
    {
        string divDom;
        string divDomFull;

        public override void FixtureSetUp()
        {
            base.FixtureSetUp();

            divDom = "<div class=\"content\"></div>";
            divDomFull = "<html><head></head><body>" + divDom + "</body></html>";
        }


        [Test, TestMethod]
        public void Create_String()
        {
            
            var div = CQ.Create("<div></div>")
               .AddClass("content");

            Assert.AreEqual(divDom, div.Render());
        }

        [Test, TestMethod]
        public void Create_Stream()
        {
            byte[] byteArray = Encoding.Unicode.GetBytes(divDom);
            Stream stream = new MemoryStream(byteArray);
          
            var div = CQ.Create(stream,Encoding.Unicode);

            Assert.AreEqual(divDom, div.Render());
        }

        [Test, TestMethod]
        public void Create_Element()
        {
            var el = DomElement.Create("div");
            el.ClassName = "content";

            var div = CQ.Create(el);
            Assert.AreEqual(divDom, div.Render());
        }
        
        [Test, TestMethod]
        public void Create_Elements()
        {
            var el = DomElement.Create("div");
            el.ClassName = "content";

            var div = CQ.Create(Objects.Enumerate(el));
            Assert.AreEqual(divDom, div.Render());
        }

        [Test, TestMethod]
        public void Create_TextReader()
        {

            var div = CQ.Create(new StringReader(divDom));

            Assert.AreEqual(divDom, div.Render());
        }

        [Test, TestMethod]
        public void Create_String_Document()
        {

            var div = CQ.Create(divDom, HtmlParsingMode.Document);

            Assert.AreEqual(divDomFull, div.Render());
        }

        [Test, TestMethod]
        public void Create_String_Content()
        {
            string row = "<tr><td><span>text</span></td></tr>";

            string table = "<table>"+row+"</table>";
            
            var div = CQ.Create(row, HtmlParsingMode.Fragment);
            Assert.AreEqual(row, div.Render());

            div = CQ.Create(row, HtmlParsingMode.Content);
            Assert.AreEqual("<span>text</span>", div.Render());

            div = CQ.Create(table, HtmlParsingMode.Content);
            Assert.AreEqual("<table><tbody>"+row+"</tbody></table>", div.Render());

            div = CQ.Create(table, HtmlParsingMode.Document);
            Assert.AreEqual("<html><head></head><body><table><tbody>"+row+"</tbody></table></body></html>", div.Render());

        }
   }
}