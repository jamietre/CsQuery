using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute;
using MsTestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using TestContext = NUnit.Framework.TestContext;
using CsQuery;
using CsQuery.ExtensionMethods;
using CsQuery.Utility;

namespace CsQuery.Tests.jQuery
{
    [TestClass,TestFixture]
    public class Manipulation : CsQueryTest
    {

        Func<object, object> bareObj = (input) => { return input; };
        Func<string,Func<int,string,string>> functionReturningObj = (input) =>
        {
            Func<int, string,string> returnFunc = (index,inputInner) => { return inputInner; };
            return returnFunc;
        };
        [SetUp]
        public override void FixtureSetUp()
        {
            base.FixtureSetUp();
            ResetQunit();
        }

        [TestMethod, Test]
        public void Text()
        {
            var expected = "This link has class=\"blog\": Simon Willison's Weblog";
            Assert.AreEqual(expected,jQuery("#sap").Text(), "Check for merged text of more then one element.");

            // Check serialization of text values
            var textNode = document.CreateTextNode("foo");

            Assert.AreEqual("foo", jQuery(textNode).Text(), "Text node was retreived from .Text().");
            Assert.AreNotEqual("", jQuery(Dom.Document).Text(), "Retrieving text for the document retrieves all text (#10724).");
        }


        protected void TestText(Func<string, object> valueObj)
        {
            var val = valueObj("<div><b>Hello</b> cruel world!</div>");
             jQuery("#foo").Text(val.ToString());
            var text = jQuery("#foo")[0].InnerHTML;
            Assert.AreEqual(text, "&lt;div&gt;&lt;b&gt;Hello&lt;/b&gt; cruel world!&lt;/div&gt;", "Check escaped text" );

            // using contents will get comments regular, text, and comment nodes
            var j = jQuery("#nonnodes").Contents();
            j.Text((string)valueObj("hi!"));
            Assert.AreEqual( jQuery(j[0]).Text(), "hi!", "Check node,textnode,comment with Text()" );
            Assert.AreEqual( j[1].NodeValue, " there ", "Check node,textnode,comment with Text()" );

            // Blackberry 4.6 doesn't maintain comments in the DOM
            Assert.AreEqual( jQuery("#nonnodes")[0].ChildNodes.Length < 3 ? 8 :(int)j[2].NodeType, 8, "Check node,textnode,comment with Text()" );
        }
        [TestMethod, Test]
        public void TestTextString()
        {
            TestText(bareObj);
        }

        //test("text(Function)", function() {
        //    testText(functionReturningObj);
        //});

//test("text(Function) with incoming value", function() {
//    expect(2);

//    var old = "This link has class=\"blog\": Simon Willison's Weblog";

//    jQuery("#sap").text(function(i, val) {
//        Assert.AreEqual( val, old, "Make sure the incoming value is correct." );
//        return "foobar";
//    });

//    Assert.AreEqual( jQuery("#sap").Text(), "foobar", "Check for merged text of more then one element." );

//    Assert.AreEqual(;
//});

        void TestWrap(Func<object,object> val) {
        
            var defaultText = "Try them out:";
            string textResult = jQuery("#first").Wrap((string)val("<div class='red'><span></span></div>")).Text();
            Assert.AreEqual( defaultText, defaultText, "Check for wrapping of on-the-fly html" );
            Assert.IsTrue( jQuery("#first").Parent().Parent().Is(".red"), "Check if wrapper has class 'red'" );

            ResetQunit();

            var result = jQuery("#first").Wrap(Dom.Document.GetElementById("empty"));
            result= result.Parent();

            Assert.IsTrue( result.Is("ol"), "Check for element wrapping" );
            Assert.AreEqual( result.Text(), defaultText, "Check for element wrapping" );

            // using contents will get comments regular, text, and comment nodes
            var j = jQuery("#nonnodes").Contents();
            j.Wrap((string)val("<i></i>"));

            // Blackberry 4.6 doesn't maintain comments in the DOM
            Assert.AreEqual( jQuery("#nonnodes > i").Length, jQuery("#nonnodes")[0].ChildNodes.Length, "Check node,textnode,comment wraps ok" );
            Assert.AreEqual( jQuery("#nonnodes > i").Text(), j.Text(), "Check node,textnode,comment wraps doesn't hurt text" );

            // Try wrapping a disconnected node
            //var cacheLength = 0;
            //for (var i in jQuery.cache) {
            //    cacheLength++;
            //}

            j = jQuery("<label/>").Wrap((string)val("<li/>"));
            Assert.AreEqual( j[0].NodeName.ToUpper(), "LABEL", "Element is a label" );
            Assert.AreEqual( j[0].ParentNode.NodeName.ToUpper(), "LI", "Element has been wrapped" );

            //for (i in jQuery.cache) {
            //    cacheLength--;
            //}
           // Assert.AreEqual(cacheLength, 0, "No memory leak in jQuery.cache (bug #7165)");

            // Wrap an element containing a text node
            j = jQuery("<span/>").Wrap("<div>test</div>");
            Assert.AreEqual( (int)j[0].PreviousSibling.NodeType, 3, "Make sure the previous node is a text element" );
            Assert.AreEqual( j[0].ParentNode.NodeName.ToUpper(), "DIV", "And that we're in the div element." );

            // Try to wrap an element with multiple elements (should fail)
            j = jQuery("<div><span></span></div>").Children().Wrap("<p></p><div></div>");
            Assert.AreEqual( j[0].ParentNode.ParentNode.ChildNodes.Length, 1, "There should only be one element wrapping." );
            Assert.AreEqual( j.Length, 1, "There should only be one element (no cloning)." );
            Assert.AreEqual( j[0].ParentNode.NodeName.ToUpper(), "P", "The span should be in the paragraph." );

            // Wrap an element with a jQuery set
            j = jQuery("<span/>").Wrap(jQuery("<div></div>"));
            Assert.AreEqual( j[0].ParentNode.NodeName.ToLower(), "div", "Wrapping works." );

            //// Wrap an element with a jQuery set and event
            result = jQuery("<div></div>");
            //.click(function(){
            //    Assert.IsTrue(true, "Event triggered.");

            //    // Remove handlers on detached elements
            //    result.unbind();
            //    jQuery(this).unbind();
            //});

            j = jQuery("<span/>").Wrap(result);
            Assert.AreEqual( j[0].ParentNode.NodeName.ToLower(), "div", "Wrapping works." );

    //        j.Parent().trigger("click");
   
     
            //jQuery("#check1").click(function() {
            //    var checkbox = this;
            //    Assert.IsTrue( checkbox.checked, "Checkbox's state is erased after wrap() action, see #769" );
            //    jQuery(checkbox).Wrap(val( "<div id='c1' style='display:none;'></div>" ));
            //    Assert.IsTrue( checkbox.checked, "Checkbox's state is erased after wrap() action, see #769" );
            //}).click();

            // clean up attached elements
            ResetQunit();
        }

        [Test, TestMethod]
        public void TestWrapString()
        {
            TestWrap(bareObj);
        }

        //test("wrap(Function)", function() {
        //    testWrap(functionReturningObj);
        //});

        //test("wrap(Function) with index (#10177)", function() {
        //    var expectedIndex = 0,
        //        targets = jQuery("#qunit-fixture p");

        //    expect(targets.Length);
        //    targets.Wrap(function(i) {
        //        Assert.AreEqual( i, expectedIndex, "Check if the provided index (" + i + ") is as expected (" + expectedIndex + ")" );
        //        expectedIndex++;

        //        return "<div id='wrap_index_'" + i + "'></div>";
        //    });
        //});

        [Test, TestMethod]
        public void TestWrapStringConsecutive()
        {
            //test("wrap(String) consecutive elements (#10177)", function() {
            var targets = jQuery("#qunit-fixture p");

        
            targets.Wrap("<div class='wrapper'></div>");

            targets.Each((int i, IDomObject e) =>
            {
                CQ _this = jQuery(e);

                Assert.IsTrue( _this.Parent().Is(".wrapper"), "Check each elements parent is correct (.wrapper)" );
                Assert.AreEqual( _this.Siblings().Length, 0, "Each element should be wrapped individually" );
            });
        }


