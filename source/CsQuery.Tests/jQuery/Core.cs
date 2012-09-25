using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute;
using MsTestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using TestContext = NUnit.Framework.TestContext;
using CsQuery;
using CsQuery.Utility;

namespace CsQuery.Tests.jQuery
{
    [TestClass,TestFixture, Category("Core")]
    public class Core: CsQueryTest 
    {

        [SetUp]
        public override void FixtureSetUp()
        {
            base.FixtureSetUp();
            ResetQunit();
            
        }


        // Basic constructor's behavior
       [Test,TestMethod]
       public void Constructor()
       {
           Assert.AreEqual(0, (new CQ()).Length, "new CsQuery() === CsQuery([])");
           Assert.AreEqual(0, (CQ.Create((string)null)).Length, "new CsQuery(null) === CsQuery([])");
           Assert.AreEqual(0, (CQ.Create(new List<IDomElement>())).Length, "new CsQuery(IEnumerable<IDomElement> {}) === CsQuery([])");
           Assert.AreEqual(0, (CQ.Create("")).Length, "new CsQuery(\"\") === CsQuery([])");
        // Note - unlike jQuery, we want to pass on all HTML unchanged if reasonable. So any constructor selector will return a DOM from a string.
           Assert.AreEqual(1, (CQ.Create("#")).Length, "new CsQuery(\"#\") === CsQuery([])");
        
        // var obj = jQuery("div");
        // is the selector property officially supported? the doc page is missing.
        //    equals( jQuery(obj).selector, "div", "jQuery(jQueryObj) == jQueryObj" );
        //    equals( jQuery(window).length, 1, "Correct number of elements generated for jQuery(window)" );

       }
        [Test,TestMethod]
        public void CreatingElements()
        {
            var main = jQuery("#qunit-fixture");
            Assert.AreEqual(q("sndp", "en", "sap"), (new CQ("div p", main)).Get(), "Basic selector with jQuery object as context");
            
            var code = jQuery("<code/>");
            Assert.AreEqual(1,code.Length , "Correct number of elements generated for code" );
            Assert.AreEqual(0, code.Parent().Length, "Make sure that the generated HTML has no parent.");
            
            var img = jQuery("<img/>");
            Assert.AreEqual(1, img.Length, "Correct number of elements generated for img");
            Assert.AreEqual(0, img.Parent().Length, "Make sure that the generated HTML has no parent.");
            
            var div = jQuery("<div/><hr/><code/><b/>");
            
            Assert.AreEqual(4, div.Length, "Correct number of elements generated for div hr code b");
            Assert.AreEqual("<div></div><hr><code></code><b></b>", div.RenderSelection());
            
            Assert.AreEqual(0, div.Parent().Length, "Make sure that the generated HTML has no parent." );
            
            // why would I want to do this?
            //    equals( jQuery([1,2,3]).get(1), 2, "Test passing an array to the factory" );

            // Cheating - since we have no direct way to obtain the "body" element other than CsQuery
            // but the point it to test passing a node to the factory so it doesn't matter how we get it.

            IDomElement body = (IDomElement)jQuery("body")[0];

            Assert.AreEqual(jQuery("body").Get(0),jQuery(body).Get(0), "Test passing an html node to the factory" );
            
            // TODO: 1) not set up for this in the first place. 
            // 2. Need to parse #s into PX
            // 3. Add a "style" object

//            bool exec = false;
            var elem = CQ.Create("<div/>", @"{
                    width: 10,
                    css: { padding-left:1, padding-right:1 },
                    click: 'function(){ ok(exec, ""Click executed.""); }',
                    text: 'test',
                    class:'test2',
                    id: 'test3'}");
            
           Assert.AreEqual("10px", elem[0].Style["width"], "jQuery() quick setter width");

           Assert.AreEqual("1px", elem[0].Style["padding-left"], "jQuery quick setter css");
           Assert.AreEqual("1px", elem[0].Style["padding-right"], "jQuery quick setter css");
            Assert.AreEqual(1,elem[0].ChildNodes.Length, "jQuery quick setter text");
            Assert.AreEqual(elem[0].FirstChild.NodeValue, "test", "jQuery quick setter text");
            Assert.AreEqual(elem[0].ClassName, "test2", "jQuery() quick setter class");
            Assert.AreEqual(elem[0].Id, "test3", "jQuery() quick setter id");

            // Unnecessary

            //    for ( var i = 0; i < 3; ++i ) {
            //        elem = jQuery("<input type='text' value='TEST' />");
            //    }
            //    equals( elem[0].defaultValue, "TEST", "Ensure cached nodes are cloned properly (Bug #6655)" );

            Assert.AreEqual(3, CQ.CreateFragment(" <div/> ").Length, "Make sure whitespace is trimmed.");
            Assert.AreEqual(3, CQ.CreateFragment(" a<div></div>b ").Length, "Make sure whitespace and other characters are NOT trimmed.");
            
            // Unnecessary
            
            //    var long = "";
            //    for ( var i = 0; i < 128; i++ ) {
            //        long += "12345678";
            //    }

            //    equals( jQuery(" <div>" + long + "</div> ").length, 1, "Make sure whitespace is trimmed on long strings." );
            //    equals( jQuery(" a<div>" + long + "</div>b ").length, 1, "Make sure whitespace and other characters are trimmed on long strings." );

        }

