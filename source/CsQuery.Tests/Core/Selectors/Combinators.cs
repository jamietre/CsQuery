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
    
    [TestFixture, TestClass]
    public class Combinators: CsQueryTest
    {
        
        public override void FixtureSetUp()
        {
            base.FixtureSetUp();
            Dom = TestDom("TestHtml");
        }

        [Test,TestMethod]
        public void Adjacent()
        {
            
            CQ res = Dom.Find("span+a");
            Assert.AreEqual(1, res.Length);
            Assert.AreEqual("profile-link", res[0].ClassName);
        }

        [Test, TestMethod]
        public void AdjacentSelf()
        {

            CQ res = Dom.Find("#hlinks-user > span+span");
            Assert.AreEqual(1, res.Length);
            Assert.AreEqual("13 bronze badges", res[0]["title"]);
        }

        /// <summary>
        /// Siblings self; the first element should not end up in the selection set.
        /// </summary>

        [Test, TestMethod]
        public void SiblingsSelf()
        {
            var dom= CQ.Create("<table><tr id=1><tr id=2><tr id=3></table>");

            var res = dom["table tr+tr"];
            Assert.AreEqual(2, res.Length);
            CollectionAssert.AreEqual(Arrays.String("2", "3"), res.Select(item => item.Id).ToList());
        }


        [Test,TestMethod]
        public void AdjacentMultiple() {

            var res = Dom.Select("a+*");
            Assert.AreEqual(2, res.Length);
            Assert.AreEqual("reputation_link", res[0].Id);

            res = Dom.Select("a+span+span");
            Assert.AreEqual(1, res.Length);
            Assert.AreEqual("13 bronze badges", res[0]["title"]);

            res = Dom.Select("a+*+span");
            Assert.AreEqual(2, res.Length);
        }
        [Test, TestMethod]
        public void Siblings()
        {

            
            var res = Dom.Find("p~span");
            Assert.AreEqual(1, res.Length);
            Assert.AreEqual("hlinks-user", res[0].Id);

            res = Dom.Find("div~span");
            Assert.AreEqual(0, res.Length);


            res = Dom.Find("a~a");
            Assert.AreEqual(1, res.Length);
            Assert.AreEqual("reputation_link", res[0].Id);
        }
        [Test, TestMethod]
        public void Complex()
        {
            var res = Dom.Find("p~span+div");
            Assert.AreEqual(1, res.Length);
            Assert.AreEqual("test-show", res[0].Id);
        }

        //[Test, TestMethod]
        //public void Inherited()
        //{
        //    var dom = TestDom("TestHtml2");
        //    var res = dom["div > span:lang(en)"];
        //    Assert.AreEqual(res.Length, 2);

        //    res = dom["div > span:lang(en-uk)"];
        //    Assert.AreEqual(res.Length, 1);
        //}


    }
}