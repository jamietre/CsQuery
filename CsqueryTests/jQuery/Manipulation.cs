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
using Jtc.CsQuery;

namespace CsqueryTests.jQuery
{
    [TestClass,TestFixture]
    public class Manipulation : CsQueryTest
    {
        
        Func<string,string> bareObj = (input) => { return input; };
        Func<string,Func<int,string,string>> functionReturningObj = (input) =>
        {
            Func<int, string,string> returnFunc = (index,inputInner) => { return inputInner; };
            return returnFunc;
        };
        [SetUp]
        public override void FixtureSetUp()
        {
            ResetQunit();
        }
        protected void ResetQunit()
        {
            Dom = CsQuery.Create(Support.GetFile("csquerytests\\resources\\jquery-unit-index.htm"));
        }

        [TestMethod, Test]
        public void Text()
        {
            var expected = "This link has class=\"blog\": Simon Willison's Weblog";
            Assert.AreEqual(jQuery("#sap").Text(), expected, "Check for merged text of more then one element.");

            // Check serialization of text values
            var textNode = document.CreateTextNode("foo");

            Assert.AreEqual("foo", jQuery(textNode).Text(), "Text node was retreived from .Text().");
            Assert.AreNotEqual("", jQuery(Dom.Document).Text(), "Retrieving text for the document retrieves all text (#10724).");
        }


