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
    /// Tests from sizzle.js test suite as of June 13, 2011
    /// https://github.com/jquery/sizzle/tree/master/test
    /// </summary>
    [TestClass, TestFixture]
    public class Sizzle_IDSelectors : SizzleTest
    {

        [Test,TestMethod]
        public void ID() {

            t( "ID Selector", "#body", Arrays.String("body") );
            t( "ID Selector w/ Element", "body#body",Arrays.String("body") );
            t( "ID Selector w/ Element", "ul#first");
            t( "ID selector with existing ID descendant", "#firstp #simon1", Arrays.String("simon1") );
            t( "ID selector with non-existant descendant", "#firstp #foobar");
            t( "ID selector using UTF8", "#台北Táiběi", Arrays.String("台北Táiběi") );
            t( "Multiple ID selectors using UTF8", "#台北Táiběi, #台北", Arrays.String("台北Táiběi","台北") );
            t( "Descendant ID selector using UTF8", "div #台北", Arrays.String("台北") );
            t( "Child ID selector using UTF8", "form > #台北", Arrays.String("台北") );

            t( "Escaped ID", "#foo\\:bar", Arrays.String("foo:bar"));
            t( "Escaped ID", "#test\\.foo\\[5\\]bar", Arrays.String("test.foo[5]bar") );
            t( "Descendant escaped ID", "div #foo\\:bar", Arrays.String("foo:bar") );
            t( "Descendant escaped ID", "div #test\\.foo\\[5\\]bar", Arrays.String("test.foo[5]bar") );
            t( "Child escaped ID", "form > #foo\\:bar", Arrays.String("foo:bar") );
            t( "Child escaped ID", "form > #test\\.foo\\[5\\]bar", Arrays.String("test.foo[5]bar") );

            t( "ID Selector, child ID present", "#form > #radio1", Arrays.String("radio1") ); // bug #267
            t( "ID Selector, not an ancestor ID", "#form #first" );
            t( "ID Selector, not a child ID", "#form > #option1a" );

            t( "All Children of ID", "#foo > *", Arrays.String("sndp", "en", "sap") );
            t( "All Children of ID with no children", "#firstUL > *");

            var a = jQuery(@"<div><a name=""tName1"">tName1 A</a><a name=""tName2"">tName2 A</a>
                <div id=""tName1"">tName1 Div</div></div>").AppendTo("#qunit-fixture");

            Assert.AreEqual( Dom["#tName1"][0].Id, "tName1", "ID selector with same value for a name attribute" );
            Assert.AreEqual( Dom["#tName2"].Length, 0, "ID selector non-existing but name attribute on an A tag" );
            a.Remove();

            t( "ID Selector on Form with an input that has a name of 'id'", "#lengthtest", Arrays.String("lengthtest") );

            t( "ID selector with non-existant ancestor", "#asdfasdf #foobar" ); // bug #986

            Assert.AreEqual( 0,Dom["div#form", document.Body].Length, "ID selector within the context of another element" );

            t( "Underscore ID", "#types_all", Arrays.String("types_all") );
            t( "Dash ID", "#fx-queue", Arrays.String("fx-queue") );

            t( "ID with weird characters in it", "#name\\+value", Arrays.String("name+value") );
        }
        
    }

}