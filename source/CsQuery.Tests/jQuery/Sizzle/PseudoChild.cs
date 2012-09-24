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
   
    [TestClass,TestFixture]
    public class Sizzle_PseudoChild : SizzleTest
    {
        [Test,TestMethod]
        public void PseudoChild()
        {
	        t( "First Child", "#qunit-fixture p:first-child",Arrays.String("firstp","sndp") );
	        t( "First Child (case-insensitive)", "#qunit-fixture p:FIRST-CHILD",Arrays.String("firstp","sndp"));
	        t( "Last Child", "p:last-child",Arrays.String("sap") );
	        t( "Only Child", "#qunit-fixture a:only-child",Arrays.String("simon1","anchor1","yahoo","anchor2","liveLink1","liveLink2") );
	        t( "Empty", "ul:empty",Arrays.String("firstUL") );
            t("Empty with comment node", "ol:empty", Arrays.String("qunit-tests", "empty"));
	        t( "Is A Parent", "#qunit-fixture p:parent",Arrays.String("firstp","ap","sndp","en","sap","first") );

	        t( "First Child", "p:first-child",Arrays.String("firstp","sndp") );
	        t( "First Child", ".nothiddendiv div:first-child",Arrays.String("nothiddendivchild") );
	        t( "Nth Child", "p:nth-child(1)",Arrays.String("firstp","sndp") );
	        t( "Nth Child With Whitespace", "p:nth-child( 1 )",Arrays.String("firstp","sndp") );
	        t( "Not Nth Child", "#qunit-fixture p:not(:nth-child(1))",Arrays.String("ap","en","sap","first") );

	        // Verify that the child position isn't being cached improperly
	        var firstChildren = jQuery("p:first-child").Before("<div></div>");

	        t( "No longer First Child", "p:nth-child(1)",Arrays.String() );
	        firstChildren.Prev().Remove();
	        t( "Restored First Child", "p:nth-child(1)",Arrays.String("firstp","sndp") );

            ResetQunit();

	        t( "Last Child", "p:last-child",Arrays.String("sap") );
	        t( "Last Child", "#qunit-fixture a:last-child",Arrays.String("simon1","anchor1","mark","yahoo","anchor2","simon","liveLink1","liveLink2") );

	        t( "Nth-child", "#qunit-fixture form#form > *:nth-child(2)",Arrays.String("text1") );
	        t( "Nth-child", "#qunit-fixture form#form > :nth-child(2)",Arrays.String("text1") );

	        t( "Nth-child", "#form select:first option:nth-child(-1)",Arrays.String() );
	        t( "Nth-child", "#form select:first option:nth-child(3)",Arrays.String("option1c") );
	        t( "Nth-child(case-insensitive)", "#form select:first option:NTH-child(3)",Arrays.String("option1c") );
	        t( "Nth-child", "#form select:first option:nth-child(0n+3)",Arrays.String("option1c") );
	        t( "Nth-child", "#form select:first option:nth-child(1n+0)",Arrays.String("option1a", "option1b", "option1c", "option1d") );
	        t( "Nth-child", "#form select:first option:nth-child(1n)",Arrays.String("option1a", "option1b", "option1c", "option1d") );
	        t( "Nth-child", "#form select:first option:nth-child(n)",Arrays.String("option1a", "option1b", "option1c", "option1d") );
	        t( "Nth-child", "#form select:first option:nth-child(even)",Arrays.String("option1b", "option1d") );
	        t( "Nth-child", "#form select:first option:nth-child(odd)",Arrays.String("option1a", "option1c") );
	        t( "Nth-child", "#form select:first option:nth-child(2n)",Arrays.String("option1b", "option1d") );
	        t( "Nth-child", "#form select:first option:nth-child(2n+1)",Arrays.String("option1a", "option1c") );
	        t( "Nth-child", "#form select:first option:nth-child(2n + 1)",Arrays.String("option1a", "option1c") );
	        t( "Nth-child", "#form select:first option:nth-child(+2n + 1)",Arrays.String("option1a", "option1c") );
	        t( "Nth-child", "#form select:first option:nth-child(3n)",Arrays.String("option1c") );
	        t( "Nth-child", "#form select:first option:nth-child(3n+1)",Arrays.String("option1a", "option1d") );
	        t( "Nth-child", "#form select:first option:nth-child(3n+2)",Arrays.String("option1b") );
	        t( "Nth-child", "#form select:first option:nth-child(3n+3)",Arrays.String("option1c") );
	        t( "Nth-child", "#form select:first option:nth-child(3n-1)",Arrays.String("option1b") );
	        t( "Nth-child", "#form select:first option:nth-child(3n-2)",Arrays.String("option1a", "option1d") );
	        t( "Nth-child", "#form select:first option:nth-child(3n-3)",Arrays.String("option1c") );
	        t( "Nth-child", "#form select:first option:nth-child(3n+0)",Arrays.String("option1c") );
	        t( "Nth-child", "#form select:first option:nth-child(-1n+3)",Arrays.String("option1a", "option1b", "option1c") );
	        t( "Nth-child", "#form select:first option:nth-child(-n+3)",Arrays.String("option1a", "option1b", "option1c") );
	        t( "Nth-child", "#form select:first option:nth-child(-1n + 3)",Arrays.String("option1a", "option1b", "option1c") );
                }

       [Test, TestMethod]
        public void NthChildWithLeadingOperator()
        {
            t("Nth-child", "#form select:first option:nth-child(+n)", Arrays.String("option1a", "option1b", "option1c", "option1d"));
            t("Nth-child", "#form select:first option:nth-child(+2n + 1)", Arrays.String("option1a", "option1c"));
            t("Nth-child", "#form select:first option:nth-child(-1n+3)", Arrays.String("option1a", "option1b", "option1c"));
            t("Nth-child", "#form select:first option:nth-child(-n+3)", Arrays.String("option1a", "option1b", "option1c"));
            t("Nth-child", "#form select:first option:nth-child(-1n + 3)", Arrays.String("option1a", "option1b", "option1c"));

        }
       [Test, TestMethod]
        public void Miscellaneous()
        {

            t( "Headers", ":header",  Arrays.String("qunit-header", "qunit-banner", "qunit-userAgent") );
            t( "Has Children - :has()", "p:has(a)", Arrays.String("firstp","ap","en","sap") );

            var select = document.GetElementById("select1");

            Assert.IsTrue( match( select, ":has(option)" ), "Has Option Matches" );

            t( "Text Contains", "a:contains(Google)", Arrays.String("google","groups") );

            
            t( "Text Contains", "a:contains(Google Groups)", Arrays.String("groups") );

            //Array.String(CsQuery] We don't at this time allow constructst like "a:contains(Google Groups(link))"
            // without quoting (e.g. inner parsing of parens without quotes). Deal with it. This is changed
            // from the original to quote the selection. 
            
            t( "Text Contains", "a:contains('Google Groups (Link)')", Arrays.String("groups") );
            t( "Text Contains", "a:contains('(Link)')", Arrays.String("groups") );

            var tmp = document.CreateElement("div");
            tmp.Id = "tmp_input";
            document.Body.AppendChild( tmp );

            CQ.Each( Arrays.String("button", "submit", "reset" ), ( type ) => {
                jQuery( tmp ).Append( 
                    "<input id='input_T' type='T'/><button id='button_T' type='T'>test</button>".Replace("T",type) );

                t( "Input Buttons :" + type, "#tmp_input :" + type, Arrays.String("input_" + type, "button_" + type ) );

                Assert.IsTrue( match( Sizzle["#input_" + type][0], ":" + type ), "Input Matches :" + type );
                Assert.IsTrue(match(Sizzle["#button_" + type][0], ":" + type), "Button Matches :" + type);
            });

            document.Body.RemoveChild( tmp );


            //[CsQuery] These tests are UI related and therefore not needed

            //var input = document.CreateElement("input");
            //input.Type = "text";
            //input.Id = "focus-input";

            //document.Body.AppendChild( input );
            //input.focus();

            // Inputs can't be focused unless the document has focus
            //if ( document.activeElement !== input || (document.hasFocus && !document.hasFocus()) ||
            //    (document.querySelectorAll && !document.querySelectorAll("input:focus").length) ) {
            //    ok( true, "The input was not focused. Skip checking the :focus match." );
            //    ok( true, "The input was not focused. Skip checking the :focus match." );

            //} else {
            //    t( "Element focused", "input:focus", Arrays.String("focus-input" ) );
            //    ok( match( input, ":focus" ), ":focus Matches" );
            //}

            // :active selector: this selector does not depend on document focus
            //if ( document.activeElement === input ) {
            //    ok( match( input, ":active" ), ":active Matches" );
            //} else {
            //    ok( true, "The input did not become active. Skip checking the :active match." );
            //}

            //input.blur();

            // When IE is out of focus, blur does not work. Force it here.
            //if ( document.activeElement === input ) {
            //    document.body.focus();
            //}

            //ok( !match( input, ":focus" ), ":focus doesn't match" );
            //ok( !match( input, ":active" ), ":active doesn't match" );
            //document.body.removeChild( input );
        }

      [Test, TestMethod]
        public void MultipleSubSelections()
        {          

            t( "Not", "a.blog:not(.link)", Arrays.String("mark") );

            //t( "Not - multiple", "#form option:not(:contains(Nothing),#option1b,:selected)", 
            //    Arrays.String("option1c", "option1d", "option2b", "option2c", "option3d", "option3e", "option4e", "option5b", "option5c"));
                
            //Array.String(CsQuery] option5a is implicity selected and not caught with out selector. 

            t( "Not - multiple", "#form option:not(:contains(Nothing),#option1b,:selected)", 
                Arrays.String("option1c", "option1d", "option2b", "option2c", "option3d", "option3e", "option4e", "option5a","option5b", "option5c"));
                
            t( "Not - recursive", "#form option:not(:not(:selected))[id^='option3']",  Arrays.String("option3b", "option3c"));

            t( ":not() failing interior", "#qunit-fixture p:not(.foo)", Arrays.String("firstp","ap","sndp","en","sap","first"));
            t( ":not() failing interior", "#qunit-fixture p:not(div.foo)", Arrays.String("firstp","ap","sndp","en","sap","first"));
            t( ":not() failing interior", "#qunit-fixture p:not(p.foo)", Arrays.String("firstp","ap","sndp","en","sap","first"));
            t( ":not() failing interior", "#qunit-fixture p:not(#blargh)", Arrays.String("firstp","ap","sndp","en","sap","first"));
            t( ":not() failing interior", "#qunit-fixture p:not(div#blargh)", Arrays.String("firstp","ap","sndp","en","sap","first"));
            t( ":not() failing interior", "#qunit-fixture p:not(p#blargh)", Arrays.String("firstp","ap","sndp","en","sap","first"));

            t( ":not Multiple", "#qunit-fixture p:not(a)", Arrays.String("firstp","ap","sndp","en","sap","first"));
            t( ":not Multiple", "#qunit-fixture p:not(a, b)", Arrays.String("firstp","ap","sndp","en","sap","first"));
            t( ":not Multiple", "#qunit-fixture p:not(a, b, div)", Arrays.String("firstp","ap","sndp","en","sap","first"));
            t( ":not Multiple", "p:not(p)");
            t( ":not Multiple", "p:not(a,p)");
            t( ":not Multiple", "p:not(p,a)");
            t( ":not Multiple", "p:not(a,p,b)");
            t( ":not Multiple", ":input:not(:image,:input,:submit)");

            t( "No element not selector", ".container div:not(.excluded) div");

            t( ":not() Existing attribute", "#form select:not([multiple])", Arrays.String("select1", "select2", "select5"));
            t( ":not() Equals attribute", "#form select:not([name=select1])", Arrays.String("select2", "select3", "select4","select5"));
            t( ":not() Equals quoted attribute", "#form select:not([name='select1'])", Arrays.String("select2", "select3", "select4", "select5"));

            t( ":not() Multiple Class", "#foo a:not(.blog)", Arrays.String("yahoo","anchor2"));
            t( ":not() Multiple Class", "#foo a:not(.link)", Arrays.String("yahoo","anchor2"));
            t( ":not() Multiple Class", "#foo a:not(.blog.link)", Arrays.String("yahoo","anchor2"));

        }

       [Test, TestMethod]
        public void PseudoPosition()
        {

            t("Check position filtering", "select > :not(:gt(2))", Arrays.String("option1a", "option1b", "option1c"));
            t("Check position filtering", "div#nothiddendiv:eq(0)", Arrays.String("nothiddendiv"));


            //Array.String(CsQuery] nth is not documented. Therefore we have not implemented it.
            //t( "nth Element", "#qunit-fixture p:nth(1)", Arrays.String("ap") );

            t( "First Element", "#qunit-fixture p:first", Arrays.String("firstp") );
            t( "Last Element", "p:last", Arrays.String("first") );
            t( "Even Elements", "#qunit-fixture p:even", Arrays.String("firstp","sndp","sap") );
            t( "Odd Elements", "#qunit-fixture p:odd", Arrays.String("ap","en","first") );
            t( "Position Equals", "#qunit-fixture p:eq(1)", Arrays.String("ap") );
            t( "Position Greater Than", "#qunit-fixture p:gt(0)", Arrays.String("ap","sndp","en","sap","first") );
            t( "Position Less Than", "#qunit-fixture p:lt(3)", Arrays.String("firstp","ap","sndp") );

            
            t( "Check position filtering", "div#nothiddendiv:last", Arrays.String("nothiddendiv") );
            t( "Check position filtering", "div#nothiddendiv:not(:gt(0))", Arrays.String("nothiddendiv") );
            t( "Check position filtering", "#foo > :not(:first)", Arrays.String("en", "sap") );
            
            t( "Check position filtering", "select:lt(2) :not(:first)", Arrays.String("option1b", "option1c", "option1d", "option2a", "option2b", "option2c", "option2d") );
            t( "Check position filtering", "div.nothiddendiv:eq(0)", Arrays.String("nothiddendiv") );
            t( "Check position filtering", "div.nothiddendiv:last", Arrays.String("nothiddendiv") );
            t( "Check position filtering", "div.nothiddendiv:not(:lt(0))", Arrays.String("nothiddendiv") );

            t( "Check element position", "div div:eq(0)", Arrays.String("nothiddendivchild") );
            t( "Check element position", "div div:eq(5)", Arrays.String("t2037") );
            
            //Array.String(CsQuery] It's not at all clear how jQuery is getting "fx-queue" at position 28; running 'div div' shows it at 15.
            // Think this is a bug in jQuery. :eq refers to the position in a selection set, so it should be in the set so far
            // which is 15.

            //t( "Check element position", "div div:eq(28)", Arrays.String("fx-queue") );
            t( "Check element position", "div div:first", Arrays.String("nothiddendivchild") );
            t( "Check element position", "div > div:first", Arrays.String("nothiddendivchild") );
            t( "Check element position", "#dl div:first div:first", Arrays.String("foo") );
            t( "Check element position", "#dl div:first > div:first", Arrays.String("foo") );
            t( "Check element position", "div#nothiddendiv:first > div:first", Arrays.String("nothiddendivchild") );
        }

      [Test, TestMethod]
        public void PseudoForm()
        {

            var extraTexts = jQuery("<input id=\"impliedText\"/><input id=\"capitalText\" type=\"TEXT\">").AppendTo("#form");
            t("Form element :input", "#form :input", Arrays.String("text1", "text2", "radio1", "radio2", "check1", "check2", "hidden1", "hidden2", "name", "search", "button", "area1", "select1", "select2", "select3", "select4", "select5", "impliedText", "capitalText"));
            
            t("Form element :radio", "#form :radio", Arrays.String("radio1", "radio2"));

            
            
            t( "Form element :checkbox", "#form :checkbox", Arrays.String("check1", "check2") );
            t( "Form element :text", "#form :text", Arrays.String("text1", "text2", "hidden2", "name", "impliedText", "capitalText") );
            t( "Form element :radio:checked", "#form :radio:checked", Arrays.String("radio2") );
            t( "Form element :checkbox:checked", "#form :checkbox:checked", Arrays.String("check1") );
            t( "Form element :radio:checked, :checkbox:checked", "#form :radio:checked, #form :checkbox:checked", Arrays.String("radio2", "check1") );

            //Array.String(CsQuery] option1a and option5a are not selected per note on option groups above

            t("Selected Option Element", "#form option:selected", Arrays.String( "option2d", "option3b", "option3c", "option4b", "option4c", "option4d"));
            //t( "Selected Option Element", "#form option:selected", Arrays.String("option1a","option2d","option3b","option3c","option4b","option4c","option4d","option5a") );
            
            t( "Selected Option Element are also :checked", "#form option:checked", Arrays.String("option1a","option2d","option3b","option3c","option4b","option4c","option4d","option5a") );
            t( "Hidden inputs should be treated as enabled. See QSA test.", "#hidden1:enabled", Arrays.String("hidden1") );

            extraTexts.Remove();
        }

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