        protected void TestWrapAll(Func<object, object> val)
        {
            var prev = jQuery("#firstp")[0].PreviousSibling;
            var p = jQuery("#firstp,#first")[0].ParentNode;

            var result = jQuery("#firstp,#first").WrapAll((string)val("<div class='red'><div class='tmp'></div></div>"));
            Assert.AreEqual(result.Parent().Length, 1, "Check for wrapping of on-the-fly html");
            Assert.IsTrue(jQuery("#first").Parent().Parent().Is(".red"), "Check if wrapper has class 'red'");
            Assert.IsTrue(jQuery("#firstp").Parent().Parent().Is(".red"), "Check if wrapper has class 'red'");
            Assert.AreEqual(jQuery("#first").Parent().Parent()[0].PreviousSibling, prev, "Correct Previous Sibling");
            Assert.AreEqual(jQuery("#first").Parent().Parent()[0].ParentNode, p, "Correct Parent");


            prev = jQuery("#firstp")[0].PreviousSibling;
            p = jQuery("#first")[0].ParentNode;
            result = jQuery("#firstp,#first").WrapAll((IDomElement)val(document.GetElementById("empty")));
            Assert.AreEqual(jQuery("#first").Parent()[0], jQuery("#firstp").Parent()[0], "Same Parent");
            Assert.AreEqual(jQuery("#first").Parent()[0].PreviousSibling, prev, "Correct Previous Sibling");
            Assert.AreEqual(jQuery("#first").Parent()[0].ParentNode, p, "Correct Parent");

        }
        [Test, TestMethod]
        public void TestWrapAllString()
        {
            TestWrapAll(bareObj);

        }

  
        protected void TestWrapInner(Func<object,object> val) {
            var num = jQuery("#first").Children().Length;
            var result = jQuery("#first").WrapInner((string)val("<div class='red'><div id='tmp'></div></div>"));
            Assert.AreEqual( jQuery("#first").Children().Length, 1, "Only one child" );
            Assert.IsTrue( jQuery("#first").Children().Is(".red"), "Verify Right Element" );
            Assert.AreEqual( jQuery("#first").Children().Children().Children().Length, num, "Verify Elements Intact" );

            ResetQunit();
            num = jQuery("#first").Html("foo<div>test</div><div>test2</div>").Children().Length;
            result = jQuery("#first").WrapInner((string)val("<div class='red'><div id='tmp'></div></div>"));
            Assert.AreEqual( jQuery("#first").Children().Length, 1, "Only one child" );
            Assert.IsTrue( jQuery("#first").Children().Is(".red"), "Verify Right Element" );
            Assert.AreEqual( jQuery("#first").Children().Children().Children().Length, num, "Verify Elements Intact" );

            ResetQunit();
            num = jQuery("#first").Children().Length;
            result = jQuery("#first").WrapInner((IDomElement)val(document.GetElementById("empty")));
            Assert.AreEqual( jQuery("#first").Children().Length, 1, "Only one child" );
            Assert.IsTrue( jQuery("#first").Children().Is("#empty"), "Verify Right Element" );
            Assert.AreEqual( jQuery("#first").Children().Children().Length, num, "Verify Elements Intact" );

            var div = jQuery("<div/>");
            div.WrapInner((string)val("<span></span>"));
            Assert.AreEqual(div.Children().Length, 1, "The contents were wrapped.");
            Assert.AreEqual(div.Children()[0].NodeName.ToLower(), "span", "A span was inserted.");
        }
        [Test, TestMethod]
        public void TestWrappInnerString() {
            TestWrapInner(bareObj);
        }

        //test("wrapInner(Function)", function() {
        //    testWrapInner(functionReturningObj)
        //});

        [Test, TestMethod]
        public void Unwrap()
        {
            jQuery("body").Append("  <div id='unwrap' style='display: none;'> <div id='unwrap1'> <span class='unwrap'>a</span> <span class='unwrap'>b</span> </div> <div id='unwrap2'> <span class='unwrap'>c</span> <span class='unwrap'>d</span> </div> <div id='unwrap3'> <b><span class='unwrap unwrap3'>e</span></b> <b><span class='unwrap unwrap3'>f</span></b> </div> </div>");

            var abcd = jQuery("#unwrap1 > span, #unwrap2 > span").Get();
            var abcdef = jQuery("#unwrap span").Get();

            Assert.AreEqual(jQuery("#unwrap1 span").Add("#unwrap2 span:first").Unwrap().Length, 3, "make #unwrap1 and #unwrap2 go away");
            Assert.AreEqual(jQuery("#unwrap > span").Get(), abcd, "all four spans should still exist");

            Assert.AreEqual(jQuery("#unwrap3 span").Unwrap().Get(), jQuery("#unwrap3 > span").Get(), "make all b in #unwrap3 go away");

            Assert.AreEqual(jQuery("#unwrap3 span").Unwrap().Get(), jQuery("#unwrap > span.unwrap3").Get(), "make #unwrap3 go away");
            Assert.AreEqual(jQuery("#unwrap").Children().Get(), abcdef, "#unwrap only contains 6 child spans");

            Assert.AreEqual(jQuery("#unwrap > span").Unwrap().Get(), jQuery("body > span.unwrap").Get(), "make the 6 spans become children of body");

            //CHANGE: We explcitly DO want to allow unwrapping children of body. Not relevant.

            //Assert.AreEqual(jQuery("body > span.unwrap").Unwrap().Get(), jQuery("body > span.unwrap").Get(), "can't unwrap children of body");
            //Assert.AreEqual(jQuery("body > span.unwrap").Unwrap().Get(), abcdef, "can't unwrap children of body");

            Assert.AreEqual(jQuery("body > span.unwrap").Get(), abcdef, "body contains 6 .unwrap child spans");
        }
        
        protected void Append(Func<object,object> valueObj)
        {
            var defaultText = "Try them out:";
            var result = jQuery("#first").Append((string)valueObj("<b>buga</b>"));
            Assert.AreEqual( result.Text(), defaultText + "buga", "Check if text appending works" );

            jQuery("#select3").Append((string)valueObj("<option value='appendTest'>Append Test</option>"));
            Assert.AreEqual(jQuery("#select3").Append((string)valueObj("<option value='appendTest'>Append Test</option>"))
                .Find("option:last-child")
                .Attr("value"), "appendTest", "Appending html options to select element");

            ResetQunit();
            var expected = "This link has class=\"blog\": Simon Willison's WeblogTry them out:";
            jQuery("#sap").Append(document.GetElementById("first"));
            Assert.AreEqual( jQuery("#sap").Text(), expected, "Check for appending of element" );

            
            expected = "This link has class=\"blog\": Simon Willison's WeblogTry them out:Yahoo";
            jQuery("#sap").Append(Objects.Enumerate(document.GetElementById("first"), document.GetElementById("yahoo")));
            Assert.AreEqual( jQuery("#sap").Text(), expected, "Check for appending of array of elements" );

            ResetQunit();
            expected = "This link has class=\"blog\": Simon Willison's WeblogYahooTry them out:";
            jQuery("#sap").Append(jQuery("#yahoo, #first"));
            Assert.AreEqual( jQuery("#sap").Text(), expected, "Check for appending of jQuery object" );

            
            jQuery("#sap").Append("5");
            Assert.IsTrue( jQuery("#sap")[0].InnerHTML.IndexOf("5")>0, "Check for appending a number" );

            
            jQuery("#sap").Append((string)valueObj( " text with spaces " ));
            Assert.IsTrue( jQuery("#sap")[0].InnerHTML.IndexOf(" text with spaces ")>0, "Check for appending text with spaces" );

            ResetQunit();
            var old = jQuery("#sap").Children().Length;
            Assert.IsTrue( jQuery("#sap").Append(Objects.EmptyEnumerable<IDomObject>()).Children().Length ==old, "Check for appending an empty array." );
            Assert.IsTrue(jQuery("#sap").Append((string)valueObj("")).Children().Length == old, "Check for appending an empty string.");
            Assert.IsTrue(jQuery("#sap").Append(document.GetElementsByTagName("foo")).Children().Length == old, "Check for appending an empty nodelist.");

            
            jQuery("form").Append((string)valueObj("<input name='radiotest' type='radio' checked='checked' />"));
            jQuery("form input[name=radiotest]").Each((int i, IDomObject e) =>
            {
                Assert.IsTrue( jQuery(e).Is(":checked"), "Append checked radio");
            }).Remove();

            
            jQuery("form").Append((string)valueObj("<input name='radiotest' type='radio' checked    =   'checked' />"));
            jQuery("form input[name=radiotest]").Each((int i, IDomObject e) =>
            {
                Assert.IsTrue( jQuery(e).Is(":checked"), "Append alternately formated checked radio");
            }).Remove();

            
            jQuery("form").Append((string)valueObj("<input name='radiotest' type='radio' checked />"));
            jQuery("form input[name=radiotest]").Each((int i, IDomObject e) =>
            {
                Assert.IsTrue( jQuery(e).Is(":checked"), "Append HTML5-formated checked radio");
            }).Remove();

            
            jQuery("#sap").Append(document.GetElementById("form"));
            Assert.AreEqual( jQuery("#sap>form").Length, 1, "Check for appending a form" ); // Bug #910

            
            //var pass = true;
            //try {
            //    var body = jQuery("#iframe body")[0].contentWindow.document.body;

            //    pass = false;
            //    jQuery( body ).Append(valueObj( "<div>test</div>" ));
            //    pass = true;
            //} 
            //catch() 
            //{}

            //Assert.IsTrue( pass, "Test for appending a DOM node to the contents of an IFrame" );

            jQuery("<fieldset/>").AppendTo("#form").Append((string)valueObj( "<legend id='legend'>test</legend>" ));
            t( "Append legend", "#legend", Objects.Enumerate("legend") );

            
            jQuery("#select1").Append((string)valueObj( "<OPTION>Test</OPTION>" ));
            Assert.AreEqual( jQuery("#select1 option:last").Text(), "Test", "Appending &lt;OPTION&gt; (all caps)" );

            jQuery("#table").Append((string)valueObj( "<colgroup></colgroup>" ));
            Assert.IsTrue( jQuery("#table colgroup").Length>0, "Append colgroup" );

            jQuery("#table colgroup").Append((string)valueObj( "<col/>" ));
            Assert.IsTrue( jQuery("#table colgroup col").Length>0, "Append col" );

   
            jQuery("#table").Append((string)valueObj( "<caption></caption>" ));
            Assert.IsTrue( jQuery("#table caption").Length>0, "Append caption" );

            jQuery("form:last")
                .Append((string)valueObj( "<select id='appendSelect1'></select>" ))
                .Append((string)valueObj( "<select id='appendSelect2'><option>Test</option></select>" ));

            t( "Append Select", "#appendSelect1, #appendSelect2", Objects.Enumerate("appendSelect1", "appendSelect2") );

            Assert.AreEqual( "Two nodes", jQuery("<div />").Append("Two", " nodes").Text(), "Appending two text nodes (#4011)" );

            // using contents will get comments regular, text, and comment nodes
            var j = jQuery("#nonnodes").Contents();
            var d = jQuery("<div/>").AppendTo("#nonnodes").Append(j);
            Assert.AreEqual( jQuery("#nonnodes").Length, 1, "Check node,textnode,comment append moved leaving just the div" );
            Assert.IsTrue( d.Contents().Length >= 2, "Check node,textnode,comment append works" );
            d.Contents().AppendTo("#nonnodes");
            d.Remove();
            Assert.IsTrue( jQuery("#nonnodes").Contents().Length >= 2, "Check node,textnode,comment append cleanup worked" );

            var input = jQuery("<input />").AttrSet("{ type: 'checkbox', checked: true }").AppendTo("#testForm");
            Assert.AreEqual( input[0].Checked, true, "A checked checkbox that is appended stays checked" );


            var radios = jQuery("input:radio[name='R1']");
            
            
            var radioNot = jQuery("<input type='radio' name='R1' checked='checked'/>").InsertAfter( radios );
            var radio = radios.Eq(1);

            radioNot[0].Checked = false;
            radios.Parent().Wrap("<div></div>");
            //Assert.AreEqual( radio[0].Checked, true, "Reappending radios uphold which radio is checked" );
            //Assert.AreEqual( radioNot[0].Checked, false, "Reappending radios uphold not being checked" );
            

            var prev = jQuery("#sap").Children().Length;

            jQuery("#sap").Append(
                "<span></span>",
                "<span></span>",
                "<span></span>"
            );

            Assert.AreEqual( jQuery("#sap").Children().Length, prev + 3, "Make sure that multiple arguments works." );

        }
        [Test, TestMethod]
        public void TestAppend()
        {
            Append(bareObj);
        }


