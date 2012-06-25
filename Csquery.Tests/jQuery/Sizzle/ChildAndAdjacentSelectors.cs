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

namespace CsqueryTests.jQuery.Sizzle
{
    /// <summary>
    /// Tests from sizzle.js test suite as of June 13, 2012
    /// https://github.com/jquery/sizzle/tree/master/test
    /// </summary>
    [TestClass, TestFixture]
    public class Sizzle_ChildAndAdjacentSelectors : SizzleTest
    {

        [Test, TestMethod]
        public void Multiple()
        {
            t( "Comma Support", "h2, #qunit-fixture p", Arrays.String("qunit-banner","qunit-userAgent","firstp","ap","sndp","en","sap","first"));
            t( "Comma Support", "h2 , #qunit-fixture p", Arrays.String("qunit-banner","qunit-userAgent","firstp","ap","sndp","en","sap","first"));
            t( "Comma Support", "h2 , #qunit-fixture p", Arrays.String("qunit-banner","qunit-userAgent","firstp","ap","sndp","en","sap","first"));
            t( "Comma Support", "h2,#qunit-fixture p", Arrays.String("qunit-banner","qunit-userAgent","firstp","ap","sndp","en","sap","first"));
        }
        [Test, TestMethod]
        public void ChildAndAdjacent()
        {
            t( "Child", "p > a",  Arrays.String("simon1","google","groups","mark","yahoo","simon") );
            t( "Child", "p> a",  Arrays.String("simon1","google","groups","mark","yahoo","simon") );
            t( "Child", "p >a",  Arrays.String("simon1","google","groups","mark","yahoo","simon") );
            t( "Child", "p>a",  Arrays.String("simon1","google","groups","mark","yahoo","simon") );
            t( "Child w/ Class", "p > a.blog",  Arrays.String("mark","simon") );
            t( "All Children", "code > *", Arrays.String("anchor1","anchor2") );
            t( "All Grandchildren", "p > * > *",  Arrays.String("anchor1","anchor2") );
            t( "Adjacent", "#qunit-fixture a + a",  Arrays.String("groups") );
            t( "Adjacent", "#qunit-fixture a +a",  Arrays.String("groups") );
            t( "Adjacent", "#qunit-fixture a+ a",  Arrays.String("groups") );
            t( "Adjacent", "#qunit-fixture a+a",  Arrays.String("groups") );
            t( "Adjacent", "p + p",  Arrays.String("ap","en","sap") );
            t( "Adjacent", "p#firstp + p", Arrays.String("ap") );
            t( "Adjacent", "p[lang=en] + p",  Arrays.String("sap") );
            t( "Adjacent", "a.GROUPS + code + a",  Arrays.String("mark") );
            t( "Comma, Child, and Adjacent", "#qunit-fixture a + a, code > a",  Arrays.String("groups","anchor1","anchor2") );
            t( "Element Preceded By", "#qunit-fixture p ~ div",  Arrays.String("foo", "moretests","tabindex-tests", "liveHandlerOrder", "siblingTest") );
            t( "Element Preceded By", "#first ~ div",  Arrays.String("moretests","tabindex-tests", "liveHandlerOrder", "siblingTest") );
            t( "Element Preceded By", "#groups ~ a",  Arrays.String("mark") );
            t( "Element Preceded By", "#length ~ input",  Arrays.String("idTest") );
            t( "Element Preceded By", "#siblingfirst ~ em",  Arrays.String("siblingnext"));
            t( "Element Preceded By, Containing", "#liveHandlerOrder ~ div em:contains('1')", Arrays.String("siblingfirst") );
        }

        [Test, TestMethod]
        public void ChildFromContext()
        {

            var siblingFirst = document.GetElementById("siblingfirst");

            CollectionAssert.AreEqual(Sizzle["~ em", siblingFirst], q("siblingnext"), "Element Preceded By with a context.");
            CollectionAssert.AreEqual( Sizzle["+ em", siblingFirst], q("siblingnext"), "Element Directly Preceded By with a context." );

            Assert.AreEqual(Sizzle["#listWithTabIndex"].Length, 1, "Parent div for next test is found via ID (#8310)");
            Assert.AreEqual(Sizzle["#listWithTabIndex li:eq(2) ~ li"].Length, 1, "Find by general sibling combinator (#8310)");
            Assert.AreEqual(Sizzle["#__sizzle__"].Length, 0, "Make sure the temporary id assigned by sizzle is cleared out (#8310)");
            Assert.AreEqual(Sizzle["#listWithTabIndex"].Length, 1, "Parent div for previous test is still found via ID (#8310)");

            t("Verify deep class selector", "div.blah > p > a");

            t("No element deep selector", "div.foo > span > a");
        }

        [Test, TestMethod]
        public void AdjacentFromContext() {
            var nothiddendiv = document.GetElementById("nothiddendiv");
            CollectionAssert.AreEqual( Sizzle["> :first", nothiddendiv], q("nothiddendivchild"), "Verify child context positional selctor" );
            CollectionAssert.AreEqual( Sizzle["> :eq(0)", nothiddendiv], q("nothiddendivchild"), "Verify child context positional selctor" );
            CollectionAssert.AreEqual( Sizzle["> *:first", nothiddendiv], q("nothiddendivchild"), "Verify child context positional selctor" );

            t( "Non-existant ancestors", ".fototab > .thumbnails > a");
        }

        
    }
}