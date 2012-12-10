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
        public void End()
        {

            var dom = TestDom("TestHtml");

            var res = dom["#hlinks-user"].Find("span");
            Assert.AreEqual("profile-triangle", res[0].ClassName);
            Assert.AreEqual("badge2", res.Find("span")[0].ClassName);
            Assert.AreEqual("profile-triangle", res.Find("span").End()[0].ClassName);
        }

    }
}