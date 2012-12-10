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
        public void Even()
        {
            var res = Dom["#hlinks-user span:even"];
            Assert.AreEqual(5, res.Length);
            Assert.AreEqual("profile-triangle", res[0].ClassName);
            Assert.AreEqual("badgecount", res[2].ClassName);
        }

    }

}