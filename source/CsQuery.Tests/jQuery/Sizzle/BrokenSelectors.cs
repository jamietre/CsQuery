using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using CollectionAssert= NUnit.Framework.CollectionAssert;
using Description = Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;

namespace CsQuery.Tests.jQuery.Sizzle
{
    /// <summary>
    /// Tests from sizzle.js test suite as of June 13, 2012
    /// https://github.com/jquery/sizzle/tree/master/test
    /// </summary>
    [TestClass, TestFixture]
    public class Sizzle_BrokenSelectors : SizzleTest
    {
        /// <summary>
        /// Test for invalid selectors
        /// </summary>
        [Test, TestMethod]
        public void InvalidSelectors()
        {

            broken( "Broken Selector", "[" );

            // [CsQuery] does not at this time reject potentially invalid selectors, it treats them as HTML.
            // Should this be addressed?

            //broken( "Broken Selector", "(" );
            //broken( "Broken Selector", "{" );
            //broken( "Broken Selector", "<");
            //broken( "Broken Selector", "()" );
            //broken( "Broken Selector", "<>" );
            //broken( "Broken Selector", "{}" );

            broken("Doesn't exist", ":visble");
            broken( "Nth-child", ":nth-child" );

        }

        [Test, TestMethod]
        public void BrokenNthChildSelectors()
        {


            // Sigh. WebKit thinks this is a real selector in qSA
            // They've already fixed this and it'll be coming into
            // current browsers soon. Currently, Safari 5.0 still has this problem
            broken<ArgumentException>("Nth-child", ":nth-child(asdf)");

            // Sigh again. IE 9 thinks this is also a real selector
            // not super critical that we fix this case
            broken<ArgumentException>("Nth-child", ":nth-child(-)");

            broken<ArgumentException>("Nth-child", ":nth-child(2n+-0)");
            broken<ArgumentException>("Nth-child", ":nth-child(2+0)");
            broken<ArgumentException>("Nth-child", ":nth-child(- 1n)");

            // [CsQuery] our expression parser ignores whitespace between associative operands, I think this is ok

            //broken<ArgumentException>("Nth-child", ":nth-child(-1 n)");
        }
        [Test, TestMethod]
        public void ParamsToParameterlessSelectors() {

            broken( "First-child", ":first-child(n)" );
            broken( "Last-child", ":last-child(n)" );
            broken( "Only-child", ":only-child(n)" );
        }

        [Test, TestMethod]
        public void BadEscaping() {
            // Make sure attribute value quoting works correctly. See: #6093
            var attrbad = CQ.Create("<input type='hidden' value='2' name='foo.baz' id='attrbad1'/><input type='hidden' value='2' name='foo[baz]' id='attrbad2'/>");

            // [CsQuery] doesn't currently require the . to be escaped in an attribute selector, why would we??
            // both of these can be parsed

            //broken( "Attribute not escaped", "input[name=foo.baz]",attrbad );
            //broken( "Attribute not escaped", "input[name=foo[baz]]",attrbad );

            var res = attrbad["input[name=foo.baz]"];
            Assert.AreEqual(res.Single(), attrbad["input:first"].Single());

            // [CsQuery] see must quote things with stoppers
            res = attrbad["input[name='foo[baz]']"];
            Assert.AreEqual(res.Single(), attrbad["input:last"].Single());
        }
        protected void broken<T>(string name, string selector, CQ context = null) where T : Exception
        {

            Assert.Throws<T>(() =>
            {
                var sourceContext = context ?? Sizzle;
                var test = sourceContext[selector];

            }, String.Format("Selector {0} should fail", selector));

        }
        protected void broken(string name, string selector, CQ context = null)
        {
            broken<ArgumentException>(name, selector, context);
        }
    }
}
