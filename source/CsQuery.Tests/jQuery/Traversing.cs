using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;

namespace CsQuery.Tests.jQuery
{
    [TestFixture, TestClass]
    public class Traversing: CsQueryTest 
    {
        [SetUp]
        public override void FixtureSetUp()
        {
            base.FixtureSetUp();
            ResetQunit();
        }
        [Test,TestMethod]
        public void Find_String()
        {
            Assert.AreEqual(jQuery("#qunit-fixture").Find("> #foo, > #moretests").Get(), q("foo", "moretests"), "find child elements");

            Assert.AreEqual("Yahoo", jQuery("#foo").Find(".blogTest").Text(), "Check for find" );

            // using contents will get comments regular, text, and comment nodes
            var j = jQuery("#nonnodes").Contents();
            Assert.AreEqual(j.Find("div").Length, 0, "Check node,textnode,comment to find zero divs" );

            Assert.AreEqual(jQuery("#qunit-fixture").Find("> div").Get(), q("foo", "moretests", "tabindex-tests", "liveHandlerOrder", "siblingTest"), "find child elements" );
            
            Assert.AreEqual(jQuery("#qunit-fixture").Find("> #foo > p").Get(), q("sndp", "en", "sap"), "find child elements" );
        }
        [Test,TestMethod]
        public void Find_Node_Object()
    {
            var jfoo = jQuery("#foo");
            var jblog = jQuery(".blogTest");
            var jfirst = jQuery("#first");
            var jtwo = jblog.Add( jfirst );
            var jfooTwo = jfoo.Add( jblog );

            Assert.AreEqual(jfoo.Find( jblog ).Text(), "Yahoo", "Find with blog jQuery object" );
            Assert.AreEqual(jfoo.Find( jblog[0] ).Text(), "Yahoo", "Find with blog node" );
            Assert.AreEqual(jfoo.Find( jfirst ).Length, 0, "#first is not in #foo" );
            Assert.AreEqual(jfoo.Find( jfirst[0]).Length, 0, "#first not in #foo (node)" );
            
            Assert.IsTrue(jfoo.Find( jtwo ).Is(".blogTest"), "Find returns only nodes within #foo" );
            Assert.IsTrue(jfooTwo.Find(jblog).Is(".blogTest"), "Blog is part of the collection, but also within foo");
            Assert.IsTrue(jfooTwo.Find(jblog[0]).Is(".blogTest"), "Blog is part of the collection, but also within foo(node)");

            Assert.AreEqual(jtwo.Find( jfoo ).Length, 0, "Foo is not in two elements" );
            Assert.AreEqual(jtwo.Find( jfoo[0] ).Length, 0, "Foo is not in two elements(node)" );
            Assert.AreEqual(jtwo.Find( jfirst ).Length, 0, "first is in the collection and not within two" );
            Assert.AreEqual(jtwo.Find( jfirst ).Length, 0, "first is in the collection and not within two(node)" );

        }

        [Test,TestMethod]
        public void Is_String_Undefined()
        {
                Assert.IsTrue( jQuery("#form").Is("form"), "Check for element: A form must be a form" );
                Assert.IsTrue( !jQuery("#form").Is("div"), "Check for element: A form is not a div" );
                Assert.IsTrue( jQuery("#mark").Is(".blog"), "Check for class: Expected class 'blog'" );
                Assert.IsTrue( !jQuery("#mark").Is(".link"), "Check for class: Did not expect class 'link'" );
                Assert.IsTrue( jQuery("#simon").Is(".blog.link"), "Check for multiple classes: Expected classes 'blog' and 'link'" );
                Assert.IsTrue( !jQuery("#simon").Is(".blogTest"), "Check for multiple classes: Expected classes 'blog' and 'link', but not 'blogTest'" );
                Assert.IsTrue( jQuery("#en").Is("[lang=\"en\"]"), "Check for attribute: Expected attribute lang to be 'en'" );
                Assert.IsTrue( !jQuery("#en").Is("[lang=\"de\"]"), "Check for attribute: Expected attribute lang to be 'en', not 'de'" );
                Assert.IsTrue( jQuery("#text1").Is("[type=\"text\"]"), "Check for attribute: Expected attribute type to be 'text'" );
                Assert.IsTrue( !jQuery("#text1").Is("[type=\"radio\"]"), "Check for attribute: Expected attribute type to be 'text', not 'radio'" );
                Assert.IsTrue( jQuery("#text2").Is(":disabled"), "Check for pseudoclass: Expected to be disabled" );
                Assert.IsTrue( !jQuery("#text1").Is(":disabled"), "Check for pseudoclass: Expected not disabled" );
                Assert.IsTrue( jQuery("#radio2").Is(":checked"), "Check for pseudoclass: Expected to be checked" );
                Assert.IsTrue( !jQuery("#radio1").Is(":checked"), "Check for pseudoclass: Expected not checked" );
                Assert.IsTrue( jQuery("#foo").Is(":has(p)"), "Check for child: Expected a child 'p' element" );
                Assert.IsTrue( !jQuery("#foo").Is(":has(ul)"), "Check for child: Did not expect 'ul' element" );
                Assert.IsTrue( jQuery("#foo").Is(":has(p):has(a):has(code)"), "Check for childs: Expected 'p', 'a' and 'code' child elements" );
                Assert.IsTrue( !jQuery("#foo").Is(":has(p):has(a):has(code):has(ol)"), "Check for childs: Expected 'p', 'a' and 'code' child elements, but no 'ol'" );

                //Assert.IsTrue( !jQuery("#foo").Is(0), "Expected false for an invalid expression - 0" );
                //Assert.IsTrue( !jQuery("#foo").Is(null), "Expected false for an invalid expression - null" );
                Assert.IsTrue( !jQuery("#foo").Is(""), "Expected false for an invalid expression - \"\"" );
                //Assert.IsTrue( !jQuery("#foo").Is(undefined), "Expected false for an invalid expression - undefined" );
                //Assert.IsTrue( !jQuery("#foo").Is({ plain: "object" }), "Check passing invalid object" );

                // test is() with comma-seperated expressions
                Assert.IsTrue( jQuery("#en").Is("[lang=\"en\"],[lang=\"de\"]"), "Comma-seperated; Check for lang attribute: Expect en or de" );
                Assert.IsTrue( jQuery("#en").Is("[lang=\"de\"],[lang=\"en\"]"), "Comma-seperated; Check for lang attribute: Expect en or de" );
                Assert.IsTrue( jQuery("#en").Is("[lang=\"en\"] , [lang=\"de\"]"), "Comma-seperated; Check for lang attribute: Expect en or de" );
                Assert.IsTrue( jQuery("#en").Is("[lang=\"de\"] , [lang=\"en\"]"), "Comma-seperated; Check for lang attribute: Expect en or de" );
        }