        // Skipping SelectorState tests
        // Skipping GlobalEval tests
        // Skipping BrowserTests
        // Skipping NoConflic tests
        // Skipping a bunch of support function tests
        
        [Test,TestMethod]
        public void Html_ElementTypes()
        {
                // Test multi-line HTML
            IDomObject  div = jQuery("<div>\r\nsome text\n<p>some p</p>\nmore text\r\n</div>")[0];
            Assert.AreEqual("DIV",div.NodeName.ToUpper(),  "Make sure we're getting a div." );
            Assert.AreEqual(3,(int)div.FirstChild.NodeType,  "Text node." );
            Assert.AreEqual(3,(int)div.LastChild.NodeType,  "Text node.");
            Assert.AreEqual(1,(int)div.ChildNodes[1].NodeType, "Paragraph." );
            Assert.AreEqual(3, (int)div.ChildNodes[1].FirstChild.NodeType, "Paragraph text.");

                // Test multi-line HTML
            div = jQuery("<div>\r\nsome text\n<p>some p</p>\nmore text\r\n</div>")[0];
            Assert.AreEqual("DIV",div.NodeName.ToUpper(), "DIV", "Make sure we're getting a div." );
            Assert.AreEqual(3, (int)div.FirstChild.NodeType, "Text node.");
            Assert.AreEqual(3, (int)div.LastChild.NodeType, "Text node.");
            Assert.AreEqual(1, (int)div.ChildNodes[1].NodeType, "Paragraph.");
            Assert.AreEqual(3, (int)div.ChildNodes[1].FirstChild.NodeType, "Paragraph text.");

        }
        [Test,TestMethod]
        public void Html_Create()
        {
            Assert.IsTrue(jQuery("<link rel='stylesheet'/>").Length>0, "Creating a link");

            //Assert.AreEqual(jQuery("<script/>")[0].ParentNode,null,"Create a script");
            Assert.AreEqual(jQuery("<script/>")[0].ParentNode.NodeType, NodeType.DOCUMENT_FRAGMENT_NODE, "Create a script");
            Assert.IsTrue(jQuery("<input/>").Attr("type", "hidden").Length>0, "Create an input and set the type.");

            var j = CQ.CreateFragment("<span>hi</span> there <!-- mon ami -->");
            Assert.IsTrue(j.Length ==3 , "Check node,textnode,comment creation (some browsers delete comments [but not csQuery])");
            Assert.AreEqual(NodeType.COMMENT_NODE, j.Get(2).NodeType, "Third node is a comment");
            Assert.IsTrue(!jQuery("<option>test</option>")[0].Selected, "Make sure that options are auto-selected #2050");
            Assert.IsTrue(jQuery("<div></div>").Length>0, "Create a div with closing tag." );
            Assert.IsTrue(jQuery("<table></table>").Length > 0, "Create a table with closing tag.");
            
            var li = "<li>very large html string</li>";
            StringBuilder sb = new StringBuilder();
            sb.Append("<ul>");
            for (int i = 0; i < 50000; i += 1 ) {
                sb.Append(li);
            }
            sb.Append("</ul>");

            var el = CQ.CreateFragment(sb.ToString())[0];

            Assert.AreEqual(el.NodeName.ToUpper(), "UL");
            Assert.AreEqual(el.FirstChild.NodeName.ToUpper(), "LI");
            Assert.AreEqual(el.ChildNodes.Length, 50000 );

        }

        [Test,TestMethod]
        public void Length()
        {
            Assert.AreEqual(6,jQuery("#qunit-fixture p").Length, "Get Number of Elements Found");
        }
        
        // We are not implementing Size, it's basically obsolete
        //test("size()", function() {
        //    expect(1);
        //    equals( jQuery("#qunit-fixture p").size(), 6, "Get Number of Elements Found" );
        //});

