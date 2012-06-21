using System;
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

namespace CsqueryTests.Csharp.HtmlParser
{

    [TestFixture, TestClass]
    public class Html5Compliance : CsQueryTest
    {

        [Test, TestMethod]
        public void TabsInClassNames()
        {
            string html = "<html><body><div class=\"class1\tclass2\"></div></body></html>";
            var dom = CQ.Create(html);

            var div = dom["div"].FirstElement();
            Assert.AreEqual(2, div.Classes.Count());
            Assert.IsTrue(div.HasClass("class1"));
            Assert.IsTrue(div.HasClass("class2"));
            
 
        }

        [Test, TestMethod]
        public void NewLinesInClassNames()
        {
            var html = "<html><body><div class=\"class1" + System.Environment.NewLine + "class2  class3\r\n\t class4\"></div></body></html>";
            var dom = CQ.Create(html);

            var div = dom["div"].FirstElement();
            Assert.AreEqual(4, div.Classes.Count());
            Assert.IsTrue(div.HasClass("class1"));
            Assert.IsTrue(div.HasClass("class4"));
        }

        [Test, TestMethod]
        public void AutoCloseTwoTagsInARow()
        {
            var html = @" <table id=table-uda>
    <thead>
        <tr>
            <th>Attribute
             <th>Setter Condition
   <tbody><tr><td><dfn id=dom-uda-protocol title=dom-uda-protocol><code>protocol</code></dfn>
     <td><a href=#url-scheme title=url-scheme>&lt;scheme&gt;</a>
     </tr></table>";

            var dom = CQ.Create(html);

            Assert.AreEqual(1,dom["tbody"].Length);
            Assert.AreEqual("TABLE", dom["tbody"][0].ParentNode.NodeName);
        }
        [Test, TestMethod]
        public void AutoCreateTableTags()
        {
            var html = @" <table id=table-uda>
        <tr>
            <th>Attribute
             <th>Setter Condition
        <tr><td><dfn id=dom-uda-protocol title=dom-uda-protocol><code>protocol</code></dfn>
     <td><a href=#url-scheme title=url-scheme>&lt;scheme&gt;</a>
     </tr></table>";
            var dom = CQ.CreateFragment(html);

            // should not create wrapper
            Assert.AreEqual(0, dom["body"].Length);
            Assert.AreEqual(0, dom["head"].Length);
            
            AutoCreateTests(dom);

            dom = CQ.Create(html);

            // should not create wrapper
            Assert.AreEqual(1, dom["body"].Length);
            Assert.AreEqual(1, dom["html"].Length);
            Assert.AreEqual(1, dom["head"].Length);

            Assert.AreEqual(Arrays.Create("HEAD", "BODY"), dom["html > *"].Select(item => item.NodeName));
            AutoCreateTests(dom);

       

        }

        protected void AutoCreateTests(CQ dom)
        {
            Assert.AreEqual(1, dom["tbody"].Length);
            Assert.AreEqual(2, dom["th"].Length);
            Assert.AreEqual(2, dom["tr"].Length);
            Assert.AreEqual("TABLE", dom["tbody"][0].ParentNode.NodeName);

            var len = dom["body"].Length > 0 ?
                dom["body *"].Length : dom["*"].Length;

            Assert.AreEqual(11,len);


        }
    }

    
}