        [Test,TestMethod]
        public void Is_Jquery()
        {

            Assert.IsTrue( jQuery("#form").Is( jQuery("form") ), "Check for element: A form is a form" );
            Assert.IsTrue( !jQuery("#form").Is( jQuery("div") ), "Check for element: A form is not a div" );
            Assert.IsTrue( jQuery("#mark").Is( jQuery(".blog") ), "Check for class: Expected class 'blog'" );
            Assert.IsTrue( !jQuery("#mark").Is( jQuery(".link") ), "Check for class: Did not expect class 'link'" );
            Assert.IsTrue( jQuery("#simon").Is( jQuery(".blog.link") ), "Check for multiple classes: Expected classes 'blog' and 'link'" );
            Assert.IsTrue( !jQuery("#simon").Is( jQuery(".blogTest") ), "Check for multiple classes: Expected classes 'blog' and 'link', but not 'blogTest'" );
            Assert.IsTrue( jQuery("#en").Is( jQuery("[lang=\"en\"]") ), "Check for attribute: Expected attribute lang to be 'en'" );
            Assert.IsTrue( !jQuery("#en").Is( jQuery("[lang=\"de\"]") ), "Check for attribute: Expected attribute lang to be 'en', not 'de'" );
            Assert.IsTrue( jQuery("#text1").Is( jQuery("[type=\"text\"]") ), "Check for attribute: Expected attribute type to be 'text'" );
            Assert.IsTrue( !jQuery("#text1").Is( jQuery("[type=\"radio\"]") ), "Check for attribute: Expected attribute type to be 'text', not 'radio'" );
            Assert.IsTrue( !jQuery("#text1").Is( jQuery("input:disabled") ), "Check for pseudoclass: Expected not disabled" );
            Assert.IsTrue( jQuery("#radio2").Is( jQuery("input:checked") ), "Check for pseudoclass: Expected to be checked" );
            Assert.IsTrue( !jQuery("#radio1").Is( jQuery("input:checked") ), "Check for pseudoclass: Expected not checked" );
            Assert.IsTrue( jQuery("#foo").Is( jQuery("div:has(p)") ), "Check for child: Expected a child 'p' element" );
            Assert.IsTrue( !jQuery("#foo").Is( jQuery("div:has(ul)") ), "Check for child: Did not expect 'ul' element" );

            // Some raw elements
            Assert.IsTrue( jQuery("#form").Is( jQuery("form")[0] ), "Check for element: A form is a form" );
            Assert.IsTrue( !jQuery("#form").Is( jQuery("div")[0] ), "Check for element: A form is not a div" );
            Assert.IsTrue( jQuery("#mark").Is( jQuery(".blog")[0] ), "Check for class: Expected class 'blog'" );
            Assert.IsTrue( !jQuery("#mark").Is( jQuery(".link")[0] ), "Check for class: Did not expect class 'link'" );
            Assert.IsTrue( jQuery("#simon").Is( jQuery(".blog.link")[0] ), "Check for multiple classes: Expected classes 'blog' and 'link'" );
            Assert.IsTrue( !jQuery("#simon").Is( jQuery(".blogTest")[0] ), "Check for multiple classes: Expected classes 'blog' and 'link', but not 'blogTest'" );
        }
        [Test,TestMethod]
        public void Index()
        {
            Assert.AreEqual(jQuery("#text2").Index(), 2, "Returns the index of a child amongst its siblings");
        }
        [Test,TestMethod]
        public void Index_object_string_undefined()
        {

        }
        [Test, TestMethod]
        public void IndexStringSelector()
        {

            // switched out Window with Body, no concept of Window here
            var window = jQuery("body")[0];
            var elements = jQuery(document).Add(window);

            var inputElements = jQuery("#radio1,#radio2,#check1,#check2");

            // Passing a node
            Assert.AreEqual(elements.Index(window), 1, "Check for index of elements" );
            Assert.AreEqual(elements.Index(Dom.Document),0, "Check for index of elements" );
            Assert.AreEqual(inputElements.Index(document.GetElementById("radio1")), 0, "Check for index of elements" );
            Assert.AreEqual(inputElements.Index(document.GetElementById("radio2")), 1, "Check for index of elements" );
            Assert.AreEqual(inputElements.Index(document.GetElementById("check1")), 2, "Check for index of elements" );
            Assert.AreEqual(inputElements.Index(document.GetElementById("check2")), 3, "Check for index of elements" );
            Assert.AreEqual(inputElements.Index(window), -1, "Check for not found index" );
            Assert.AreEqual(inputElements.Index(Dom.Document), -1, "Check for not found index" );

            // Passing a jQuery object
            // enabled since [5500]
            Assert.AreEqual(elements.Index( elements ), 0, "Pass in a jQuery object" );
            Assert.AreEqual(elements.Index( elements.Eq(1) ), 1, "Pass in a jQuery object" );
            Assert.AreEqual(jQuery("#form :radio").Index( jQuery("#radio2") ), 1, "Pass in a jQuery object" );

            // Passing a selector or nothing
            // enabled since [6330]
            Assert.AreEqual(jQuery("#text2").Index(), 2, "Check for index amongst siblings" );
            Assert.AreEqual(jQuery("#form").Children().Eq(4).Index(), 4, "Check for index amongst siblings" );

            Assert.AreEqual(jQuery("#radio2").Index("#form :radio") , 1, "Check for index within a selector" );
            Assert.AreEqual(jQuery("#form :radio").Index( jQuery("#radio2") ), 1, "Check for index within a selector" );
            Assert.AreEqual(jQuery("#radio2").Index("#form :text") , -1, "Check for index not found within a selector" );
            
        }

