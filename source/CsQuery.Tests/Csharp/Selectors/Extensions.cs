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
using CsQuery.Engine;

namespace CsQuery.Tests.Csharp.Selectors
{
    
    [TestFixture, TestClass]
    public class Extensions: CsQueryTest
    {


        [Test, TestMethod]
        public void SimpleExtension()
        {
            var dom = TestDom("TestHtml");
            CsQuery.Config.PseudoClassFilters.Register("isnumeric", typeof(ContentIsNumeric));
            
            CQ res = dom.Select("span :isnumeric");
            Assert.AreEqual(3, res.Length);
            Assert.AreEqual("reputation-score", res[0].ClassName);

            // this time the actual spans with the numeric won't be selected, since has only operates on children./`
            res = dom.Select("span:has(:isnumeric)");
            Assert.AreEqual(3, res.Length);
            Assert.AreEqual("hlinks-user", res[0].Id);
        }

        [Test, TestMethod]
        public void ParameterizedExtension()
        {
            var dom = TestDom("TestHtml");
            CsQuery.Config.PseudoClassFilters.Register("is-child-of-type", typeof(IsChildOfTag));

            CQ res = dom.Select(":is-child-of-type(div)");
            
            Assert.AreEqual(7, res.Length);
            foreach (var item in res) {
                Assert.AreEqual("DIV", item.ParentNode.NodeName);
            }
        }




        /// <summary>
        /// An example filter that chooses only elements with direct numeric content (e.g. not descendant nodes)
        /// </summary>

        private class ContentIsNumeric : PseudoSelectorFilter
        {

            public override bool Matches(IDomObject element)
            {
                if (element.HasChildren)
                {
                    foreach (var item in element.ChildNodes)
                    {
                        if (item.NodeType == NodeType.TEXT_NODE)
                        {
                            double val;
                            if (double.TryParse(item.NodeValue, out val))
                            {
                                return true;
                            }
                        }
                    }
                }
                return false; 
            }
        }

        private class IsChildOfTag : PseudoSelectorChild
        {
            public override string Arguments
            {
                get
                {
                    return base.Arguments;
                }
                set
                {
                    base.Arguments = value.ToUpper();
                }
            }

            public override bool Matches(IDomObject element)
            {
                return element.ParentNode.NodeName == Arguments;
            }

            public override int MinimumParameterCount
            {
                get
                {
                    return 1;
                }
            }
            public override int MaximumParameterCount
            {
                get
                {
                    return 1;
                }
            }
        }
        
        //private bool ContentGreaterThan(IDomObject obj, string parms)
        //{
        //    if (obj.NodeType == NodeType.TEXT_NODE)
        //    {
        //        double val;
        //        if (double.TryParse(obj.NodeValue, out val))
        //        {
                    
        //        }
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

    }
}