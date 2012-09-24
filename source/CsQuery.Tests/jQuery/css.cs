using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;
using CsQuery.StringScanner;

namespace CsQuery.Tests.jQuery
{
    [TestClass, TestFixture, Category("Attributes")]
    public class Css : CsQueryTest
    {
        [SetUp]
        public override void FixtureSetUp()
        {
            base.FixtureSetUp();
            ResetQunit();
        }


        [Test,TestMethod]
        public void CssStringHash() 
        {
            Assert.AreEqual( jQuery("#qunit-fixture").Css("display"), "block", "Check for css property \"display\"");

            Assert.IsTrue( jQuery("#nothiddendiv").Is(":visible"), "Modifying CSS display: Assert element is visible");
            jQuery("#nothiddendiv").CssSet("{display: 'none'}");
            Assert.IsTrue( !jQuery("#nothiddendiv").Is(":visible"), "Modified CSS display: Assert element is hidden");
            jQuery("#nothiddendiv").CssSet("{display: 'block'}");
            Assert.IsTrue( jQuery("#nothiddendiv").Is(":visible"), "Modified CSS display: Assert element is visible");
            //Assert.IsTrue( jQuery(window).Is(":visible"), "Calling is(':visible') on window does not throw an error in IE.");
            //Assert.IsTrue( jQuery(document).Is(":visible"), "Calling is(':visible') on document does not throw an error in IE.");

            var div = jQuery( "<div>" );

            // These should be "auto" (or some better value)
            // temporarily provide "0px" for backwards compat
            //Assert.AreEqual( div.Css("width"), "0px", "Width on disconnected node." );
            //Assert.AreEqual( div.Css("height"), "0px", "Height on disconnected node." );

            div.CssSet("{ width: 4, height: 4 }");

            Assert.AreEqual( div.Css("width"), "4px", "Width on disconnected node." );
            Assert.AreEqual( div.Css("height"), "4px", "Height on disconnected node." );

            var div2 = jQuery( "<div style='display:none;'><input type='text' style='height:20px;'/><textarea style='height:20px;'></textarea><div style='height:20px;'></div></div>")
                .AppendTo("body");

            Assert.AreEqual( div2.Find("input").Css("height"), "20px", "Height on hidden input." );
            Assert.AreEqual( div2.Find("textarea").Css("height"), "20px", "Height on hidden textarea." );
            Assert.AreEqual( div2.Find("div").Css("height"), "20px", "Height on hidden textarea." );

            div2.Remove();

            // handle negative numbers by ignoring #1599, #4216
            jQuery("#nothiddendiv").CssSet( "{width: 1, height: 1}" );

            var width = parseFloat(jQuery("#nothiddendiv").Css("width"));
            var height = parseFloat(jQuery("#nothiddendiv").Css("height"));
            
            //TODO - add detailed CSS validations. For now this would create the start of probably many
            //special cases, not going down that road.

            //jQuery("#nothiddendiv").CssSet("{ width: -1, height: -1 }");
            
            
            //Assert.AreEqual( parseFloat(jQuery("#nothiddendiv").Css("width")), width, "Test negative width ignored");
            //Assert.AreEqual( parseFloat(jQuery("#nothiddendiv").Css("height")), height, "Test negative height ignored");

            Assert.AreEqual( jQuery("<div style='display: none;'>").Css("display"), "none", "Styles on disconnected nodes");

            jQuery("#floatTest").CssSet("{'float': 'right'}");
            Assert.AreEqual(jQuery("#floatTest").Css("float"), "right",
                "Modified CSS float using \"float\": Assert float is right");

            jQuery("#floatTest").CssSet("{'font-size': '30px'}");
            Assert.AreEqual(jQuery("#floatTest").Css("font-size"), "30px", "Modified CSS font-size: Assert font-size is 30px");

            CQ.Each("0,0.25,0.5,0.75,1".Split(','), (string n) =>
            {

                jQuery("#foo").CssSet("{ opacity: " + n + "}");

                Assert.AreEqual(jQuery("#foo").Css("opacity"), n, "Assert opacity is " + parseFloat(n) + " as a String");
                jQuery("#foo").CssSet("{opacity: " + n + "}");
                Assert.AreEqual(jQuery("#foo").Css("opacity"), n, "Assert opacity is " + parseFloat(n) + " as a Number");
            });
            jQuery("#foo").CssSet("{opacity: ''}");
            Assert.AreEqual(jQuery("#foo").Css("opacity"), "1", "Assert opacity is 1 when set to an empty String");

            //Assert.AreEqual( jQuery("#empty").Css("opacity"), "0", "Assert opacity is accessible via filter property set in stylesheet in IE" );
            jQuery("#empty").Css("{ opacity: '1'}");
            Assert.AreEqual(jQuery("#empty").Css("opacity"), "1", "Assert opacity is taken from style attribute when set vs stylesheet in IE with filters");
            
            //jQuery.support.opacity ?
            //    Assert.IsTrue(true, "Requires the same number of tests"):
            //    Assert.IsTrue( ~jQuery("#empty")[0].currentStyle.filter.indexOf("gradient"), "Assert setting opacity doesn't overwrite other filters of the stylesheet in IE" );

            div = jQuery("#nothiddendiv");
            var child = jQuery("#nothiddendivchild");

            // STOPPED HERE:  Things get wierd with CSS because obviously we can only set values and not expect browser interpretation upon reading.

            /// Assert.AreEqual( parseInt(div.Css("fontSize")), 16, "Verify fontSize px set." );
            
            //// We always return strings.
            //Assert.AreEqual(parseInt(div.Css("font-size")), "16px", "Verify fontSize px set.");
            ////Assert.AreEqual( parseInt(child.Css("fontSize")), 16, "Verify fontSize px set." );
            //Assert.AreEqual(parseInt(child.Css("font-size")), 16, "Verify fontSize px set.");

            //child.Css("height", "100%");
            //Assert.AreEqual(child[0].Style.Height, "100%", "Make sure the height is being set correctly.");

            //child.Attr("class", "em");
            //Assert.AreEqual(parseInt(child.Css("font-size")), 32, "Verify fontSize em set.");

            //// Have to verify this as the result depends upon the browser's CSS
            //// support for font-size percentages
            //child.Attr("class", "prct");
            //var prctval = parseInt(child.Css("font-size"));
            //int? checkval = 0;
            //if (prctval == 16 || prctval == 24)
            //{
            //    checkval = prctval;
            //}

            //Assert.AreEqual(prctval, checkval, "Verify fontSize % set.");

            //Assert.AreEqual(child.Css("width").GetType(), typeof(string), "Make sure that a string width is returned from css('width').");

            //var old = child[0].Style.Height;

            //// Test NaN
            //child.Css("height", parseFloat("zoo"));
            //Assert.AreEqual(child[0].Style.Height, old, "Make sure height isn't changed on NaN.");

            //// Test null
            //child.Css("height", null);
            //Assert.AreEqual(child[0].Style.Height, old, "Make sure height isn't changed on null.");

            //old = child[0].Style["font-size"];

            //// Test NaN
            //child.Css("font-size", parseFloat("zoo"));
            //Assert.AreEqual(child[0].Style["font-size"], old, "Make sure font-size isn't changed on NaN.");

            //// Test null
            //child.Css("font-size", null);
            //Assert.AreEqual(child[0].Style["font-size"], old, "Make sure font-size isn't changed on null.");

        }
        double? parseFloat(string num)
        {

            double output = 0;
            var scanner = Scanner.Create(num);
            if (scanner.TryGetNumber(out output))
            {
                return output;
            }
            else
            {
                return null;
            }

        }
        int? parseInt(string num)
        {
            int output = 0;
            var scanner = Scanner.Create(num);
            if (scanner.TryGetNumber(out output))
            {
                return output;
            }
            else
            {
                return null;
            }

        }
    }


        

//test("css() explicit and relative values", function() {
//    expect(29);
//    var $elem = jQuery("#nothiddendiv");

//    $elem.Css({ width: 1, height: 1, paddingLeft: "1px", opacity: 1 });
//    Assert.AreEqual( $elem.width(), 1, "Initial css set or width/height works (hash)" );
//    Assert.AreEqual( $elem.Css("paddingLeft"), "1px", "Initial css set of paddingLeft works (hash)" );
//    Assert.AreEqual( $elem.Css("opacity"), "1", "Initial css set of opacity works (hash)" );

//    $elem.Css({ width: "+=9" });
//    Assert.AreEqual( $elem.width(), 10, "'+=9' on width (hash)" );

//    $elem.Css({ width: "-=9" });
//    Assert.AreEqual( $elem.width(), 1, "'-=9' on width (hash)" );

//    $elem.Css({ width: "+=9px" });
//    Assert.AreEqual( $elem.width(), 10, "'+=9px' on width (hash)" );

//    $elem.Css({ width: "-=9px" });
//    Assert.AreEqual( $elem.width(), 1, "'-=9px' on width (hash)" );

//    $elem.Css( "width", "+=9" );
//    Assert.AreEqual( $elem.width(), 10, "'+=9' on width (params)" );

//    $elem.Css( "width", "-=9" ) ;
//    Assert.AreEqual( $elem.width(), 1, "'-=9' on width (params)" );

//    $elem.Css( "width", "+=9px" );
//    Assert.AreEqual( $elem.width(), 10, "'+=9px' on width (params)" );

//    $elem.Css( "width", "-=9px" );
//    Assert.AreEqual( $elem.width(), 1, "'-=9px' on width (params)" );

//    $elem.Css( "width", "-=-9px" );
//    Assert.AreEqual( $elem.width(), 10, "'-=-9px' on width (params)" );

//    $elem.Css( "width", "+=-9px" );
//    Assert.AreEqual( $elem.width(), 1, "'+=-9px' on width (params)" );

//    $elem.Css({ paddingLeft: "+=4" });
//    Assert.AreEqual( $elem.Css("paddingLeft"), "5px", "'+=4' on paddingLeft (hash)" );

//    $elem.Css({ paddingLeft: "-=4" });
//    Assert.AreEqual( $elem.Css("paddingLeft"), "1px", "'-=4' on paddingLeft (hash)" );

//    $elem.Css({ paddingLeft: "+=4px" });
//    Assert.AreEqual( $elem.Css("paddingLeft"), "5px", "'+=4px' on paddingLeft (hash)" );

//    $elem.Css({ paddingLeft: "-=4px" });
//    Assert.AreEqual( $elem.Css("paddingLeft"), "1px", "'-=4px' on paddingLeft (hash)" );

//    $elem.Css({ "padding-left": "+=4" });
//    Assert.AreEqual( $elem.Css("paddingLeft"), "5px", "'+=4' on padding-left (hash)" );

//    $elem.Css({ "padding-left": "-=4" });
//    Assert.AreEqual( $elem.Css("paddingLeft"), "1px", "'-=4' on padding-left (hash)" );

//    $elem.Css({ "padding-left": "+=4px" });
//    Assert.AreEqual( $elem.Css("paddingLeft"), "5px", "'+=4px' on padding-left (hash)" );

//    $elem.Css({ "padding-left": "-=4px" });
//    Assert.AreEqual( $elem.Css("paddingLeft"), "1px", "'-=4px' on padding-left (hash)" );

//    $elem.Css( "paddingLeft", "+=4" );
//    Assert.AreEqual( $elem.Css("paddingLeft"), "5px", "'+=4' on paddingLeft (params)" );

//    $elem.Css( "paddingLeft", "-=4" );
//    Assert.AreEqual( $elem.Css("paddingLeft"), "1px", "'-=4' on paddingLeft (params)" );

//    $elem.Css( "padding-left", "+=4px" );
//    Assert.AreEqual( $elem.Css("paddingLeft"), "5px", "'+=4px' on padding-left (params)" );

//    $elem.Css( "padding-left", "-=4px" );
//    Assert.AreEqual( $elem.Css("paddingLeft"), "1px", "'-=4px' on padding-left (params)" );

//    $elem.Css({ opacity: "-=0.5" });
//    Assert.AreEqual( $elem.Css("opacity"), "0.5", "'-=0.5' on opacity (hash)" );

//    $elem.Css({ opacity: "+=0.5" });
//    Assert.AreEqual( $elem.Css("opacity"), "1", "'+=0.5' on opacity (hash)" );

//    $elem.Css( "opacity", "-=0.5" );
//    Assert.AreEqual( $elem.Css("opacity"), "0.5", "'-=0.5' on opacity (params)" );

//    $elem.Css( "opacity", "+=0.5" );
//    Assert.AreEqual( $elem.Css("opacity"), "1", "'+=0.5' on opacity (params)" );
//});

//test("css(String, Object)", function() {
//    expect(22);

//    Assert.IsTrue( jQuery("#nothiddendiv").Is(":visible"), "Modifying CSS display: Assert element is visible");
//    jQuery("#nothiddendiv").Css("display", "none");
//    Assert.IsTrue( !jQuery("#nothiddendiv").Is(":visible"), "Modified CSS display: Assert element is hidden");
//    jQuery("#nothiddendiv").Css("display", "block");
//    Assert.IsTrue( jQuery("#nothiddendiv").Is(":visible"), "Modified CSS display: Assert element is visible");

//    jQuery("#nothiddendiv").Css("top", "-1em");
//    Assert.IsTrue( jQuery("#nothiddendiv").Css("top"), -16, "Check negative number in EMs." );

//    jQuery("#floatTest").Css("float", "left");
//    Assert.AreEqual( jQuery("#floatTest").Css("float"), "left", "Modified CSS float using \"float\": Assert float is left");
//    jQuery("#floatTest").Css("font-size", "20px");
//    Assert.AreEqual( jQuery("#floatTest").Css("font-size"), "20px", "Modified CSS font-size: Assert font-size is 20px");

//    jQuery.each("0,0.25,0.5,0.75,1".split(","), function(i, n) {
//        jQuery("#foo").Css("opacity", n);
//        Assert.AreEqual( jQuery("#foo").Css("opacity"), parseFloat(n), "Assert opacity is " + parseFloat(n) + " as a String" );
//        jQuery("#foo").Css("opacity", parseFloat(n));
//        Assert.AreEqual( jQuery("#foo").Css("opacity"), parseFloat(n), "Assert opacity is " + parseFloat(n) + " as a Number" );
//    });
//    jQuery("#foo").Css("opacity", "");
//    Assert.AreEqual( jQuery("#foo").Css("opacity"), "1", "Assert opacity is 1 when set to an empty String" );

//    // using contents will get comments regular, text, and comment nodes
//    var j = jQuery("#nonnodes").contents();
//    j.Css("overflow", "visible");
//    Assert.AreEqual( j.Css("overflow"), "visible", "Check node,textnode,comment css works" );
//    // opera sometimes doesn't update 'display' correctly, see #2037
//    jQuery("#t2037")[0].innerHTML = jQuery("#t2037")[0].innerHTML;
//    Assert.AreEqual( jQuery("#t2037 .hidden").Css("display"), "none", "Make sure browser thinks it is hidden" );

//    var div = jQuery("#nothiddendiv"),
//        display = div.Css("display"),
//        ret = div.Css("display", undefined);

//    Assert.AreEqual( ret, div, "Make sure setting undefined returns the original set." );
//    Assert.AreEqual( div.Css("display"), display, "Make sure that the display wasn't changed." );

//    // Test for Bug #5509
//    var success = true;
//    try {
//        jQuery("#foo").Css("backgroundColor", "rgba(0, 0, 0, 0.1)");
//    }
//    catch (e) {
//        success = false;
//    }
//    Assert.IsTrue( success, "Setting RGBA values does not throw Error" );
//});

//if ( !jQuery.support.opacity ) {
//    test("css(String, Object) for MSIE", function() {
//        // for #1438, IE throws JS error when filter exists but doesn't have opacity in it
//        jQuery("#foo").Css("filter", "progid:DXImageTransform.Microsoft.Chroma(color='red');");
//        Assert.AreEqual( jQuery("#foo").Css("opacity"), "1", "Assert opacity is 1 when a different filter is set in IE, #1438" );

//        var filterVal = "progid:DXImageTransform.Microsoft.Alpha(opacity=30) progid:DXImageTransform.Microsoft.Blur(pixelradius=5)";
//        var filterVal2 = "progid:DXImageTransform.Microsoft.alpha(opacity=100) progid:DXImageTransform.Microsoft.Blur(pixelradius=5)";
//        var filterVal3 = "progid:DXImageTransform.Microsoft.Blur(pixelradius=5)";
//        jQuery("#foo").Css("filter", filterVal);
//        Assert.AreEqual( jQuery("#foo").Css("filter"), filterVal, "css('filter', val) works" );
//        jQuery("#foo").Css("opacity", 1);
//        Assert.AreEqual( jQuery("#foo").Css("filter"), filterVal2, "Setting opacity in IE doesn't duplicate opacity filter" );
//        Assert.AreEqual( jQuery("#foo").Css("opacity"), 1, "Setting opacity in IE with other filters works" );
//        jQuery("#foo").Css("filter", filterVal3).Css("opacity", 1);
//        Assert.IsTrue( jQuery("#foo").Css("filter").indexOf(filterVal3) !== -1, "Setting opacity in IE doesn't clobber other filters" );
//    });

//    test( "Setting opacity to 1 properly removes filter: style (#6652)", function() {
//        var rfilter = /filter:[^;]*/i,
//            test = jQuery( "#t6652" ).Css( "opacity", 1 ),
//            test2 = test.find( "div" ).Css( "opacity", 1 );

//        function hasFilter( elem ) {
//            var match = rfilter.exec( elem[0].style.CssText );
//            if ( match ) {
//                return true;
//            }
//            return false;
//        }
//        expect( 2 );
//        Assert.IsTrue( !hasFilter( test ), "Removed filter attribute on element without filter in stylesheet" );
//        Assert.IsTrue( hasFilter( test2 ), "Filter attribute remains on element that had filter in stylesheet" );
//    });
//}

//test("css(String, Function)", function() {
//    expect(3);

//    var sizes = ["10px", "20px", "30px"];

//    jQuery("<div id='cssFunctionTest'><div class='cssFunction'></div>" +
//                 "<div class='cssFunction'></div>" +
//                 "<div class='cssFunction'></div></div>")
//        .appendTo("body");

//    var index = 0;

//    jQuery("#cssFunctionTest div").Css("font-size", function() {
//        var size = sizes[index];
//        index++;
//        return size;
//    });

//    index = 0;

//    jQuery("#cssFunctionTest div").each(function() {
//        var computedSize = jQuery(this).Css("font-size")
//        var expectedSize = sizes[index]
//        Assert.AreEqual( computedSize, expectedSize, "Div #" + index + " should be " + expectedSize );
//        index++;
//    });

//    jQuery("#cssFunctionTest").remove();
//});

//test("css(String, Function) with incoming value", function() {
//    expect(3);

//    var sizes = ["10px", "20px", "30px"];

//    jQuery("<div id='cssFunctionTest'><div class='cssFunction'></div>" +
//                 "<div class='cssFunction'></div>" +
//                 "<div class='cssFunction'></div></div>")
//        .appendTo("body");

//    var index = 0;

//    jQuery("#cssFunctionTest div").Css("font-size", function() {
//        var size = sizes[index];
//        index++;
//        return size;
//    });

//    index = 0;

//    jQuery("#cssFunctionTest div").Css("font-size", function(i, computedSize) {
//        var expectedSize = sizes[index]
//        Assert.AreEqual( computedSize, expectedSize, "Div #" + index + " should be " + expectedSize );
//        index++;
//        return computedSize;
//    });

//    jQuery("#cssFunctionTest").remove();
//});

//test("css(Object) where values are Functions", function() {
//    expect(3);

//    var sizes = ["10px", "20px", "30px"];

//    jQuery("<div id='cssFunctionTest'><div class='cssFunction'></div>" +
//                 "<div class='cssFunction'></div>" +
//                 "<div class='cssFunction'></div></div>")
//        .appendTo("body");

//    var index = 0;

//    jQuery("#cssFunctionTest div").Css({fontSize: function() {
//        var size = sizes[index];
//        index++;
//        return size;
//    }});

//    index = 0;

//    jQuery("#cssFunctionTest div").each(function() {
//        var computedSize = jQuery(this).Css("font-size")
//        var expectedSize = sizes[index]
//        Assert.AreEqual( computedSize, expectedSize, "Div #" + index + " should be " + expectedSize );
//        index++;
//    });

//    jQuery("#cssFunctionTest").remove();
//});

//test("css(Object) where values are Functions with incoming values", function() {
//    expect(3);

//    var sizes = ["10px", "20px", "30px"];

//    jQuery("<div id='cssFunctionTest'><div class='cssFunction'></div>" +
//                 "<div class='cssFunction'></div>" +
//                 "<div class='cssFunction'></div></div>")
//        .appendTo("body");

//    var index = 0;

//    jQuery("#cssFunctionTest div").Css({fontSize: function() {
//        var size = sizes[index];
//        index++;
//        return size;
//    }});

//    index = 0;

//    jQuery("#cssFunctionTest div").Css({"font-size": function(i, computedSize) {
//        var expectedSize = sizes[index]
//        Assert.AreEqual( computedSize, expectedSize, "Div #" + index + " should be " + expectedSize );
//        index++;
//        return computedSize;
//    }});

//    jQuery("#cssFunctionTest").remove();
//});

//test("jQuery.Css(elem, 'height') doesn't clear radio buttons (bug #1095)", function () {
//    expect(4);

//    var $checkedtest = jQuery("#checkedtest");
//    // IE6 was clearing "checked" in jQuery.Css(elem, "height");
//    jQuery.Css($checkedtest[0], "height");
//    Assert.IsTrue( !! jQuery(":radio:first", $checkedtest).attr("checked"), "Check first radio still checked." );
//    Assert.IsTrue( ! jQuery(":radio:last", $checkedtest).attr("checked"), "Check last radio still NOT checked." );
//    Assert.IsTrue( !! jQuery(":checkbox:first", $checkedtest).attr("checked"), "Check first checkbox still checked." );
//    Assert.IsTrue( ! jQuery(":checkbox:last", $checkedtest).attr("checked"), "Check last checkbox still NOT checked." );
//});

//test(":visible selector works properly on table elements (bug #4512)", function () {
//    expect(1);

//    jQuery("#table").html("<tr><td style='display:none'>cell</td><td>cell</td></tr>");
//    Assert.AreEqual(jQuery("#table td:visible").length, 1, "hidden cell is not perceived as visible");
//});

//test(":visible selector works properly on children with a hidden parent (bug #4512)", function () {
//    expect(1);
//    jQuery("#table").Css("display", "none").html("<tr><td>cell</td><td>cell</td></tr>");
//    Assert.AreEqual(jQuery("#table td:visible").length, 0, "hidden cell children not perceived as visible");
//});

//test("internal ref to elem.runtimeStyle (bug #7608)", function () {
//    expect(1);
//    var result = true;

//    try {
//        jQuery("#foo").Css( { width: "0%" } ).Css("width");
//    } catch (e) {
//        result = false;
//    }

//    Assert.IsTrue( result, "elem.runtimeStyle does not throw exception" );
//});

//test("marginRight computed style (bug #3333)", function() {
//    expect(1);

//    var $div = jQuery("#foo");
//    $div.Css({
//        width: "1px",
//        marginRight: 0
//    });

//    Assert.AreEqual($div.Css("marginRight"), "0px", "marginRight correctly calculated with a width and display block");
//});

//test("jQuery.CssProps behavior, (bug #8402)", function() {
//    var div = jQuery( "<div>" ).appendTo(document.body).Css({
//        position: "absolute",
//        top: 0,
//        left: 10
//    });
//    jQuery.CssProps.top = "left";
//    Assert.AreEqual( div.Css("top"), "10px", "the fixed property is used when accessing the computed style");
//    div.Css("top", "100px");
//    Assert.AreEqual( div[0].style.left, "100px", "the fixed property is used when setting the style");
//    // cleanup jQuery.CssProps
//    jQuery.CssProps.top = undefined;
//});

//test("widows & orphans #8936", function () {

//    var $p = jQuery("<p>").appendTo("#qunit-fixture");

//    if ( "widows" in $p[0].style ) {
//        expect(4);
//        $p.Css({
//            widows: 0,
//            orphans: 0
//        });

//        Assert.AreEqual( $p.Css("widows") || jQuery.style( $p[0], "widows" ), 0, "widows correctly start with value 0");
//        Assert.AreEqual( $p.Css("orphans") || jQuery.style( $p[0], "orphans" ), 0, "orphans correctly start with value 0");

//        $p.Css({
//            widows: 3,
//            orphans: 3
//        });

//        Assert.AreEqual( $p.Css("widows") || jQuery.style( $p[0], "widows" ), 3, "widows correctly set to 3");
//        Assert.AreEqual( $p.Css("orphans") || jQuery.style( $p[0], "orphans" ), 3, "orphans correctly set to 3");
//    } else {

//        expect(1);
//        Assert.IsTrue( true, "jQuery does not attempt to test for style props that definitely don't exist in older versions of IE");
//    }


//    $p.remove();
//});

//test("can't get css for disconnected in IE<9, see #10254 and #8388", function() {
//    expect( 2 );
//    var span = jQuery( "<span/>" ).Css( "background-image", "url(http://static.jquery.com/files/rocker/images/logo_jquery_215x53.gif)" );
//    notAssert.AreEqual( span.Css( "background-image" ), null, "can't get background-image in IE<9, see #10254" );

//    var div = jQuery( "<div/>" ).Css( "top", 10 );
//    Assert.AreEqual( div.Css( "top" ), "10px", "can't get top in IE<9, see #8388" );
//});

//test("Do not append px to 'fill-opacity' #9548", 1, function() {

//    var $div = jQuery("<div>").appendTo("#qunit-fixture");

//    $div.Css("fill-opacity", 0).animate({ "fill-opacity": 1.0 }, 0, function () {
//        Assert.AreEqual( jQuery(this).Css("fill-opacity"), 1, "Do not append px to 'fill-opacity'");
//    });

//});


}