        [Test,TestMethod]
        public void Filter_Selector()
        {
        
            Assert.AreEqual(jQuery("#form input").Filter(":checked").Get(), q("radio2", "check1"), "filter(String)" );
            Assert.AreEqual(jQuery("p").Filter("#ap, #sndp").Get(), q("ap", "sndp"), "filter('String, String')" );
            Assert.AreEqual(jQuery("p").Filter("#ap,#sndp").Get(), q("ap", "sndp"), "filter('String,String')" );

            Assert.AreEqual(jQuery("p").Filter((string)null).Length, 0, "filter(null) should return an empty jQuery object");
            Assert.AreEqual(jQuery("p").Filter((string)null).Length, 0, "filter(undefined) should return an empty jQuery object");
            //Assert.AreEqual(jQuery("p").Filter(0).Get(),         [], "filter(0) should return an empty jQuery object");
            Assert.AreEqual(jQuery("p").Filter("").Length,        0, "filter('') should return an empty jQuery object");

            // using contents will get comments regular, text, and comment nodes
            var j = jQuery("#nonnodes").Contents();
            Assert.AreEqual(j.Filter("span").Length, 1, "Check node,textnode,comment to filter the one span" );
            Assert.AreEqual(j.Filter("[name]").Length, 0, "Check node,textnode,comment to filter the one span" );

            
        }
        [Test,TestMethod]
        public void Filter_Function()
        {
            Func<IDomObject,bool> testFunction1 = (obj) => {
                return jQuery(obj).Find("a").Length == 0;
            };

            Func<IDomObject,int,bool> testFunction2 = (obj,i) => {
                return jQuery(obj).Find("a").Length == 0;
            };

            Assert.AreEqual(jQuery("#qunit-fixture p").Filter(testFunction1)
                .Get(), q("sndp", "first"), "filter(Function)" );

            Assert.AreEqual(jQuery("#qunit-fixture p").Filter(testFunction2)
                .Get(), q("sndp", "first"), "filter(Function) using arg" );
        }

        [Test, TestMethod]
        public void FilterElement()
        {
            var element = document.GetElementById("text1");
            Assert.AreEqual(jQuery("#form input").Filter(element).Get(), q("text1"), "filter(Element)");
        }

        [Test, TestMethod]
        public void FilterElementList()
        {

            var elements = new List<IDomElement>();
            elements.Add(document.GetElementById("text1"));
            elements.Add(document.GetElementById("text2"));

            Assert.AreEqual(jQuery("#form input").Filter(elements).Get(), q("text1","text2"), "filter(Element)" );
        }