        // Linq does this

        //test("toArray()", function() {
        //    expect(1);
        //    same( jQuery("#qunit-fixture p").toArray(),
        //        q("firstp","ap","sndp","en","sap","first"),
        //        "Convert jQuery object to an Array" )
        //})
        [Test,TestMethod]
        public void Get()
        {
            Assert.AreEqual(q("firstp", "ap", "sndp", "en", "sap", "first"), jQuery("#qunit-fixture p").Get(),  "Get All Elements");
            // no point
            //    equals( jQuery("#qunit-fixture p").get(0), document.getElementById("firstp"), "Get A Single Element" );
            Assert.IsNull(jQuery("#firstp").Get(1), "Try get with index larger elements count" );
            var p = jQuery("p");
            Assert.AreEqual(p.Get(-1), jQuery("p").Get(p.Length-2), "Get a single element with negative index" );
            Assert.IsNull(jQuery("#firstp").Get(-2), "Try get with index negative index larger then elements count" );

        }
        [Test,TestMethod]
        public void Each()
        {
            var div = jQuery("div");
            div.Each((IDomObject e) =>
           {
               e.SetAttribute("foo","zoo");
           });
            var pass = true;
            for ( var i = 0; i < div.Length; i++ ) {
                if ( div[i].GetAttribute("foo")!= "zoo" ) pass = false;
            }
            Assert.IsTrue(pass, "Execute a function, Relative" );

        }
        [Test,TestMethod]
        public void FirstLast()
        {
            var links = jQuery("#ap a");
            var none = jQuery("asdf");

            Assert.AreEqual(q("google"),links.First().Get(), "first()" );
            Assert.AreEqual(q("mark"),links.Last().Get(), "last()" );
            Assert.IsTrue(none.First().Get().IsEmpty());
            Assert.IsTrue(none.Last().Get().IsEmpty());
            
        }

        // Map has not been implemented. While this would not be hard, it's also very easily accomplished in C#. For later.

        //test("map()", function() {
        //    expect(8);

        //    same(
        //        jQuery("#ap").map(function(){
        //            return jQuery(this).find("a").get();
        //        }).get(),
        //        q("google", "groups", "anchor1", "mark"),
        //        "Array Map"
        //    );

        //    same(
        //        jQuery("#ap > a").map(function(){
        //            return this.parentNode;
        //        }).get(),
        //        q("ap","ap","ap"),
        //        "Single Map"
        //    );

        //    //for #2616
        //    var keys = jQuery.map( {a:1,b:2}, function( v, k ){
        //        return k;
        //    });
        //    equals( keys.join(""), "ab", "Map the keys from a hash to an array" );

        //    var values = jQuery.map( {a:1,b:2}, function( v, k ){
        //        return v;
        //    });
        //    equals( values.join(""), "12", "Map the values from a hash to an array" );

        //    // object with length prop
        //    var values = jQuery.map( {a:1,b:2, length:3}, function( v, k ){
        //        return v;
        //    });
        //    equals( values.join(""), "123", "Map the values from a hash with a length property to an array" );

        //    var scripts = document.getElementsByTagName("script");
        //    var mapped = jQuery.map( scripts, function( v, k ){
        //        return v;
        //    });
        //    equals( mapped.length, scripts.length, "Map an array(-like) to a hash" );

        //    var nonsense = document.getElementsByTagName("asdf");
        //    var mapped = jQuery.map( nonsense, function( v, k ){
        //        return v;
        //    });
        //    equals( mapped.length, nonsense.length, "Map an empty array(-like) to a hash" );

        //    var flat = jQuery.map( Array(4), function( v, k ){
        //        return k % 2 ? k : [k,k,k];//try mixing array and regular returns
        //    });
        //    equals( flat.join(""), "00012223", "try the new flatten technique(#2616)" );
        //});

        // A utility function, skip

        //test("jQuery.merge()", function() {
        //    expect(8);

        //    var parse = jQuery.merge;

        //    same( parse([],[]), [], "Empty arrays" );

        //    same( parse([1],[2]), [1,2], "Basic" );
        //    same( parse([1,2],[3,4]), [1,2,3,4], "Basic" );

        //    same( parse([1,2],[]), [1,2], "Second empty" );
        //    same( parse([],[1,2]), [1,2], "First empty" );

        //    // Fixed at [5998], #3641
        //    same( parse([-2,-1], [0,1,2]), [-2,-1,0,1,2], "Second array including a zero (falsy)");

