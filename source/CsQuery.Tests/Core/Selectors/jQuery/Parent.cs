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
    public partial class jQuery : PseudoSelector
    {
        [Test, TestMethod]
        public void Parent()
        {
            var dom = CQ.Create("<html><div></div></html>");
            var res2 = dom[":parent"];
            Assert.AreEqual("HTML",res2[0].NodeName);

            /// when testing this with jsfiddle, you will get 26: it includes title, style, script from the wrapper
            /// 
            var res = Dom[":parent"];
            Assert.AreEqual(23, res.Length);

            res = Dom["span:parent"];
            Assert.AreEqual(10, res.Length);

            res = Dom["body :parent"];
            Assert.AreEqual(20, res.Length);


        }

    }

}