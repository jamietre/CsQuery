using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using CollectionAssert = NUnit.Framework.CollectionAssert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.HtmlParser;
using CsQuery.Utility;
using CsQuery.Web;

namespace CsQuery.Tests.HtmlParser
{

    [TestFixture, TestClass]
    public class CharacterSetEncoding : CsQueryTest
    {
        string htmlStart = @"<html><head>";
        string htmlStartMeta = @"<META HTTP-EQUIV='Content-Type' content='text/html;charset=windows-1255'>";
        string htmlStart3 = @"</head><body><div id=test>";
        string htmlEnd = "</div></body></html>";
        char hebrewChar = (char)164;

      

        [TestMethod, Test]
        public void MetaTag()
        {

            var encoder = Encoding.GetEncoding("windows-1255");

            var html = htmlStart + htmlStartMeta + htmlStart3 +hebrewChar + htmlEnd;
            var htmlNoRecode = htmlStart + htmlStart3 + hebrewChar + htmlEnd;


            // create a windows-1255 encoded stream
            
            var dom = CQ.Create(GetMemoryStream(htmlNoRecode, encoder));

            // grab the character from CsQuery's output, and ensure that this all worked out.
            
            var csqueryHebrewChar = dom["#test"].Text();

            // Test directly from the stream

            string htmlHebrew = new StreamReader(GetMemoryStream(htmlNoRecode,encoder),encoder).ReadToEnd();

            var sourceHebrewChar = htmlHebrew.Substring(htmlHebrew.IndexOf("test>") + 5, 1);

            // CsQuery should fail to parse it

            Assert.AreNotEqual(sourceHebrewChar, csqueryHebrewChar);


            // the actual character from codepage 1255
            Assert.AreEqual("₪", sourceHebrewChar);

            // Now try it same as the original test - but with the meta tag identifying character set.

            var htmlWindows1255 = GetMemoryStream(html, encoder);

            // Now run again with the charset meta tag, but no encoding specified.
            dom = CQ.Create(htmlWindows1255);

            csqueryHebrewChar = dom["#test"].Text();

            Assert.AreEqual(sourceHebrewChar,csqueryHebrewChar);
        }


        [TestMethod, Test]
        public void MetaTagOutsideBlock()
        {

            var encoder = Encoding.GetEncoding("windows-1255");

            string filler = "<script type=\"text/javascript\" src=\"dummy\"></script>";
            var html = htmlStart;
            
            for (int i = 1; i < 5000 / filler.Length; i++)
            {
                html += filler;
            }
            html += htmlStartMeta + htmlStart3;

            // pad enough after the meta so that the hebrew character is in block 3.
            // If it were in block 2, it could only reflect the prior encoding.
            
            for (int i = 1; i < 5000 / filler.Length; i++)
            {
                html += filler;
            }
            
            html+=hebrewChar + htmlEnd;

            // Now try it  same as the original test - but with the meta tag identifying character set.

            var htmlWindows1255 = GetMemoryStream(html, encoder);

            var dom = CQ.Create(htmlWindows1255, null);
            var outputHebrewChar = dom["#test"].Text();

            Assert.AreEqual("₪", outputHebrewChar);

        }

        [TestMethod,Test]
        public void ContentTypeHeader()
        {
            var creator = new Mocks.MockWebRequestCreator();
            creator.CharacterSet = "windows-1255";
            creator.ResponseStream = GetMemoryStream(htmlStart + htmlStart3 + hebrewChar + htmlEnd, Encoding.GetEncoding("windows-1255"));
            
            CsqWebRequest request = new CsqWebRequest("http://test.com", creator);
            
            var dom1 = CQ.Create(request.Get());

            creator.CharacterSet = "";
            request = new CsqWebRequest("http://test.com", creator);
            var dom2 = CQ.Create(request.Get());

            var output = dom1.Render(OutputFormatters.HtmlEncodingMinimum);

            // The characters should be encoded differently.

            var outputHebrewChar = dom1["#test"].Text();
            var outputUTF8Char = dom2["#test"].Text();
            Assert.AreNotEqual(outputHebrewChar, outputUTF8Char);

            // try it again, using the meta tag
            creator.CharacterSet = "windows-1255";
            creator.ResponseStream = GetMemoryStream(htmlStart + htmlStartMeta + htmlStart3 + hebrewChar + htmlEnd, Encoding.GetEncoding("windows-1255"));

            /// CreateFromUrl process
            
            request = new CsqWebRequest("http://test.com", creator);
            var httpRequest = request.GetWebRequest();
            var response = httpRequest.GetResponse();
            var responseStream = response.GetResponseStream();
            var encoding = CsqWebRequest.GetEncoding(response);

            var dom3 = CQ.CreateDocument(responseStream, encoding);
            var outputHebrewChar2 = dom3["#test"].Text();

            Assert.AreEqual(outputHebrewChar, outputHebrewChar2);

        }

