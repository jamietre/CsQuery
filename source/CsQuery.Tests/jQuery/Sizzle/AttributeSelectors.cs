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
    public class Sizzle_AttributeSelectors : SizzleTest
    {

        [Test,TestMethod]
        public void NameAttributeSelectors() {

        

            t( "Name selector", "input[name=action]", Arrays.String("text1") );
            t( "Name selector with single quotes", "input[name='action']", Arrays.String("text1") );
            t( "Name selector with double quotes", @"input[name=""action""]", Arrays.String("text1") );

            t( "Name selector non-input", "[name=test]", Arrays.String("length", "fx-queue") );
            t( "Name selector non-input", "[name=div]", Arrays.String("fadein") );
            t( "Name selector non-input", "*[name=iframe]", Arrays.String("iframe") );

            t( "Name selector for grouped input", "input[name='types[]']", Arrays.String("types_all", "types_anime", "types_movie") );

            var form = document.GetElementById("form");
            CollectionAssert.AreEqual( Dom["input[name=action]", form], q("text1"), "Name selector within the context of another element" );
            CollectionAssert.AreEqual( Dom["input[name='foo[bar]']", form], q("hidden2"), "Name selector for grouped form element within the context of another element" );

            var newform = jQuery("<form><input name='id'/></form>").AppendTo("body");
            Assert.AreEqual(Dom["input", newform[0]].Length, 1, "Make sure that rooted queries on forms (with possible expandos) work." );

            newform.Remove();

            var a = jQuery(@"<div><a id=""tName1ID"" name=""tName1"">tName1 A</a>
                <a id=""tName2ID"" name=""tName2"">tName2 A</a>
                <div id=""tName1"">tName1 Div</div></div>")
                   .AppendTo("#qunit-fixture").Children();

            Assert.AreEqual( a.Length, 3, "Make sure the right number of elements were inserted." );
            Assert.AreEqual( a[1].Id, "tName2ID", "Make sure the right number of elements were inserted." );

            Assert.AreEqual( Sizzle["[name=tName1]"][0], a[0], "Find elements that have similar IDs" );
            Assert.AreEqual( Sizzle["[name=tName2]"][0], a[1], "Find elements that have similar IDs" );
            t( "Find elements that have similar IDs", "#tName2ID", Arrays.String("tName2ID"));

            a.Remove();
        }

        [TestMethod, Test]
        public void Attributes()
        {

            t( "Attribute Exists", "a[title]", Arrays.String("google") );
            t( "Attribute Exists", "*[title]", Arrays.String("google") );
            t( "Attribute Exists", "[title]", Arrays.String("google") );
            t( "Attribute Exists", "a[ title ]", Arrays.String("google") );

            t( "Attribute Equals", "a[rel='bookmark']", Arrays.String("simon1") );
            t( "Attribute Equals", @"a[rel=""bookmark""]", Arrays.String("simon1") );
            t( "Attribute Equals", "a[rel=bookmark]", Arrays.String("simon1") );
            t( "Attribute Equals", "a[href='http://www.google.com/']", Arrays.String("google") );
            t( "Attribute Equals", "a[ rel = 'bookmark' ]",Arrays.String("simon1") );

            document.GetElementById("anchor2")["href"] = "#2";
            t( "href Attribute", "p a[href^=#]", Arrays.String("anchor2") );
            t( "href Attribute", "p a[href*=#]", Arrays.String("simon1", "anchor2") );

            t( "for Attribute", "form label[for]", Arrays.String("label-for") );
            t( "for Attribute in form", "#form [for=action]", Arrays.String("label-for") );

            t( "Attribute containing []", "input[name^='foo[']", Arrays.String("hidden2") );
            t( "Attribute containing []", "input[name^='foo[bar]']", Arrays.String("hidden2") );
            t( "Attribute containing []", "input[name*='[bar]']", Arrays.String("hidden2") );
            t( "Attribute containing []", "input[name$='bar]']", Arrays.String("hidden2") );
            t( "Attribute containing []", "input[name$='[bar]']", Arrays.String("hidden2") );
            t( "Attribute containing []", "input[name$='foo[bar]']", Arrays.String("hidden2") );
            t( "Attribute containing []", "input[name*='foo[bar]']", Arrays.String("hidden2") );

            t( "Multiple Attribute Equals", "#form input[type='radio'], #form input[type='hidden']", Arrays.String("radio1", "radio2", "hidden1") );
            t( "Multiple Attribute Equals", "#form input[type='radio'], #form input[type=\"hidden\"]", Arrays.String("radio1", "radio2", "hidden1"));
            t( "Multiple Attribute Equals", "#form input[type='radio'], #form input[type=hidden]", Arrays.String("radio1", "radio2", "hidden1") );

            t( "Attribute selector using UTF8", "span[lang=中文]", Arrays.String("台北") );

            t( "Attribute Begins With", "a[href ^= 'http://www']", Arrays.String("google","yahoo") );
            t( "Attribute Ends With", "a[href $= 'org/']", Arrays.String("mark") );
            t( "Attribute Contains", "a[href *= 'google']", Arrays.String("google","groups") );
            t( "Attribute Is Not Equal", "#ap a[hreflang!='en']", Arrays.String("google","groups","anchor1") );
        }



        [Test,TestMethod]
        public void MatchesSizzleSelect() {

            var opt = document.GetElementById("option1a");
            
            opt.SetAttribute("test", "");

            Assert.IsTrue( match( opt, "[id*=option1][type!=checkbox]" ), "Attribute Is Not Equal Matches" );
            Assert.IsTrue(match(opt, "[id*=option1]"), "Attribute With No Quotes Contains Matches");
            Assert.IsTrue(match(opt, "[test=]"), "Attribute With No Quotes No Content Matches");
            Assert.IsTrue(!match(opt, "[test^='']"), "Attribute with empty string value does not match startsWith selector (^=)");
            Assert.IsTrue(match(opt, "[id=option1a]"), "Attribute With No Quotes Equals Matches");
            Assert.IsTrue(match(document.GetElementById("simon1"), "a[href*=#]"), "Attribute With No Quotes Href Contains Matches");

            t( "Empty values", "#select1 option[value='']", Arrays.String("option1a") );
            t( "Empty values", "#select1 option[value!='']",  Arrays.String("option1b","option1c","option1d") );

            t( "Select options via :selected", "#select1 option:selected",  Arrays.String("option1a") );
            t( "Select options via :selected", "#select2 option:selected",  Arrays.String("option2d") );
            t( "Select options via :selected", "#select3 option:selected",  Arrays.String("option3b", "option3c") );

            t( "Grouped Form Elements", "input[name='foo[bar]']",  Arrays.String("hidden2"));

            //Uncomment if the boolHook is removed
            // [CsQuery] checked should match the selector after checked is set to true

            IDomElement check2 = document.GetElementById("check2");
            check2.Checked = true;
            // the sizzle test said assert.AreNotEqual
            Assert.IsTrue(match(check2, "[checked]"), "Dynamic boolean attributes match when they should with Sizzle.matches (#11115)");

            //Make sure attribute value quoting works correctly. See: #6093

            var attrbad = jQuery("<input type=\"hidden\" value=\"2\" name=\"foo.baz\" id=\"attrbad1\"/><input type=\"hidden\" value=\"2\" name=\"foo[baz]\" id=\"attrbad2\"/>")
                .AppendTo("body");

            t( "Find escaped attribute value", "input[name=foo\\.baz]", Arrays.String("attrbad1") );
            t( "Find escaped attribute value", "input[name=foo\\[baz\\]]", Arrays.String("attrbad2") );

            t( "input[type=text]", "#form input[type=text]", Arrays.String("text1", "text2", "hidden2", "name") );
            t( "input[type=search]", "#form input[type=search]", Arrays.String("search") );

            attrbad.Remove();

             //   #6428
            t( "Find escaped attribute value", "#form input[name=foo\\[bar\\]]", Arrays.String("hidden2") );

                //#3279
            var div = document.CreateElement("div");
            div.InnerHTML = "<div id='foo' xml:test='something'></div>";

            CollectionAssert.AreEqual( Sizzle[ "[xml\\:test]", div ], Arrays.Create<IDomObject>(div.FirstChild), "Finding by attribute with escaped characters." );
            div = null;
        

        }

    }

}