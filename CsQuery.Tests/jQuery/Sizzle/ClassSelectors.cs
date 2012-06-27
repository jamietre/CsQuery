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

    }
}