        [Test,TestMethod]
        public void Closest()
        {
            Assert.AreEqual(jQuery("body").Closest("body").Get(), q("body"), "closest(body)" );
            Assert.AreEqual(jQuery("body").Closest("html").Get(), q("html"), "closest(html)" );
            Assert.AreEqual(jQuery("body").Closest("div").Length, 0, "closest(div)" );
            Assert.AreEqual(jQuery("#qunit-fixture").Closest("span,#html").Get(), q("html"), "closest(span,#html)" );

            Assert.AreEqual(jQuery("div:eq(1)").Closest("div:first").Length, 0, "closest(div:first)" );
            Assert.AreEqual(jQuery("div").Closest("body:first div:last").Get(), q("fx-tests"), "closest(body:first div:last)" );

            // Test .Closest() limited by the context
            var jq = jQuery("#nothiddendivchild");
            var body = jQuery("body")[0];

            //Assert.AreEqual(jq.Closest("html", body).Get(), [], "Context limited." );
            //Assert.AreEqual(jq.Closest("body", body).Get(), [], "Context limited." );
            //Assert.AreEqual(jq.Closest("#nothiddendiv", document.body).Get(), q("nothiddendiv"), "Context not reached." );

            //Test that .Closest() returns unique'd set
            Assert.AreEqual(jQuery("#qunit-fixture p").Closest("#qunit-fixture").Length, 1, "Closest should return a unique set" );

            // Test on disconnected node
            Assert.AreEqual(jQuery("<div><p></p></div>").Find("p").Closest("table").Length, 0, "Make sure disconnected closest work." );

            // Bug #7369
            Assert.AreEqual(CQ.CreateFragment("<div foo='bar'></div>").Closest("[foo]").Length, 1, "Disconnected nodes with attribute selector");
            Assert.AreEqual(CQ.CreateFragment("<div>text</div>").Closest("[lang]").Length, 0, "Disconnected nodes with text and non-existent attribute selector");
        }

//test("closest(Array)", function() {
//    expect(7);
//    Assert.AreEqual(jQuery("body").closest(["body"]), [{selector:"body", elem:document.body, level:1}], "closest([body])" );
//    Assert.AreEqual(jQuery("body").closest(["html"]), [{selector:"html", elem:document.documentElement, level:2}], "closest([html])" );
//    Assert.AreEqual(jQuery("body").closest(["div"]), [], "closest([div])" );
//    Assert.AreEqual(jQuery("#yahoo").closest(["div"]), [{"selector":"div", "elem": document.getElementById("foo"), "level": 3}, { "selector": "div", "elem": document.getElementById("qunit-fixture"), "level": 4 }], "closest([div])" );
//    Assert.AreEqual(jQuery("#qunit-fixture").closest(["span,#html"]), [{selector:"span,#html", elem:document.documentElement, level:4}], "closest([span,#html])" );

//    Assert.AreEqual(jQuery("body").closest(["body","html"]), [{selector:"body", elem:document.body, level:1}, {selector:"html", elem:document.documentElement, level:2}], "closest([body, html])" );
//    Assert.AreEqual(jQuery("body").closest(["span","html"]), [{selector:"html", elem:document.documentElement, level:2}], "closest([body, html])" );
//});
        [Test,TestMethod]
        public void Closest2()
        {
        
        
            var jchild = jQuery("#nothiddendivchild");
            var jparent = jQuery("#nothiddendiv");
            var jmain = jQuery("#qunit-fixture");
            var jbody = jQuery("body");
            Assert.IsTrue(jchild.Closest( jparent ).Is("#nothiddendiv"), "closest( jQuery('#nothiddendiv') )" );
            Assert.IsTrue( jchild.Closest( jparent[0] ).Is("#nothiddendiv"), "closest( jQuery('#nothiddendiv') ) :: node" );
            Assert.IsTrue( jchild.Closest( jchild ).Is("#nothiddendivchild"), "child is included" );
            Assert.IsTrue( jchild.Closest( jchild[0] ).Is("#nothiddendivchild"), "child is included  :: node" );
            Assert.AreEqual(jchild.Closest( document.CreateElement("div") ).Length, 0, "created element is not related" );
            Assert.AreEqual(jchild.Closest( jmain ).Length, 0, "Main not a parent of child" );
            Assert.AreEqual(jchild.Closest( jmain[0] ).Length, 0, "Main not a parent of child :: node" );
            Assert.IsTrue( jchild.Closest( jbody.Add(jparent) ).Is("#nothiddendiv"), "Closest ancestor retrieved." );

        }
        [Test,TestMethod]
        public void NotSelector()
        {
            Assert.AreEqual(jQuery("#qunit-fixture > p#ap > a").Not("#google").Length, 2, "not('selector')" );
            Assert.AreEqual(jQuery("p").Not(".result").Get(), q("firstp", "ap", "sndp", "en", "sap", "first"), "not('.class')" );
            Assert.AreEqual(jQuery("p").Not("#ap, #sndp, .result").Get(), q("firstp", "en", "sap", "first"), "not('selector, selector')" );

            // # I believe option5a should be returned, and the jQuery test is buggy
           // Assert.AreEqual(jQuery("#form option").Not("option.emptyopt:contains('Nothing'),[selected],[value='1']").Get(), q("option1c", "option1d", "option2c", "option3d", "option3e", "option4e","option5b"), "not('complex selector')");
            Assert.AreEqual(jQuery("#form option").Not("option.emptyopt:contains('Nothing'),[selected],[value='1']").Get(), q("option1c", "option1d", "option2c", "option3d", "option3e", "option4e", "option5a", "option5b"), "not('complex selector')");
            Assert.AreEqual(jQuery("#ap *").Not("code").Get(), q("google", "groups", "anchor1", "mark"), "not('tag selector')" );
            Assert.AreEqual(jQuery("#ap *").Not("code, #mark").Get(), q("google", "groups", "anchor1"), "not('tag, ID selector')" );
            Assert.AreEqual(jQuery("#ap *").Not("#mark, code").Get(), q("google", "groups", "anchor1"), "not('ID, tag selector')");

            var all = jQuery("p").Get();
            Assert.AreEqual(jQuery("p").Not((string)null).Get(),      all, "not(null) should have no effect");
            Assert.AreEqual(jQuery("p").Not((string)null).Get(), all, "not(undefined) should have no effect");
            //Assert.AreEqual(jQuery("p").Not(0).Get(),         all, "not(0) should have no effect");
            Assert.AreEqual(jQuery("p").Not("").Get(),        all, "not('') should have no effect");
        }

