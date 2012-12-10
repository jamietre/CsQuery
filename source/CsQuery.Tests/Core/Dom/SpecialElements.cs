using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery.EquationParser;
using CsQuery.EquationParser.Implementation;
using CsQuery.EquationParser.Implementation.Functions;

namespace CsQuery.Tests.Core.Dom
{
    /// <summary>
    /// Tests to ensure that overridden property handling works for certain elements
    /// </summary>

    [TestFixture, TestClass]
    public class SpecialElements: CsQueryTest
    {
        [Test, TestMethod]
        public void TextArea()
        {
            var ta= CQ.Create("<textarea id='ta1' value=2><span>abc</textarea>​​​​​​​​​​​​​​​​​");
            var el=ta["textarea"][0];
            
            var inner="<span>abc";
            Assert.AreEqual(inner,el.Value);
            Assert.AreEqual("2", el["value"]);
            Assert.AreEqual(inner, ta["textarea"].Val());

        }

        [Test, TestMethod]
        public void Option()
        {
            var dom = TestDom("jquery-unit-index");
            var res = dom["#select4"];
            
            var el = (IHTMLOptionElement)res.Find("#option4a")[0];

            Assert.AreEqual("", el.Value);
            Assert.IsFalse(el.Selected);
            Assert.IsTrue(el.Disabled);

            el = (IHTMLOptionElement)el.NextElementSibling;

            Assert.AreEqual("1", el.Value);
            Assert.IsTrue(el.Selected);
            Assert.IsTrue(el.Disabled);

            // this one inherits disabled from the opt group
            el = (IHTMLOptionElement)el.NextElementSibling;

            Assert.AreEqual("2", el.Value);
            Assert.IsTrue(el.Selected);
            Assert.IsTrue(el.Disabled);

        }

        [Test, TestMethod]
        public void Anchor()
        {
            var dom = CQ.Create(@"<a id='a1' href='test' rel='alternate'></a>
                <a id='a2' href='#' rel='invalid'></a>");

            var el = dom.Document.GetElementById<IHTMLAnchorElement>("a1");

            Assert.AreEqual(RelAnchor.Alternate, el.Rel);
            el = dom.Document.GetElementById<IHTMLAnchorElement>("a2");

            Assert.IsTrue(0==el.Rel);
            Assert.AreEqual("invalid", el.GetAttribute("rel"));

            el.Rel = RelAnchor.License;
            Assert.AreEqual(@"<a id=""a2"" href=""#"" rel=""license""></a>", el.Render());
        }

        [Test, TestMethod]
        public void Label()
        {
            var dom = CQ.Create(@"<input type='text' id='input1' />
                <label for='input1'><span>test</span></label>");

            var el = (IHTMLLabelElement)dom["label"][0];

            var input1 = dom.Document.GetElementById("input1");

            Assert.AreEqual(input1, el.Control);
            Assert.AreEqual("input1", el.HtmlFor);

            el.HtmlFor = "";
            Assert.AreEqual(null, el.Control);

            el.Cq().Append(input1);
            Assert.AreEqual(input1, el.Control);

        }
    }

}
 