        //    // After fixing #5527
        //    same( parse([], [null, undefined]), [null, undefined], "Second array including null and undefined values");
        //    same( parse({length:0}, [1,2]), {length:2, 0:1, 1:2}, "First array like");
        //});

        /*
         * extend is not implemented now - there is actually a sensible use for it using expando objects. Todo.
         */

        [Test,TestMethod]
        public void Extend()
        {
            dynamic settings = CQ.ParseJSON("{ 'xnumber1': 5, 'xnumber2': 7, 'xstring1': 'peter', 'xstring2': 'pan' }");
            dynamic options = CQ.ParseJSON("{ 'xnumber2': 1, 'xstring2': 'x', 'xxx': 'newstring'}");
            dynamic optionsCopy = CQ.ParseJSON("{ 'xnumber2': 1, 'xstring2': 'x', 'xxx': 'newstring' }");
            dynamic merged = CQ.ParseJSON("{ 'xnumber1': 5, 'xnumber2': 1, 'xstring1': 'peter', 'xstring2': 'x', 'xxx': 'newstring' }");

            dynamic deep1 = CQ.ParseJSON("{ 'foo': { 'bar': true } }");
            dynamic deep1copy = CQ.ParseJSON("{ 'foo': { 'bar': true } }");


            dynamic deep2 = CQ.ParseJSON("{ 'foo': { 'baz': true }, 'foo2': 'document' }");
            dynamic deep2copy = CQ.ParseJSON("{ 'foo': { 'baz': true }, 'foo2': 'document' }");

            dynamic deepmerged = CQ.ParseJSON("{ 'foo': { 'bar': true, 'baz': true }, 'foo2': 'document' }");
            
            var arr = new int[] {1, 2, 3};
            dynamic nestedarray = new ExpandoObject();
            nestedarray.arr = arr;

            CQ.Extend(settings, options);

            Assert.AreEqual(merged,settings, "Check if extended: settings must be extended");
            Assert.AreEqual(optionsCopy,options, "Check if not modified: options must not be modified" );


            CQ.Extend(settings, null, options);
            Assert.AreEqual(settings, merged, "Check if extended: settings must be extended" );
            Assert.AreEqual(optionsCopy,options, "Check if not modified: options must not be modified" );

            // Test deep copying

            CQ.Extend(true, deep1, deep2);
            Assert.AreEqual(deepmerged.foo, deep1.foo,  "Check if foo: settings must be extended" );
            Assert.AreEqual(deep2copy.foo, deep2.foo, "Check if not deep2: options must not be modified" );
            
            // n/a
            //Assert.AreEqual(document, deep1.foo2, "Make sure that a deep clone was not attempted on the document");


            var nullUndef = CQ.Extend(null, options, CQ.ParseJSON("{ 'xnumber2': null }"));
            Assert.IsTrue( nullUndef.xnumber2 == null, "Check to make sure null values are copied");

            nullUndef = CQ.Extend(null, options, CQ.ParseJSON("{ 'xnumber0': null }"));
            Assert.IsTrue(nullUndef.xnumber0 == null, "Check to make sure null values are inserted");
            
            // Test nested arrays

            dynamic extendedArr = CQ.Extend(true, null, nestedarray);


            Assert.AreNotSame(extendedArr.arr,arr, "Deep extend of object must clone child array");
            Assert.AreNotSame(extendedArr.arr,arr, "Deep extend of object must clone child array");

            // #5991
            dynamic emptyArrObj = CQ.ParseJSON("{ arr: {} }");
            dynamic extendedArr2 = CQ.Extend(true, emptyArrObj, nestedarray);

            Assert.IsTrue(extendedArr2.arr is IEnumerable, "Cloned array heve to be an Array" );

            dynamic simpleArrObj = new ExpandoObject();
            simpleArrObj.arr = arr;
            emptyArrObj = CQ.ParseJSON("{ arr: {} }");
            dynamic result = CQ.Extend(true, simpleArrObj, emptyArrObj);
            Assert.IsTrue(!(result.arr is Array), "Cloned object heve to be an plain object" );


            dynamic empty = new JsObject();
            dynamic optionsWithLength = CQ.ParseJSON("{ 'foo': { 'length': -1 } }");
            CQ.Extend(true, empty, optionsWithLength);
            Assert.AreEqual( empty.foo, optionsWithLength.foo, "The length property must copy correctly" );


            //? not really relevant

            empty = new JsObject();
            dynamic optionsWithDate = new JsObject();
            optionsWithDate.foo = new JsObject();
            optionsWithDate.foo.date = new DateTime();
            CQ.Extend(true, empty, optionsWithDate);
            Assert.AreEqual(empty.foo, optionsWithDate.foo, "Dates copy correctly" );


            var customObject = DateTime.Now;
            dynamic optionsWithCustomObject = new { foo= new { date= customObject } };

            empty = CQ.Extend(true, null, optionsWithCustomObject);
            Assert.IsTrue( empty.foo !=null && empty.foo.date == customObject, "Custom objects copy correctly (no methods)" );

            //// Makes the class a little more realistic
            //myKlass.prototype = { someMethod: function(){} };
            //empty = {};
            //jQuery.extend(true, empty, optionsWithCustomObject);
            //ok( empty.foo && empty.foo.date === customObject, "Custom objects copy correctly" );

            dynamic ret = CQ.Extend(true, CQ.ParseJSON("{'foo': 4 }"), new { foo = 5 });
            Assert.IsTrue( ret.foo == 5, "Wrapped numbers copy correctly" );

            nullUndef = CQ.Extend(null, options, "{ 'xnumber2': null }");
            Assert.IsTrue( nullUndef.xnumber2 == null, "Check to make sure null values are copied");

            //    nullUndef = jQuery.extend({}, options, { xnumber2: undefined });
            //    ok( nullUndef.xnumber2 === options.xnumber2, "Check to make sure undefined values are not copied");

            nullUndef = CQ.Extend(null, options, "{ 'xnumber0': null }");
            Assert.IsTrue( nullUndef.xnumber0 == null, "Check to make sure null values are inserted");

            dynamic target = new ExpandoObject();
            dynamic recursive = new ExpandoObject();
            recursive.bar = 5;
            recursive.foo = target;

            CQ.Extend(true, target, recursive);
            dynamic compare = CQ.ParseJSON("{ 'bar':5 }");

            Assert.AreEqual(target, compare, "Check to make sure a recursive obj doesn't go never-ending loop by not copying it over");

            // These next checks don't really apply as they are specific to javascript nuances, but can't hurt

            ret = CQ.Extend(true, CQ.ParseJSON(" { 'foo': [] }"), CQ.ParseJSON("{ 'foo': [0] }")); // 1907
            // arrays are returned as List<object> when casting to expando object
            Assert.AreEqual(1,ret.foo.Count, "Check to make sure a value with coersion 'false' copies over when necessary to fix #1907" );

            ret = CQ.Extend(true, CQ.ParseJSON("{ 'foo': '1,2,3' }"), CQ.ParseJSON("{ 'foo': [1, 2, 3] }"));
            Assert.IsTrue( !(ret.foo is string), "Check to make sure values equal with coersion (but not actually equal) overwrite correctly" );



            dynamic obj = CQ.ParseJSON("{ 'foo':null }");
            CQ.Extend(true, obj, CQ.ParseJSON("{ 'foo':'notnull' }"));
            Assert.AreEqual(obj.foo, "notnull", "Make sure a null value can be overwritten" );

            //    function func() {}
            //    jQuery.extend(func, { key: "value" } );
            //    equals( func.key, "value", "Verify a function can be extended" );

            dynamic defaults = CQ.ParseJSON("{ 'xnumber1': 5, 'xnumber2': 7, 'xstring1': 'peter', 'xstring2': 'pan' }");
            dynamic defaultsCopy = CQ.ParseJSON(" { xnumber1: 5, xnumber2: 7, xstring1: 'peter', xstring2: 'pan' }");
            dynamic options1 = CQ.ParseJSON("{ xnumber2: 1, xstring2: 'x' }");
            dynamic options1Copy = CQ.ParseJSON(" { xnumber2: 1, xstring2: 'x' }");
            dynamic options2 = CQ.ParseJSON(" { xstring2: 'xx', xxx: 'newstringx'}");
            dynamic options2Copy = CQ.ParseJSON(" { xstring2: 'xx', xxx: 'newstringx'}");
            dynamic merged2 = CQ.ParseJSON(" { xnumber1: 5, xnumber2: 1, xstring1: 'peter', xstring2: 'xx', xxx: 'newstringx' }");


            ret = CQ.Extend(true, CQ.ParseJSON(" { foo:'bar' }"), CQ.ParseJSON("{ foo:null }"));
            Assert.IsTrue(ret.foo ==null, "Make sure a null value doesn't crash with deep extend, for #1908" );

            settings = CQ.Extend(null, defaults, options1, options2);
            Assert.AreEqual(merged2, settings, "Check if extended: settings must be extended");
            Assert.AreEqual(defaults, defaultsCopy, "Check if not modified: options1 must not be modified");
            Assert.AreEqual(options1, options1Copy, "Check if not modified: options1 must not be modified");
            Assert.AreEqual(options2, options2Copy, "Check if not modified: options2 must not be modified");
        }

