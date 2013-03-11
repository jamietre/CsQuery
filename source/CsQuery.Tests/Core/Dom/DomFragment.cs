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
    public class DomFragment_
    {

        [Test, TestMethod]
        public void CheckFragmentIndex()
        {
            var frag = TestFragment();

            DomFragment concreteFrag = (DomFragment)frag[0].Document;
            Assert.IsTrue(concreteFrag.IsFragment);
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
        public void FragmentProperties()
        {
            var frag = TestFragment();

            Assert.IsTrue(frag.Document is IDomFragment);
            Assert.IsTrue(frag.Document.IsIndexed);
        }
        [Test, TestMethod]
        public void NonIndexedSelectors()
        {
            var frag = TestFragment();

            Assert.AreEqual("el-id", frag["#el-id"][0].Id);
            Assert.AreEqual("el-class", frag[".el-class"][0].ClassName);
            Assert.AreEqual(3, frag["div"].Length);
        }

        [Test, TestMethod]
        public void DisconnectedSelectors()
        {
            var frag = TestFragment().Clone();

            Assert.AreEqual("el-id", frag["#el-id"][0].Id);
            Assert.AreEqual("el-class", frag[".el-class"][0].ClassName);
            Assert.AreEqual(3, frag["div"].Length);
        }

        private CQ TestFragment()
        {
            return CQ.CreateFragment(@"<div>
                <span>
                    <input type=""text"">
                    <div id=""el-id""></div>
                    <div class=""el-class""></div>
                        content
                </span>
            </div>");
        }
    }
}

