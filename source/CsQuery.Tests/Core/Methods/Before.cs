using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;

namespace CsQuery.Tests.Core
{
    /// <summary>
    /// This method is largely covered by the jQuery tests
    /// </summary>

    public partial class Methods: CsQueryTest
    {

        [Test, TestMethod]
        public void Before_MissingTarget()
        {
            
            var dom = TestDom("TestHtml");
            var len = dom["*"].Length;

            var target = dom["does-not-exist"];
            var content = dom["<div id='content' />"];

            target.Before(content);

            // nothing was added
            Assert.AreEqual(dom["*"].Length, len);
        }

        [Test, TestMethod]
        public void Before()
        {

            var dom = TestDom("TestHtml");

            var target = dom["#hlinks-user"];
            var content = CQ.Create("<div id='content' />");

            target.Before(content);

            // verify that content is now bound do the DOM, and that its in the right place

            Assert.AreEqual(content[0],target[0].PreviousElementSibling);
        }

        [Test, TestMethod]
        public void Before_FirstElement()
        {

            var dom = CQ.Create("<div id='first'></div><div id='second'></div>");

            var target = dom["#first"];
            var content = CQ.Create("<div id='content' />");

            target.Before(content);

            // verify that content is now bound do the DOM, and that its in the right place

            Assert.AreEqual(content[0], dom["*"].ElementAt(0));
            Assert.AreEqual(dom["#first"][0], dom["*"].ElementAt(1));

            // make sure the selection set of dom did not change

            Assert.AreEqual(dom.First()[0], dom["#first"][0]);
        }


   }
}