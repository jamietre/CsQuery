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

namespace CsQuery.Tests.Core
{
    /// <summary>
    /// This method is largely covered by the jQuery tests
    /// </summary>

    public partial class Methods: CsQueryTest
    {

        [Test, TestMethod]
        public void Render()
        {

            string textIn = "><&" + (char)160 + "æøå";
            var dom = CQ.CreateFragment("<span></span>");

            dom["span"].Text(textIn);

            var text = dom.Render();
            Assert.AreEqual("<span>&gt;&lt;&amp;&nbsp;&#230;&#248;&#229;</span>", text);

            //text = dom.Render(DomRenderingOptions.HtmlEncodingMinimum);
            text = dom.Render(OutputFormatters.Create(HtmlEncoders.Minimum));

            Assert.AreEqual("<span>&gt;&lt;&amp;" + (char)160 + "æøå</span>", text);

            var noEncoding = "<span>" + textIn + "</span>";
            //text = dom.Render(DomRenderingOptions.HtmlEncodingNone);
             text = dom.Render(OutputFormatters.Create(HtmlEncoders.None));
            Assert.AreEqual(noEncoding, text);

        }
   }
}