        [Test, TestMethod]
        public void NotElement()
        {
            var selects = jQuery("#form select");
            Assert.AreEqual(selects.Not(selects[1]).Get(), q("select1", "select3", "select4", "select5"), "filter out DOM element");

        }


//test("not(Function)", function() {
//    Assert.AreEqual(jQuery("#qunit-fixture p").Not(function() { return jQuery("a", this).Length }).Get(), q("sndp", "first"), "not(Function)" );
//});

        [Test, TestMethod]
        public void NotArray()
        {
            Assert.AreEqual(jQuery("#qunit-fixture > p#ap > a").Not(document.GetElementById("google")).Length, 2, "not(DOMElement)");
            Assert.AreEqual(jQuery("p").Not(document.GetElementsByTagName("p")).Length, 0, "not(Array-like DOM collection)");
        }
        
//test("not(jQuery)", function() {
//    expect(1);

//    Assert.AreEqual(jQuery("p").Not(jQuery("#ap, #sndp, .result")).Get(), q("firstp", "en", "sap", "first"), "not(jQuery)" );
//});
        [Test, TestMethod]
        public void HasElement()
        {
            
            var obj = jQuery("#qunit-fixture").Has(jQuery("#sndp")[0]);
            Assert.AreEqual(obj.Get(), q("qunit-fixture"), "Keeps elements that have the element as a descendant" );

            var multipleParent = jQuery("#qunit-fixture, #header").Has(jQuery("#sndp")[0]);
            Assert.AreEqual(obj.Get(), q("qunit-fixture"), "Does not include elements that do not have the element as a descendant" );
        }
        [Test, TestMethod]
        public void HasSelector()
        {
            var obj = jQuery("#qunit-fixture").Has("#sndp");
            Assert.AreEqual(obj.Get(), q("qunit-fixture"), "Keeps elements that have any element matching the selector as a descendant");

            var multipleParent = jQuery("#qunit-fixture, #header").Has("#sndp");
            Assert.AreEqual(obj.Get(), q("qunit-fixture"), "Does not include elements that do not have the element as a descendant");

            var multipleHas = jQuery("#qunit-fixture").Has("#sndp, #first");
            Assert.AreEqual(multipleHas.Get(), q("qunit-fixture"), "Only adds elements once");
        }

        [Test, TestMethod]
        public void HasArrayish()
        {
            var simple = jQuery("#qunit-fixture").Has(jQuery("#sndp"));
            Assert.AreEqual(simple.Get(), q("qunit-fixture"), "Keeps elements that have any element in the jQuery list as a descendant");

            var multipleParent = jQuery("#qunit-fixture, #header").Has(jQuery("#sndp"));
            Assert.AreEqual(multipleParent.Get(), q("qunit-fixture"), "Does not include elements that do not have an element in the jQuery list as a descendant");

            var multipleHas = jQuery("#qunit-fixture").Has(jQuery("#sndp, #first"));
            Assert.AreEqual(simple.Get(), q("qunit-fixture"), "Only adds elements once");
        }