        /*
         * The Each implementation only applies to the jQuery selections. 
         * No sense in duplication of regular c# language constructs. Don't test this use.
         */

        //test("jQuery.each(Object,Function)", function() {
        //    expect(14);
        //    jQuery.each( [0,1,2], function(i, n){
        //        equals( i, n, "Check array iteration" );
        //    });

        //    jQuery.each( [5,6,7], function(i, n){
        //        equals( i, n - 5, "Check array iteration" );
        //    });

        //    jQuery.each( { name: "name", lang: "lang" }, function(i, n){
        //        equals( i, n, "Check object iteration" );
        //    });

        //    var total = 0;
        //    jQuery.each([1,2,3], function(i,v){ total += v; });
        //    equals( total, 6, "Looping over an array" );
        //    total = 0;
        //    jQuery.each([1,2,3], function(i,v){ total += v; if ( i == 1 ) return false; });
        //    equals( total, 3, "Looping over an array, with break" );
        //    total = 0;
        //    jQuery.each({"a":1,"b":2,"c":3}, function(i,v){ total += v; });
        //    equals( total, 6, "Looping over an object" );
        //    total = 0;
        //    jQuery.each({"a":3,"b":3,"c":3}, function(i,v){ total += v; return false; });
        //    equals( total, 3, "Looping over an object, with break" );

