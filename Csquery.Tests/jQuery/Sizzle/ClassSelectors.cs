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
    public class Sizzle_ClassSelectors : SizzleTest
    {

        [Test,TestMethod]
        public void Class() {

           
            t( "Class Selector", ".blog", Arrays.String("mark","simon"));
            t( "Class Selector", ".GROUPS", Arrays.String("groups") );
            t( "Class Selector", ".blog.link", Arrays.String("simon") );
            t( "Class Selector w/ Element", "a.blog", Arrays.String("mark","simon") );
            t( "Parent Class Selector", "p .blog", Arrays.String("mark","simon") );

            t( "Class selector using UTF8", ".台北Táiběi", Arrays.String("utf8class1") );
            t( "Class selector using UTF8", ".台北", Arrays.String("utf8class1","utf8class2") );
            t( "Class selector using UTF8", ".台北Táiběi.台北", Arrays.String("utf8class1") );
            t( "Class selector using UTF8", ".台北Táiběi, .台北", Arrays.String("utf8class1","utf8class2") );
            t( "Descendant class selector using UTF8", "div .台北Táiběi", Arrays.String("utf8class1") );
            t( "Child class selector using UTF8", "form > .台北Táiběi", Arrays.String("utf8class1") );

            t( "Escaped Class", ".foo\\:bar", Arrays.String("foo:bar") );
            t( "Escaped Class", ".test\\.foo\\[5\\]bar", Arrays.String("test.foo[5]bar") );
            t( "Descendant scaped Class", "div .foo\\:bar",Arrays.String("foo:bar") );
            t( "Descendant scaped Class", "div .test\\.foo\\[5\\]bar", Arrays.String("test.foo[5]bar") );
            t( "Child escaped Class", "form > .foo\\:bar", Arrays.String("foo:bar") );
            t( "Child escaped Class", "form > .test\\.foo\\[5\\]bar", Arrays.String("test.foo[5]bar") );

            var div = document.CreateElement("div");
            div.InnerHTML = "<div class='test e'></div><div class='test'></div>";
            CollectionAssert.AreEqual( Dom[".e", div], Arrays.Create( div.FirstChild), "Finding a second class." );

            div.LastChild.ClassName = "e";

            CollectionAssert.AreEqual( Dom[".e", div], Arrays.Create(div.FirstChild, div.LastChild), "Finding a modified class." );
        }

        //test("name", function() {
        //    expect( 15 );

        //    t( "Name selector", "input[name=action]", ["text1"] );
        //    t( "Name selector with single quotes", "input[name='action']", ["text1"] );
        //    t( "Name selector with double quotes", 'input[name="action"]', ["text1"] );

        //    t( "Name selector non-input", "[name=test]", ["length", "fx-queue"] );
        //    t( "Name selector non-input", "[name=div]", ["fadein"] );
        //    t( "Name selector non-input", "*[name=iframe]", ["iframe"] );

        //    t( "Name selector for grouped input", "input[name='types[]']", ["types_all", "types_anime", "types_movie"] );

        //    var form = document.getElementById("form");
        //    deepEqual( Sizzle("input[name=action]", form), q("text1"), "Name selector within the context of another element" );
        //    deepEqual( Sizzle("input[name='foo[bar]']", form), q("hidden2"), "Name selector for grouped form element within the context of another element" );

        //    var form = jQuery("<form><input name='id'/></form>").appendTo("body");
        //    equal( Sizzle("input", form[0]).length, 1, "Make sure that rooted queries on forms (with possible expandos) work." );

        //    form.remove();

        //    var a = jQuery('<div><a id="tName1ID" name="tName1">tName1 A</a><a id="tName2ID" name="tName2">tName2 A</a><div id="tName1">tName1 Div</div></div>').appendTo('#qunit-fixture').children();

        //    equal( a.length, 3, "Make sure the right number of elements were inserted." );
        //    equal( a[1].id, "tName2ID", "Make sure the right number of elements were inserted." );

        //    equal( Sizzle("[name=tName1]")[0], a[0], "Find elements that have similar IDs" );
        //    equal( Sizzle("[name=tName2]")[0], a[1], "Find elements that have similar IDs" );
        //    t( "Find elements that have similar IDs", "#tName2ID", ["tName2ID"] );

        //    a.remove();
        //});

        //test("multiple", function() {
        //    expect(4);

        //    t( "Comma Support", "h2, #qunit-fixture p", ["qunit-banner","qunit-userAgent","firstp","ap","sndp","en","sap","first"]);
        //    t( "Comma Support", "h2 , #qunit-fixture p", ["qunit-banner","qunit-userAgent","firstp","ap","sndp","en","sap","first"]);
        //    t( "Comma Support", "h2 , #qunit-fixture p", ["qunit-banner","qunit-userAgent","firstp","ap","sndp","en","sap","first"]);
        //    t( "Comma Support", "h2,#qunit-fixture p", ["qunit-banner","qunit-userAgent","firstp","ap","sndp","en","sap","first"]);
        //});

        //test("child and adjacent", function() {
        //    expect( 34 );

        //    t( "Child", "p > a", ["simon1","google","groups","mark","yahoo","simon"] );
        //    t( "Child", "p> a", ["simon1","google","groups","mark","yahoo","simon"] );
        //    t( "Child", "p >a", ["simon1","google","groups","mark","yahoo","simon"] );
        //    t( "Child", "p>a", ["simon1","google","groups","mark","yahoo","simon"] );
        //    t( "Child w/ Class", "p > a.blog", ["mark","simon"] );
        //    t( "All Children", "code > *", ["anchor1","anchor2"] );
        //    t( "All Grandchildren", "p > * > *", ["anchor1","anchor2"] );
        //    t( "Adjacent", "#qunit-fixture a + a", ["groups"] );
        //    t( "Adjacent", "#qunit-fixture a +a", ["groups"] );
        //    t( "Adjacent", "#qunit-fixture a+ a", ["groups"] );
        //    t( "Adjacent", "#qunit-fixture a+a", ["groups"] );
        //    t( "Adjacent", "p + p", ["ap","en","sap"] );
        //    t( "Adjacent", "p#firstp + p", ["ap"] );
        //    t( "Adjacent", "p[lang=en] + p", ["sap"] );
        //    t( "Adjacent", "a.GROUPS + code + a", ["mark"] );
        //    t( "Comma, Child, and Adjacent", "#qunit-fixture a + a, code > a", ["groups","anchor1","anchor2"] );
        //    t( "Element Preceded By", "#qunit-fixture p ~ div", ["foo", "moretests","tabindex-tests", "liveHandlerOrder", "siblingTest"] );
        //    t( "Element Preceded By", "#first ~ div", ["moretests","tabindex-tests", "liveHandlerOrder", "siblingTest"] );
        //    t( "Element Preceded By", "#groups ~ a", ["mark"] );
        //    t( "Element Preceded By", "#length ~ input", ["idTest"] );
        //    t( "Element Preceded By", "#siblingfirst ~ em", ["siblingnext"] );
        //    t( "Element Preceded By, Containing", "#liveHandlerOrder ~ div em:contains('1')", ["siblingfirst"] );

        //    var siblingFirst = document.getElementById("siblingfirst");

        //    deepEqual( Sizzle("~ em", siblingFirst), q("siblingnext"), "Element Preceded By with a context." );
        //    deepEqual( Sizzle("+ em", siblingFirst), q("siblingnext"), "Element Directly Preceded By with a context." );

        //    equal( Sizzle("#listWithTabIndex").length, 1, "Parent div for next test is found via ID (#8310)" );
        //    equal( Sizzle("#listWithTabIndex li:eq(2) ~ li").length, 1, "Find by general sibling combinator (#8310)" );
        //    equal( Sizzle("#__sizzle__").length, 0, "Make sure the temporary id assigned by sizzle is cleared out (#8310)" );
        //    equal( Sizzle("#listWithTabIndex").length, 1, "Parent div for previous test is still found via ID (#8310)" );

        //    t( "Verify deep class selector", "div.blah > p > a", [] );

        //    t( "No element deep selector", "div.foo > span > a", [] );

        //    var nothiddendiv = document.getElementById("nothiddendiv");
        //    deepEqual( Sizzle("> :first", nothiddendiv), q("nothiddendivchild"), "Verify child context positional selctor" );
        //    deepEqual( Sizzle("> :eq(0)", nothiddendiv), q("nothiddendivchild"), "Verify child context positional selctor" );
        //    deepEqual( Sizzle("> *:first", nothiddendiv), q("nothiddendivchild"), "Verify child context positional selctor" );

        //    t( "Non-existant ancestors", ".fototab > .thumbnails > a", [] );
        //});

        //test("attributes", function() {
        //    expect( 46 );

        //    t( "Attribute Exists", "a[title]", ["google"] );
        //    t( "Attribute Exists", "*[title]", ["google"] );
        //    t( "Attribute Exists", "[title]", ["google"] );
        //    t( "Attribute Exists", "a[ title ]", ["google"] );

        //    t( "Attribute Equals", "a[rel='bookmark']", ["simon1"] );
        //    t( "Attribute Equals", 'a[rel="bookmark"]', ["simon1"] );
        //    t( "Attribute Equals", "a[rel=bookmark]", ["simon1"] );
        //    t( "Attribute Equals", "a[href='http://www.google.com/']", ["google"] );
        //    t( "Attribute Equals", "a[ rel = 'bookmark' ]", ["simon1"] );

        //    document.getElementById("anchor2").href = "#2";
        //    t( "href Attribute", "p a[href^=#]", ["anchor2"] );
        //    t( "href Attribute", "p a[href*=#]", ["simon1", "anchor2"] );

        //    t( "for Attribute", "form label[for]", ["label-for"] );
        //    t( "for Attribute in form", "#form [for=action]", ["label-for"] );

        //    t( "Attribute containing []", "input[name^='foo[']", ["hidden2"] );
        //    t( "Attribute containing []", "input[name^='foo[bar]']", ["hidden2"] );
        //    t( "Attribute containing []", "input[name*='[bar]']", ["hidden2"] );
        //    t( "Attribute containing []", "input[name$='bar]']", ["hidden2"] );
        //    t( "Attribute containing []", "input[name$='[bar]']", ["hidden2"] );
        //    t( "Attribute containing []", "input[name$='foo[bar]']", ["hidden2"] );
        //    t( "Attribute containing []", "input[name*='foo[bar]']", ["hidden2"] );

        //    t( "Multiple Attribute Equals", "#form input[type='radio'], #form input[type='hidden']", ["radio1", "radio2", "hidden1"] );
        //    t( "Multiple Attribute Equals", "#form input[type='radio'], #form input[type=\"hidden\"]", ["radio1", "radio2", "hidden1"] );
        //    t( "Multiple Attribute Equals", "#form input[type='radio'], #form input[type=hidden]", ["radio1", "radio2", "hidden1"] );

        //    t( "Attribute selector using UTF8", "span[lang=中文]", ["台北"] );

        //    t( "Attribute Begins With", "a[href ^= 'http://www']", ["google","yahoo"] );
        //    t( "Attribute Ends With", "a[href $= 'org/']", ["mark"] );
        //    t( "Attribute Contains", "a[href *= 'google']", ["google","groups"] );
        //    t( "Attribute Is Not Equal", "#ap a[hreflang!='en']", ["google","groups","anchor1"] );

        //    var opt = document.getElementById("option1a"),
        //        match = Sizzle.matchesSelector;

        //    opt.setAttribute("test", "");

        //    ok( match( opt, "[id*=option1][type!=checkbox]" ), "Attribute Is Not Equal Matches" );
        //    ok( match( opt, "[id*=option1]" ), "Attribute With No Quotes Contains Matches" );
        //    ok( match( opt, "[test=]" ), "Attribute With No Quotes No Content Matches" );
        //    ok( !match( opt, "[test^='']" ), "Attribute with empty string value does not match startsWith selector (^=)" );
        //    ok( match( opt, "[id=option1a]" ), "Attribute With No Quotes Equals Matches" );
        //    ok( match( document.getElementById("simon1"), "a[href*=#]" ), "Attribute With No Quotes Href Contains Matches" );

        //    t( "Empty values", "#select1 option[value='']", ["option1a"] );
        //    t( "Empty values", "#select1 option[value!='']", ["option1b","option1c","option1d"] );

        //    t( "Select options via :selected", "#select1 option:selected", ["option1a"] );
        //    t( "Select options via :selected", "#select2 option:selected", ["option2d"] );
        //    t( "Select options via :selected", "#select3 option:selected", ["option3b", "option3c"] );

        //    t( "Grouped Form Elements", "input[name='foo[bar]']", ["hidden2"] );

        //    // Uncomment if the boolHook is removed
        //    // var check2 = document.getElementById("check2");
        //    // check2.checked = true;
        //    // ok( !Sizzle.matches("[checked]", [ check2 ] ), "Dynamic boolean attributes match when they should with Sizzle.matches (#11115)" );

        //    // Make sure attribute value quoting works correctly. See: #6093
        //    var attrbad = jQuery("<input type=\"hidden\" value=\"2\" name=\"foo.baz\" id=\"attrbad1\"/><input type=\"hidden\" value=\"2\" name=\"foo[baz]\" id=\"attrbad2\"/>").appendTo("body");

        //    t( "Find escaped attribute value", "input[name=foo\\.baz]", ["attrbad1"] );
        //    t( "Find escaped attribute value", "input[name=foo\\[baz\\]]", ["attrbad2"] );

        //    t( "input[type=text]", "#form input[type=text]", ["text1", "text2", "hidden2", "name"] );
        //    t( "input[type=search]", "#form input[type=search]", ["search"] );

        //    attrbad.remove();

        //    // #6428
        //    t( "Find escaped attribute value", "#form input[name=foo\\[bar\\]]", ["hidden2"] );

        //    // #3279
        //    var div = document.createElement("div");
        //    div.innerHTML = "<div id='foo' xml:test='something'></div>";

        //    deepEqual( Sizzle( "[xml\\:test]", div ), [ div.firstChild ], "Finding by attribute with escaped characters." );
        //    div = null;
        //});

        //test("pseudo - child", function() {
        //    expect( 38 );
        //    t( "First Child", "#qunit-fixture p:first-child", ["firstp","sndp"] );
        //    t( "Last Child", "p:last-child", ["sap"] );
        //    t( "Only Child", "#qunit-fixture a:only-child", ["simon1","anchor1","yahoo","anchor2","liveLink1","liveLink2"] );
        //    t( "Empty", "ul:empty", ["firstUL"] );
        //    t( "Is A Parent", "#qunit-fixture p:parent", ["firstp","ap","sndp","en","sap","first"] );

        //    t( "First Child", "p:first-child", ["firstp","sndp"] );
        //    t( "Nth Child", "p:nth-child(1)", ["firstp","sndp"] );
        //    t( "Nth Child With Whitespace", "p:nth-child( 1 )", ["firstp","sndp"] );
        //    t( "Not Nth Child", "#qunit-fixture p:not(:nth-child(1))", ["ap","en","sap","first"] );

        //    // Verify that the child position isn't being cached improperly
        //    jQuery("p:first-child").after("<div></div>");
        //    jQuery("p:first-child").before("<div></div>").next().remove();

        //    t( "First Child", "p:first-child", [] );

        //    QUnit.reset();

        //    t( "Last Child", "p:last-child", ["sap"] );
        //    t( "Last Child", "#qunit-fixture a:last-child", ["simon1","anchor1","mark","yahoo","anchor2","simon","liveLink1","liveLink2"] );

        //    t( "Nth-child", "#qunit-fixture form#form > *:nth-child(2)", ["text1"] );
        //    t( "Nth-child", "#qunit-fixture form#form > :nth-child(2)", ["text1"] );

        //    t( "Nth-child", "#form select:first option:nth-child(-1)", [] );
        //    t( "Nth-child", "#form select:first option:nth-child(3)", ["option1c"] );
        //    t( "Nth-child", "#form select:first option:nth-child(0n+3)", ["option1c"] );
        //    t( "Nth-child", "#form select:first option:nth-child(1n+0)", ["option1a", "option1b", "option1c", "option1d"] );
        //    t( "Nth-child", "#form select:first option:nth-child(1n)", ["option1a", "option1b", "option1c", "option1d"] );
        //    t( "Nth-child", "#form select:first option:nth-child(n)", ["option1a", "option1b", "option1c", "option1d"] );
        //    t( "Nth-child", "#form select:first option:nth-child(+n)", ["option1a", "option1b", "option1c", "option1d"] );
        //    t( "Nth-child", "#form select:first option:nth-child(even)", ["option1b", "option1d"] );
        //    t( "Nth-child", "#form select:first option:nth-child(odd)", ["option1a", "option1c"] );
        //    t( "Nth-child", "#form select:first option:nth-child(2n)", ["option1b", "option1d"] );
        //    t( "Nth-child", "#form select:first option:nth-child(2n+1)", ["option1a", "option1c"] );
        //    t( "Nth-child", "#form select:first option:nth-child(2n + 1)", ["option1a", "option1c"] );
        //    t( "Nth-child", "#form select:first option:nth-child(+2n + 1)", ["option1a", "option1c"] );
        //    t( "Nth-child", "#form select:first option:nth-child(3n)", ["option1c"] );
        //    t( "Nth-child", "#form select:first option:nth-child(3n+1)", ["option1a", "option1d"] );
        //    t( "Nth-child", "#form select:first option:nth-child(3n+2)", ["option1b"] );
        //    t( "Nth-child", "#form select:first option:nth-child(3n+3)", ["option1c"] );
        //    t( "Nth-child", "#form select:first option:nth-child(3n-1)", ["option1b"] );
        //    t( "Nth-child", "#form select:first option:nth-child(3n-2)", ["option1a", "option1d"] );
        //    t( "Nth-child", "#form select:first option:nth-child(3n-3)", ["option1c"] );
        //    t( "Nth-child", "#form select:first option:nth-child(3n+0)", ["option1c"] );
        //    t( "Nth-child", "#form select:first option:nth-child(-1n+3)", ["option1a", "option1b", "option1c"] );
        //    t( "Nth-child", "#form select:first option:nth-child(-n+3)", ["option1a", "option1b", "option1c"] );
        //    t( "Nth-child", "#form select:first option:nth-child(-1n + 3)", ["option1a", "option1b", "option1c"] );
        //});

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