        [Test, TestMethod]
        public void AndSelf()
        {
            Assert.AreEqual(jQuery("#en").Siblings().AndSelf().Get(), q("sndp", "en", "sap"), "Check for siblings and self");
            Assert.AreEqual(jQuery("#foo").Children().AndSelf().Get(), q("foo", "sndp", "en", "sap"), "Check for children and self");
            Assert.AreEqual(jQuery("#sndp, #en").Parent().AndSelf().Get(), q("foo", "sndp", "en"), "Check for parent and self");
            Assert.AreEqual(jQuery("#groups").Parents("p, div").AndSelf().Get(), q("qunit-fixture", "ap", "groups"), "Check for parents and self");

        }
        [Test, TestMethod]
        public void Siblings()
        {
            Assert.AreEqual(jQuery("#en").Siblings().Get(), q("sndp", "sap"), "Check for siblings");
            Assert.AreEqual(jQuery("#sndp").Siblings(":has(code)").Get(), q("sap"), "Check for filtered siblings (has code child element)");
            Assert.AreEqual(jQuery("#sndp").Siblings(":has(a)").Get(), q("en", "sap"), "Check for filtered siblings (has anchor child element)");
            Assert.AreEqual(jQuery("#foo").Siblings("form, b").Get(), q("form", "floatTest", "lengthtest", "name-tests", "testForm"), "Check for multiple filters");
            var set = q("sndp", "en", "sap");
            Assert.AreEqual(jQuery("#en, #sndp").Siblings().Get(), set, "Check for unique results from siblings");
        }
        [Test, TestMethod]
        public void Children()
        {
            Assert.AreEqual(jQuery("#foo").Children().Get(), q("sndp", "en", "sap"), "Check for children");
            Assert.AreEqual(jQuery("#foo").Children(":has(code)").Get(), q("sndp", "sap"), "Check for filtered children");
            Assert.AreEqual(jQuery("#foo").Children("#en, #sap").Get(), q("en", "sap"), "Check for multiple filters");

        }
        [Test, TestMethod]
        public void Parent()
        {
            Assert.AreEqual(jQuery("#groups").Parent()[0].Id, "ap", "Simple parent check");
            Assert.AreEqual(jQuery("#groups").Parent("p")[0].Id , "ap", "Filtered parent check");
            Assert.AreEqual(jQuery("#groups").Parent("div").Length, 0, "Filtered parent check, no match");
            Assert.AreEqual(jQuery("#groups").Parent("div, p")[0].Id, "ap", "Check for multiple filters");
            Assert.AreEqual(jQuery("#en, #sndp").Parent().Get(), q("foo"), "Check for unique results from parent");
            
        }
        [Test, TestMethod]
        public void Parents()
        {
            Assert.AreEqual(jQuery("#groups").Parents()[0].Id, "ap", "Simple parents check" );
            Assert.AreEqual(jQuery("#groups").Parents("p")[0].Id, "ap", "Filtered parents check" );
            Assert.AreEqual(jQuery("#groups").Parents("div")[0].Id, "qunit-fixture", "Filtered parents check2" );
            Assert.AreEqual(jQuery("#groups").Parents("p, div").Get(), q("ap", "qunit-fixture"), "Check for multiple filters" );
            Assert.AreEqual(jQuery("#en, #sndp").Parents().Get(), q("foo", "qunit-fixture", "dl", "body", "html"), "Check for unique results from parents" );

        }
        [Test, TestMethod]
        public void ParentsUntil()
        {
            var parents = jQuery("#groups").Parents();
            Assert.AreEqual(jQuery("#groups").ParentsUntil().Get(), parents.Get(), "parentsUntil with no selector (nextAll)" );
            Assert.AreEqual(jQuery("#groups").ParentsUntil(".foo").Get(), parents.Get(), "parentsUntil with invalid selector (nextAll)" );
            Assert.AreEqual(jQuery("#groups").ParentsUntil("#html").Get(), parents.Not(":last").Get(), "Simple parentsUntil check" );
            Assert.AreEqual(jQuery("#groups").ParentsUntil("#ap").Length, 0, "Simple parentsUntil check" );
            Assert.AreEqual(jQuery("#groups").ParentsUntil("#html, #body").Get(), parents.Slice( 0, 3 ).Get(), "Less simple parentsUntil check" );
            Assert.AreEqual(jQuery("#groups").ParentsUntil("#html", "div").Get(), jQuery("#qunit-fixture").Get(), "Filtered parentsUntil check" );
            Assert.AreEqual(jQuery("#groups").ParentsUntil("#html", "p,div,dl").Get(), parents.Slice( 0, 3 ).Get(), "Multiple-filtered parentsUntil check" );
            Assert.AreEqual(jQuery("#groups").ParentsUntil("#html", "span").Length, 0, "Filtered parentsUntil check, no match" );
            Assert.AreEqual(jQuery("#groups, #ap").ParentsUntil("#html", "p,div,dl").Get(), parents.Slice( 0, 3 ).Get(), "Multi-source, multiple-filtered parentsUntil check" );
        
        }

        [Test, TestMethod]
        public void Next()
        {
            Assert.AreEqual(jQuery("#ap").Next()[0].Id, "foo", "Simple next check" );
            Assert.AreEqual(jQuery("#ap").Next("div")[0].Id, "foo", "Filtered next check");
            Assert.AreEqual(jQuery("#ap").Next("p").Length, 0, "Filtered next check, no match" );
            Assert.AreEqual(jQuery("#ap").Next("div, p")[0].Id, "foo", "Multiple filters");
   
        }
        [Test, TestMethod]
        public void Prev()
        {
            Assert.AreEqual(jQuery("#foo").Prev()[0].Id, "ap", "Simple prev check");
            Assert.AreEqual(jQuery("#foo").Prev("p")[0].Id, "ap", "Filtered prev check");
            Assert.AreEqual(jQuery("#foo").Prev("div").Length, 0, "Filtered prev check, no match");
            Assert.AreEqual(jQuery("#foo").Prev("p, div")[0].Id, "ap", "Multiple filters");
        }
        [Test, TestMethod]
        public void NextAll()
        {
            var elems = jQuery("#form").Children();

            Assert.AreEqual(jQuery("#label-for").NextAll().Get(), elems.Not(":first").Get(), "Simple nextAll check");
            Assert.AreEqual(jQuery("#label-for").NextAll("input").Get(), elems.Not(":first").Filter("input").Get(), "Filtered nextAll check");
            Assert.AreEqual(jQuery("#label-for").NextAll("input,select").Get(), elems.Not(":first").Filter("input,select").Get(), "Multiple-filtered nextAll check");
            Assert.AreEqual(jQuery("#label-for, #hidden1").NextAll("input,select").Get(), elems.Not(":first").Filter("input,select").Get(), "Multi-source, multiple-filtered nextAll check");

        }
        [Test, TestMethod]
        public void PrevAll()
        {
            var elems = jQuery(jQuery("#form").Children().Slice(0, 12).Get().Reverse());

            Assert.AreEqual(jQuery("#area1").PrevAll().Get(), elems.Get(), "Simple prevAll check");
            Assert.AreEqual(jQuery("#area1").PrevAll("input").Get(), elems.Filter("input").Get(), "Filtered prevAll check");
            Assert.AreEqual(jQuery("#area1").PrevAll("input,select").Get(), elems.Filter("input,select").Get(), "Multiple-filtered prevAll check");
            Assert.AreEqual(jQuery("#area1, #hidden1").PrevAll("input,select").Get(), elems.Filter("input,select").Get(), "Multi-source, multiple-filtered prevAll check");

        }
        [Test, TestMethod]
        public void NextUntil()
        {

            var elems = jQuery("#form").Children().Slice(2, 12);

            Assert.AreEqual(jQuery("#text1").NextUntil().Get(), jQuery("#text1").NextAll().Get(), "NextUntil with no selector (nextAll)");
            Assert.AreEqual(jQuery("#text1").NextUntil(".foo").Get(), jQuery("#text1").NextAll().Get(), "NextUntil with invalid selector (nextAll)");
            Assert.AreEqual(jQuery("#text1").NextUntil("#area1").Get(), elems.Get(), "Simple NextUntil check");
            Assert.AreEqual(jQuery("#text1").NextUntil("#text2").Length, 0, "Simple NextUntil check");
            Assert.AreEqual(jQuery("#text1").NextUntil("#area1, #radio1").Get(), jQuery("#text1").Next().Get(), "Less simple NextUntil check");
            Assert.AreEqual(jQuery("#text1").NextUntil("#area1", "input").Get(), elems.Not("button").Get(), "Filtered NextUntil check");
            Assert.AreEqual(jQuery("#text1").NextUntil("#area1", "button").Get(), elems.Not("input").Get(), "Filtered NextUntil check");
            Assert.AreEqual(jQuery("#text1").NextUntil("#area1", "button,input").Get(), elems.Get(), "Multiple-filtered NextUntil check");
            Assert.AreEqual(jQuery("#text1").NextUntil("#area1", "div").Length, 0, "Filtered NextUntil check, no match");
            Assert.AreEqual(jQuery("#text1, #hidden1").NextUntil("#area1", "button,input").Get(), elems.Get(), "Multi-source, multiple-filtered NextUntil check");

            Assert.AreEqual(jQuery("#text1").NextUntil("[class=foo]").Get(), jQuery("#text1").NextAll().Get(), "Non-element nodes must be skipped, since they have no attributes");

        }
       