        //test("Append(Function)", function() {
        //    testAppend(functionReturningObj);
        //});

        [Test,TestMethod]
        public void AppendWithIncoming() {
            var defaultText = "Try them out:";
            var old = jQuery("#first").Html();

            var result = jQuery("#first").Append((i, val)=>{
                Assert.AreEqual( val, old, "Make sure the incoming value is correct." );
                return "<b>buga</b>";
            });
            Assert.AreEqual( result.Text(), defaultText + "buga", "Check if text appending works" );

            var select = jQuery("#select3");
            old = select.Html();

            var adding = select.Append((i, val) =>
            {
                Assert.AreEqual(val, old, "Make sure the incoming value is correct.");
                return "<option value='appendTest'>Append Test</option>";
            });
            Assert.AreEqual( adding.Find("option:last-child").Attr("value"), "appendTest", "Appending html options to select element");

            ResetQunit();
            var expected = "This link has class=\"blog\": Simon Willison's WeblogTry them out:";
            old = jQuery("#sap").Html();

            jQuery("#sap").Append((i, val)=>{
                Assert.AreEqual( val, old, "Make sure the incoming value is correct." );
                return document.GetElementById("first");
            });
            Assert.AreEqual( jQuery("#sap").Text(), expected, "Check for appending of element" );

            ResetQunit();
            expected = "This link has class=\"blog\": Simon Willison's WeblogTry them out:Yahoo";
            old = jQuery("#sap").Html();

            jQuery("#sap").Append((i, val)=>{
                Assert.AreEqual( val, old, "Make sure the incoming value is correct." );
                return Objects.Enumerate(document.GetElementById("first"), document.GetElementById("yahoo"));
            });
            Assert.AreEqual( jQuery("#sap").Text(), expected, "Check for appending of array of elements" );

            ResetQunit();
            expected = "This link has class=\"blog\": Simon Willison's WeblogYahooTry them out:";
            old = jQuery("#sap").Html();

            jQuery("#sap").Append((i, val)=>{
                Assert.AreEqual( val, old, "Make sure the incoming value is correct." );
                return jQuery("#yahoo, #first").Elements;
            });
            Assert.AreEqual( jQuery("#sap").Text(), expected, "Check for appending of jQuery object" );

            ResetQunit();
            old = jQuery("#sap").Html();

            jQuery("#sap").Append((i, val)=>{
                Assert.AreEqual( val, old, "Make sure the incoming value is correct." );
                return "5";
            });
            Assert.IsTrue( jQuery("#sap")[0].InnerHTML.IndexOf("5")>=0, "Check for appending a number" );

            ResetQunit();
        }
        [Test, TestMethod]
        public void AppendHTML5Sectioning()
        {
            jQuery("#qunit-fixture").Append("<article style='font-size:10px'><section><aside>HTML5 elements</aside></section></article>");

                var article = jQuery("article");
                var aside = jQuery("aside");

                Assert.AreEqual( article.Css("font-size"), "10px", "HTML5 elements are styleable");
                Assert.AreEqual(aside.Length, 1, "HTML5 elements do not collapse their children");
        }

        [Test, TestMethod]
        public void AppendTo()
        {
            var defaultText = "Try them out:";
            jQuery("<b>buga</b>").AppendTo("#first");
            Assert.AreEqual(jQuery("#first").Text(), defaultText + "buga", "Check if text appending works");
            Assert.AreEqual(jQuery("<option value='appendTest'>Append Test</option>")
                .AppendTo("#select3").Parent().Find("option:last-child").Attr("value"), "appendTest", "Appending html options to select element");

            ResetQunit();
            var l = jQuery("#first").Children().Length + 2;
            jQuery("<strong>test</strong>");
            jQuery("<strong>test</strong>");
            var newChildren =jQuery(Objects.Enumerate(jQuery("<strong>test</strong>")[0], jQuery("<strong>test</strong>")[0]));
            newChildren.AppendTo("#first");
            Assert.AreEqual(jQuery("#first").Children().Length, l, "Make sure the elements were inserted.");
            Assert.AreEqual(jQuery("#first").Children().Last()[0].NodeName.ToLower(), "strong", "Verify the last element.");

            ResetQunit();
            var expected = "This link has class=\"blog\": Simon Willison's WeblogTry them out:";
            jQuery(document.GetElementById("first")).AppendTo("#sap");
            Assert.AreEqual(jQuery("#sap").Text(), expected, "Check for appending of element");

            ResetQunit();
            //CHANGE: this is the originally desired test result using Append instead
            expected = "This link has class=\"blog\": Simon Willison's WeblogTry them out:Yahoo";
            jQuery("#sap").Append(Objects.Enumerate(document.GetElementById("first"), document.GetElementById("yahoo")));
            Assert.AreEqual(jQuery("#sap").Text(), expected, "Check for appending of array of elements (Append)");

            ResetQunit();

            expected = "This link has class=\"blog\": Simon Willison's WeblogTry them out:Yahoo";
            jQuery(Objects.Enumerate(document.GetElementById("first"), document.GetElementById("yahoo"))).AppendTo("#sap");
            Assert.AreEqual(jQuery("#sap").Text(), expected, "Check for appending of array of elements");

            ResetQunit();
            Assert.IsTrue(jQuery(document.CreateElement("script")).AppendTo("body").Length > 0, "Make sure a disconnected script can be appended.");

            ResetQunit();
            expected = "This link has class=\"blog\": Simon Willison's WeblogYahooTry them out:";
            jQuery("#yahoo, #first").AppendTo("#sap");
            Assert.AreEqual(jQuery("#sap").Text(), expected, "Check for appending of jQuery object");

            ResetQunit();
            jQuery("#select1").AppendTo("#foo");
            t( "Append select", "#foo select", Objects.Enumerate("select1") );

            //ResetQunit();
            //var div = jQuery("<div/>").click(function(){
            //    Assert.IsTrue(true, "Running a cloned click.");
            //});
            
            // this part is just to ensure the DOM looks the same as the jQuery tests since we didn't do the click thing about
            // 
            var div = jQuery("<div/>");
            div.AppendTo("#qunit-fixture, #moretests");

            //jQuery("#qunit-fixture div:last").click();
            //jQuery("#moretests div:last").click();

            ResetQunit();
             div = jQuery("<div/>").AppendTo("#qunit-fixture, #moretests");

            Assert.AreEqual(div.Length, 2, "appendTo returns the inserted elements");
            Assert.AreEqual(div[0].NodeName, "DIV", "appendTo returns the inserted elements");

            div.AddClass("test");

            Assert.IsTrue(jQuery("#qunit-fixture div:last").HasClass("test"), "appendTo element was modified after the insertion");
            Assert.IsTrue(jQuery("#moretests div:last").HasClass("test"), "appendTo element was modified after the insertion");

            ResetQunit();

            div = jQuery("<div/>");
            jQuery("<span>a</span><b>b</b>").Filter("span").AppendTo(div);

            Assert.AreEqual(div.Children().Length, 1, "Make sure the right number of children were inserted.");

            div = jQuery("#moretests div");

            var num = jQuery("#qunit-fixture div").Length;
            div.Remove().AppendTo("#qunit-fixture");

            Assert.AreEqual(jQuery("#qunit-fixture div").Length, num, "Make sure all the removed divs were inserted.");

            //ResetQunit();

            //stop();
            //jQuery.getScript('data/test.js', function() {
            //    jQuery('script[src*="data\\/test\\.js"]').Remove();
            //    start();
            //});
        }
        [Test, TestMethod]
        public void Prepend()
        {
            var defaultText = "Try them out:";
            var result = jQuery("#first").Prepend("<b>buga</b>" );
            Assert.AreEqual( result.Text(), "buga" + defaultText, "Check if text prepending works" );
            Assert.AreEqual( jQuery("#select3").Prepend( "<option value='prependTest'>Prepend Test</option>" )
                .Find("option:first-child").Attr("value"), "prependTest", "Prepending html options to select element");

            ResetQunit();
            var expected = "Try them out:This link has class=\"blog\": Simon Willison's Weblog";
            jQuery("#sap").Prepend( document.GetElementById("first") );
            Assert.AreEqual( jQuery("#sap").Text(), expected, "Check for prepending of element" );

            ResetQunit();
            expected = "Try them out:YahooThis link has class=\"blog\": Simon Willison's Weblog";
            jQuery("#sap").Prepend(Objects.Enumerate(document.GetElementById("first"), document.GetElementById("yahoo")));
            Assert.AreEqual( jQuery("#sap").Text(), expected, "Check for prepending of array of elements" );

            ResetQunit();
            expected = "YahooTry them out:This link has class=\"blog\": Simon Willison's Weblog";
            jQuery("#sap").Prepend(jQuery("#yahoo, #first"));
            Assert.AreEqual( jQuery("#sap").Text(), expected, "Check for prepending of jQuery object" );
        }

