using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using CollectionAssert = NUnit.Framework.CollectionAssert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;
using CsQuery.Engine;
using CsQuery.ExtensionMethods;

namespace CsQuery.Tests.Core.Selectors
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
        public void AutoAddExtension()
        {
            var dom = TestDom("TestHtml");
            
            CQ res = dom.Select("span :isnotnumeric");

            Assert.AreEqual(11, res.Length);
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

        [Test, TestMethod]
        public void AutoAddRegex()
        {

            // The Regexp class should be automatically registered with the default startup options.
            // 12-30-2012- the regex code was moved to the main codebase so this test doesn't really do anything except
            // test the regex filter itself.
            
            //CsQuery.Config.PseudoClassFilters.Register("regex", typeof(Regexp));


            var dom = TestDom("TestHtml");
            
            
            var res = dom.Select("span:regex(class,[0-9])");

            CollectionAssert.AreEqual(res.Select(item => item.ClassName).ToArray(), Arrays.String("badge2", "badge3"),"Regex with class attribute worked");

            // match anything with a width
            res = dom.Select(":regex(css:width,'.+')");

            Assert.AreEqual(1, res.Length);
            Assert.AreEqual(dom["#hidden-div > :first-child"][0], res[0]);
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

       


    }
}