        [Test, TestMethod]
        public void PrevUntil()
        {
            var elems = jQuery("#area1").PrevAll();

            Assert.AreEqual(jQuery("#area1").PrevUntil().Get(), elems.Get(), "prevUntil with no selector (prevAll)");
            Assert.AreEqual(jQuery("#area1").PrevUntil(".foo").Get(), elems.Get(), "prevUntil with invalid selector (prevAll)");
            Assert.AreEqual(jQuery("#area1").PrevUntil("label").Get(), elems.Not(":last").Get(), "Simple prevUntil check");
            Assert.AreEqual(jQuery("#area1").PrevUntil("#button").Length, 0, "Simple prevUntil check");
            Assert.AreEqual(jQuery("#area1").PrevUntil("label, #search").Get(), jQuery("#area1").Prev().Get(), "Less simple prevUntil check");
            Assert.AreEqual(jQuery("#area1").PrevUntil("label", "input").Get(), elems.Not(":last").Not("button").Get(), "Filtered prevUntil check");
            Assert.AreEqual(jQuery("#area1").PrevUntil("label", "button").Get(), elems.Not(":last").Not("input").Get(), "Filtered prevUntil check");
            Assert.AreEqual(jQuery("#area1").PrevUntil("label", "button,input").Get(), elems.Not(":last").Get(), "Multiple-filtered prevUntil check");
            Assert.AreEqual(jQuery("#area1").PrevUntil("label", "div").Length, 0, "Filtered prevUntil check, no match");
            Assert.AreEqual(jQuery("#area1, #hidden1").PrevUntil("label", "button,input").Get(), elems.Not(":last").Get(), "Multi-source, multiple-filtered prevUntil check");

        }