        [Test, TestMethod]
        public void PrependTo()
        {
            var defaultText = "Try them out:";
            jQuery("<b>buga</b>").PrependTo("#first");
            Assert.AreEqual( jQuery("#first").Text(), "buga" + defaultText, "Check if text prepending works" );
            Assert.AreEqual( jQuery("<option value='prependTest'>Prepend Test</option>").PrependTo("#select3")
                .Parent().Find("option:first-child").Attr("value"), "prependTest", "Prepending html options to select element");

            ResetQunit();
            var expected = "Try them out:This link has class=\"blog\": Simon Willison's Weblog";
            jQuery(document.GetElementById("first")).PrependTo("#sap");
            Assert.AreEqual( jQuery("#sap").Text(), expected, "Check for prepending of element" );

            ResetQunit();
            expected = "Try them out:YahooThis link has class=\"blog\": Simon Willison's Weblog";
            jQuery(Objects.Enumerate(document.GetElementById("first"), document.GetElementById("yahoo"))).PrependTo("#sap");
            Assert.AreEqual( jQuery("#sap").Text(), expected, "Check for prepending of array of elements" );

            ResetQunit();
            expected = "YahooTry them out:This link has class=\"blog\": Simon Willison's Weblog";
            jQuery("#yahoo, #first").PrependTo("#sap");
            Assert.AreEqual( jQuery("#sap").Text(), expected, "Check for prepending of jQuery object" );

            ResetQunit();
            jQuery("<select id='prependSelect1'></select>").PrependTo("form:last");
            jQuery("<select id='prependSelect2'><option>Test</option></select>").PrependTo("form:last");

            t( "Prepend Select", "#prependSelect2, #prependSelect1", Objects.Enumerate("prependSelect2", "prependSelect1") );
        }
        [Test,TestMethod]
        public void Before() {
            var expected = "This is a normal link: bugaYahoo";
            jQuery("#yahoo").Before("<b>buga</b>");
            Assert.AreEqual( jQuery("#en").Text(), expected, "Insert String before" );

            ResetQunit();
            expected = "This is a normal link: Try them out:Yahoo";
            jQuery("#yahoo").Before(document.GetElementById("first"));
            Assert.AreEqual( jQuery("#en").Text(), expected, "Insert element before" );

            ResetQunit();
            expected = "This is a normal link: Try them out:diveintomarkYahoo";
            jQuery("#yahoo").Before(Objects.Enumerate(document.GetElementById("first"), document.GetElementById("mark")));
            Assert.AreEqual( jQuery("#en").Text(), expected, "Insert array of elements before" );

            ResetQunit();
            expected = "This is a normal link: diveintomarkTry them out:Yahoo";
            jQuery("#yahoo").Before( jQuery("#mark, #first") );
            Assert.AreEqual( jQuery("#en").Text(), expected, "Insert jQuery before" );

            var set = jQuery("<div/>").Before("<span>test</span>");
            Assert.AreEqual( set[0].NodeName.ToLower(), "span", "Insert the element before the disconnected node." );
            Assert.AreEqual( set.Length, 2, "Insert the element before the disconnected node." );
        }
        [Test, TestMethod]
        public void BeforeAfterEmpty()
        {
            // This test was removed when migrating to validator.nu. It's not clear to me why this
            // behaviour should work this way. An empty selector means don't do anything because nothing
            // matched.  Every other jQuery method would do nothing. This seems inconsistent, and if you
            // added anything other than text it stops acting like a disconnected result anyway. 
            
            //var res = jQuery("#notInTheDocument").Before("(").After(")");
            //Assert.AreEqual(res.Length, 2, "didn't choke on empty object");

            var res = CQ.Create("()");
            Assert.AreEqual(res.WrapAll("<div/>").Parent().Text(), "()", "correctly appended text");
        }
        [Test, TestMethod]
        public void InsertBefore()
        {
            var expected = "This is a normal link: bugaYahoo";
            jQuery("<b>buga</b>").InsertBefore("#yahoo");
            Assert.AreEqual( jQuery("#en").Text(), expected, "Insert String before" );

            ResetQunit();
            expected = "This is a normal link: Try them out:Yahoo";
            jQuery(document.GetElementById("first")).InsertBefore("#yahoo");
            Assert.AreEqual( jQuery("#en").Text(), expected, "Insert element before" );

            ResetQunit();
            expected = "This is a normal link: Try them out:diveintomarkYahoo";
            jQuery(Objects.Enumerate(document.GetElementById("first"), document.GetElementById("mark"))).InsertBefore("#yahoo");
            Assert.AreEqual( jQuery("#en").Text(), expected, "Insert array of elements before" );

            ResetQunit();
            expected = "This is a normal link: diveintomarkTry them out:Yahoo";
            jQuery("#mark, #first").InsertBefore("#yahoo");
            Assert.AreEqual( jQuery("#en").Text(), expected, "Insert jQuery before" );
        }
        [Test, TestMethod]
        public void After()
        {
            var expected = "This is a normal link: Yahoobuga";
            jQuery("#yahoo").After( "<b>buga</b>" );
            Assert.AreEqual( jQuery("#en").Text(), expected, "Insert String after" );

            ResetQunit();
            expected = "This is a normal link: YahooTry them out:";
            jQuery("#yahoo").After( document.GetElementById("first") );
            Assert.AreEqual( jQuery("#en").Text(), expected, "Insert element after" );

           ResetQunit();
            expected = "This is a normal link: YahooTry them out:diveintomark";
            jQuery("#yahoo").After(Objects.Enumerate(document.GetElementById("first"), document.GetElementById("mark")));
            Assert.AreEqual( jQuery("#en").Text(), expected, "Insert array of elements after" );

           ResetQunit();
            expected = "This is a normal link: YahoodiveintomarkTry them out:";
            jQuery("#yahoo").After(jQuery("#mark, #first") );
            Assert.AreEqual( jQuery("#en").Text(), expected, "Insert jQuery after" );

            var set = jQuery("<div/>").After("<span>test</span>");
            Assert.AreEqual( set[1].NodeName.ToLower(), "span", "Insert the element after the disconnected node." );
            Assert.AreEqual( set.Length, 2, "Insert the element after the disconnected node." );
        }
        [Test, TestMethod]
        public void InsertAfter()
        {
            var expected = "This is a normal link: Yahoobuga";
            jQuery("<b>buga</b>").InsertAfter("#yahoo");
            Assert.AreEqual( jQuery("#en").Text(), expected, "Insert String after" );

            ResetQunit();
            expected = "This is a normal link: YahooTry them out:";
            jQuery(document.GetElementById("first")).InsertAfter("#yahoo");
            Assert.AreEqual( jQuery("#en").Text(), expected, "Insert element after" );

            ResetQunit();
            expected = "This is a normal link: YahooTry them out:diveintomark";
            jQuery(Objects.Enumerate(document.GetElementById("first"), document.GetElementById("mark"))).InsertAfter("#yahoo");
            Assert.AreEqual( jQuery("#en").Text(), expected, "Insert array of elements after" );

            ResetQunit();
            expected = "This is a normal link: YahoodiveintomarkTry them out:";
            jQuery("#mark, #first").InsertAfter("#yahoo");
            Assert.AreEqual( jQuery("#en").Text(), expected, "Insert jQuery after" );
        }
        [Test, TestMethod]
        public void ReplaceWith()
        {
            jQuery("#yahoo").ReplaceWith( "<b id='replace'>buga</b>" );
            Assert.IsTrue( jQuery("#replace").Length==1, "Replace element with string" );
            Assert.IsTrue( jQuery("#yahoo").Length==0, "Verify that original element is gone, after string" );

            ResetQunit();
            jQuery("#yahoo").ReplaceWith( document.GetElementById("first") );
            Assert.IsTrue( jQuery("#first").Length==1, "Replace element with element" );
            Assert.IsTrue( jQuery("#yahoo").Length==0, "Verify that original element is gone, after element" );

            ResetQunit();
            jQuery("#qunit-fixture").Append("<div id='bar'><div id='baz'</div></div>");
            jQuery("#baz").ReplaceWith("Baz");
            Assert.AreEqual( jQuery("#bar").Text(),"Baz", "Replace element with text" );
            Assert.IsTrue( jQuery("#baz").Length==0, "Verify that original element is gone, after element" );

            ResetQunit();
            jQuery("#yahoo").ReplaceWith(Objects.Enumerate(document.GetElementById("first"), document.GetElementById("mark")));
            Assert.IsTrue( jQuery("#first").Length==1, "Replace element with array of elements" );
            Assert.IsTrue( jQuery("#mark").Length==1, "Replace element with array of elements" );
            Assert.IsTrue( jQuery("#yahoo").Length==0, "Verify that original element is gone, after array of elements" );

            ResetQunit();
            jQuery("#yahoo").ReplaceWith( jQuery("#mark, #first") );
            Assert.IsTrue( jQuery("#first").Length==1, "Replace element with set of elements" );
            Assert.IsTrue( jQuery("#mark").Length==1, "Replace element with set of elements" );
            Assert.IsTrue( jQuery("#yahoo").Length==0, "Verify that original element is gone, after set of elements" );

            //ResetQunit();
            //var tmp = jQuery("<div/>").AppendTo("body").click(function(){ Assert.IsTrue(true, "Newly bound click run." ); });
            //var y = jQuery("<div/>").AppendTo("body").click(function(){ Assert.IsTrue(true, "Previously bound click run." ); });
            //var child = y.Append("<b>test</b>").Find("b").click(function(){ Assert.IsTrue(true, "Child bound click run." ); return false; });

            //y.ReplaceWith( tmp );

            //tmp.click();
            //y.click(); // Shouldn't be run
            //child.click(); // Shouldn't be run

            //tmp.Remove();
            //y.Remove();
            //child.Remove();

            //ResetQunit();
            //y = jQuery("<div/>").AppendTo("body").click(function(){ Assert.IsTrue(true, "Previously bound click run." ); });
            //var child2 = y.Append("<u>test</u>").Find("u").click(function(){ Assert.IsTrue(true, "Child 2 bound click run." ); return false; });

            //y.ReplaceWith( child2 );
            
            //child2.click();

            //y.Remove();
            //child2.Remove();

            ResetQunit();

            var set = jQuery("<div/>").ReplaceWith("<span>test</span>");
            Assert.AreEqual( set[0].NodeName.ToLower(), "span", "Replace the disconnected node." );
            Assert.AreEqual( set.Length, 1, "Replace the disconnected node." );

            var non_existant = jQuery("#does-not-exist").ReplaceWith( "<b>should not throw an error</b>" );
            Assert.AreEqual( non_existant.Length, 0, "Length of non existant element." );

            var div = jQuery("<div class='replacewith'></div>").AppendTo("body");
            // TODO: Work on jQuery(...) inline script execution
            //$div.ReplaceWith("<div class='replacewith'></div><script>" +
                //"Assert.AreEqual(jQuery('.ReplaceWith').Length, 1, 'Check number of elements in page.');" +
                //"</script>");
            Assert.AreEqual(jQuery(".replacewith").Length, 1, "Check number of elements in page.");
            jQuery(".ReplaceWith").Remove();

            ResetQunit();

            jQuery("#qunit-fixture").Append("<div id='replaceWith'></div>");
            Assert.AreEqual( jQuery("#qunit-fixture").Find("div[id=replaceWith]").Length, 1, "Make sure only one div exists." );

            jQuery("#replaceWith").ReplaceWith("<div id='replaceWith'></div>" );
            Assert.AreEqual( jQuery("#qunit-fixture").Find("div[id=replaceWith]").Length, 1, "Make sure only one div exists." );

            jQuery("#replaceWith").ReplaceWith( "<div id='replaceWith'></div>" );
            Assert.AreEqual( jQuery("#qunit-fixture").Find("div[id=replaceWith]").Length, 1, "Make sure only one div exists." );
        }

