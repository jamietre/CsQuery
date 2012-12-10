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
using CollectionAssert = NUnit.Framework.CollectionAssert ;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.HtmlParser;
using CsQuery.Utility;

namespace CsQuery.Tests.HtmlParser
{
    
    [TestFixture, TestClass]
    public class Classes : CsQueryTest
    {


        /// <summary>
        /// Issue#10
        /// </summary>
        [Test, TestMethod]
        public void ClassNameCase()
        {
            var dom = CQ.CreateFragment("<div class=\"class1 CLASS2 claSS3\" x=\"y\" />");
            var el= dom.Select("div").FirstElement();

            Assert.AreEqual(3,el.Classes.Count());
            CollectionAssert.AreEqual(Arrays.String("class1","CLASS2","claSS3"),el.Classes.ToList());

            Assert.AreEqual(0, dom[".class2"].Length);
            Assert.AreEqual(1, dom[".CLASS2"].Length);

             el= CQ.CreateFragment("<div class=\"class CLASS\" />").Select("div").FirstElement();

            Assert.AreEqual(2,el.Classes.Count());
            CollectionAssert.AreEqual(Arrays.String("class","CLASS"),el.Classes.ToList());

        }
    }
}