        protected void TestText(Func<string, string> valueObj)
        {
            var val = valueObj("<div><b>Hello</b> cruel world!</div>");
             jQuery("#foo").Text(val);
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

    void TestWrap(Func<string,string> val) {
        
        var defaultText = "Try them out:";
        string textResult = jQuery("#first").Wrap(val( "<div class='red'><span></span></div>" )).Text();
        Assert.AreEqual( defaultText, defaultText, "Check for wrapping of on-the-fly html" );
        Assert.IsTrue( jQuery("#first").Parent().Parent().Is(".red"), "Check if wrapper has class 'red'" );

        ResetQunit();

        var result = jQuery("#first").Wrap(Dom.Document.GetElementById("empty"));
        result= result.Parent();

        Assert.IsTrue( result.Is("ol"), "Check for element wrapping" );
        Assert.AreEqual( result.Text(), defaultText, "Check for element wrapping" );

        // using contents will get comments regular, text, and comment nodes
        var j = jQuery("#nonnodes").Contents();
        j.Wrap(val( "<i></i>" ));

        // Blackberry 4.6 doesn't maintain comments in the DOM
        Assert.AreEqual( jQuery("#nonnodes > i").Length, jQuery("#nonnodes")[0].ChildNodes.Length, "Check node,textnode,comment wraps ok" );
        Assert.AreEqual( jQuery("#nonnodes > i").Text(), j.Text(), "Check node,textnode,comment wraps doesn't hurt text" );

        // Try wrapping a disconnected node
        //var cacheLength = 0;
        //for (var i in jQuery.cache) {
        //    cacheLength++;
        //}

        j = jQuery("<label/>").Wrap(val( "<li/>" ));
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
    
        targets.Each((int i, IDomElement e)=> {
            CsQuery _this = jQuery(e);

            Assert.IsTrue( _this.Parent().Is(".wrapper"), "Check each elements parent is correct (.wrapper)" );
            Assert.AreEqual( _this.Siblings().Length, 0, "Each element should be wrapped individually" );
        });
    }


//    protected void TestWrapAll(Func<string,string> val)
//    {
//        var prev = jQuery("#firstp")[0].PreviousSibling;
//        var p = jQuery("#firstp,#first")[0].ParentNode;

//        var result = jQuery("#firstp,#first").WrapAll(val( "<div class='red'><div class='tmp'></div></div>" ));
//        Assert.AreEqual( result.Parent().Length, 1, "Check for wrapping of on-the-fly html" );
//        Assert.IsTrue( jQuery("#first").Parent().Parent().Is(".red"), "Check if wrapper has class 'red'" );
//        Assert.IsTrue( jQuery("#firstp").Parent().Parent().Is(".red"), "Check if wrapper has class 'red'" );
//        Assert.AreEqual( jQuery("#first").Parent().Parent()[0].PreviousSibling, prev, "Correct Previous Sibling" );
//        Assert.AreEqual( jQuery("#first").Parent().Parent()[0].ParentNode, p, "Correct Parent" );

            
//        prev = jQuery("#firstp")[0].PreviousSibling;
//        p = jQuery("#first")[0].parentNode;
//        result = jQuery("#firstp,#first").wrapAll(val( document.GetElementById("empty") ));
//        Assert.AreEqual( jQuery("#first").Parent()[0], jQuery("#firstp").Parent()[0], "Same Parent" );
//        Assert.AreEqual( jQuery("#first").Parent()[0].previousSibling, prev, "Correct Previous Sibling" );
//        Assert.AreEqual( jQuery("#first").Parent()[0].parentNode, p, "Correct Parent" );

//}

//test("wrapAll(String|Element)", function() {
//    testWrapAll(bareObj);
//});

//var testWrapInner = function(val) {
//    expect(11);
//    var num = jQuery("#first").Children().Length;
//    var result = jQuery("#first").wrapInner(val("<div class='red'><div id='tmp'></div></div>"));
//    Assert.AreEqual( jQuery("#first").Children().Length, 1, "Only one child" );
//    Assert.IsTrue( jQuery("#first").Children().Is(".red"), "Verify Right Element" );
//    Assert.AreEqual( jQuery("#first").Children().Children().Children().Length, num, "Verify Elements Intact" );

//    Assert.AreEqual(;
//    var num = jQuery("#first").html("foo<div>test</div><div>test2</div>").Children().Length;
//    var result = jQuery("#first").wrapInner(val("<div class='red'><div id='tmp'></div></div>"));
//    Assert.AreEqual( jQuery("#first").Children().Length, 1, "Only one child" );
//    Assert.IsTrue( jQuery("#first").Children().Is(".red"), "Verify Right Element" );
//    Assert.AreEqual( jQuery("#first").Children().Children().Children().Length, num, "Verify Elements Intact" );

//    Assert.AreEqual(;
//    var num = jQuery("#first").Children().Length;
//    var result = jQuery("#first").wrapInner(val(document.GetElementById("empty")));
//    Assert.AreEqual( jQuery("#first").Children().Length, 1, "Only one child" );
//    Assert.IsTrue( jQuery("#first").Children().Is("#empty"), "Verify Right Element" );
//    Assert.AreEqual( jQuery("#first").Children().Children().Length, num, "Verify Elements Intact" );

//    var div = jQuery("<div/>");
//    div.wrapInner(val("<span></span>"));
//    Assert.AreEqual(div.Children().Length, 1, "The contents were wrapped.");
//    Assert.AreEqual(div.Children()[0].nodeName.toLowerCase(), "span", "A span was inserted.");
//}

//test("wrapInner(String|Element)", function() {
//    testWrapInner(bareObj);
//});

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
        
        protected void Append(Func<string, string> valueObj)
        {
            var defaultText = "Try them out:";
            var result = jQuery("#first").Append(valueObj("<b>buga</b>"));
            Assert.AreEqual( result.Text(), defaultText + "buga", "Check if text appending works" );
            
            jQuery("#select3").Append(valueObj("<option value='appendTest'>Append Test</option>"));
            Assert.AreEqual( jQuery("#select3").Append(valueObj("<option value='appendTest'>Append Test</option>"))
                .Find("option:last-child")
                .Attr("value"), "appendTest", "Appending html options to select element");

            ResetQunit();
            var expected = "This link has class=\"blog\": Simon Willison's WeblogTry them out:";
            jQuery("#sap").Append(document.GetElementById("first"));
            Assert.AreEqual( jQuery("#sap").Text(), expected, "Check for appending of element" );

            
            expected = "This link has class=\"blog\": Simon Willison's WeblogTry them out:Yahoo";
            jQuery("#sap").Append(Objects.ToEnumerable(document.GetElementById("first"), document.GetElementById("yahoo")));
            Assert.AreEqual( jQuery("#sap").Text(), expected, "Check for appending of array of elements" );

            ResetQunit();
            expected = "This link has class=\"blog\": Simon Willison's WeblogYahooTry them out:";
            jQuery("#sap").Append(jQuery("#yahoo, #first"));
            Assert.AreEqual( jQuery("#sap").Text(), expected, "Check for appending of jQuery object" );

            
            jQuery("#sap").Append("5");
            Assert.IsTrue( jQuery("#sap")[0].InnerHTML.IndexOf("5")>0, "Check for appending a number" );

            
            jQuery("#sap").Append(valueObj( " text with spaces " ));
            Assert.IsTrue( jQuery("#sap")[0].InnerHTML.IndexOf(" text with spaces ")>0, "Check for appending text with spaces" );

            ResetQunit();
            var old = jQuery("#sap").Children().Length;
            Assert.IsTrue( jQuery("#sap").Append(Objects.EmptyEnumerable<IDomObject>()).Children().Length ==old, "Check for appending an empty array." );
            Assert.IsTrue(jQuery("#sap").Append(valueObj("")).Children().Length == old, "Check for appending an empty string.");
            Assert.IsTrue(jQuery("#sap").Append(document.GetElementsByTagName("foo")).Children().Length == old, "Check for appending an empty nodelist.");

            
            jQuery("form").Append(valueObj("<input name='radiotest' type='radio' checked='checked' />"));
            jQuery("form input[name=radiotest]").Each((int i, IDomElement e)=>{
                Assert.IsTrue( jQuery(e).Is(":checked"), "Append checked radio");
            }).Remove();

            
            jQuery("form").Append(valueObj("<input name='radiotest' type='radio' checked    =   'checked' />"));
            jQuery("form input[name=radiotest]").Each((int i, IDomElement e)=>{
                Assert.IsTrue( jQuery(e).Is(":checked"), "Append alternately formated checked radio");
            }).Remove();

            
            jQuery("form").Append(valueObj("<input name='radiotest' type='radio' checked />"));
            jQuery("form input[name=radiotest]").Each((int i, IDomElement e)=>{
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

            //jQuery("<fieldset/>").AppendTo("#form").Append(valueObj( "<legend id='legend'>test</legend>" ));
            //t( "Append legend", "#legend", ["legend"] );

            
            jQuery("#select1").Append(valueObj( "<OPTION>Test</OPTION>" ));
            Assert.AreEqual( jQuery("#select1 option:last").Text(), "Test", "Appending &lt;OPTION&gt; (all caps)" );

            jQuery("#table").Append(valueObj( "<colgroup></colgroup>" ));
            Assert.IsTrue( jQuery("#table colgroup").Length>0, "Append colgroup" );

            jQuery("#table colgroup").Append(valueObj( "<col/>" ));
            Assert.IsTrue( jQuery("#table colgroup col").Length>0, "Append col" );

   
            jQuery("#table").Append(valueObj( "<caption></caption>" ));
            Assert.IsTrue( jQuery("#table caption").Length>0, "Append caption" );

            jQuery("form:last")
                .Append(valueObj( "<select id='appendSelect1'></select>" ))
                .Append(valueObj( "<select id='appendSelect2'><option>Test</option></select>" ));

            //t( "Append Select", "#appendSelect1, #appendSelect2", ["appendSelect1", "appendSelect2"] );

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

            //TODO don't understand this test
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
                return Objects.ToEnumerable(document.GetElementById("first"), document.GetElementById("yahoo"));
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
            var newChildren =jQuery(Objects.ToEnumerable(jQuery("<strong>test</strong>")[0], jQuery("<strong>test</strong>")[0]));
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
            jQuery("#sap").Append(Objects.ToEnumerable(document.GetElementById("first"), document.GetElementById("yahoo")));
            Assert.AreEqual(jQuery("#sap").Text(), expected, "Check for appending of array of elements (Append)");

            ResetQunit();
            //CHANGE: CsQuery always returns selection sets ordered by DOM appearance. You would use "append(IEnumerable)" to append these in the order shown
            expected = "This link has class=\"blog\": Simon Willison's WeblogYahooTry them out:";
            jQuery(Objects.ToEnumerable(document.GetElementById("first"), document.GetElementById("yahoo"))).AppendTo("#sap");
            Assert.AreEqual(jQuery("#sap").Text(), expected, "Check for appending of array of elements");

            ResetQunit();
            Assert.IsTrue(jQuery(document.CreateElement("script")).AppendTo("body").Length > 0, "Make sure a disconnected script can be appended.");

            ResetQunit();
            expected = "This link has class=\"blog\": Simon Willison's WeblogYahooTry them out:";
            jQuery("#yahoo, #first").AppendTo("#sap");
            Assert.AreEqual(jQuery("#sap").Text(), expected, "Check for appending of jQuery object");

            ResetQunit();
            jQuery("#select1").AppendTo("#foo");
            //t( "Append select", "#foo select", ["select1"] );

            //ResetQunit();
            //var div = jQuery("<div/>").click(function(){
            //    Assert.IsTrue(true, "Running a cloned click.");
            //});
            //div.appendTo("#qunit-fixture, #moretests");

            //jQuery("#qunit-fixture div:last").click();
            //jQuery("#moretests div:last").click();

            ResetQunit();
            var div = jQuery("<div/>").AppendTo("#qunit-fixture, #moretests");

            Assert.AreEqual(div.Length, 2, "appendTo returns the inserted elements");

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
            //    jQuery('script[src*="data\\/test\\.js"]').remove();
            //    start();
            //});
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
//        var clone = element.clone(true).unbind("click");
//        clone[0].fireEvent("onclick");
//        element[0].fireEvent("onclick");

//        // manually clean up detached elements
//        clone.remove();
//    }

//    element = jQuery("<a class='test6997'></a>").click(function () {
//        Assert.IsTrue(true, "Append second element events work");
//    });

//    jQuery("#listWithTabIndex li").Append(element)
//        .find("a.test6997").eq(1).click();

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

//    jQuery("<article><section><aside>HTML5 elements</aside></section></article>").appendTo("#qunit-fixture");

//    var clone = jQuery("article").clone();

//    jQuery("#qunit-fixture").Append( clone );

//    Assert.AreEqual( jQuery("aside").Length, 2, "clone()ing HTML5 elems does not collapse them" );
//});

//test("html(String) with HTML5 (Bug #6485)", function() {
//    expect(2);

//    jQuery("#qunit-fixture").html("<article><section><aside>HTML5 elements</aside></section></article>");
//    Assert.AreEqual( jQuery("#qunit-fixture").Children().Children().Length, 1, "Make sure HTML5 article elements can hold children. innerHTML shortcut path" );
//    Assert.AreEqual( jQuery("#qunit-fixture").Children().Children().Children().Length, 1, "Make sure nested HTML5 elements can hold children." );
//});

//test("Append(xml)", function() {
//    expect( 1 );

//    function createXMLDoc() {
//        // Initialize DOM based upon latest installed MSXML or Netscape
//        var elem,
//            aActiveX =
//                [ "MSXML6.DomDocument",
//                "MSXML3.DomDocument",
//                "MSXML2.DomDocument",
//                "MSXML.DomDocument",
//                "Microsoft.XmlDom" ];

//        if ( document.implementation && "createDocument" in document.implementation ) {
//            return document.implementation.createDocument( "", "", null );
//        } else {
//            // IE
//            for ( var n = 0, len = aActiveX.Length; n < len; n++ ) {
//                try {
//                    elem = new ActiveXObject( aActiveX[ n ] );
//                    return elem;
//                } catch(_){};
//            }
//        }
//    }

//    var xmlDoc = createXMLDoc(),
//        xml1 = xmlDoc.createElement("head"),
//        xml2 = xmlDoc.createElement("test");

//    Assert.IsTrue( jQuery( xml1 ).Append( xml2 ), "Append an xml element to another without raising an exception." );

//});

        
//test("appendTo(String|Element|Array&lt;Element&gt;|jQuery)", function() {
//    expect(17);

//    
//});

//var testPrepend = function(val) {
//    expect(5);
//    var defaultText = "Try them out:"
//    var result = jQuery("#first").prepend(val( "<b>buga</b>" ));
//    Assert.AreEqual( result.Text(), "buga" + defaultText, "Check if text prepending works" );
//    Assert.AreEqual( jQuery("#select3").prepend(val( "<option value='prependTest'>Prepend Test</option>" )).find("option:first-child").attr("value"), "prependTest", "Prepending html options to select element");

//    Assert.AreEqual(;
//    var expected = "Try them out:This link has class=\"blog\": Simon Willison's Weblog";
//    jQuery("#sap").prepend(val( document.GetElementById("first") ));
//    Assert.AreEqual( jQuery("#sap").Text(), expected, "Check for prepending of element" );

//    Assert.AreEqual(;
//    expected = "Try them out:YahooThis link has class=\"blog\": Simon Willison's Weblog";
//    jQuery("#sap").prepend(val( [document.GetElementById("first"), document.GetElementById("yahoo")] ));
//    Assert.AreEqual( jQuery("#sap").Text(), expected, "Check for prepending of array of elements" );

//    Assert.AreEqual(;
//    expected = "YahooTry them out:This link has class=\"blog\": Simon Willison's Weblog";
//    jQuery("#sap").prepend(val( jQuery("#yahoo, #first") ));
//    Assert.AreEqual( jQuery("#sap").Text(), expected, "Check for prepending of jQuery object" );
//};

//test("prepend(String|Element|Array&lt;Element&gt;|jQuery)", function() {
//    testPrepend(bareObj);
//});

//test("prepend(Function)", function() {
//    testPrepend(functionReturningObj);
//});

//test("prepend(Function) with incoming value", function() {
//    expect(10);

//    var defaultText = "Try them out:", old = jQuery("#first").html();
//    var result = jQuery("#first").prepend(function(i, val) {
//        Assert.AreEqual( val, old, "Make sure the incoming value is correct." );
//        return "<b>buga</b>";
//    });
//    Assert.AreEqual( result.Text(), "buga" + defaultText, "Check if text prepending works" );

//    old = jQuery("#select3").html();

//    Assert.AreEqual( jQuery("#select3").prepend(function(i, val) {
//        Assert.AreEqual( val, old, "Make sure the incoming value is correct." );
//        return "<option value='prependTest'>Prepend Test</option>";
//    }).find("option:first-child").attr("value"), "prependTest", "Prepending html options to select element");

//    Assert.AreEqual(;
//    var expected = "Try them out:This link has class=\"blog\": Simon Willison's Weblog";
//    old = jQuery("#sap").html();

//    jQuery("#sap").prepend(function(i, val) {
//        Assert.AreEqual( val, old, "Make sure the incoming value is correct." );
//        return document.GetElementById("first");
//    });

//    Assert.AreEqual( jQuery("#sap").Text(), expected, "Check for prepending of element" );

//    Assert.AreEqual(;
//    expected = "Try them out:YahooThis link has class=\"blog\": Simon Willison's Weblog";
//    old = jQuery("#sap").html();

//    jQuery("#sap").prepend(function(i, val) {
//        Assert.AreEqual( val, old, "Make sure the incoming value is correct." );
//        return [document.GetElementById("first"), document.GetElementById("yahoo")];
//    });

//    Assert.AreEqual( jQuery("#sap").Text(), expected, "Check for prepending of array of elements" );

//    Assert.AreEqual(;
//    expected = "YahooTry them out:This link has class=\"blog\": Simon Willison's Weblog";
//    old = jQuery("#sap").html();

//    jQuery("#sap").prepend(function(i, val) {
//        Assert.AreEqual( val, old, "Make sure the incoming value is correct." );
//        return jQuery("#yahoo, #first");
//    });

//    Assert.AreEqual( jQuery("#sap").Text(), expected, "Check for prepending of jQuery object" );
//});

//test("prependTo(String|Element|Array&lt;Element&gt;|jQuery)", function() {
//    expect(6);
//    var defaultText = "Try them out:"
//    jQuery("<b>buga</b>").prependTo("#first");
//    Assert.AreEqual( jQuery("#first").Text(), "buga" + defaultText, "Check if text prepending works" );
//    Assert.AreEqual( jQuery("<option value='prependTest'>Prepend Test</option>").prependTo("#select3").Parent().find("option:first-child").attr("value"), "prependTest", "Prepending html options to select element");

//    Assert.AreEqual(;
//    var expected = "Try them out:This link has class=\"blog\": Simon Willison's Weblog";
//    jQuery(document.GetElementById("first")).prependTo("#sap");
//    Assert.AreEqual( jQuery("#sap").Text(), expected, "Check for prepending of element" );

//    Assert.AreEqual(;
//    expected = "Try them out:YahooThis link has class=\"blog\": Simon Willison's Weblog";
//    jQuery([document.GetElementById("first"), document.GetElementById("yahoo")]).prependTo("#sap");
//    Assert.AreEqual( jQuery("#sap").Text(), expected, "Check for prepending of array of elements" );

//    Assert.AreEqual(;
//    expected = "YahooTry them out:This link has class=\"blog\": Simon Willison's Weblog";
//    jQuery("#yahoo, #first").prependTo("#sap");
//    Assert.AreEqual( jQuery("#sap").Text(), expected, "Check for prepending of jQuery object" );

//    Assert.AreEqual(;
//    jQuery("<select id='prependSelect1'></select>").prependTo("form:last");
//    jQuery("<select id='prependSelect2'><option>Test</option></select>").prependTo("form:last");

//    t( "Prepend Select", "#prependSelect2, #prependSelect1", ["prependSelect2", "prependSelect1"] );
//});

//var testBefore = function(val) {
//    expect(6);
//    var expected = "This is a normal link: bugaYahoo";
//    jQuery("#yahoo").before(val( "<b>buga</b>" ));
//    Assert.AreEqual( jQuery("#en").Text(), expected, "Insert String before" );

//    Assert.AreEqual(;
//    expected = "This is a normal link: Try them out:Yahoo";
//    jQuery("#yahoo").before(val( document.GetElementById("first") ));
//    Assert.AreEqual( jQuery("#en").Text(), expected, "Insert element before" );

//    Assert.AreEqual(;
//    expected = "This is a normal link: Try them out:diveintomarkYahoo";
//    jQuery("#yahoo").before(val( [document.GetElementById("first"), document.GetElementById("mark")] ));
//    Assert.AreEqual( jQuery("#en").Text(), expected, "Insert array of elements before" );

//    Assert.AreEqual(;
//    expected = "This is a normal link: diveintomarkTry them out:Yahoo";
//    jQuery("#yahoo").before(val( jQuery("#mark, #first") ));
//    Assert.AreEqual( jQuery("#en").Text(), expected, "Insert jQuery before" );

//    var set = jQuery("<div/>").before("<span>test</span>");
//    Assert.AreEqual( set[0].nodeName.toLowerCase(), "span", "Insert the element before the disconnected node." );
//    Assert.AreEqual( set.Length, 2, "Insert the element before the disconnected node." );
//}

//test("before(String|Element|Array&lt;Element&gt;|jQuery)", function() {
//    testBefore(bareObj);
//});

//test("before(Function)", function() {
//    testBefore(functionReturningObj);
//})

//test("before and after w/ empty object (#10812)", function() {
//    expect(2);

//    var res = jQuery( "#notInTheDocument" ).before( "(" ).after( ")" );
//    Assert.AreEqual( res.Length, 2, "didn't choke on empty object" );
//    Assert.AreEqual( res.wrapAll("<div/>").Parent().Text(), "()", "correctly appended text" );
//});

//test("insertBefore(String|Element|Array&lt;Element&gt;|jQuery)", function() {
//    expect(4);
//    var expected = "This is a normal link: bugaYahoo";
//    jQuery("<b>buga</b>").insertBefore("#yahoo");
//    Assert.AreEqual( jQuery("#en").Text(), expected, "Insert String before" );

//    Assert.AreEqual(;
//    expected = "This is a normal link: Try them out:Yahoo";
//    jQuery(document.GetElementById("first")).insertBefore("#yahoo");
//    Assert.AreEqual( jQuery("#en").Text(), expected, "Insert element before" );

//    Assert.AreEqual(;
//    expected = "This is a normal link: Try them out:diveintomarkYahoo";
//    jQuery([document.GetElementById("first"), document.GetElementById("mark")]).insertBefore("#yahoo");
//    Assert.AreEqual( jQuery("#en").Text(), expected, "Insert array of elements before" );

//    Assert.AreEqual(;
//    expected = "This is a normal link: diveintomarkTry them out:Yahoo";
//    jQuery("#mark, #first").insertBefore("#yahoo");
//    Assert.AreEqual( jQuery("#en").Text(), expected, "Insert jQuery before" );
//});

//var testAfter = function(val) {
//    expect(6);
//    var expected = "This is a normal link: Yahoobuga";
//    jQuery("#yahoo").after(val( "<b>buga</b>" ));
//    Assert.AreEqual( jQuery("#en").Text(), expected, "Insert String after" );

//    Assert.AreEqual(;
//    expected = "This is a normal link: YahooTry them out:";
//    jQuery("#yahoo").after(val( document.GetElementById("first") ));
//    Assert.AreEqual( jQuery("#en").Text(), expected, "Insert element after" );

//    Assert.AreEqual(;
//    expected = "This is a normal link: YahooTry them out:diveintomark";
//    jQuery("#yahoo").after(val( [document.GetElementById("first"), document.GetElementById("mark")] ));
//    Assert.AreEqual( jQuery("#en").Text(), expected, "Insert array of elements after" );

//    Assert.AreEqual(;
//    expected = "This is a normal link: YahoodiveintomarkTry them out:";
//    jQuery("#yahoo").after(val( jQuery("#mark, #first") ));
//    Assert.AreEqual( jQuery("#en").Text(), expected, "Insert jQuery after" );

//    var set = jQuery("<div/>").after("<span>test</span>");
//    Assert.AreEqual( set[1].nodeName.toLowerCase(), "span", "Insert the element after the disconnected node." );
//    Assert.AreEqual( set.Length, 2, "Insert the element after the disconnected node." );
//};

//test("after(String|Element|Array&lt;Element&gt;|jQuery)", function() {
//    testAfter(bareObj);
//});

//test("after(Function)", function() {
//    testAfter(functionReturningObj);
//})

//test("insertAfter(String|Element|Array&lt;Element&gt;|jQuery)", function() {
//    expect(4);
//    var expected = "This is a normal link: Yahoobuga";
//    jQuery("<b>buga</b>").insertAfter("#yahoo");
//    Assert.AreEqual( jQuery("#en").Text(), expected, "Insert String after" );

//    Assert.AreEqual(;
//    expected = "This is a normal link: YahooTry them out:";
//    jQuery(document.GetElementById("first")).insertAfter("#yahoo");
//    Assert.AreEqual( jQuery("#en").Text(), expected, "Insert element after" );

//    Assert.AreEqual(;
//    expected = "This is a normal link: YahooTry them out:diveintomark";
//    jQuery([document.GetElementById("first"), document.GetElementById("mark")]).insertAfter("#yahoo");
//    Assert.AreEqual( jQuery("#en").Text(), expected, "Insert array of elements after" );

//    Assert.AreEqual(;
//    expected = "This is a normal link: YahoodiveintomarkTry them out:";
//    jQuery("#mark, #first").insertAfter("#yahoo");
//    Assert.AreEqual( jQuery("#en").Text(), expected, "Insert jQuery after" );
//});

//var testReplaceWith = function(val) {
//    expect(21);
//    jQuery("#yahoo").replaceWith(val( "<b id='replace'>buga</b>" ));
//    Assert.IsTrue( jQuery("#replace")[0], "Replace element with string" );
//    Assert.IsTrue( !jQuery("#yahoo")[0], "Verify that original element is gone, after string" );

//    Assert.AreEqual(;
//    jQuery("#yahoo").replaceWith(val( document.GetElementById("first") ));
//    Assert.IsTrue( jQuery("#first")[0], "Replace element with element" );
//    Assert.IsTrue( !jQuery("#yahoo")[0], "Verify that original element is gone, after element" );

//    Assert.AreEqual(;
//    jQuery("#qunit-fixture").Append("<div id='bar'><div id='baz'</div></div>");
//    jQuery("#baz").replaceWith("Baz");
//    Assert.AreEqual( jQuery("#bar").Text(),"Baz", "Replace element with text" );
//    Assert.IsTrue( !jQuery("#baz")[0], "Verify that original element is gone, after element" );

//    Assert.AreEqual(;
//    jQuery("#yahoo").replaceWith(val( [document.GetElementById("first"), document.GetElementById("mark")] ));
//    Assert.IsTrue( jQuery("#first")[0], "Replace element with array of elements" );
//    Assert.IsTrue( jQuery("#mark")[0], "Replace element with array of elements" );
//    Assert.IsTrue( !jQuery("#yahoo")[0], "Verify that original element is gone, after array of elements" );

//    Assert.AreEqual(;
//    jQuery("#yahoo").replaceWith(val( jQuery("#mark, #first") ));
//    Assert.IsTrue( jQuery("#first")[0], "Replace element with set of elements" );
//    Assert.IsTrue( jQuery("#mark")[0], "Replace element with set of elements" );
//    Assert.IsTrue( !jQuery("#yahoo")[0], "Verify that original element is gone, after set of elements" );

//    Assert.AreEqual(;
//    var tmp = jQuery("<div/>").appendTo("body").click(function(){ Assert.IsTrue(true, "Newly bound click run." ); });
//    var y = jQuery("<div/>").appendTo("body").click(function(){ Assert.IsTrue(true, "Previously bound click run." ); });
//    var child = y.Append("<b>test</b>").find("b").click(function(){ Assert.IsTrue(true, "Child bound click run." ); return false; });

//    y.replaceWith( tmp );

//    tmp.click();
//    y.click(); // Shouldn't be run
//    child.click(); // Shouldn't be run

//    tmp.remove();
//    y.remove();
//    child.remove();

//    Assert.AreEqual(;

//    y = jQuery("<div/>").appendTo("body").click(function(){ Assert.IsTrue(true, "Previously bound click run." ); });
//    var child2 = y.Append("<u>test</u>").find("u").click(function(){ Assert.IsTrue(true, "Child 2 bound click run." ); return false; });

//    y.replaceWith( child2 );

//    child2.click();

//    y.remove();
//    child2.remove();

//    Assert.AreEqual(;

//    var set = jQuery("<div/>").replaceWith(val("<span>test</span>"));
//    Assert.AreEqual( set[0].nodeName.toLowerCase(), "span", "Replace the disconnected node." );
//    Assert.AreEqual( set.Length, 1, "Replace the disconnected node." );

//    var non_existant = jQuery("#does-not-exist").replaceWith( val("<b>should not throw an error</b>") );
//    Assert.AreEqual( non_existant.Length, 0, "Length of non existant element." );

//    var $div = jQuery("<div class='replacewith'></div>").appendTo("body");
//    // TODO: Work on jQuery(...) inline script execution
//    //$div.replaceWith("<div class='replacewith'></div><script>" +
//        //"Assert.AreEqual(jQuery('.replacewith').Length, 1, 'Check number of elements in page.');" +
//        //"</script>");
//    Assert.AreEqual(jQuery(".replacewith").Length, 1, "Check number of elements in page.");
//    jQuery(".replacewith").remove();

//    Assert.AreEqual(;

//    jQuery("#qunit-fixture").Append("<div id='replaceWith'></div>");
//    Assert.AreEqual( jQuery("#qunit-fixture").find("div[id=replaceWith]").Length, 1, "Make sure only one div exists." );

//    jQuery("#replaceWith").replaceWith( val("<div id='replaceWith'></div>") );
//    Assert.AreEqual( jQuery("#qunit-fixture").find("div[id=replaceWith]").Length, 1, "Make sure only one div exists." );

//    jQuery("#replaceWith").replaceWith( val("<div id='replaceWith'></div>") );
//    Assert.AreEqual( jQuery("#qunit-fixture").find("div[id=replaceWith]").Length, 1, "Make sure only one div exists." );
//}

//test("replaceWith(String|Element|Array&lt;Element&gt;|jQuery)", function() {
//    testReplaceWith(bareObj);
//});

//test("replaceWith(Function)", function() {
//    testReplaceWith(functionReturningObj);

//    expect(22);

//    var y = jQuery("#yahoo")[0];

//    jQuery(y).replaceWith(function(){
//        Assert.AreEqual( this, y, "Make sure the context is coming in correctly." );
//    });

//    Assert.AreEqual(;
//});

//test("replaceWith(string) for more than one element", function(){
//    expect(3);

//    Assert.AreEqual(jQuery("#foo p").Length, 3, "ensuring that test data has not changed");

//    jQuery("#foo p").replaceWith("<span>bar</span>");
//    Assert.AreEqual(jQuery("#foo span").Length, 3, "verify that all the three original element have been replaced");
//    Assert.AreEqual(jQuery("#foo p").Length, 0, "verify that all the three original element have been replaced");
//});

//test("replaceAll(String|Element|Array&lt;Element&gt;|jQuery)", function() {
//    expect(10);
//    jQuery("<b id='replace'>buga</b>").replaceAll("#yahoo");
//    Assert.IsTrue( jQuery("#replace")[0], "Replace element with string" );
//    Assert.IsTrue( !jQuery("#yahoo")[0], "Verify that original element is gone, after string" );

//    Assert.AreEqual(;
//    jQuery(document.GetElementById("first")).replaceAll("#yahoo");
//    Assert.IsTrue( jQuery("#first")[0], "Replace element with element" );
//    Assert.IsTrue( !jQuery("#yahoo")[0], "Verify that original element is gone, after element" );

//    Assert.AreEqual(;
//    jQuery([document.GetElementById("first"), document.GetElementById("mark")]).replaceAll("#yahoo");
//    Assert.IsTrue( jQuery("#first")[0], "Replace element with array of elements" );
//    Assert.IsTrue( jQuery("#mark")[0], "Replace element with array of elements" );
//    Assert.IsTrue( !jQuery("#yahoo")[0], "Verify that original element is gone, after array of elements" );

//    Assert.AreEqual(;
//    jQuery("#mark, #first").replaceAll("#yahoo");
//    Assert.IsTrue( jQuery("#first")[0], "Replace element with set of elements" );
//    Assert.IsTrue( jQuery("#mark")[0], "Replace element with set of elements" );
//    Assert.IsTrue( !jQuery("#yahoo")[0], "Verify that original element is gone, after set of elements" );
//});

//test("jQuery.clone() (#8017)", function() {

//    expect(2);

//    Assert.IsTrue( jQuery.clone && jQuery.isFunction( jQuery.clone ) , "jQuery.clone() utility exists and is a function.");

//    var main = jQuery("#qunit-fixture")[0],
//            clone = jQuery.clone( main );

//    Assert.AreEqual( main.childNodes.Length, clone.childNodes.Length, "Simple child length to ensure a large dom tree copies correctly" );
//});

//test("clone() (#8070)", function () {
//    expect(2);

//    jQuery("<select class='test8070'></select><select class='test8070'></select>").appendTo("#qunit-fixture");
//    var selects = jQuery(".test8070");
//    selects.Append("<OPTION>1</OPTION><OPTION>2</OPTION>");

//    Assert.AreEqual( selects[0].childNodes.Length, 2, "First select got two nodes" );
//    Assert.AreEqual( selects[1].childNodes.Length, 2, "Second select got two nodes" );

//    selects.remove();
//});

//test("clone()", function() {
//    expect(39);
//    Assert.AreEqual( "This is a normal link: Yahoo", jQuery("#en").Text(), "Assert text for #en" );
//    var clone = jQuery("#yahoo").clone();
//    Assert.AreEqual( "Try them out:Yahoo", jQuery("#first").Append(clone).Text(), "Check for clone" );
//    Assert.AreEqual( "This is a normal link: Yahoo", jQuery("#en").Text(), "Reassert text for #en" );

//    var cloneTags = [
//        "<table/>", "<tr/>", "<td/>", "<div/>",
//        "<button/>", "<ul/>", "<ol/>", "<li/>",
//        "<input type='checkbox' />", "<select/>", "<option/>", "<textarea/>",
//        "<tbody/>", "<thead/>", "<tfoot/>", "<iframe/>"
//    ];
//    for (var i = 0; i < cloneTags.Length; i++) {
//        var j = jQuery(cloneTags[i]);
//        Assert.AreEqual( j[0].tagName, j.clone()[0].tagName, "Clone a " + cloneTags[i]);
//    }

//    // using contents will get comments regular, text, and comment nodes
//    var cl = jQuery("#nonnodes").contents().clone();
//    Assert.IsTrue( cl.Length >= 2, "Check node,textnode,comment clone works (some browsers delete comments on clone)" );

//    var div = jQuery("<div><ul><li>test</li></ul></div>").click(function(){
//        Assert.IsTrue( true, "Bound event still exists." );
//    });

//    clone = div.clone(true);

//    // manually clean up detached elements
//    div.remove();

//    div = clone.clone(true);

//    // manually clean up detached elements
//    clone.remove();

//    Assert.AreEqual( div.Length, 1, "One element cloned" );
//    Assert.AreEqual( div[0].nodeName.toUpperCase(), "DIV", "DIV element cloned" );
//    div.trigger("click");

//    // manually clean up detached elements
//    div.remove();

//    div = jQuery("<div/>").Append([ document.createElement("table"), document.createElement("table") ]);
//    div.find("table").click(function(){
//        Assert.IsTrue( true, "Bound event still exists." );
//    });

//    clone = div.clone(true);
//    Assert.AreEqual( clone.Length, 1, "One element cloned" );
//    Assert.AreEqual( clone[0].nodeName.toUpperCase(), "DIV", "DIV element cloned" );
//    clone.find("table:last").trigger("click");

//    // manually clean up detached elements
//    div.remove();
//    clone.remove();

//    var divEvt = jQuery("<div><ul><li>test</li></ul></div>").click(function(){
//        Assert.IsTrue( false, "Bound event still exists after .clone()." );
//    }),
//        cloneEvt = divEvt.clone();

//    // Make sure that doing .clone() doesn't clone events
//    cloneEvt.trigger("click");

//    cloneEvt.remove();
//    divEvt.remove();

//    // Test both html() and clone() for <embed and <object types
//    div = jQuery("<div/>").html('<embed height="355" width="425" src="http://www.youtube.com/v/3KANI2dpXLw&amp;hl=en"></embed>');

//    clone = div.clone(true);
//    Assert.AreEqual( clone.Length, 1, "One element cloned" );
//    Assert.AreEqual( clone.html(), div.html(), "Element contents cloned" );
//    Assert.AreEqual( clone[0].nodeName.toUpperCase(), "DIV", "DIV element cloned" );

//    // this is technically an invalid object, but because of the special
//    // classid instantiation it is the only kind that IE has trouble with,
//    // so let's test with it too.
//    div = jQuery("<div/>").html("<object height='355' width='425' classid='clsid:D27CDB6E-AE6D-11cf-96B8-444553540000'>  <param name='movie' value='http://www.youtube.com/v/3KANI2dpXLw&amp;hl=en'>  <param name='wmode' value='transparent'> </object>");

//    clone = div.clone(true);
//    Assert.AreEqual( clone.Length, 1, "One element cloned" );
//    // Assert.AreEqual( clone.html(), div.html(), "Element contents cloned" );
//    Assert.AreEqual( clone[0].nodeName.toUpperCase(), "DIV", "DIV element cloned" );

//    // and here's a valid one.
//    div = jQuery("<div/>").html("<object height='355' width='425' type='application/x-shockwave-flash' data='http://www.youtube.com/v/3KANI2dpXLw&amp;hl=en'>  <param name='movie' value='http://www.youtube.com/v/3KANI2dpXLw&amp;hl=en'>  <param name='wmode' value='transparent'> </object>");

//    clone = div.clone(true);
//    Assert.AreEqual( clone.Length, 1, "One element cloned" );
//    Assert.AreEqual( clone.html(), div.html(), "Element contents cloned" );
//    Assert.AreEqual( clone[0].nodeName.toUpperCase(), "DIV", "DIV element cloned" );

//    div = jQuery("<div/>").data({ a: true });
//    clone = div.clone(true);
//    Assert.AreEqual( clone.data("a"), true, "Data cloned." );
//    clone.data("a", false);
//    Assert.AreEqual( clone.data("a"), false, "Ensure cloned element data object was correctly modified" );
//    Assert.AreEqual( div.data("a"), true, "Ensure cloned element data object is copied, not referenced" );

//    // manually clean up detached elements
//    div.remove();
//    clone.remove();

//    var form = document.createElement("form");
//    form.action = "/test/";
//    var div = document.createElement("div");
//    div.appendChild( document.createTextNode("test") );
//    form.appendChild( div );

//    Assert.AreEqual( jQuery(form).clone().Children().Length, 1, "Make sure we just get the form back." );

//    Assert.AreEqual( jQuery("body").clone().Children()[0].id, "qunit-header", "Make sure cloning body works" );
//});

//test("clone(form element) (Bug #3879, #6655)", function() {
//    expect(5);
//    var element = jQuery("<select><option>Foo</option><option selected>Bar</option></select>");

//    Assert.AreEqual( element.clone().find("option:selected").val(), element.find("option:selected").val(), "Selected option cloned correctly" );

//    element = jQuery("<input type='checkbox' value='foo'>").attr("checked", "checked");
//    clone = element.clone();

//    Assert.AreEqual( clone.Is(":checked"), element.Is(":checked"), "Checked input cloned correctly" );
//    Assert.AreEqual( clone[0].defaultValue, "foo", "Checked input defaultValue cloned correctly" );

//    // defaultChecked also gets set now due to setAttribute in attr, is this check still valid?
//    // Assert.AreEqual( clone[0].defaultChecked, !jQuery.support.noCloneChecked, "Checked input defaultChecked cloned correctly" );

//    element = jQuery("<input type='text' value='foo'>");
//    clone = element.clone();
//    Assert.AreEqual( clone[0].defaultValue, "foo", "Text input defaultValue cloned correctly" );

//    element = jQuery("<textarea>foo</textarea>");
//    clone = element.clone();
//    Assert.AreEqual( clone[0].defaultValue, "foo", "Textarea defaultValue cloned correctly" );
//});

//test("clone(multiple selected options) (Bug #8129)", function() {
//    expect(1);
//    var element = jQuery("<select><option>Foo</option><option selected>Bar</option><option selected>Baz</option></select>");

//    Assert.AreEqual( element.clone().find("option:selected").Length, element.find("option:selected").Length, "Multiple selected options cloned correctly" );

//});

//if (!isLocal) {
//test("clone() on XML nodes", function() {
//    expect(2);
//    stop();
//    jQuery.get("data/dashboard.xml", function (xml) {
//        var root = jQuery(xml.documentElement).clone();
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

//var testHtml = function(valueObj) {
//    expect(34);

//    jQuery.scriptorder = 0;

//    var div = jQuery("#qunit-fixture > div");
//    div.html(valueObj("<b>test</b>"));
//    var pass = true;
//    for ( var i = 0; i < div.size(); i++ ) {
//        if ( div.get(i).childNodes.Length != 1 ) pass = false;
//    }
//    Assert.IsTrue( pass, "Set HTML" );

//    div = jQuery("<div/>").html( valueObj("<div id='parent_1'><div id='child_1'/></div><div id='parent_2'/>") );

//    Assert.AreEqual( div.Children().Length, 2, "Make sure two child nodes exist." );
//    Assert.AreEqual( div.Children().Children().Length, 1, "Make sure that a grandchild exists." );

//    var space = jQuery("<div/>").html(valueObj("&#160;"))[0].innerHTML;
//    Assert.IsTrue( /^\xA0$|^&nbsp;$/.test( space ), "Make sure entities are passed through correctly." );
//    Assert.AreEqual( jQuery("<div/>").html(valueObj("&amp;"))[0].innerHTML, "&amp;", "Make sure entities are passed through correctly." );

//    jQuery("#qunit-fixture").html(valueObj("<style>.foobar{color:green;}</style>"));

//    Assert.AreEqual( jQuery("#qunit-fixture").Children().Length, 1, "Make sure there is a child element." );
//    Assert.AreEqual( jQuery("#qunit-fixture").Children()[0].nodeName.toUpperCase(), "STYLE", "And that a style element was inserted." );

//    Assert.AreEqual(;
//    // using contents will get comments regular, text, and comment nodes
//    var j = jQuery("#nonnodes").contents();
//    j.html(valueObj("<b>bold</b>"));

//    // this is needed, or the expando added by jQuery unique will yield a different html
//    j.find("b").removeData();
//    Assert.AreEqual( j.html().replace(/ xmlns="[^"]+"/g, "").toLowerCase(), "<b>bold</b>", "Check node,textnode,comment with html()" );

//    jQuery("#qunit-fixture").html(valueObj("<select/>"));
//    jQuery("#qunit-fixture select").html(valueObj("<option>O1</option><option selected='selected'>O2</option><option>O3</option>"));
//    Assert.AreEqual( jQuery("#qunit-fixture select").val(), "O2", "Selected option correct" );

//    var $div = jQuery("<div />");
//    Assert.AreEqual( $div.html(valueObj( 5 )).html(), "5", "Setting a number as html" );
//    Assert.AreEqual( $div.html(valueObj( 0 )).html(), "0", "Setting a zero as html" );

//    var $div2 = jQuery("<div/>"), insert = "&lt;div&gt;hello1&lt;/div&gt;";
//    Assert.AreEqual( $div2.html(insert).html().replace(/>/g, "&gt;"), insert, "Verify escaped insertion." );
//    Assert.AreEqual( $div2.html("x" + insert).html().replace(/>/g, "&gt;"), "x" + insert, "Verify escaped insertion." );
//    Assert.AreEqual( $div2.html(" " + insert).html().replace(/>/g, "&gt;"), " " + insert, "Verify escaped insertion." );

//    var map = jQuery("<map/>").html(valueObj("<area id='map01' shape='rect' coords='50,50,150,150' href='http://www.jquery.com/' alt='jQuery'>"));

//    Assert.AreEqual( map[0].childNodes.Length, 1, "The area was inserted." );
//    Assert.AreEqual( map[0].firstChild.nodeName.toLowerCase(), "area", "The area was inserted." );

//    Assert.AreEqual(;

//    jQuery("#qunit-fixture").html(valueObj("<script type='something/else'>Assert.IsTrue( false, 'Non-script evaluated.' );</script><script type='text/javascript'>Assert.IsTrue( true, 'text/javascript is evaluated.' );</script><script>Assert.IsTrue( true, 'No type is evaluated.' );</script><div><script type='text/javascript'>Assert.IsTrue( true, 'Inner text/javascript is evaluated.' );</script><script>Assert.IsTrue( true, 'Inner No type is evaluated.' );</script><script type='something/else'>Assert.IsTrue( false, 'Non-script evaluated.' );</script></div>"));

//    var child = jQuery("#qunit-fixture").find("script");

//    Assert.AreEqual( child.Length, 2, "Make sure that two non-JavaScript script tags are left." );
//    Assert.AreEqual( child[0].type, "something/else", "Verify type of script tag." );
//    Assert.AreEqual( child[1].type, "something/else", "Verify type of script tag." );

//    jQuery("#qunit-fixture").html(valueObj("<script>Assert.IsTrue( true, 'Test repeated injection of script.' );</script>"));
//    jQuery("#qunit-fixture").html(valueObj("<script>Assert.IsTrue( true, 'Test repeated injection of script.' );</script>"));
//    jQuery("#qunit-fixture").html(valueObj("<script>Assert.IsTrue( true, 'Test repeated injection of script.' );</script>"));

//    jQuery("#qunit-fixture").html(valueObj("<script type='text/javascript'>Assert.IsTrue( true, 'jQuery().html().evalScripts() Evals Scripts Twice in Firefox, see #975 (1)' );</script>"));

//    jQuery("#qunit-fixture").html(valueObj("foo <form><script type='text/javascript'>Assert.IsTrue( true, 'jQuery().html().evalScripts() Evals Scripts Twice in Firefox, see #975 (2)' );</script></form>"));

//    jQuery("#qunit-fixture").html(valueObj("<script>Assert.AreEqual(jQuery.scriptorder++, 0, 'Script is executed in order');Assert.AreEqual(jQuery('#scriptorder').Length, 1,'Execute after html (even though appears before)')<\/script><span id='scriptorder'><script>Assert.AreEqual(jQuery.scriptorder++, 1, 'Script (nested) is executed in order');Assert.AreEqual(jQuery('#scriptorder').Length, 1,'Execute after html')<\/script></span><script>Assert.AreEqual(jQuery.scriptorder++, 2, 'Script (unnested) is executed in order');Assert.AreEqual(jQuery('#scriptorder').Length, 1,'Execute after html')<\/script>"));
//}

//test("html(String)", function() {
//    testHtml(bareObj);
//});

//test("html(Function)", function() {
//    testHtml(functionReturningObj);

//    expect(36);

//    Assert.AreEqual(;

//    jQuery("#qunit-fixture").html(function(){
//        return jQuery(this).Text();
//    });

//    Assert.IsTrue( !/</.test( jQuery("#qunit-fixture").html() ), "Replace html with text." );
//    Assert.IsTrue( jQuery("#qunit-fixture").html().Length > 0, "Make sure text exists." );
//});

//test("html(Function) with incoming value", function() {
//    expect(20);

//    var div = jQuery("#qunit-fixture > div"), old = div.map(function(){ return jQuery(this).html() });

//    div.html(function(i, val) {
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
//    old = j.map(function(){ return jQuery(this).html(); });

//    j.html(function(i, val) {
//        Assert.AreEqual( val, old[i], "Make sure the incoming value is correct." );
//        return "<b>bold</b>";
//    });

//    // Handle the case where no comment is in the document
//    if ( j.Length === 2 ) {
//        Assert.AreEqual( null, null, "Make sure the incoming value is correct." );
//    }

//    j.find("b").removeData();
//    Assert.AreEqual( j.html().replace(/ xmlns="[^"]+"/g, "").toLowerCase(), "<b>bold</b>", "Check node,textnode,comment with html()" );

//    var $div = jQuery("<div />");

//    Assert.AreEqual( $div.html(function(i, val) {
//        Assert.AreEqual( val, "", "Make sure the incoming value is correct." );
//        return 5;
//    }).html(), "5", "Setting a number as html" );

//    Assert.AreEqual( $div.html(function(i, val) {
//        Assert.AreEqual( val, "5", "Make sure the incoming value is correct." );
//        return 0;
//    }).html(), "0", "Setting a zero as html" );

//    var $div2 = jQuery("<div/>"), insert = "&lt;div&gt;hello1&lt;/div&gt;";
//    Assert.AreEqual( $div2.html(function(i, val) {
//        Assert.AreEqual( val, "", "Make sure the incoming value is correct." );
//        return insert;
//    }).html().replace(/>/g, "&gt;"), insert, "Verify escaped insertion." );

//    Assert.AreEqual( $div2.html(function(i, val) {
//        Assert.AreEqual( val.replace(/>/g, "&gt;"), insert, "Make sure the incoming value is correct." );
//        return "x" + insert;
//    }).html().replace(/>/g, "&gt;"), "x" + insert, "Verify escaped insertion." );

//    Assert.AreEqual( $div2.html(function(i, val) {
//        Assert.AreEqual( val.replace(/>/g, "&gt;"), "x" + insert, "Make sure the incoming value is correct." );
//        return " " + insert;
//    }).html().replace(/>/g, "&gt;"), " " + insert, "Verify escaped insertion." );
//});

//var testRemove = function(method) {
//    expect(9);

//    var first = jQuery("#ap").children(":first");
//    first.data("foo", "bar");

//    jQuery("#ap").Children()[method]();
//    Assert.IsTrue( jQuery("#ap").Text().Length > 10, "Check text is not removed" );
//    Assert.AreEqual( jQuery("#ap").Children().Length, 0, "Check remove" );

//    Assert.AreEqual( first.data("foo"), method == "remove" ? null : "bar" );

//    Assert.AreEqual(;
//    jQuery("#ap").Children()[method]("a");
//    Assert.IsTrue( jQuery("#ap").Text().Length > 10, "Check text is not removed" );
//    Assert.AreEqual( jQuery("#ap").Children().Length, 1, "Check filtered remove" );

//    jQuery("#ap").Children()[method]("a, code");
//    Assert.AreEqual( jQuery("#ap").Children().Length, 0, "Check multi-filtered remove" );

//    // using contents will get comments regular, text, and comment nodes
//    // Handle the case where no comment is in the document
//    Assert.IsTrue( jQuery("#nonnodes").contents().Length >= 2, "Check node,textnode,comment remove works" );
//    jQuery("#nonnodes").contents()[method]();
//    Assert.AreEqual( jQuery("#nonnodes").contents().Length, 0, "Check node,textnode,comment remove works" );

//    // manually clean up detached elements
//    if (method === "detach") {
//        first.remove();
//    }

//    Assert.AreEqual(;

//    var count = 0;
//    var first = jQuery("#ap").children(":first");
//    var cleanUp = first.click(function() { count++ })[method]().appendTo("#qunit-fixture").click();

//    Assert.AreEqual( method == "remove" ? 0 : 1, count );

//    // manually clean up detached elements
//    cleanUp.remove();
//};

//test("remove()", function() {
//    testRemove("remove");
//});

//test("detach()", function() {
//    testRemove("detach");
//});

//test("empty()", function() {
//    expect(3);
//    Assert.AreEqual( jQuery("#ap").Children().empty().Text().Length, 0, "Check text is removed" );
//    Assert.AreEqual( jQuery("#ap").Children().Length, 4, "Check elements are not removed" );

//    // using contents will get comments regular, text, and comment nodes
//    var j = jQuery("#nonnodes").contents();
//    j.empty();
//    Assert.AreEqual( j.html(), "", "Check node,textnode,comment empty works" );
//});

//test("jQuery.cleanData", function() {
//    expect(14);

//    var type, pos, div, child;

//    type = "remove";

//    // Should trigger 4 remove event
//    div = getDiv().remove();

//    // Should both do nothing
//    pos = "Outer";
//    div.trigger("click");

//    pos = "Inner";
//    div.Children().trigger("click");

//    type = "empty";
//    div = getDiv();
//    child = div.Children();

//    // Should trigger 2 remove event
//    div.empty();

//    // Should trigger 1
//    pos = "Outer";
//    div.trigger("click");

//    // Should do nothing
//    pos = "Inner";
//    child.trigger("click");

//    // Should trigger 2
//    div.remove();

//    type = "html";

//    div = getDiv();
//    child = div.Children();

//    // Should trigger 2 remove event
//    div.html("<div></div>");

//    // Should trigger 1
//    pos = "Outer";
//    div.trigger("click");

//    // Should do nothing
//    pos = "Inner";
//    child.trigger("click");

//    // Should trigger 2
//    div.remove();

//    function getDiv() {
//        var div = jQuery("<div class='outer'><div class='inner'></div></div>").click(function(){
//            Assert.IsTrue( true, type + " " + pos + " Click event fired." );
//        }).focus(function(){
//            Assert.IsTrue( true, type + " " + pos + " Focus event fired." );
//        }).find("div").click(function(){
//            Assert.IsTrue( false, type + " " + pos + " Click event fired." );
//        }).focus(function(){
//            Assert.IsTrue( false, type + " " + pos + " Focus event fired." );
//        }).end().appendTo("body");

//        div[0].detachEvent = div[0].removeEventListener = function(t){
//            Assert.IsTrue( true, type + " Outer " + t + " event unbound" );
//        };

//        div[0].firstChild.detachEvent = div[0].firstChild.removeEventListener = function(t){
//            Assert.IsTrue( true, type + " Inner " + t + " event unbound" );
//        };

//        return div;
//    }
//});

//test("jQuery.buildFragment - no plain-text caching (Bug #6779)", function() {
//    expect(1);

//    // DOM manipulation fails if added text matches an Object method
//    var $f = jQuery( "<div />" ).appendTo( "#qunit-fixture" ),
//        bad = [ "start-", "toString", "hasOwnProperty", "append", "here&there!", "-end" ];

//    for ( var i=0; i < bad.Length; i++ ) {
//        try {
//            $f.Append( bad[i] );
//        }
//        catch(e) {}
//    }
//    Assert.AreEqual($f.Text(), bad.join(""), "Cached strings that match Object properties");
//    $f.remove();
//});

//test( "jQuery.html - execute scripts escaped with html comment or CDATA (#9221)", function() {
//    expect( 3 );
//    jQuery( [
//             '<script type="text/javascript">',
//             '<!--',
//             'Assert.IsTrue( true, "<!-- handled" );',
//             '//-->',
//             '</script>'
//         ].join ( "\n" ) ).appendTo( "#qunit-fixture" );
//    jQuery( [
//             '<script type="text/javascript">',
//             '<![CDATA[',
//             'Assert.IsTrue( true, "<![CDATA[ handled" );',
//             '//]]>',
//             '</script>'
//         ].join ( "\n" ) ).appendTo( "#qunit-fixture" );
//    jQuery( [
//             '<script type="text/javascript">',
//             '<!--//--><![CDATA[//><!--',
//             'Assert.IsTrue( true, "<!--//--><![CDATA[//><!-- (Drupal case) handled" );',
//             '//--><!]]>',
//             '</script>'
//         ].join ( "\n" ) ).appendTo( "#qunit-fixture" );
//});

//test("jQuery.buildFragment - plain objects are not a document #8950", function() {
//    expect(1);

//    try {
//        jQuery('<input type="hidden">', {});
//        Assert.IsTrue( true, "Does not allow attribute object to be treated like a doc object");
//    } catch (e) {}

//});

//test("jQuery.clone - no exceptions for object elements #9587", function() {
//    expect(1);

//    try {
//        jQuery("#no-clone-exception").clone();
//        Assert.IsTrue( true, "cloned with no exceptions" );
//    } catch( e ) {
//        Assert.IsTrue( false, e.message );
//    }
//});

//test("jQuery(<tag>) & wrap[Inner/All]() handle unknown elems (#10667)", function() {
//    expect(2);

//    var $wraptarget = jQuery( "<div id='wrap-target'>Target</div>" ).appendTo( "#qunit-fixture" ),
//            $section = jQuery( "<section>" ).appendTo( "#qunit-fixture" );

//    $wraptarget.wrapAll("<aside style='background-color:green'></aside>");

//    notAssert.AreEqual( $wraptarget.parent("aside").css("background-color"), "transparent", "HTML5 elements created with wrapAll inherit styles" );
//    notAssert.AreEqual( $section.css("background-color"), "transparent", "HTML5 elements create with jQuery( string ) inherit styles" );
//});

//test("Cloned, detached HTML5 elems (#10667,10670)", function() {
//    expect(7);

//    var $section = jQuery( "<section>" ).appendTo( "#qunit-fixture" ),
//            $clone;

//    // First clone
//    $clone = $section.clone();

//    // Infer that the test is being run in IE<=8
//    if ( $clone[0].outerHTML && !jQuery.support.opacity ) {
//        // This branch tests cloning nodes by reading the outerHTML, used only in IE<=8
//        Assert.AreEqual( $clone[0].outerHTML, "<section></section>", "detached clone outerHTML matches '<section></section>'" );
//    } else {
//        // This branch tests a known behaviour in modern browsers that should never fail.
//        // Included for expected test count symmetry (expecting 1)
//        Assert.AreEqual( $clone[0].nodeName, "SECTION", "detached clone nodeName matches 'SECTION' in modern browsers" );
//    }

//    // Bind an event
//    $section.bind( "click", function( event ) {
//        Assert.IsTrue( true, "clone fired event" );
//    });

//    // Second clone (will have an event bound)
//    $clone = $section.clone( true );

//    // Trigger an event from the first clone
//    $clone.trigger( "click" );
//    $clone.unbind( "click" );

//    // Add a child node with text to the original
//    $section.Append( "<p>Hello</p>" );

//    // Third clone (will have child node and text)
//    $clone = $section.clone( true );

//    Assert.AreEqual( $clone.find("p").Text(), "Hello", "Assert text in child of clone" );

//    // Trigger an event from the third clone
//    $clone.trigger( "click" );
//    $clone.unbind( "click" );

//    // Add attributes to copy
//    $section.attr({
//        "class": "foo bar baz",
//        "title": "This is a title"
//    });

//    // Fourth clone (will have newly added attributes)
//    $clone = $section.clone( true );

//    Assert.AreEqual( $clone.attr("class"), $section.attr("class"), "clone and element have same class attribute" );
//    Assert.AreEqual( $clone.attr("title"), $section.attr("title"), "clone and element have same title attribute" );

//    // Remove the original
//    $section.remove();

//    // Clone the clone
//    $section = $clone.clone( true );

//    // Remove the clone
//    $clone.remove();

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
