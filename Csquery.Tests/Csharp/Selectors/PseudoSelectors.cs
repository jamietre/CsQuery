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

namespace CsqueryTests.Csharp
{

    [TestFixture, TestClass]
    public class PseudoSelectors : CsQueryTest
    {

        [Test, TestMethod]
        public void LastChild()
        {

            var res = Dom["body > [id] > :last-child"]; // two elements with IDs
            Assert.AreEqual(q("textarea", "test-show-last"), res, "last-child with child works");

            int count = res["[id]"].Length;
            int bodyChildIDCount = res["body [id] > :last-child[id]"].Length;


            res = res["#textarea"].Attr("id", null);
            Assert.AreEqual(res["[id]"].Length + 1, count, "One less ID");
            Assert.AreEqual(bodyChildIDCount - 1, res["body > [id]"].Length, "Setting ID to null removed it from the index too");

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

        
        [Test, TestMethod]
        public void NthChild()
        {
            var res = Dom["body > :nth-child(2)"];
            Assert.AreEqual(jQuery("body").Children().Eq(1)[0], res[0], "nth-child(x) works");

            // odd & even are reversed for the jQuery pseudoselectors from nth-child version.

            var even =  Dom["body > :nth-child(2n)"];
            Assert.AreEqual(jQuery("body").Children(":odd").Elements, even.Elements, "Simple math nth child workd");

            var odd = Dom["body > :nth-child(2n+1)"];
            Assert.AreEqual(jQuery("body").Children(":even").Elements, odd.Elements, "Simple math nth child workd");

            res = Dom["#hlinks-user > :nth-child(2(n+1))"];
            Assert.AreEqual(jQuery("#hlinks-user").Children(":odd").Not("#profile-triangle").Elements, res.Elements, "Simple math nth child workd");

            // odd & even parms

            res = Dom["body > :nth-child(even)"];
            Assert.AreEqual(res.Elements, even.Elements);

            res = Dom["body > :nth-child(odd)"];
            Assert.AreEqual(res.Elements, odd.Elements);
        }
        
        [Test, TestMethod]
        public void OnlyChild()
        {
            var res = Dom["span:only-child"];
            Assert.AreEqual(1,res.Length);
            Assert.AreEqual("reputation-score", res[0].ClassName);

            res = Dom["body :only-child"];
            Assert.AreEqual(2, res.Length);


            // CsQuery allows you to select ALL NODES including the doctype node - so this also returns "title"
            // when not selecting from body. 

            res = Dom[":only-child"];
            Assert.AreEqual(3, res.Length);
        }
          

        [Test, TestMethod]
        public void NthChildOfType()
        {
            var res = Dom["body > span:nth-of-type(1)"];
            Assert.AreEqual(1, res.Length);
            Assert.AreEqual("hlinks-user", res[0].Id);

            res = Dom["body span:nth-of-type(2)"];
            Assert.AreEqual(3,res.Length);
            Assert.AreEqual("badgecount", res[1].ClassName);
            Assert.AreEqual("badgecount", res[2].ClassName);

            // odd & even are reversed for the jQuery pseudoselectors from nth-child version.

            var even = Dom["body span:nth-of-type(2n)"];
            Assert.AreEqual(4, even.Length);
            Assert.AreEqual("lsep", even[3].ClassName);
            
        }

        /// <summary>
        /// First of Type selector returns elements which are the first child matching the type of the specific element type.
        /// Differs from first-child in that elements that aren't the same type are not consisdered when evaluating "first"
        /// </summary>
        [Test, TestMethod]
        public void FirstOfType()
        {
            var res = Dom["#hlinks-user a:first-of-type"];
            Assert.AreEqual(1, res.Length);
            Assert.IsTrue(res.Is(".profile-link"));

            res = Dom["#hlinks-user a:first-child"];
            Assert.AreEqual(0, res.Length);

            // with no tag type, should return anything that is the first of its type
            res = Dom["#hlinks-user :first-of-type"];
            Assert.AreEqual(7, res.Length);
            Assert.AreEqual("profile-triangle",res[0].ClassName);
            Assert.AreEqual("profile-link",res[1].ClassName);
            Assert.AreEqual("reputation-score",res[2].ClassName);
            Assert.AreEqual("badge2",res[3].ClassName);
            Assert.AreEqual("badge3",res[4].ClassName);
            Assert.AreEqual("INPUT",res[5].NodeName);
            Assert.AreEqual("TEXTAREA",res[6].NodeName);
        }

        [Test, TestMethod]
        public void LastOfType()
        {
            var res = Dom["#hlinks-user > span:last-of-type"];
            Assert.AreEqual(1, res.Length);
            Assert.IsTrue(res.Is(".lsep"));

            res = Dom["#hlinks-user span:last-of-type"];
            Assert.AreEqual(4, res.Length);

            // with no tag type, should return all children
            res = Dom["#hlinks-user :last-of-type"];
            Assert.AreEqual(7, res.Length);
        }


        [Test, TestMethod]
        public void Empty()
        {
            var res = Dom["span:empty"];
            Assert.AreEqual(2, res.Length);
            CollectionAssert.AreEquivalent(res,Dom[".badge2,.badge3"]);


            res = Dom["body :empty"];
            Assert.AreEqual(5, res.Length);

            res = Dom[":empty"];
            Assert.AreEqual(6, res.Length, "6 total element seleced (5 in body, + title & html nopde ");
        }

        [Test, TestMethod]
        public void OnlyOfType()
        {
            var res = Dom["span:only-of-type"];
            Assert.AreEqual(4, res.Length);
            CollectionAssert.AreEquivalent(res, Dom["#hlinks-user,.reputation-score,#hidden-span,#non-hidden-span"]);


            res = Dom["body :only-of-type"];
            Assert.AreEqual(11, res.Length);

            res = Dom["a:only-of-type"];
            Assert.AreEqual(0, res.Length);

        }

        #region Setup
        
        public override void FixtureSetUp()
        {
            Dom = TestDom("TestHtml");
        }
       
        #endregion
    }
}