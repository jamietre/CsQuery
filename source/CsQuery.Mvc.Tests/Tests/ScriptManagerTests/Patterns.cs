using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Hosting;
using System.Web.Optimization;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CsQuery.Mvc.Tests.Controllers;
using CsQuery.Mvc;
using CsQuery.Mvc.ClientScript;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.Mvc.Tests
{
    [TestClass]
    public class PatternsObject
    {


        private bool Matches(Regex regex, string text,string value, string groupName="dep")
        {
            var match = regex.Match(text);
            return match.Success &&
                value == match.Groups[groupName].Value;

        }

        [TestMethod]
        public void Dependency()
        {
            var pat = Patterns.Dependency;


            Assert.IsTrue(Matches(pat,"using test","test"));           
            Assert.IsTrue(Matches(pat,"    using test","test"));
            Assert.IsTrue(Matches(pat,"\tusing test","test"));
            Assert.IsTrue(Matches(pat,"\t using test","test"));
            Assert.IsTrue(Matches(pat,"using test;","test"));
            Assert.IsTrue(Matches(pat,"using test.test2","test.test2"));
            Assert.IsTrue(Matches(pat, "using test.test2 ;", "test.test2"));
            Assert.IsFalse(pat.IsMatch("xusing test;"));

            // a valid match - but should fail when looking up. We don't want to just not match things that start with "using"
            // but are invalid.
            Assert.IsTrue(Matches(pat,"using test something", "test"));


            Assert.IsTrue(Matches(pat, "using test/test2", "test/test2"));
            Assert.IsTrue(Matches(pat, "using test/test2 ;", "test/test2"));

            Assert.IsTrue(Matches(pat, "using test%test2 ;", "test%test2"));


            
        }
        [TestMethod]
        public void FullLineComment()
        {
            var pat = Patterns.FullLineComment;

            Assert.IsTrue(pat.IsMatch("// a comment"));
            Assert.IsTrue(pat.IsMatch("   // a comment"));
            Assert.IsFalse(pat.IsMatch("xxx   // a comment"));

        }

        [TestMethod]
        public void RegexOneLineComment()
        {
            var pat = Patterns.OneLineComment;

            Assert.IsTrue(pat.IsMatch("/* a comment */"));
            Assert.IsFalse(pat.IsMatch("/* a start comment"));
            Assert.IsFalse(pat.IsMatch("an end comment */"));

        }


        [TestMethod]
        public void RegexStartComment()
        {
            var pat = Patterns.StartComment;

            Assert.IsTrue(pat.IsMatch("/* a comment */"));
            Assert.IsTrue(Matches(pat, "/* a comment */", "a comment ","comment"));

            Assert.IsTrue(pat.IsMatch("/* a start comment"));
            Assert.IsFalse(pat.IsMatch("an end comment */"));

        }
    }
}