        private string arabicExpected = @"البابا: اوقفوا ""المجزرة"" في سوريا قبل ان تتحول البلاد الى ""أطلال""";

        [TestMethod, Test]
        public void Utf8NoContentType()
        {

            var creator = new Mocks.MockWebRequestCreator();
            creator.CharacterSet = "ISO-8859-1";
            creator.ResponseStream = GetMemoryStream(TestHtml("arabic"), new UTF8Encoding(false));

            CsqWebRequest request = new CsqWebRequest("http://test.com", creator);
            
            // remove the content type header
            var html = ReplaceCharacterSet(request.Get());

            var dom = CQ.CreateDocument(html);

            Assert.AreNotEqual(arabicExpected, dom["h1"].Text());

            //test synchronous: this is the code that CreateFromURL uses 

            creator.CharacterSet = null;
            request = new CsqWebRequest("http://test.com", creator);

            var httpRequest = request.GetWebRequest();
            var response = httpRequest.GetResponse();
            var responseStream = response.GetResponseStream();
            var encoding = CsqWebRequest.GetEncoding(response);
            var dom2 = CQ.CreateDocument(responseStream, encoding);

            Assert.AreEqual(arabicExpected, dom2["h1"].Text());

            // Test async version now

            request = new CsqWebRequest("http://test.com", creator);
            
            bool? done=null;
            CQ dom3=null;

            request.GetAsync((r) =>
            {
                dom3 = r.Dom;
                done = true;
            }, (r)=>{
                done = false;   
            });

            while (done == null) ;
            Assert.IsTrue((bool)done);
            Assert.AreEqual(arabicExpected, dom3["h1"].Text());

        }

      
        /// <summary>
        /// Ensure that BOM takes precendence over anything else
        /// </summary>

        [TestMethod, Test]
        public void MismatchedContentTypeHeaderAndBOM()
        {

            var creator = new Mocks.MockWebRequestCreator();
            creator.CharacterSet = "ISO-8859-1";
            
            // response stream has UTF8 BOM; the latin encoding should be ignored.
            var html = ReplaceCharacterSet(TestHtml("arabic"));
            creator.ResponseStream = GetMemoryStream(html, new UTF8Encoding(true));

            CsqWebRequest request = new CsqWebRequest("http://test.com", creator);


            var dom = ProcessMockWebRequestSync(request);

            Assert.AreEqual(arabicExpected, dom["h1"].Text());

          
        }

        /// <summary>
        /// XML encoding declarations should kick in if there's no content-type or BOM.
        /// </summary>

        [TestMethod, Test]
        public void XmlEncodingDeclaration()
        {

            var creator = new Mocks.MockWebRequestCreator();
            creator.CharacterSet = null;
            
            var html =  ReplaceCharacterSet(TestHtml("arabic"),"ISO-8859-1");
                       
            creator.ResponseStream = GetMemoryStream(html, null);
            CsqWebRequest request = new CsqWebRequest("http://test.com", creator);

            var dom = ProcessMockWebRequestSync(request);
            Assert.AreNotEqual(arabicExpected, dom["h1"].Text());


            // contains xml UTF8 and inline ISO encoding
            
            html = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + html;
            creator.ResponseStream = GetMemoryStream(html, null);
            request = new CsqWebRequest("http://test.com", creator);
            
            dom = ProcessMockWebRequestSync(request);
            Assert.AreEqual(arabicExpected, dom["h1"].Text());
          
        }
        /// <summary>
        /// Removes the "meta http-equiv='Content-Type'" header, or replaces it with a different character set
        /// </summary>
        ///
        /// <param name="html">
        /// The HTML.
        /// </param>
        ///
        /// <returns>
        /// Cleaner HTML
        /// </returns>

        private string ReplaceCharacterSet(string html, string with="")
        {
            // remove the content type header
            var start = html.IndexOf(@"<meta http-equiv=""Content-Type""");
            var end = html.IndexOf(">", start);

            string replaceWith = "";
            if (!String.IsNullOrEmpty(with))
            {
                replaceWith = String.Format(@"<meta http-equiv=""Content-Type"" content=""text/html; charset={0}"" />", with);
            }
            return html.Substring(0, start) + replaceWith + html.Substring(end + 1);
        }

        /// <summary>
        /// Process the mock web request synchronously using same rules as CreateFromUrl
        /// </summary>
        ///
        /// <param name="request">
        /// The request.
        /// </param>
        ///
        /// <returns>
        /// .
        /// </returns>

        private CQ ProcessMockWebRequestSync(CsqWebRequest request)
        {
            var httpRequest = request.GetWebRequest();
            var response = httpRequest.GetResponse();
            var responseStream = response.GetResponseStream();
            var encoding = CsqWebRequest.GetEncoding(response);
           return  CQ.CreateDocument(responseStream, encoding);
        }
    }
}
