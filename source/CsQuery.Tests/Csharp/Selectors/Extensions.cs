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
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;
using CsQuery.Engine;
using CsQuery.ExtensionMethods;

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

        [Test, TestMethod]
        public void Regex()
        {

            var dom = TestDom("TestHtml");
            CsQuery.Config.PseudoClassFilters.Register("regex", typeof(Regexp));
           
            var res = dom.Select("span:regex(class,[0-9])");

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

        /// <summary>
        /// Port of James Padolsey's regex jQuery selector: http://james.padolsey.com/javascript/regex-selector-for-jquery/
        /// </summary>

        private class Regexp : PseudoSelectorFilter
        {
            private enum Modes {
                Data=1,
                Css=2,
                Attr=3
            }

            private string Property;
            private Modes Mode;
            private Regex Expression;

            public override bool Matches(IDomObject element)
            {

                switch (Mode)
                {
                    case Modes.Attr:
                        return Expression.IsMatch(element[Property]);

                    case Modes.Css:
                        return Expression.IsMatch(element.Style[Property]);
                    case Modes.Data:
                        return Expression.IsMatch(element.Cq().DataRaw(Property));
                    default: 
                        throw new NotImplementedException();
                }
            }

            private void Configure()
            {
                var validLabels = new Regex("/^(data|css):/");
                
                if (validLabels.IsMatch(Parameters[0]))
                {
                    string[] subParm =Parameters[0].Split(':');
                    string methodName = subParm[0];

                    if (methodName == "data")
                    {
                        Mode = Modes.Data;
                    }
                    else if (methodName == "css")
                    {
                        Mode = Modes.Css;
                    }
                    else
                    {
                        throw new ArgumentException("Unknown mode for regex pseudoselector.");
                    }
                    Property = subParm[1];
                }
                else
                {
                    Mode = Modes.Attr;
                    Property = Parameters[0];
                }
                Expression = new Regex(Parameters[1].RegexReplace(@"^\s+|\s+$/g",""),RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }


            // we override arguments to do some setup when this selector is first created since there's no need to do it
            // on each call, as in the original code

            public override string Arguments
            {
                get
                {
                    return base.Arguments;
                }
                set
                {
                    base.Arguments = value;
                    Configure();
                }
            }

            // Allow either parameter to be optionally quoted
            protected override bool?  ParameterQuoted(int index)
            {
 	             return null;
            }

            public override int  MaximumParameterCount
            {
	            get 
	            { 
		             return 2;
	            }
            }
            public override int  MinimumParameterCount
            {
	            get 
	            { 
		             return 2;
	            }
            }

        }

//        jQuery.expr[':'].regex = function(elem, index, match) {
//    var matchParams = match[3].split(','),
//        validLabels = /^(data|css):/,
//        attr = {
//            method: matchParams[0].match(validLabels) ? 
//                        matchParams[0].split(':')[0] : 'attr',
//            property: matchParams.shift().replace(validLabels,'')
//        },
//        regexFlags = 'ig',
//        regex = new RegExp(matchParams.join('').replace(/^\s+|\s+$/g,''), regexFlags);
//    return regex.test(jQuery(elem)[attr.method](attr.property));
//}
    }
}