using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery.Implementation;

namespace CsQuery.Tests.Core.Implementation
{
    [TestFixture, TestClass]
    public class CssStyleDeclaration_ : CsQueryTest
    {
        
        [Test, TestMethod]
        public void StyleInIndex()
        {
            CQ test = "<div></div><div id=target style='width: 10px' ></div>";

            var target = test["div[style]"];

            Assert.AreEqual(1, target.Length);
            Assert.AreEqual("10px", target.Css("width"));

            target.Css("width", null);

            target = test["div[style]"];
            Assert.AreEqual(1, target.Length);
            Assert.AreEqual("", target.Attr("style"));

            target[0].RemoveAttribute("style");

            Assert.AreEqual(null, target.Attr("style"));
            target = test["div[style]"];
            Assert.AreEqual(0, target.Length);
           
        }

    }
}

