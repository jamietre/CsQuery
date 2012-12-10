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


    public partial class Methods: CsQueryTest
    {

        [Test, TestMethod]
        public void Width()
        {

            var dom = CQ.CreateFragment("<div style='width: 120px'>");

            Assert.AreEqual(120, dom.Css<int>("width"));
            dom.Width(150);

            Assert.AreEqual(150, dom.Css<int>("width"));
            Assert.AreEqual("150px", dom.Css("width"));

            dom.Width("150in");
            Assert.AreEqual("150in", dom.Css("width"));


        }

    }
}