        [Test, TestMethod]
        public void ReplaceWithString()
        {

            Assert.AreEqual(jQuery("#foo p").Length, 3, "ensuring that test data has not changed");

            jQuery("#foo p").ReplaceWith("<span>bar</span>");
            Assert.AreEqual(jQuery("#foo span").Length, 3, "verify that all the three original element have been replaced");
            Assert.AreEqual(jQuery("#foo p").Length, 0, "verify that all the three original element have been replaced");
        }
        [Test, TestMethod]
        public void ReplaceAll()
        {

            jQuery("<b id='replace'>buga</b>").ReplaceAll("#yahoo");
            Assert.IsTrue( jQuery("#replace").Length>0, "Replace element with string" );
            Assert.IsTrue( jQuery("#yahoo").Length==0, "Verify that original element is gone, after string" );

            ResetQunit();
            jQuery(document.GetElementById("first")).ReplaceAll("#yahoo");
            Assert.IsTrue( jQuery("#first").Length>0, "Replace element with element" );
            Assert.IsTrue( jQuery("#yahoo").Length==0, "Verify that original element is gone, after element" );

            ResetQunit();
            jQuery(new IDomElement[] {document.GetElementById("first"), document.GetElementById("mark")})
                .ReplaceAll("#yahoo");
            Assert.IsTrue( jQuery("#first").Length>0, "Replace element with array of elements" );
            Assert.IsTrue( jQuery("#mark").Length>0, "Replace element with array of elements" );
            Assert.IsTrue( jQuery("#yahoo").Length==0, "Verify that original element is gone, after array of elements" );

            ResetQunit();
            jQuery("#mark, #first").ReplaceAll("#yahoo");
            Assert.IsTrue( jQuery("#first").Length>0, "Replace element with set of elements" );
            Assert.IsTrue( jQuery("#mark").Length>0, "Replace element with set of elements" );
            Assert.IsTrue( jQuery("#yahoo").Length==0, "Verify that original element is gone, after set of elements" );
        
        }
        //test("jQuery.Clone() (#8017)", function() {

        //    expect(2);

        //    Assert.IsTrue( jQuery.Clone && jQuery.isFunction( jQuery.Clone ) , "jQuery.Clone() utility exists and is a function.");

        //    var main = jQuery("#qunit-fixture")[0],
        //            clone = jQuery.Clone( main );

        //    Assert.AreEqual( main.childNodes.Length, clone.childNodes.Length, "Simple child length to ensure a large dom tree copies correctly" );
        //});

        //test("clone() (#8070)", function () {
        //    expect(2);

        //    jQuery("<select class='test8070'></select><select class='test8070'></select>").AppendTo("#qunit-fixture");
        //    var selects = jQuery(".test8070");
        //    selects.Append("<OPTION>1</OPTION><OPTION>2</OPTION>");

        //    Assert.AreEqual( selects[0].childNodes.Length, 2, "First select got two nodes" );
        //    Assert.AreEqual( selects[1].childNodes.Length, 2, "Second select got two nodes" );

        //    selects.Remove();
        //});

