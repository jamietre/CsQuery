using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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
    public class DomObject
    {
        [Test, TestMethod]
        public void GetAncestors()
        {
            var doc = GetTestDocument();
            var e = doc["#c1"].FirstElement();

            IDomContainer[] ancestors = e.GetAncestors().ToArray();

            Assert.AreEqual(5, ancestors.Length);

            // these are parsed from the HTML
            Assert.AreEqual("p1", ancestors[0].Id);
            Assert.AreEqual("p2", ancestors[1].Id);

            // these are automatically generated in CQ.CreateDocument
            Assert.AreEqual("BODY", ancestors[2].NodeName);
            Assert.AreEqual("HTML", ancestors[3].NodeName);
            Assert.AreEqual(typeof(DomDocument), ancestors[4].GetType());
        }

        [Test, TestMethod]
        public void GetDescendentElements()
        {
            var doc = GetTestDocument();
            var e = doc["#p2"].FirstElement();

            IDomElement[] descendents = e.GetDescendentElements().ToArray();

            Assert.AreEqual(3, descendents.Length);

            Assert.AreEqual("p1", descendents[0].Id);
            Assert.AreEqual("c1", descendents[1].Id);
            Assert.AreEqual("c2", descendents[2].Id);
        }

        [Test, TestMethod]
        public void GetDescendents()
        {
            var doc = GetTestDocument();
            var e = doc["#p2"].FirstElement();

            IDomObject[] descendents = e.GetDescendents().ToArray();

            Assert.AreEqual(3, descendents.Length);

            Assert.AreEqual("p1", descendents[0].Id);
            Assert.AreEqual("c1", descendents[1].Id);
            Assert.AreEqual("c2", descendents[2].Id);
        }

        public CQ GetTestDocument()
        {
            return CQ.CreateDocument("<div id='p2'><span id='p1'><p id='c1'></p><p id='c2'></p></span></div>");
        }
    }
}

