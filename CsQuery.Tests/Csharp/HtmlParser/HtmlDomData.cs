﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.HtmlParser;
using CsQuery.Utility;

namespace CsQuery.Tests.Csharp.HtmlParser
{
    
    [TestFixture, TestClass]
    public class HtmlDomData : CsQueryTest
    {
      
        [Test, TestMethod]
        public void IsBlock()
        {
            Assert.IsTrue(HtmlData.IsBlock("div"));
            Assert.IsFalse(HtmlData.IsBlock("b"));
            Assert.IsFalse(HtmlData.IsBlock("input"));
            Assert.IsFalse(HtmlData.IsBlock("random"));
        }

        [Test, TestMethod]
        public void IsBoolean()
        {
            Assert.IsTrue(HtmlData.IsBoolean("checked"));
            Assert.IsFalse(HtmlData.IsBoolean("p"));
            Assert.IsFalse(HtmlData.IsBoolean("input"));
            Assert.IsFalse(HtmlData.IsBoolean("random"));
        }

        [Test, TestMethod]
        public void InnerTextAllowed()
        {
            Assert.IsTrue(HtmlData.InnerTextAllowed("textarea"));
            Assert.IsTrue(HtmlData.InnerTextAllowed("script"));
            Assert.IsTrue(HtmlData.InnerTextAllowed("style"));
            Assert.IsTrue(HtmlData.InnerTextAllowed("div"));
            Assert.IsTrue(HtmlData.InnerTextAllowed("p"));
            Assert.IsTrue(HtmlData.InnerTextAllowed("option"));
            Assert.IsTrue(HtmlData.InnerTextAllowed("random"));

            Assert.IsFalse(HtmlData.InnerTextAllowed("br"));
            Assert.IsFalse(HtmlData.InnerTextAllowed("link"));
        }

        [Test, TestMethod]
        public void HtmlChildrenNotAllowed()
        {

            Assert.IsTrue(HtmlData.HtmlChildrenNotAllowed("br"));
            Assert.IsTrue(HtmlData.HtmlChildrenNotAllowed("link"));

            Assert.IsTrue(HtmlData.HtmlChildrenNotAllowed("textarea"));
            Assert.IsTrue(HtmlData.HtmlChildrenNotAllowed("script"));
            Assert.IsTrue(HtmlData.HtmlChildrenNotAllowed("style"));

            Assert.IsFalse(HtmlData.HtmlChildrenNotAllowed("div"));
            Assert.IsFalse(HtmlData.HtmlChildrenNotAllowed("p"));
            Assert.IsFalse(HtmlData.HtmlChildrenNotAllowed("option"));
            Assert.IsFalse(HtmlData.HtmlChildrenNotAllowed("random"));

        }

        [Test, TestMethod]
        public void TagHasImplicitClose()
        {
            Assert.AreEqual(HtmlData.tagActionClose,HtmlData.SpecialTagAction("p","div"));
            Assert.AreEqual(HtmlData.tagActionNothing, HtmlData.SpecialTagAction("p", "b"));
            Assert.AreEqual(HtmlData.tagActionNothing, HtmlData.SpecialTagAction("p", "random"));

            Assert.AreEqual(HtmlData.tagActionClose, HtmlData.SpecialTagAction("td", "tr"));
            Assert.AreEqual(HtmlData.tagActionNothing, HtmlData.SpecialTagAction("td", "div"));

            Assert.AreEqual(HtmlData.tagActionClose, HtmlData.SpecialTagAction("li", "li"));
            Assert.AreEqual(HtmlData.tagActionNothing, HtmlData.SpecialTagAction("li", "ol"));

            Assert.AreEqual(HtmlData.tagActionNothing, HtmlData.SpecialTagAction("random", "random"));
            Assert.AreEqual(HtmlData.tagActionNothing, HtmlData.SpecialTagAction("div", "div"));
        }
    }

    
}