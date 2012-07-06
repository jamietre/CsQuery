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

namespace CsQuery.Tests.Csharp.Selectors
{
    
    [TestFixture, TestClass]
    public class Extensions: CsQueryTest
    {
        

        //[Test,TestMethod]
        //public void SimpleExtension()
        //{
        //    var dom = TestDom("TestHtml");
        //    CsQuery.Engine.PseudoSelectors.Extensions.Add("isnumeric",ContentIsNumeric);

        //    CQ res = dom.Select("span :isnumeric");            
        //    Assert.AreEqual(3, res.Length);

        //    res = dom.Select("span:has(:isnumeric)");
        //    Assert.AreEqual(6, res.Length);
        //}

        //[Test, TestMethod]
        //public void ParameterizedExtension()
        //{
        //    var dom = TestDom("TestHtml");
        //    CsQuery.Engine.PseudoSelectors.Extensions.Add("valuegt", ContentGreaterThan);

        //    CQ res = dom.Select("span :valuegt(1000)");
        //    Assert.AreEqual(1, res.Length);

        //}



        private bool ContentIsNumeric(IDomObject obj, string parms)
        {
            if (obj.NodeType == NodeType.TEXT_NODE)
            {
                double val;
                return double.TryParse(obj.NodeValue, out val);
            }
            else
            {
                return false;
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