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
    [TestFixture, TestClass]
    public class DomDocument
    {

        [Test, TestMethod]
        public void CheckDocumentIndex()
        {
            var frag = TestDocument();

            var concreteFrag = (Implementation.DomDocument)frag[0].Document;
            Assert.IsFalse(concreteFrag.IsDisconnected);
            Assert.IsTrue(concreteFrag.IsIndexed);
            Assert.AreNotEqual(0, concreteFrag.SelectorXref.Count);
        }

        [Test, TestMethod]
        public void DocumentProperties()
        {
            var frag = TestDocument();

            Assert.IsTrue(frag.Document is IDomDocument);
            Assert.IsTrue(frag.Document.IsIndexed);
        }

        private CQ TestDocument()
        {
            return CQ.CreateDocument("<div><span></span></div>");
        }
    }
}

