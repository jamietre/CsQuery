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

namespace CsQuery.Tests.Csharp.HtmlParser
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

            string expected = @"
<a href=""http://www.test.com/"" target=""_blank"">
<img src=""http://www.test.com/image.jpg"" style=""border:0"">
</a>
";

            Assert.AreEqual(expected, ta.Val());

        }
    }
}