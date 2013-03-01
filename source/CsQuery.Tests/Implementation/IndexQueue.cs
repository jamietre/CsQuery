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
    public class IndexQueue_ : CsQueryTest
    {
        
        [Test, TestMethod]
        public void TestFastIndexChanges()
        {
            var dom = TestDom("TestHtml");
            
            var target = dom["#hlinks-user"];
            target.Children().Remove();

            // removing & readding to the same place should cause the indexqueue to recycle the index keys.
            
            target.Append("<span class=added />");
            target.Append("<span class=added />");
            target.Append("<div class=added />");

            Assert.AreEqual(3, dom[".added"].Length);



        }

    }
}

