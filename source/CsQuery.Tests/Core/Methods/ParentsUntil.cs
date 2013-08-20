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
using CsQuery.ExtensionMethods;

namespace CsQuery.Tests.Core
{
    public partial class Methods: CsQueryTest
    {
       
        [Test, TestMethod]
        public void ParentsTextNode()
        {
            CQ dom = CQ.Create("<div>abc <a href=\"\">def</a> ghi</div>", HtmlParsingMode.Content);
            CQ textNodes = new CQ(dom.Select("*").Contents().Where(item => item.NodeType == NodeType.TEXT_NODE));
            
            var p = textNodes.Parents();

            CollectionAssert.AreEquivalent(
                new IDomObject[] { dom["div"][0], dom["a"][0] }, p
                );


        }
    }
}