        //    var f = function(){};
        //    f.foo = "bar";
        //    jQuery.each(f, function(i){
        //        f[i] = "baz";
        //    });
        //    equals( "baz", f.foo, "Loop over a function" );

        //    var stylesheet_count = 0;
        //    jQuery.each(document.styleSheets, function(i){
        //        stylesheet_count++;
        //    });
        //    equals(stylesheet_count, 2, "should not throw an error in IE while looping over document.styleSheets and return proper amount");

        //});
        
        /*
         * Not implemented - utility
         */

        //test("jQuery.makeArray", function(){
        //    expect(17);

        //    equals( jQuery.makeArray(jQuery("html>*"))[0].nodeName.toUpperCase(), "HEAD", "Pass makeArray a jQuery object" );

        //    equals( jQuery.makeArray(document.getElementsByName("PWD")).slice(0,1)[0].name, "PWD", "Pass makeArray a nodelist" );

        //    equals( (function(){ return jQuery.makeArray(arguments); })(1,2).join(""), "12", "Pass makeArray an arguments array" );

        //    equals( jQuery.makeArray([1,2,3]).join(""), "123", "Pass makeArray a real array" );

        //    equals( jQuery.makeArray().length, 0, "Pass nothing to makeArray and expect an empty array" );

        //    equals( jQuery.makeArray( 0 )[0], 0 , "Pass makeArray a number" );

        //    equals( jQuery.makeArray( "foo" )[0], "foo", "Pass makeArray a string" );

        //    equals( jQuery.makeArray( true )[0].constructor, Boolean, "Pass makeArray a boolean" );

        //    equals( jQuery.makeArray( document.createElement("div") )[0].nodeName.toUpperCase(), "DIV", "Pass makeArray a single node" );

        //    equals( jQuery.makeArray( {length:2, 0:"a", 1:"b"} ).join(""), "ab", "Pass makeArray an array like map (with length)" );

        //    ok( !!jQuery.makeArray( document.documentElement.childNodes ).slice(0,1)[0].nodeName, "Pass makeArray a childNodes array" );

        //    // function, is tricky as it has length
        //    equals( jQuery.makeArray( function(){ return 1;} )[0](), 1, "Pass makeArray a function" );

        //    //window, also has length
        //    equals( jQuery.makeArray(window)[0], window, "Pass makeArray the window" );

        //    equals( jQuery.makeArray(/a/)[0].constructor, RegExp, "Pass makeArray a regex" );

        //    ok( jQuery.makeArray(document.getElementById("form")).length >= 13, "Pass makeArray a form (treat as elements)" );

        //    // For #5610
        //    same( jQuery.makeArray({length: "0"}), [], "Make sure object is coerced properly.");
        //    same( jQuery.makeArray({length: "5"}), [], "Make sure object is coerced properly.");
        //});

        /*
         * See Objects.*
         */
 
        //test("jQuery.isEmptyObject", function(){
        //    expect(2);