        [Test, TestMethod]
        public void Clone()
        {
            Assert.AreEqual( "This is a normal link: Yahoo", jQuery("#en").Text(), "Assert text for #en" );
            var clone = jQuery("#yahoo").Clone();
            Assert.AreEqual( "Try them out:Yahoo", jQuery("#first").Append(clone).Text(), "Check for clone" );
            Assert.AreEqual( "This is a normal link: Yahoo", jQuery("#en").Text(), "Reassert text for #en" );

            var cloneTags = Objects.Enumerate(
                "<table/>", "<tr/>", "<td/>", "<div/>",
                "<button/>", "<ul/>", "<ol/>", "<li/>",
                "<input type='checkbox' />", "<select/>", "<option/>", "<textarea/>",
                "<tbody/>", "<thead/>", "<tfoot/>", "<iframe/>");
            
            foreach (var tag in cloneTags) {
                var j = jQuery(tag);
                Assert.AreEqual( j[0].NodeName, j.Clone()[0].NodeName, "Clone a " + tag);
            }

            // using contents will get comments regular, text, and comment nodes
            var cl = jQuery("#nonnodes").Contents().Clone();
            Assert.IsTrue( cl.Length >= 2, "Check node,textnode,comment clone works (some browsers delete comments on clone)" );

            var div = jQuery("<div><ul><li>test</li></ul></div>");
            //.click(function(){
            //    Assert.IsTrue( true, "Bound event still exists." );
            //});

            // There is no "Clone(true) since there are no events
            //clone = div.Clone(true);
            clone = div.Clone();
            // manually clean up detached elements
            div.Remove();

            //div = clone.Clone(true);
            div = clone.Clone();

            // manually clean up detached elements
            clone.Remove();

            Assert.AreEqual( div.Length, 1, "One element cloned" );
            Assert.AreEqual( div[0].NodeName.ToUpper(), "DIV", "DIV element cloned" );
            //div.trigger("click");

            // manually clean up detached elements
            div.Remove();

            div = jQuery("<div/>").Append(Objects.Enumerate(document.CreateElement("table"), document.CreateElement("table")));
            //div.Find("table").click(function(){
            //    Assert.IsTrue( true, "Bound event still exists." );
            //});

            //clone = div.Clone(true);
            clone = div.Clone();

            Assert.AreEqual( clone.Length, 1, "One element cloned" );
            Assert.AreEqual( clone[0].NodeName.ToUpper(), "DIV", "DIV element cloned" );
            //clone.Find("table:last").trigger("click");

            // manually clean up detached elements
            div.Remove();
            clone.Remove();

            var divEvt = jQuery("<div><ul><li>test</li></ul></div>");
            //.click(function(){
            //    Assert.IsTrue( false, "Bound event still exists after .Clone()." );
            //});
            var cloneEvt = divEvt.Clone();

            // Make sure that doing .Clone() doesn't clone events
            //cloneEvt.trigger("click");

            cloneEvt.Remove();
            divEvt.Remove();

            // Test both html() and clone() for <embed and <object types
            div = jQuery("<div/>").Html("<embed height='355' width='425' src='http://www.youtube.com/v/3KANI2dpXLw&amp;hl=en'></embed>");

            //clone = div.Clone(true);
            clone = div.Clone();
            Assert.AreEqual( clone.Length, 1, "One element cloned" );
            Assert.AreEqual( clone.Html(), div.Html(), "Element contents cloned" );
            Assert.AreEqual( clone[0].NodeName.ToUpper(), "DIV", "DIV element cloned" );

            // this is technically an invalid object, but because of the special
            // classid instantiation it is the only kind that IE has trouble with,
            // so let's test with it too.
            div = jQuery("<div/>").Html("<object height='355' width='425' classid='clsid:D27CDB6E-AE6D-11cf-96B8-444553540000'>  <param name='movie' value='http://www.youtube.com/v/3KANI2dpXLw&amp;hl=en'>  <param name='wmode' value='transparent'> </object>");

            
            //clone = div.Clone(true);
            clone = div.Clone();

            Assert.AreEqual( clone.Length, 1, "One element cloned" );
            // Assert.AreEqual( clone.Html(), div.Html(), "Element contents cloned" );
            Assert.AreEqual( clone[0].NodeName.ToUpper(), "DIV", "DIV element cloned" );

            // and here's a valid one.
            div = jQuery("<div/>").Html("<object height='355' width='425' type='application/x-shockwave-flash' data='http://www.youtube.com/v/3KANI2dpXLw&amp;hl=en'>  <param name='movie' value='http://www.youtube.com/v/3KANI2dpXLw&amp;hl=en'>  <param name='wmode' value='transparent'> </object>");

            //clone = div.Clone(true);
            clone = div.Clone();
            Assert.AreEqual( clone.Length, 1, "One element cloned" );
            Assert.AreEqual( clone.Html(), div.Html(), "Element contents cloned" );
            Assert.AreEqual( clone[0].NodeName.ToUpper(), "DIV", "DIV element cloned" );

            div = jQuery("<div/>").DataSet("{ a: true }");
            //clone = div.Clone(true);
            clone = div.Clone();

            Assert.AreEqual( clone.Data("a"), true, "Data cloned." );
            clone.Data("a", false);
            Assert.AreEqual( clone.Data("a"), false, "Ensure cloned element data object was correctly modified" );
            Assert.AreEqual( div.Data("a"), true, "Ensure cloned element data object is copied, not referenced" );

            // manually clean up detached elements
            div.Remove();
            clone.Remove();

            var form = document.CreateElement("form");
            form["action"]= "/test/";
            var div2 = document.CreateElement("div");
            div2.AppendChild( document.CreateTextNode("test") );
            form.AppendChild( div2 );

            Assert.AreEqual( jQuery(form).Clone().Children().Length, 1, "Make sure we just get the form back." );

            Assert.AreEqual( jQuery("body").Clone().Children()[0].Id, "qunit-header", "Make sure cloning body works" );
        }


        [Test, TestMethod]
        public void CloneFormElement()
        {

            var element = jQuery("<select><option>Foo</option><option selected>Bar</option></select>");

            Assert.AreEqual( element.Clone().Find("option:selected").Val(), element.Find("option:selected")
                .Val(), "Selected option cloned correctly" );

            element = jQuery("<input type='checkbox' value='foo'>").Attr("checked", "checked");
            var clone = element.Clone();

            Assert.AreEqual( clone.Is(":checked"), element.Is(":checked"), "Checked input cloned correctly" );
            Assert.AreEqual( clone[0].DefaultValue, "foo", "Checked input defaultValue cloned correctly" );

            // defaultChecked also gets set now due to setAttribute in attr, is this check still valid?
            // Assert.AreEqual( clone[0].defaultChecked, !jQuery.support.noCloneChecked, "Checked input defaultChecked cloned correctly" );

            element = jQuery("<input type='text' value='foo'>");
            clone = element.Clone();
            Assert.AreEqual( clone[0].DefaultValue, "foo", "Text input defaultValue cloned correctly" );

            element = jQuery("<textarea>foo</textarea>");
            clone = element.Clone();
            Assert.AreEqual( clone[0].DefaultValue, "foo", "Textarea defaultValue cloned correctly" );
        }
        //test("clone(multiple selected options) (Bug #8129)", function() {
        //    expect(1);
        //    var element = jQuery("<select><option>Foo</option><option selected>Bar</option><option selected>Baz</option></select>");

        //    Assert.AreEqual( element.Clone().Find("option:selected").Length, element.Find("option:selected").Length, "Multiple selected options cloned correctly" );

        //});



       [Test,TestMethod]
       public void Html() {

            //jQuery.scriptorder = 0;
            var valueObj = new Func<object,string>(text=>{return text.ToString();});

            var div = jQuery("#qunit-fixture > div");
            div.Html(valueObj("<b>test</b>"));
            var pass = true;
            for ( var i = 0; i < div.Length; i++ ) {
                if ( div.Get(i).ChildNodes.Length != 1 ) pass = false;
            }
            Assert.IsTrue( pass, "Set HTML" );

            div = jQuery("<div/>").Html( valueObj("<div id='parent_1'><div id='child_1'/></div><div id='parent_2'/>") );

            //Assert.AreEqual( div.Children().Length, 2, "Make sure two child nodes exist." );
            

            Assert.AreEqual( div.Children().Children().Length, 1, "Make sure that a grandchild exists." );
            // Assert.AreEqual("<div id=\"parent_1\"><div id=\"child_1\"></div><div id=\"parent_2\"></div></div>", 
            // div.Html());

            var space = jQuery("<div/>").Html(valueObj("&#160;"))[0].InnerHTML;
            Assert.IsTrue("/^\xA0$|^&nbsp;$|^&#160;$/".RegexTest(space), "Make sure entities are passed through correctly.");
            Assert.AreEqual( jQuery("<div/>").Html(valueObj("&amp;"))[0].InnerHTML, "&amp;", "Make sure entities are passed through correctly." );

            jQuery("#qunit-fixture").Html(valueObj("<style>.foobar{color:green;}</style>"));

            Assert.AreEqual( jQuery("#qunit-fixture").Children().Length, 1, "Make sure there is a child element." );
            Assert.AreEqual( jQuery("#qunit-fixture").Children()[0].NodeName.ToUpper(), "STYLE", "And that a style element was inserted." );

            ResetQunit();
            // using contents will get comments regular, text, and comment nodes
            var j = jQuery("#nonnodes").Contents();
            j.Html(valueObj("<b>bold</b>"));

            // this is needed, or the expando added by jQuery unique will yield a different html
            j.Find("b").RemoveData();

            Assert.AreEqual( j.Html().RegexReplace(@"/ xmlns=""[^""]+""/g", "").ToLower(), "<b>bold</b>", "Check node,textnode,comment with html()" );

            jQuery("#qunit-fixture").Html(valueObj("<select/>"));
            jQuery("#qunit-fixture select").Html(valueObj("<option>O1</option><option selected='selected'>O2</option><option>O3</option>"));
            Assert.AreEqual( jQuery("#qunit-fixture select").Val(), "O2", "Selected option correct" );

            div = jQuery("<div />");
            Assert.AreEqual( div.Html(valueObj( 5 )).Html(), "5", "Setting a number as html" );
            Assert.AreEqual( div.Html(valueObj( 0 )).Html(), "0", "Setting a zero as html" );

            var div2 = jQuery("<div/>");
            var insert = "&lt;div&gt;hello1&lt;/div&gt;";
            Assert.AreEqual( div2.Html(insert).Html().RegexReplace(@"/>/g", "&gt;"), insert, "Verify escaped insertion." );
            Assert.AreEqual( div2.Html("x" + insert).Html().RegexReplace(@"/>/g", "&gt;"), "x" + insert, "Verify escaped insertion." );
            Assert.AreEqual( div2.Html(" " + insert).Html().RegexReplace(@"/>/g", "&gt;"), " " + insert, "Verify escaped insertion." );

            var map = jQuery("<map/>").Html(valueObj("<area id='map01' shape='rect' coords='50,50,150,150' href='http://www.jquery.com/' alt='jQuery'>"));

            Assert.AreEqual( map[0].ChildNodes.Length, 1, "The area was inserted." );
            Assert.AreEqual( map[0].FirstChild.NodeName.ToLower(), "area", "The area was inserted." );

            ResetQunit();

            // CHANGE - doesn't evalute scripts of course
            //jQuery("#qunit-fixture").Html(valueObj("<script type='something/else'>Assert.IsTrue( false, 'Non-script evaluated.' );</script><script type='text/javascript'>Assert.IsTrue( true, 'text/javascript is evaluated.' );</script><script>Assert.IsTrue( true, 'No type is evaluated.' );</script><div><script type='text/javascript'>Assert.IsTrue( true, 'Inner text/javascript is evaluated.' );</script><script>Assert.IsTrue( true, 'Inner No type is evaluated.' );</script><script type='something/else'>Assert.IsTrue( false, 'Non-script evaluated.' );</script></div>"));
            jQuery("#qunit-fixture").Html(valueObj("<script type='something/else'>Assert.IsTrue( false, 'Non-script evaluated.' );</script><div><script type='something/else'>Assert.IsTrue( false, 'Non-script evaluated.' );</script></div>"));

            var child = jQuery("#qunit-fixture").Find("script");

            
            // Assert.AreEqual( child.Length, 2, "Make sure that two non-JavaScript script tags are left." );
            Assert.AreEqual( child[0].GetAttribute("type"), "something/else", "Verify type of script tag." );
            Assert.AreEqual( child[1].GetAttribute("type"), "something/else", "Verify type of script tag." );

            jQuery("#qunit-fixture").Html(valueObj("<script>Assert.IsTrue( true, 'Test repeated injection of script.' );</script>"));
            jQuery("#qunit-fixture").Html(valueObj("<script>Assert.IsTrue( true, 'Test repeated injection of script.' );</script>"));
            jQuery("#qunit-fixture").Html(valueObj("<script>Assert.IsTrue( true, 'Test repeated injection of script.' );</script>"));

            jQuery("#qunit-fixture").Html(valueObj("<script type='text/javascript'>Assert.IsTrue( true, 'jQuery().Html().evalScripts() Evals Scripts Twice in Firefox, see #975 (1)' );</script>"));

            jQuery("#qunit-fixture").Html(valueObj("foo <form><script type='text/javascript'>Assert.IsTrue( true, 'jQuery().Html().evalScripts() Evals Scripts Twice in Firefox, see #975 (2)' );</script></form>"));

            //jQuery("#qunit-fixture").Html(valueObj("<script>Assert.AreEqual(jQuery.scriptorder++, 0, 'Script is executed in order');Assert.AreEqual(jQuery('#scriptorder').Length, 1,'Execute after html (even though appears before)')<\/script><span id='scriptorder'><script>Assert.AreEqual(jQuery.scriptorder++, 1, 'Script (nested) is executed in order');Assert.AreEqual(jQuery('#scriptorder').Length, 1,'Execute after html')<\/script></span><script>Assert.AreEqual(jQuery.scriptorder++, 2, 'Script (unnested) is executed in order');Assert.AreEqual(jQuery('#scriptorder').Length, 1,'Execute after html')</script>"));
        }

