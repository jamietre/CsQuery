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
        public void Odd()
        {
            var res = Dom["#hlinks-user span:odd"];
            Assert.AreEqual(4, res.Length);
            Assert.AreEqual("reputation-score", res[0].ClassName);
            Assert.AreEqual("badge2", res[1].ClassName);
        }

    }

}