        //    equals(true, jQuery.isEmptyObject({}), "isEmptyObject on empty object literal" );
        //    equals(false, jQuery.isEmptyObject({a:1}), "isEmptyObject on non-empty object literal" );

        //    // What about this ?
        //    // equals(true, jQuery.isEmptyObject(null), "isEmptyObject on null" );
        //});

        /*
         * Not implemented
         */

        //test("jQuery.proxy", function(){
        //    expect(7);

        //    var test = function(){ equals( this, thisObject, "Make sure that scope is set properly." ); };
        //    var thisObject = { foo: "bar", method: test };

        //    // Make sure normal works
        //    test.call( thisObject );

        //    // Basic scoping
        //    jQuery.proxy( test, thisObject )();

        //    // Another take on it
        //    jQuery.proxy( thisObject, "method" )();

        //    // Make sure it doesn't freak out
        //    equals( jQuery.proxy( null, thisObject ), undefined, "Make sure no function was returned." );

        //        // Partial application
        //        var test2 = function( a ){ equals( a, "pre-applied", "Ensure arguments can be pre-applied." ); };
        //        jQuery.proxy( test2, null, "pre-applied" )();

        //        // Partial application w/ normal arguments
        //        var test3 = function( a, b ){ equals( b, "normal", "Ensure arguments can be pre-applied and passed as usual." ); };
        //        jQuery.proxy( test3, null, "pre-applied" )( "normal" );

        //    // Test old syntax
        //    var test4 = { meth: function( a ){ equals( a, "boom", "Ensure old syntax works." ); } };
        //    jQuery.proxy( test4, "meth" )( "boom" );
        //});
        [Test,TestMethod]
        public void ParseJson()
        {
            Assert.AreEqual(CQ.ParseJSON(null), null, "Nothing in, null out.");
            Assert.AreEqual(CQ.ParseJSON(""), null, "Nothing in, null out.");

            Assert.AreEqual(CQ.ParseJSON("{}"), new ExpandoObject(), "Plain object parsing.");

            // everything else - just the JS Serializer

        }

        /*
         * We could do this.. probably useful
         */

        //test("jQuery.parseXML", 4, function(){
        //    var xml, tmp;
        //    try {
        //        xml = jQuery.parseXML( "<p>A <b>well-formed</b> xml string</p>" );
        //        tmp = xml.getElementsByTagName( "p" )[ 0 ];
        //        ok( !!tmp, "<p> present in document" );
        //        tmp = tmp.getElementsByTagName( "b" )[ 0 ];
        //        ok( !!tmp, "<b> present in document" );
        //        strictEqual( tmp.childNodes[ 0 ].nodeValue, "well-formed", "<b> text is as expected" );
        //    } catch (e) {
        //        strictEqual( e, undefined, "unexpected error" );
        //    }
        //    try {
        //        xml = jQuery.parseXML( "<p>Not a <<b>well-formed</b> xml string</p>" );
        //        ok( false, "invalid xml not detected" );
        //    } catch( e ) {
        //        strictEqual( e, "Invalid XML: <p>Not a <<b>well-formed</b> xml string</p>", "invalid xml detected" );
        //    }
        //});

        /*
         * This one really has no purpose in C#
         */

        //test("jQuery.sub() - Static Methods", function(){
        //    expect(18);
        //    var Subclass = jQuery.sub();
        //    Subclass.extend({
        //        topLevelMethod: function() {return this.debug;},
        //        debug: false,
        //        config: {
        //            locale: "en_US"
        //        },
        //        setup: function(config) {
        //            this.extend(true, this.config, config);
        //        }
        //    });
        //    Subclass.fn.extend({subClassMethod: function() { return this;}});

        //    //Test Simple Subclass
        //    ok(Subclass.topLevelMethod() === false, "Subclass.topLevelMethod thought debug was true");
        //    ok(Subclass.config.locale == "en_US", Subclass.config.locale + " is wrong!");
        //    same(Subclass.config.test, undefined, "Subclass.config.test is set incorrectly");
        //    equal(jQuery.ajax, Subclass.ajax, "The subclass failed to get all top level methods");

        //    //Create a SubSubclass
        //    var SubSubclass = Subclass.sub();

        //    //Make Sure the SubSubclass inherited properly
        //    ok(SubSubclass.topLevelMethod() === false, "SubSubclass.topLevelMethod thought debug was true");
        //    ok(SubSubclass.config.locale == "en_US", SubSubclass.config.locale + " is wrong!");
        //    same(SubSubclass.config.test, undefined, "SubSubclass.config.test is set incorrectly");
        //    equal(jQuery.ajax, SubSubclass.ajax, "The subsubclass failed to get all top level methods");

