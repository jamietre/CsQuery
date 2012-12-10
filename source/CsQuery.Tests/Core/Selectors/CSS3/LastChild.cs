using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using CollectionAssert = NUnit.Framework.CollectionAssert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;

namespace CsQuery.Tests.Core.Selectors
{
    public partial class Css3: PseudoSelector
    {
        [Test, TestMethod]
        public void LastChild()
        {
            // this test modifies DOM; clone it first because it can break other tests in this class
            // when running concurrently
            
            var Dom = this.Dom.Clone();
            
            var res = Dom["body > [id] > :last-child"]; // two elements with IDs
            Assert.AreEqual(q("textarea", "test-show-last"), res, "last-child with child works");

            int count = res["[id]"].Length;
            int bodyChildIDCount = res["body [id] > :last-child[id]"].Length;


            res = res["#textarea"].Attr("id", null);
            Assert.AreEqual(res["[id]"].Length + 1, count, "One less ID");
            Assert.AreEqual(bodyChildIDCount - 1, res["body [id] > :last-child[id]"].Length, "Setting ID to null removed it from the index too");

            res[0].Id = "";
            Assert.AreEqual(bodyChildIDCount, res["body [id] > :last-child[id]"].Length, "Setting ID to empty string added it back");
            res.Attr("id", null);
            res.Attr("id", "newid");
            Assert.AreEqual(res["body [id='newid']"][0], res[0], "Further messing with ID and everything is fine");

            res = Dom["#hlinks-user span:last-child"];
            Assert.AreEqual(3, res.Length);

            res = Dom["#hlinks-user > span:last-child"];
            Assert.AreEqual(0, res.Length);

            res = Dom["#hlinks-user > textarea:last-child"];
            Assert.AreEqual(1, res.Length);
        }

    }

}