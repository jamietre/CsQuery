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
    public class Sizzle_Element : SizzleTest
    {


        [Test, TestMethod]
        public void Element()
        {

            var all = Dom["*"];
            Assert.IsTrue(all.Length >= 30, "Select all");

            var good = true;
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i].NodeType == (NodeType)8)
                {
                    good = false;
                }
            }
            Assert.IsTrue(good, "Select all elements, no comment nodes");


            t("Element Selector", "#qunit-fixture p", Arrays.Create<string>("firstp", "ap", "sndp", "en", "sap", "first"));
            t("Element Selector", "body", Arrays.Create<string>("body"));
            t("Element Selector", "html", Arrays.Create<string>("html"));
            t("Parent Element", "div p", Arrays.Create<string>("firstp", "ap", "sndp", "en", "sap", "first"));

            Assert.AreEqual(Dom.Select("param", document.GetElementById("object1")).Length, 2, "Object/param as context");

            CollectionAssert.AreEqual(Dom["select", document.GetElementById("form")], q("select1", "select2", "select3", "select4", "select5"), "Finding selects with a context.");

            // Check for unique-ness and sort order
            CollectionAssert.AreEqual(Dom["p, div p"], Dom["p"], "Check for duplicates: p, div p");

            t("Checking sort order", "h2, h1", Arrays.Create<string>("qunit-header", "qunit-banner", "qunit-userAgent"));

            t("Checking sort order", "h2:first, h1:first", Arrays.Create<string>("qunit-header", "qunit-banner"));
            t("Checking sort order", "#qunit-fixture p, #qunit-fixture p a", Arrays.Create<string>("firstp", "simon1", "ap", "google", "groups", "anchor1", "mark", "sndp", "en", "yahoo", "sap", "anchor2", "simon", "first"));

            // Test Conflict ID
            var lengthtest = document.GetElementById("lengthtest");
            CollectionAssert.AreEqual(Dom["#idTest", lengthtest], q("idTest"), "Finding element with id of ID.");
            CollectionAssert.AreEqual(Dom["[name='id']", lengthtest], q("idTest"), "Finding element with id of ID.");
            CollectionAssert.AreEqual(Dom["input[id='idTest']", lengthtest], q("idTest"), "Finding elements with id of ID.");
        }
    

    }

}