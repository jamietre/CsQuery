using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;
using CsQuery.Output;

namespace CsQuery.Tests.Core.OutputFormatters_
{
    /// <summary>
    /// This method is largely covered by the jQuery tests
    /// </summary>

    [TestClass,TestFixture]
    public class FormatPlainText: CsQueryTest
    {

        [Test, TestMethod]
        public void Render_PlainText_Simple()
        {
            var dom = CQ.CreateDocument(@"
<div>This is a paragraph</div>
Line1<br><br>Line3      <br>Line4
<div>FinalP</div>
LooseText

");
            string output = dom.Render(OutputFormatters.PlainText);

            // Note: whitespace after Line3 is turned to a single trailing whitespace  b/c it isn't ended with a block el.
            string expected = @"This is a paragraph
Line1

Line3 
Line4
FinalP
LooseText
".NormalizeLineEndings();

            Assert.AreEqual(expected,output);

        }
    
        [Test, TestMethod]
        public void Render_PlainText_Pre()
        {
            var dom = CQ.CreateDocument(@"This is a paragraph
<pre>This
Is - Preformatted
</pre>
The End
");
            string output = dom.Render(OutputFormatters.PlainText);

            string expected = @"This is a paragraph
This
Is - Preformatted
The End
".NormalizeLineEndings();

            Assert.AreEqual(expected,output);

        }

        [Test, TestMethod]
        public void Render_PlainText_Links()
        {
            var dom = CQ.CreateDocument(@"This is some text
A link: <a href=""http://something.com"">SomethingSite</a>
The End
");
            string output = dom.Render(OutputFormatters.PlainText);

            string expected = @"This is some text A link: SomethingSite (http://something.com) The End
".NormalizeLineEndings();

            Assert.AreEqual(expected,output);

        }
   }
}

