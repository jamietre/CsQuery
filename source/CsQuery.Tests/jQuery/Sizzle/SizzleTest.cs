using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery;

namespace CsQuery.Tests.jQuery.Sizzle
{
    /// <summary>
    /// Tests from sizzle.js test suite as of June 13, 2012
    /// https://github.com/jquery/sizzle/tree/master/test
    /// </summary>
    public abstract class SizzleTest: CsQueryTest
    {
        // These functionas may be found in the CsQueryTest class

        //q(...);
        //    Returns an array of elements with the given IDs
        //    @example q("main", "foo", "bar") => [<div id="main">, <span id="foo">, <input id="bar">]

        //t( testName, selector, [ "array", "of", "ids" ] );
        //    Asserts that a select matches the given IDs
        //    @example t("Check for something", "//[a]", ["foo", "baar"]);

        //url( "some/url.php" );
        //    Add random number to url to stop caching
        //    @example url("data/test.html") => "data/test.html?10538358428943"
        //    @example url("data/test.php?foo=bar") => "data/test.php?foo=bar&10538358345554"

        // Sizzle maps to Dom, the default DOM 

        public override void  FixtureSetUp()
        {
 	         base.FixtureSetUp();
             Dom = TestDom("sizzle");
        }

        protected CQ Sizzle
        {
            get
            {
                return Dom;
            }
        }

        protected bool match(IDomObject el, string selector)
        {
            return Dom[selector].Contains(el);
        }
    }
}
