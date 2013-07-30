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
    public class FormAssociatedElement : CsQueryTest
    {
        /// <summary>
        /// Ensure the Form property returns the closest form.
        /// </summary>
        [Test, TestMethod]
        public void ClosestForm()
        {
            CQ cq = CQ.Create("<form id=parent><div><input></div></form>");
            IHTMLInputElement input = cq["input"].FirstElement() as IHTMLInputElement;

            Assert.IsNotNull(input);
            Assert.IsNotNull(input.Form);
            Assert.AreEqual("parent", input.Form.Id);
        }

        /// <summary>
        /// Ensure that the form attribute is observed on a form-reassociateable element.
        /// </summary>
        [Test, TestMethod]
        public void ExplicitFormIdSpecified()
        {
            CQ cq = CQ.Create("<form id=a><div><input form=b></div></form><form id=b></form>");
            IHTMLInputElement input = cq["input"].FirstElement() as IHTMLInputElement;

            Assert.IsNotNull(input);
            Assert.IsNotNull(input.Form);
            Assert.AreEqual("b", input.Form.Id);
        }

        /// <summary>
        /// Ensure that the form attribute is not observed on an a form associated element this is not form-reassociateable.
        /// </summary>
        [Test, TestMethod]
        public void ExplicitFormIdIgnoredOnNonFormReassociateable()
        {
            CQ cq = CQ.Create("<form id=a><div><label form=b></div></form><form id=b></form>");
            IHTMLLabelElement label = cq["label"].FirstElement() as IHTMLLabelElement;

            Assert.IsNotNull(label);
            Assert.IsNotNull(label.Form);
            Assert.AreEqual("a", label.Form.Id);
        }

        /// <summary>
        /// Ensure that the options share the the form of their select elements.
        /// </summary>
        [Test, TestMethod]
        public void OptionSharesFormOfSelect()
        {
            CQ cq = CQ.Create("<form id=a><select form=b><option></option></select></form><form id=b></form>");
            IHTMLOptionElement option = cq["option"].FirstElement() as IHTMLOptionElement;

            Assert.IsNotNull(option);
            Assert.IsNotNull(option.Form);
            Assert.AreEqual("b", option.Form.Id);
        }
    }
}