        // Detach is synonymous with remove, no test for it
        [Test,TestMethod]
        public void Remove() {
            

            var first = jQuery("#ap").Children(":first");
            first.Data("foo", "bar");

            jQuery("#ap").Children().Remove();
            Assert.IsTrue( jQuery("#ap").Text().Length > 10, "Check text is not removed" );
            Assert.AreEqual( jQuery("#ap").Children().Length, 0, "Check remove" );

            //Assert.AreEqual( first.data("foo"), method == "remove" ? null : "bar" );

            ResetQunit();
            jQuery("#ap").Children().Remove("a");
            Assert.IsTrue( jQuery("#ap").Text().Length > 10, "Check text is not removed" );
            Assert.AreEqual( jQuery("#ap").Children().Length, 1, "Check filtered remove" );

            jQuery("#ap").Children().Remove("a, code");
            Assert.AreEqual( jQuery("#ap").Children().Length, 0, "Check multi-filtered remove" );

            // using contents will get comments regular, text, and comment nodes
            // Handle the case where no comment is in the document
            Assert.IsTrue( jQuery("#nonnodes").Contents().Length >= 2, "Check node,textnode,comment remove works" );
            jQuery("#nonnodes").Contents().Remove();
            Assert.AreEqual( jQuery("#nonnodes").Contents().Length, 0, "Check node,textnode,comment remove works" );

            ResetQunit();

            var count = 0;
            first = jQuery("#ap").Children(":first");
//            var cleanUp = first.click(function() { count++ })[method]().AppendTo("#qunit-fixture").click();
            var cleanUp = first.Remove().AppendTo("#qunit-fixture");

            Assert.AreEqual( 0, count );

            // manually clean up detached elements
            cleanUp.Remove();
        }

        [Test,TestMethod]
        public void Empty() {

            Assert.AreEqual( jQuery("#ap").Children().Empty().Text().Length, 0, "Check text is removed" );
            Assert.AreEqual( jQuery("#ap").Children().Length, 4, "Check elements are not removed" );

            // using contents will get comments regular, text, and comment nodes
            var j = jQuery("#nonnodes").Contents();
            j.Empty();
            Assert.AreEqual( j.Html(), "", "Check node,textnode,comment empty works" );
        }

