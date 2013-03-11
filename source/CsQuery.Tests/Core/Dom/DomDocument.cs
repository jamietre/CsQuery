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
using CsQuery.Implementation;
using CsQuery.Engine;

namespace CsQuery.Tests.Core.Dom
{
    [TestFixture, TestClass]
    public class DomDocument_
    {

        [Test, TestMethod]
        public void CheckDocumentIndex()
        {
            var frag = TestDocument();

            DomDocument concreteFrag = (DomDocument)frag[0].Document;
            Assert.IsFalse(concreteFrag.IsDisconnected);
            Assert.IsTrue(concreteFrag.IsIndexed);
            var index = concreteFrag.DocumentIndex;
            if (index is IDomIndexSimple)
            {
                Assert.AreNotEqual(0, index.Count);
            }
            else
            {
                Assert.AreEqual(0, index.Count);
            }
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

