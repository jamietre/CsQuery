using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Reflection;
using System.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;

namespace CsQuery.Tests.Core
{
    public partial class Methods: CsQueryTest
    {
        CQ dom = "<div data-s='a string' data-n1='1' data-n2='1.3' data-b=false data-s2='\"json string\"'></div>";

        [Test, TestMethod]
        public void DataObject()
        {
            dynamic obj = dom["div"].Data();

            Assert.AreEqual("a string", obj.s);
            Assert.AreEqual(1, obj.n1);
            Assert.AreEqual(1.3, obj.n2);
            Assert.AreEqual(false, obj.b);
            Assert.AreEqual("json string", obj.s2);

        }

        [Test, TestMethod]
        public void DataComponentsUntyped()
        {
            var div = dom["div"];

            Assert.AreEqual("a string",dom.Data("s"));
            Assert.AreEqual(1, dom.Data("n1"));
            Assert.AreEqual(1.3, dom.Data("n2"));
            Assert.AreEqual(false, dom.Data("b"));
            Assert.AreEqual("json string", dom.Data("s2"));

        }


        [Test, TestMethod]
        public void DataComponentsTyped()
        {
            var div = dom["div"];

            Assert.AreEqual("a string", dom.Data<string>("s"));
            Assert.AreEqual(1, dom.Data<int>("n1"));
            
            Assert.AreEqual(1.3, dom.Data<double>("n2"));
            Assert.AreEqual(false, dom.Data<bool>("b"));
            Assert.AreEqual("json string", dom.Data<string>("s2"));

            // cast to something else
            Assert.AreEqual("1",dom.Data<string>("n1"));
            Assert.AreEqual("false",dom.Data<string>("b"));
            Assert.AreEqual(true, dom.Data<bool>("n1"));

            // bad casts
            Assert.Throws<InvalidCastException>(() =>
            {
                var x = dom.Data<int>("s");
            });
            Assert.Throws<InvalidCastException>(() =>
            {
                var x = dom.Data<bool>("s2");
            });
        }
    }
}