        //    //Modify The Subclass and test the Modifications
        //    SubSubclass.fn.extend({subSubClassMethod: function() { return this;}});
        //    SubSubclass.setup({locale: "es_MX", test: "worked"});
        //    SubSubclass.debug = true;
        //    SubSubclass.ajax = function() {return false;};
        //    ok(SubSubclass.topLevelMethod(), "SubSubclass.topLevelMethod thought debug was false");
        //    same(SubSubclass(document).subClassMethod, Subclass.fn.subClassMethod, "Methods Differ!");
        //    ok(SubSubclass.config.locale == "es_MX", SubSubclass.config.locale + " is wrong!");
        //    ok(SubSubclass.config.test == "worked", "SubSubclass.config.test is set incorrectly");
        //    notEqual(jQuery.ajax, SubSubclass.ajax, "The subsubclass failed to get all top level methods");

        //    //This shows that the modifications to the SubSubClass did not bubble back up to it's superclass
        //    ok(Subclass.topLevelMethod() === false, "Subclass.topLevelMethod thought debug was true");
        //    ok(Subclass.config.locale == "en_US", Subclass.config.locale + " is wrong!");
        //    same(Subclass.config.test, undefined, "Subclass.config.test is set incorrectly");
        //    same(Subclass(document).subSubClassMethod, undefined, "subSubClassMethod set incorrectly");
        //    equal(jQuery.ajax, Subclass.ajax, "The subclass failed to get all top level methods");
        //});

        //test("jQuery.sub() - .fn Methods", function(){
        //    expect(378);

        //    var Subclass = jQuery.sub(),
        //            SubclassSubclass = Subclass.sub(),
        //            jQueryDocument = jQuery(document),
        //            selectors, contexts, methods, method, arg, description;

        //    jQueryDocument.toString = function(){ return "jQueryDocument"; };

        //    Subclass.fn.subclassMethod = function(){};
        //    SubclassSubclass.fn.subclassSubclassMethod = function(){};

        //    selectors = [
        //        "body",
        //        "html, body",
        //        "<div></div>"
        //    ];

        //    methods = [ // all methods that return a new jQuery instance
        //        ["eq", 1],
        //        ["add", document],
        //        ["end"],
        //        ["has"],
        //        ["closest", "div"],
        //        ["filter", document],
        //        ["find", "div"]
        //    ];

        //    contexts = [undefined, document, jQueryDocument];

        //    jQuery.each(selectors, function(i, selector){

        //        jQuery.each(methods, function(){
        //            method = this[0];
        //            arg = this[1];

        //            jQuery.each(contexts, function(i, context){

        //                description = "(\""+selector+"\", "+context+")."+method+"("+(arg||"")+")";

        //                same(
        //                    jQuery(selector, context)[method](arg).subclassMethod, undefined,
        //                    "jQuery"+description+" doesn't have Subclass methods"
        //                );
        //                same(
        //                    jQuery(selector, context)[method](arg).subclassSubclassMethod, undefined,
        //                    "jQuery"+description+" doesn't have SubclassSubclass methods"
        //                );
        //                same(
        //                    Subclass(selector, context)[method](arg).subclassMethod, Subclass.fn.subclassMethod,
        //                    "Subclass"+description+" has Subclass methods"
        //                );
        //                same(
        //                    Subclass(selector, context)[method](arg).subclassSubclassMethod, undefined,
        //                    "Subclass"+description+" doesn't have SubclassSubclass methods"
        //                );
        //                same(
        //                    SubclassSubclass(selector, context)[method](arg).subclassMethod, Subclass.fn.subclassMethod,
        //                    "SubclassSubclass"+description+" has Subclass methods"
        //                );
        //                same(
        //                    SubclassSubclass(selector, context)[method](arg).subclassSubclassMethod, SubclassSubclass.fn.subclassSubclassMethod,
        //                    "SubclassSubclass"+description+" has SubclassSubclass methods"
        //                );

        //            });
        //        });
        //    });

        //});

        /*
         * Not hard, not important
         */

        //test("jQuery.camelCase()", function() {

        //    var tests = {
        //        "foo-bar": "fooBar",
        //        "foo-bar-baz": "fooBarBaz"
        //    };

        //    expect(2);

        //    jQuery.each( tests, function( key, val ) {
        //        equal( jQuery.camelCase( key ), val, "Converts: " + key + " => " + val );
        //    });
        //});
    }
}
