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
    /// Tests from sizzle.js test suite as of June 13, 2011
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

            // [CsQuery] these tests do not pass. 
            CollectionAssert.AreEqual(Sizzle["~ em", siblingFirst], q("siblingnext"), "Element Preceded By with a context.");
            //CollectionAssert.AreEqual( Sizzle["+ em", siblingFirst], q("siblingnext"), "Element Directly Preceded By with a context." );

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

        //[Test, TestMethod]
        //public void PseudoChild()
        //{


        //        expect( 38 );
        //        t( "First Child", "#qunit-fixture p:first-child", ["firstp","sndp"] );
        //        t( "Last Child", "p:last-child", ["sap"] );
        //        t( "Only Child", "#qunit-fixture a:only-child", ["simon1","anchor1","yahoo","anchor2","liveLink1","liveLink2"] );
        //        t( "Empty", "ul:empty", ["firstUL"] );
        //        t( "Is A Parent", "#qunit-fixture p:parent", ["firstp","ap","sndp","en","sap","first"] );

        //        t( "First Child", "p:first-child", ["firstp","sndp"] );
        //        t( "Nth Child", "p:nth-child(1)", ["firstp","sndp"] );
        //        t( "Nth Child With Whitespace", "p:nth-child( 1 )", ["firstp","sndp"] );
        //        t( "Not Nth Child", "#qunit-fixture p:not(:nth-child(1))", ["ap","en","sap","first"] );

        //        // Verify that the child position isn't being cached improperly
        //        jQuery("p:first-child").after("<div></div>");
        //        jQuery("p:first-child").before("<div></div>").next().remove();

        //        t( "First Child", "p:first-child", [] );

        //        QUnit.reset();

        //        t( "Last Child", "p:last-child", ["sap"] );
        //        t( "Last Child", "#qunit-fixture a:last-child", ["simon1","anchor1","mark","yahoo","anchor2","simon","liveLink1","liveLink2"] );

        //        t( "Nth-child", "#qunit-fixture form#form > *:nth-child(2)", ["text1"] );
        //        t( "Nth-child", "#qunit-fixture form#form > :nth-child(2)", ["text1"] );

        //        t( "Nth-child", "#form select:first option:nth-child(-1)", [] );
        //        t( "Nth-child", "#form select:first option:nth-child(3)", ["option1c"] );
        //        t( "Nth-child", "#form select:first option:nth-child(0n+3)", ["option1c"] );
        //        t( "Nth-child", "#form select:first option:nth-child(1n+0)", ["option1a", "option1b", "option1c", "option1d"] );
        //        t( "Nth-child", "#form select:first option:nth-child(1n)", ["option1a", "option1b", "option1c", "option1d"] );
        //        t( "Nth-child", "#form select:first option:nth-child(n)", ["option1a", "option1b", "option1c", "option1d"] );
        //        t( "Nth-child", "#form select:first option:nth-child(+n)", ["option1a", "option1b", "option1c", "option1d"] );
        //        t( "Nth-child", "#form select:first option:nth-child(even)", ["option1b", "option1d"] );
        //        t( "Nth-child", "#form select:first option:nth-child(odd)", ["option1a", "option1c"] );
        //        t( "Nth-child", "#form select:first option:nth-child(2n)", ["option1b", "option1d"] );
        //        t( "Nth-child", "#form select:first option:nth-child(2n+1)", ["option1a", "option1c"] );
        //        t( "Nth-child", "#form select:first option:nth-child(2n + 1)", ["option1a", "option1c"] );
        //        t( "Nth-child", "#form select:first option:nth-child(+2n + 1)", ["option1a", "option1c"] );
        //        t( "Nth-child", "#form select:first option:nth-child(3n)", ["option1c"] );
        //        t( "Nth-child", "#form select:first option:nth-child(3n+1)", ["option1a", "option1d"] );
        //        t( "Nth-child", "#form select:first option:nth-child(3n+2)", ["option1b"] );
        //        t( "Nth-child", "#form select:first option:nth-child(3n+3)", ["option1c"] );
        //        t( "Nth-child", "#form select:first option:nth-child(3n-1)", ["option1b"] );
        //        t( "Nth-child", "#form select:first option:nth-child(3n-2)", ["option1a", "option1d"] );
        //        t( "Nth-child", "#form select:first option:nth-child(3n-3)", ["option1c"] );
        //        t( "Nth-child", "#form select:first option:nth-child(3n+0)", ["option1c"] );
        //        t( "Nth-child", "#form select:first option:nth-child(-1n+3)", ["option1a", "option1b", "option1c"] );
        //        t( "Nth-child", "#form select:first option:nth-child(-n+3)", ["option1a", "option1b", "option1c"] );
        //        t( "Nth-child", "#form select:first option:nth-child(-1n + 3)", ["option1a", "option1b", "option1c"] );
        //    });

            //test("pseudo - misc", function() {
            //    expect( 21 );

            //    t( "Headers", ":header", ["qunit-header", "qunit-banner", "qunit-userAgent"] );
            //    t( "Has Children - :has()", "p:has(a)", ["firstp","ap","en","sap"] );

            //    var select = document.getElementById("select1"),
            //        match = Sizzle.matchesSelector;
            //    ok( match( select, ":has(option)" ), "Has Option Matches" );

            //    t( "Text Contains", "a:contains(Google)", ["google","groups"] );
            //    t( "Text Contains", "a:contains(Google Groups)", ["groups"] );

            //    t( "Text Contains", "a:contains(Google Groups (Link))", ["groups"] );
            //    t( "Text Contains", "a:contains((Link))", ["groups"] );

            //    var tmp = document.createElement("div");
            //    tmp.id = "tmp_input";
            //    document.body.appendChild( tmp );

            //    jQuery.each( [ "button", "submit", "reset" ], function( i, type ) {
            //        jQuery( tmp ).append( 
            //            "<input id='input_T' type='T'/><button id='button_T' type='T'>test</button>".replace(/T/g, type) );

            //        t( "Input Buttons :" + type, "#tmp_input :" + type, [ "input_" + type, "button_" + type ] );

            //        ok( match( Sizzle("#input_" + type)[0], ":" + type ), "Input Matches :" + type );
            //        ok( match( Sizzle("#button_" + type)[0], ":" + type ), "Button Matches :" + type );
            //    });

            //    document.body.removeChild( tmp );

            //    var input = document.createElement("input");
            //    input.type = "text";
            //    input.id = "focus-input";

            //    document.body.appendChild( input );
            //    input.focus();

            //    // Inputs can't be focused unless the document has focus
            //    if ( document.activeElement !== input || (document.hasFocus && !document.hasFocus()) ||
            //        (document.querySelectorAll && !document.querySelectorAll("input:focus").length) ) {
            //        ok( true, "The input was not focused. Skip checking the :focus match." );
            //        ok( true, "The input was not focused. Skip checking the :focus match." );

            //    } else {
            //        t( "Element focused", "input:focus", [ "focus-input" ] );
            //        ok( match( input, ":focus" ), ":focus Matches" );
            //    }

            //    // :active selector: this selector does not depend on document focus
            //    if ( document.activeElement === input ) {
            //        ok( match( input, ":active" ), ":active Matches" );
            //    } else {
            //        ok( true, "The input did not become active. Skip checking the :active match." );
            //    }

            //    input.blur();

            //    // When IE is out of focus, blur does not work. Force it here.
            //    if ( document.activeElement === input ) {
            //        document.body.focus();
            //    }

            //    ok( !match( input, ":focus" ), ":focus doesn't match" );
            //    ok( !match( input, ":active" ), ":active doesn't match" );
            //    document.body.removeChild( input );
            //});


            //test("pseudo - :not", function() {
            //    expect( 24 );

            //    t( "Not", "a.blog:not(.link)", ["mark"] );

            //    t( "Not - multiple", "#form option:not(:contains(Nothing),#option1b,:selected)", ["option1c", "option1d", "option2b", "option2c", "option3d", "option3e", "option4e", "option5b", "option5c"] );
            //    t( "Not - recursive", "#form option:not(:not(:selected))[id^='option3']", [ "option3b", "option3c"] );

            //    t( ":not() failing interior", "#qunit-fixture p:not(.foo)", ["firstp","ap","sndp","en","sap","first"] );
            //    t( ":not() failing interior", "#qunit-fixture p:not(div.foo)", ["firstp","ap","sndp","en","sap","first"] );
            //    t( ":not() failing interior", "#qunit-fixture p:not(p.foo)", ["firstp","ap","sndp","en","sap","first"] );
            //    t( ":not() failing interior", "#qunit-fixture p:not(#blargh)", ["firstp","ap","sndp","en","sap","first"] );
            //    t( ":not() failing interior", "#qunit-fixture p:not(div#blargh)", ["firstp","ap","sndp","en","sap","first"] );
            //    t( ":not() failing interior", "#qunit-fixture p:not(p#blargh)", ["firstp","ap","sndp","en","sap","first"] );

            //    t( ":not Multiple", "#qunit-fixture p:not(a)", ["firstp","ap","sndp","en","sap","first"] );
            //    t( ":not Multiple", "#qunit-fixture p:not(a, b)", ["firstp","ap","sndp","en","sap","first"] );
            //    t( ":not Multiple", "#qunit-fixture p:not(a, b, div)", ["firstp","ap","sndp","en","sap","first"] );
            //    t( ":not Multiple", "p:not(p)", [] );
            //    t( ":not Multiple", "p:not(a,p)", [] );
            //    t( ":not Multiple", "p:not(p,a)", [] );
            //    t( ":not Multiple", "p:not(a,p,b)", [] );
            //    t( ":not Multiple", ":input:not(:image,:input,:submit)", [] );

            //    t( "No element not selector", ".container div:not(.excluded) div", [] );

            //    t( ":not() Existing attribute", "#form select:not([multiple])", ["select1", "select2", "select5"]);
            //    t( ":not() Equals attribute", "#form select:not([name=select1])", ["select2", "select3", "select4","select5"]);
            //    t( ":not() Equals quoted attribute", "#form select:not([name='select1'])", ["select2", "select3", "select4", "select5"]);

            //    t( ":not() Multiple Class", "#foo a:not(.blog)", ["yahoo","anchor2"] );
            //    t( ":not() Multiple Class", "#foo a:not(.link)", ["yahoo","anchor2"] );
            //    t( ":not() Multiple Class", "#foo a:not(.blog.link)", ["yahoo","anchor2"] );
            //});

            //test("pseudo - position", function() {
            //    expect( 25 );

            //    t( "nth Element", "#qunit-fixture p:nth(1)", ["ap"] );
            //    t( "First Element", "#qunit-fixture p:first", ["firstp"] );
            //    t( "Last Element", "p:last", ["first"] );
            //    t( "Even Elements", "#qunit-fixture p:even", ["firstp","sndp","sap"] );
            //    t( "Odd Elements", "#qunit-fixture p:odd", ["ap","en","first"] );
            //    t( "Position Equals", "#qunit-fixture p:eq(1)", ["ap"] );
            //    t( "Position Greater Than", "#qunit-fixture p:gt(0)", ["ap","sndp","en","sap","first"] );
            //    t( "Position Less Than", "#qunit-fixture p:lt(3)", ["firstp","ap","sndp"] );

            //    t( "Check position filtering", "div#nothiddendiv:eq(0)", ["nothiddendiv"] );
            //    t( "Check position filtering", "div#nothiddendiv:last", ["nothiddendiv"] );
            //    t( "Check position filtering", "div#nothiddendiv:not(:gt(0))", ["nothiddendiv"] );
            //    t( "Check position filtering", "#foo > :not(:first)", ["en", "sap"] );
            //    t( "Check position filtering", "select > :not(:gt(2))", ["option1a", "option1b", "option1c"] );
            //    t( "Check position filtering", "select:lt(2) :not(:first)", ["option1b", "option1c", "option1d", "option2a", "option2b", "option2c", "option2d"] );
            //    t( "Check position filtering", "div.nothiddendiv:eq(0)", ["nothiddendiv"] );
            //    t( "Check position filtering", "div.nothiddendiv:last", ["nothiddendiv"] );
            //    t( "Check position filtering", "div.nothiddendiv:not(:lt(0))", ["nothiddendiv"] );

            //    t( "Check element position", "div div:eq(0)", ["nothiddendivchild"] );
            //    t( "Check element position", "div div:eq(5)", ["t2037"] );
            //    t( "Check element position", "div div:eq(28)", ["fx-queue"] );
            //    t( "Check element position", "div div:first", ["nothiddendivchild"] );
            //    t( "Check element position", "div > div:first", ["nothiddendivchild"] );
            //    t( "Check element position", "#dl div:first div:first", ["foo"] );
            //    t( "Check element position", "#dl div:first > div:first", ["foo"] );
            //    t( "Check element position", "div#nothiddendiv:first > div:first", ["nothiddendivchild"] );
            //});

            //test("pseudo - form", function() {
            //    expect( 10 );

            //    var extraTexts = jQuery("<input id=\"impliedText\"/><input id=\"capitalText\" type=\"TEXT\">").appendTo("#form");

            //    t( "Form element :input", "#form :input", ["text1", "text2", "radio1", "radio2", "check1", "check2", "hidden1", "hidden2", "name", "search", "button", "area1", "select1", "select2", "select3", "select4", "select5", "impliedText", "capitalText"] );
            //    t( "Form element :radio", "#form :radio", ["radio1", "radio2"] );
            //    t( "Form element :checkbox", "#form :checkbox", ["check1", "check2"] );
            //    t( "Form element :text", "#form :text", ["text1", "text2", "hidden2", "name", "impliedText", "capitalText"] );
            //    t( "Form element :radio:checked", "#form :radio:checked", ["radio2"] );
            //    t( "Form element :checkbox:checked", "#form :checkbox:checked", ["check1"] );
            //    t( "Form element :radio:checked, :checkbox:checked", "#form :radio:checked, #form :checkbox:checked", ["radio2", "check1"] );

            //    t( "Selected Option Element", "#form option:selected", ["option1a","option2d","option3b","option3c","option4b","option4c","option4d","option5a"] );
            //    t( "Selected Option Element are also :checked", "#form option:checked", ["option1a","option2d","option3b","option3c","option4b","option4c","option4d","option5a"] );
            //    t( "Hidden inputs should be treated as enabled. See QSA test.", "#hidden1:enabled", ["hidden1"] );

            //    extraTexts.remove();
            //});

            // Skipped for CsQuery

            //test("XML Document Selectors", function() {
            //    var xml = createWithFriesXML();
            //    expect( 9 );

            //    equal( Sizzle("foo_bar", xml).length, 1, "Element Selector with underscore" );
            //    equal( Sizzle(".component", xml).length, 1, "Class selector" );
            //    equal( Sizzle("[class*=component]", xml).length, 1, "Attribute selector for class" );
            //    equal( Sizzle("property[name=prop2]", xml).length, 1, "Attribute selector with name" );
            //    equal( Sizzle("[name=prop2]", xml).length, 1, "Attribute selector with name" );
            //    equal( Sizzle("#seite1", xml).length, 1, "Attribute selector with ID" );
            //    equal( Sizzle("component#seite1", xml).length, 1, "Attribute selector with ID" );
            //    equal( Sizzle.matches( "#seite1", Sizzle("component", xml) ).length, 1, "Attribute selector filter with ID" );
            //    ok( Sizzle.matchesSelector( xml.lastChild, "soap\\:Envelope" ), "Check for namespaced element" );
            //});
        
    }
}