        [Test, TestMethod]
        public void Contents()
        {
           
            Assert.AreEqual(jQuery("#ap").Contents().Length, 9, "Check element contents" );
            Assert.IsTrue( jQuery("#iframe").Length>0, "Check existence of IFrame document" );

            jQuery("#loadediframe").Append(@"<html>
              <head>
                <title>iframe</title>
              </head>
              <body>
                <div><span>span text</span></div>
              </body>
            </html>");
            
            var ibody = jQuery("#loadediframe").Contents().Find("body");
            Assert.IsTrue( ibody.Length>0, "Check existance of IFrame body" );
            var csq = new CQ("span", ibody);
            Assert.AreEqual(csq.Text(), "span text", "Find span in IFrame and check its text" );

            jQuery(ibody).Append("<div>init text</div>");
            csq = new CQ("div", ibody);
            Assert.AreEqual(csq.Length, 2, "Check the original div and the new div are in IFrame" );
            csq = new CQ("div:last", ibody);
            Assert.AreEqual(csq.Text(), "init text", "Add text to div in IFrame" );

            csq = new CQ("div:last", ibody);
            csq.Text("div text");
            Assert.AreEqual(new CQ("div:last", ibody).Text(), "div text", "Add text to div in IFrame");

            csq = new CQ("div:last", ibody).Remove();
            Assert.AreEqual(jQuery("div",ibody).Length, 1, "Delete the div and check only one div left in IFrame" );

            Assert.AreEqual(jQuery("div",ibody).Text(), "span text", "Make sure the correct div is still left after deletion in IFrame" );

            csq = new CQ("<table/>", ibody);
            csq.Append("<tr><td>cell</td></tr>").AppendTo(ibody);
            csq = new CQ("table", ibody);
            csq.Remove();
            csq = new CQ("div", ibody);
            Assert.AreEqual(csq.Length, 1, "Check for JS error on add and delete of a table in IFrame" );

            // using contents will get comments regular, text, and comment nodes
            var c = jQuery("#nonnodes").Contents().Contents();
            Assert.AreEqual(c.Length, 1, "Check node,textnode,comment contents is just one" );
            Assert.AreEqual(c[0].NodeValue, "hi", "Check node,textnode,comment contents is just the one from span" );
        }
        [Test, TestMethod]
        public void Add()
        {

            Assert.AreEqual(jQuery("#sndp").Add("#en").Add("#sap").Get(), q("sndp", "en", "sap"), "Check elements from document" );
            Assert.AreEqual(jQuery("#sndp").Add( jQuery("#en")[0] ).Add( jQuery("#sap") ).Get(), q("sndp", "en", "sap"), "Check elements from document" );

            // We no longer support .Add(form.elements), unfortunately.
            // There is no way, in browsers, to reliably determine the difference
            // between form.elements and form - and doing .Add(form) and having it
            // add the form elements is way to unexpected, so this gets the boot.
            // ok( jQuery([]).Add(jQuery("#form")[0].elements).Length >= 13, "Check elements from array" );

            // For the time being, we're discontinuing support for jQuery(form.elements) since it's ambiguous in IE
            // use jQuery([]).Add(form.elements) instead.
            //Assert.AreEqual(jQuery([]).Add(jQuery("#form")[0].elements).Length, jQuery(jQuery("#form")[0].elements).Length, "Array in constructor must equals array in add()" );

            //TODO: It would be nice to have a ParentNode exist & be typed to 11 for disconnected nodes. However this creates some complexity
            //b/c we still need to keep the nodes conceptually bound to another domain. I think that we may want a special "disconnected" heirarchy
            //within and IDomDocument. For now just compare to null instead.

            var divs = jQuery("<div/>").Add("#sndp");


            //[CsQuery] Fragments have no parent node
            Assert.IsTrue( (int)divs[0].ParentNode.NodeType ==11, "Make sure the first element is still the disconnected node." );
            //Assert.IsTrue(divs[0].ParentNode==null, "Make sure the first element is still the disconnected node.");

            divs = jQuery("<div>test</div>").Add("#sndp");
            Assert.AreEqual((int)divs[0].ParentNode.NodeType, 11, "Make sure the first element is still the disconnected node." );
            //Assert.AreEqual(divs[0].ParentNode,  null, "Make sure the first element is still the disconnected node.");

            divs = jQuery("#sndp").Add("<div/>");
            
            //Assert.AreEqual(divs[1].ParentNode,null, "Make sure the first element is still the disconnected node.");
            Assert.AreEqual((int)divs[1].ParentNode.NodeType, 11, "Make sure the first element is still the disconnected node.");

            var tmp = jQuery("<div/>");

            var x = CQ.Create().Add(jQuery("<p id='x1'>xxx</p>").AppendTo(tmp)).Add(jQuery("<p id='x2'>xxx</p>").AppendTo(tmp));
            Assert.AreEqual(x[0].Id, "x1", "Check on-the-fly element1" );
            Assert.AreEqual(x[1].Id, "x2", "Check on-the-fly element2" );

            x = CQ.Create().Add(
                    jQuery("<p id='x1'>xxx</p>").AppendTo(tmp)[0]
                ).Add(
                    jQuery("<p id='x2'>xxx</p>").AppendTo(tmp)[0]
                );
            Assert.AreEqual(x[0].Id, "x1", "Check on-the-fly element1" );
            Assert.AreEqual(x[1].Id, "x2", "Check on-the-fly element2" );

            x = CQ.Create().Add(jQuery("<p id='x1'>xxx</p>")).Add(jQuery("<p id='x2'>xxx</p>"));
            Assert.AreEqual(x[0].Id, "x1", "Check on-the-fly element1" );
            Assert.AreEqual(x[1].Id, "x2", "Check on-the-fly element2" );

            x = CQ.Create().Add("<p id='x1'>xxx</p>").Add("<p id='x2'>xxx</p>");
            Assert.AreEqual(x[0].Id, "x1", "Check on-the-fly element1" );
            Assert.AreEqual(x[1].Id, "x2", "Check on-the-fly element2" );

            IDomElement notDefined = null;
            Assert.AreEqual(CQ.Create().Add(notDefined).Length, 0, "Check that undefined adds nothing");

            Assert.AreEqual(CQ.Create().Add(document.GetElementById("form")).Length, 1, "Add a form");
            Assert.AreEqual(CQ.Create().Add(document.GetElementById("select1")).Length, 1, "Add a select");
        }
        [Test, TestMethod]
        public void AddWithContext()
        {
            Assert.AreEqual(jQuery("#firstp").Add("#ap").Get(), q("firstp", "ap"), "Add selector to selector ");
            Assert.AreEqual(jQuery(document.GetElementById("firstp")).Add("#ap").Get(), q("firstp", "ap"), "Add gEBId to selector");
            Assert.AreEqual(jQuery(document.GetElementById("firstp")).Add(document.GetElementById("ap")).Get(), q("firstp", "ap"), "Add gEBId to gEBId");

            var ctx = document.GetElementById("firstp");
            Assert.AreEqual(jQuery("#firstp").Add("#ap", ctx).Get(), q("firstp"), "Add selector to selector ");
            Assert.AreEqual(jQuery(document.GetElementById("firstp")).Add("#ap", ctx).Get(), q("firstp"), "Add gEBId to selector, not in context");
            Assert.AreEqual(jQuery(document.GetElementById("firstp")).Add("#ap", document.GetElementsByTagName("body")[0]).Get(), q("firstp", "ap"), "Add gEBId to selector, in context");

        }

    }
}
