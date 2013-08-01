using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.HtmlParser;
using CsQuery.Utility;

namespace CsQuery.Tests.HtmlParser
{
    
    [TestFixture, TestClass]
    public class TextArea : CsQueryTest
    {


        /// <summary>
        /// Ensure than inner contents of text area is parsed properly. Can't do this too often..
        /// </summary>
        [Test, TestMethod]
        public void Parsing()
        {
            var dom = TestDom("TextAreaTest");
            var ta= dom["textarea"];

            string expected = @"<a href=""http://www.test.com/"" target=""_blank"">
<img src=""http://www.test.com/image.jpg"" style=""border:0"">
</a>
".NormalizeLineEndings();

            var actual = ta.Val().NormalizeLineEndings();


            Assert.AreEqual(expected, actual);

        }

        /// <summary>
        /// Ensure that the type of a TEXTAREA element is always the string "textarea"
        /// </summary>
        [Test, TestMethod]
        public void Type()
        {
            CQ cq = CQ.Create("<textarea type=useless>Foo that bar</textarea>");
            IHTMLTextAreaElement textArea = cq["textarea"].FirstElement() as IHTMLTextAreaElement;

            Assert.IsNotNull(textArea);
            Assert.AreEqual("textarea", textArea.Type);
            Assert.AreEqual("useless", textArea.GetAttribute("type"));
        }
    }
}