        [Test,TestMethod]
        public void CloneNoExceptions() {
        //test("jQuery.Clone - no exceptions for object elements #9587", function() {
          
            try {
                jQuery("#no-clone-exception").Clone();
                Assert.IsTrue( true, "cloned with no exceptions" );
            } catch( Exception e ) {
                Assert.IsTrue( false, e.Message );
            }
        }
         [Test,TestMethod]
        public void TagWrapUnknownElems() {
        //test("jQuery(<tag>) & wrap[Inner/All]() handle unknown elems (#10667)", function() {
            

            var wraptarget = jQuery( "<div id='wrap-target'>Target</div>" ).AppendTo( "#qunit-fixture" );
            var section = jQuery( "<section>" ).AppendTo( "#qunit-fixture" );

            wraptarget.WrapAll("<aside style='background-color:green'></aside>");

            Assert.AreNotEqual( wraptarget.Parent("aside").Css("background-color"), "transparent", "HTML5 elements created with wrapAll inherit styles" );
            Assert.AreNotEqual( section.Css("background-color"), "transparent", "HTML5 elements create with jQuery( string ) inherit styles" );
        }


//test("append the same fragment with events (Bug #6997, 5566)", function () {
//    var doExtra = !jQuery.support.noCloneEvent && document.fireEvent;
//    expect(2 + (doExtra ? 1 : 0));
//    stop();

//    var element;

//    // This patch modified the way that cloning occurs in IE; we need to make sure that
//    // native event handlers on the original object don't get disturbed when they are
//    // modified on the clone
//    if ( doExtra ) {
//        element = jQuery("div:first").click(function () {
//            Assert.IsTrue(true, "Event exists on original after being unbound on clone");
//            jQuery(this).unbind("click");
//        });
//        var clone = element.Clone(true).unbind("click");
//        clone[0].fireEvent("onclick");
//        element[0].fireEvent("onclick");

//        // manually clean up detached elements
//        clone.Remove();
//    }

//    element = jQuery("<a class='test6997'></a>").click(function () {
//        Assert.IsTrue(true, "Append second element events work");
//    });

//    jQuery("#listWithTabIndex li").Append(element)
//        .Find("a.test6997").eq(1).click();

//    element = jQuery("<li class='test6997'></li>").click(function () {
//        Assert.IsTrue(true, "Before second element events work");
//        start();
//    });

//    jQuery("#listWithTabIndex li").before(element);
//    jQuery("#listWithTabIndex li.test6997").eq(1).click();
//});



//test("HTML5 Elements inherit styles from style rules (Bug #10501)", function () {
//    expect(1);

//    jQuery("#qunit-fixture").Append("<article id='article'></article>");
//    jQuery("#article").Append("<section>This section should have a pink background.</section>");

//    // In IE, the missing background color will claim its value is "transparent"
//    notAssert.AreEqual( jQuery("section").css("background-color"), "transparent", "HTML5 elements inherit styles");
//});

//test("html5 clone() cannot use the fragment cache in IE (#6485)", function () {
//    expect(1);

//    jQuery("<article><section><aside>HTML5 elements</aside></section></article>").AppendTo("#qunit-fixture");

//    var clone = jQuery("article").Clone();

//    jQuery("#qunit-fixture").Append( clone );

//    Assert.AreEqual( jQuery("aside").Length, 2, "clone()ing HTML5 elems does not collapse them" );
//});

//test("html(String) with HTML5 (Bug #6485)", function() {
//    expect(2);

//    jQuery("#qunit-fixture").Html("<article><section><aside>HTML5 elements</aside></section></article>");
//    Assert.AreEqual( jQuery("#qunit-fixture").Children().Children().Length, 1, "Make sure HTML5 article elements can hold children. innerHTML shortcut path" );
//    Assert.AreEqual( jQuery("#qunit-fixture").Children().Children().Children().Length, 1, "Make sure nested HTML5 elements can hold children." );
//});



//test("replaceWith(Function)", function() {
//    testReplaceWith(functionReturningObj);

//    expect(22);

//    var y = jQuery("#yahoo")[0];

//    jQuery(y).ReplaceWith(function(){
//        Assert.AreEqual( this, y, "Make sure the context is coming in correctly." );
//    });

//    Assert.AreEqual(;
//});



//if (!isLocal) {
//test("clone() on XML nodes", function() {
//    expect(2);
//    stop();
//    jQuery.get("data/dashboard.xml", function (xml) {
//        var root = jQuery(xml.documentElement).Clone();
//        var origTab = jQuery("tab", xml).eq(0);
//        var cloneTab = jQuery("tab", root).eq(0);
//        origTab.text("origval");
//        cloneTab.text("cloneval");
//        Assert.AreEqual(origTab.Text(), "origval", "Check original XML node was correctly set");
//        Assert.AreEqual(cloneTab.Text(), "cloneval", "Check cloned XML node was correctly set");
//        start();
//    });
//});
//}


//test("html(String)", function() {
//    testHtml(bareObj);
//});


//test("html(Function)", function() {
//    testHtml(functionReturningObj);

//    expect(36);

//    Assert.AreEqual(;

//    jQuery("#qunit-fixture").Html(function(){
//        return jQuery(this).Text();
//    });

//    Assert.IsTrue( !/</.test( jQuery("#qunit-fixture").Html() ), "Replace html with text." );
//    Assert.IsTrue( jQuery("#qunit-fixture").Html().Length > 0, "Make sure text exists." );
//});

//test("html(Function) with incoming value", function() {
//    expect(20);

//    var div = jQuery("#qunit-fixture > div"), old = div.map(function(){ return jQuery(this).Html() });

//    div.Html(function(i, val) {
//        Assert.AreEqual( val, old[i], "Make sure the incoming value is correct." );
//        return "<b>test</b>";
//    });

//    var pass = true;
//    div.each(function(){
//        if ( this.childNodes.Length !== 1 ) {
//            pass = false;
//        }
//    })
//    Assert.IsTrue( pass, "Set HTML" );

//    Assert.AreEqual(;
//    // using contents will get comments regular, text, and comment nodes
//    var j = jQuery("#nonnodes").contents();
//    old = j.map(function(){ return jQuery(this).Html(); });

//    j.Html(function(i, val) {
//        Assert.AreEqual( val, old[i], "Make sure the incoming value is correct." );
//        return "<b>bold</b>";
//    });

//    // Handle the case where no comment is in the document
//    if ( j.Length === 2 ) {
//        Assert.AreEqual( null, null, "Make sure the incoming value is correct." );
//    }

//    j.Find("b").RemoveData();
//    Assert.AreEqual( j.Html().replace(/ xmlns="[^"]+"/g, "").toLowerCase(), "<b>bold</b>", "Check node,textnode,comment with html()" );

//    var $div = jQuery("<div />");

//    Assert.AreEqual( $div.Html(function(i, val) {
//        Assert.AreEqual( val, "", "Make sure the incoming value is correct." );
//        return 5;
//    }).Html(), "5", "Setting a number as html" );

//    Assert.AreEqual( $div.Html(function(i, val) {
//        Assert.AreEqual( val, "5", "Make sure the incoming value is correct." );
//        return 0;
//    }).Html(), "0", "Setting a zero as html" );

//    var $div2 = jQuery("<div/>"), insert = "&lt;div&gt;hello1&lt;/div&gt;";
//    Assert.AreEqual( $div2.Html(function(i, val) {
//        Assert.AreEqual( val, "", "Make sure the incoming value is correct." );
//        return insert;
//    }).Html().replace(/>/g, "&gt;"), insert, "Verify escaped insertion." );

//    Assert.AreEqual( $div2.Html(function(i, val) {
//        Assert.AreEqual( val.replace(/>/g, "&gt;"), insert, "Make sure the incoming value is correct." );
//        return "x" + insert;
//    }).Html().replace(/>/g, "&gt;"), "x" + insert, "Verify escaped insertion." );

//    Assert.AreEqual( $div2.Html(function(i, val) {
//        Assert.AreEqual( val.replace(/>/g, "&gt;"), "x" + insert, "Make sure the incoming value is correct." );
//        return " " + insert;
//    }).Html().replace(/>/g, "&gt;"), " " + insert, "Verify escaped insertion." );
//});






//test("jQuery.buildFragment - no plain-text caching (Bug #6779)", function() {
//    expect(1);

//    // DOM manipulation fails if added text matches an Object method
//    var $f = jQuery( "<div />" ).AppendTo( "#qunit-fixture" ),
//        bad = [ "start-", "toString", "hasOwnProperty", "append", "here&there!", "-end" ];

//    for ( var i=0; i < bad.Length; i++ ) {
//        try {
//            $f.Append( bad[i] );
//        }
//        catch(e) {}
//    }
//    Assert.AreEqual($f.Text(), bad.join(""), "Cached strings that match Object properties");
//    $f.Remove();
//});

//test( "jQuery.Html - execute scripts escaped with html comment or CDATA (#9221)", function() {
//    expect( 3 );
//    jQuery( [
//             '<script type="text/javascript">',
//             '<!--',
//             'Assert.IsTrue( true, "<!-- handled" );',
//             '//-->',
//             '</script>'
//         ].join ( "\n" ) ).AppendTo( "#qunit-fixture" );
//    jQuery( [
//             '<script type="text/javascript">',
//             '<![CDATA[',
//             'Assert.IsTrue( true, "<![CDATA[ handled" );',
//             '//]]>',
//             '</script>'
//         ].join ( "\n" ) ).AppendTo( "#qunit-fixture" );
//    jQuery( [
//             '<script type="text/javascript">',
//             '<!--//--><![CDATA[//><!--',
//             'Assert.IsTrue( true, "<!--//--><![CDATA[//><!-- (Drupal case) handled" );',
//             '//--><!]]>',
//             '</script>'
//         ].join ( "\n" ) ).AppendTo( "#qunit-fixture" );
//});

//test("jQuery.buildFragment - plain objects are not a document #8950", function() {
//    expect(1);

//    try {
//        jQuery('<input type="hidden">', {});
//        Assert.IsTrue( true, "Does not allow attribute object to be treated like a doc object");
//    } catch (e) {}

//});

        //HERE


//test("Cloned, detached HTML5 elems (#10667,10670)", function() {
//    expect(7);

//    var $section = jQuery( "<section>" ).AppendTo( "#qunit-fixture" ),
//            $clone;

//    // First clone
//    $clone = $section.Clone();

//    // Infer that the test is being run in IE<=8
//    if ( $clone[0].outerHTML && !jQuery.support.opacity ) {
//        // This branch tests cloning nodes by reading the outerHTML, used only in IE<=8
//        Assert.AreEqual( $clone[0].outerHTML, "<section></section>", "detached clone outerHTML matches '<section></section>'" );
//    } else {
//        // This branch tests a known behaviour in modern browsers that should never fail.
//        // Included for expected test count symmetry (expecting 1)
//        Assert.AreEqual( $clone[0].NodeName, "SECTION", "detached clone nodeName matches 'SECTION' in modern browsers" );
//    }

//    // Bind an event
//    $section.bind( "click", function( event ) {
//        Assert.IsTrue( true, "clone fired event" );
//    });

//    // Second clone (will have an event bound)
//    $clone = $section.Clone( true );

//    // Trigger an event from the first clone
//    $clone.trigger( "click" );
//    $clone.unbind( "click" );

//    // Add a child node with text to the original
//    $section.Append( "<p>Hello</p>" );

//    // Third clone (will have child node and text)
//    $clone = $section.Clone( true );

//    Assert.AreEqual( $clone.Find("p").Text(), "Hello", "Assert text in child of clone" );

//    // Trigger an event from the third clone
//    $clone.trigger( "click" );
//    $clone.unbind( "click" );

//    // Add attributes to copy
//    $section.Attr({
//        "class": "foo bar baz",
//        "title": "This is a title"
//    });

//    // Fourth clone (will have newly added attributes)
//    $clone = $section.Clone( true );

//    Assert.AreEqual( $clone.Attr("class"), $section.Attr("class"), "clone and element have same class attribute" );
//    Assert.AreEqual( $clone.Attr("title"), $section.Attr("title"), "clone and element have same title attribute" );

//    // Remove the original
//    $section.Remove();

//    // Clone the clone
//    $section = $clone.Clone( true );

//    // Remove the clone
//    $clone.Remove();

//    // Trigger an event from the clone of the clone
//    $section.trigger( "click" );

//    // Unbind any remaining events
//    $section.unbind( "click" );
//    $clone.unbind( "click" );
//});

//test("jQuery.fragments cache expectations", function() {

//    expect( 10 );

//    jQuery.fragments = {};

//    function fragmentCacheSize() {
//        var n = 0, c;

//        for ( c in jQuery.fragments ) {
//            n++;
//        }
//        return n;
//    }

//    jQuery("<li></li>");
//    jQuery("<li>?</li>");
//    jQuery("<li>whip</li>");
//    jQuery("<li>it</li>");
//    jQuery("<li>good</li>");
//    jQuery("<div></div>");
//    jQuery("<div><div><span></span></div></div>");
//    jQuery("<tr><td></td></tr>");
//    jQuery("<tr><td></tr>");
//    jQuery("<li>aaa</li>");
//    jQuery("<ul><li>?</li></ul>");
//    jQuery("<div><p>arf</p>nnn</div>");
//    jQuery("<div><p>dog</p>?</div>");
//    jQuery("<span><span>");

//    Assert.AreEqual( fragmentCacheSize(), 12, "12 entries exist in jQuery.fragments, 1" );

//    jQuery.each( [
//        "<tr><td></td></tr>",
//        "<ul><li>?</li></ul>",
//        "<div><p>dog</p>?</div>",
//        "<span><span>"
//    ], function( i, frag ) {

//        jQuery( frag );

//        Assert.AreEqual( jQuery.fragments[ frag ].nodeType, 11, "Second call with " + frag + " creates a cached DocumentFragment, has nodeType 11" );
//        Assert.IsTrue( jQuery.fragments[ frag ].childNodes.Length, "Second call with " + frag + " creates a cached DocumentFragment, has childNodes with length" );
//    });

//    Assert.AreEqual( fragmentCacheSize(), 12, "12 entries exist in jQuery.fragments, 2" );
//});
    }
}
