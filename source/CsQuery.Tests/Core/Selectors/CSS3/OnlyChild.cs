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
    public partial class Css3 : PseudoSelector
    {

        [Test, TestMethod]
        public void OnlyChild()
        {
            var res = Dom["span:only-child"];
            Assert.AreEqual(1, res.Length);
            Assert.AreEqual("reputation-score", res[0].ClassName);

            res = Dom["body :only-child"];
            Assert.AreEqual(2, res.Length);


            // CsQuery allows you to select ALL NODES including the doctype node - so this also returns "title"
            // when not selecting from body. 

            res = Dom[":only-child"];
            Assert.AreEqual(3, res.Length);
        }

    }

}