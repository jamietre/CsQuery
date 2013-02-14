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
    public class ChildNodeList_ : CsQueryTest
    {
        
        [Test, TestMethod]
        public void ChangeEvent()
        {
            var dom = TestDom("TestHtml");
            
            INodeList list = dom["#hlinks-user > span:eq(1)"][0].ChildNodes;

            IDomObject changedNode = null;
            list.OnChanged += (sender, e) =>
            {
                changedNode = e.Node;
            };

            dom[".badge3"].Remove();
            Assert.AreEqual(null, changedNode);

            var nodeToRemove =  dom[".badge2"][0];
            nodeToRemove.Remove();
            Assert.AreEqual(nodeToRemove, changedNode);

            var nodeToAdd = dom["<div id=test />"][0];
            dom["[title='2 silver badges']"].Append(nodeToAdd);

            Assert.AreEqual(nodeToAdd, changedNode);
        }

    }
}

