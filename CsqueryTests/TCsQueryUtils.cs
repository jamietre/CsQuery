using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Dynamic;
using Jtc.CsQuery;
using NUnit.Framework;

namespace Jtc.CsQuery.Tests
{

    [TestFixture, Description("CsQuery Utility Tests (Not from Jquery test suite)")]
    public class CsQueryUtils
    {
        protected CsQuery csq;
        [SetUp]
        public void Init()
        {
            //string html = Support.GetFile("Resources\\TestHtml.htm");
            //csq = CsQuery.Create(html);
        }

        [Test]
        public void Json()
        {
            
            

            dynamic obj = CsQuery.FromJSON("{\"test\": \"value\", \"number\": 2}");
            
            Assert.AreEqual("value", obj.test);

            obj = CsQuery.FromJSON("{'test':  {'subprop1': 'subproperty1 value', 'subprop2': null }, 'number': 2}");

            Assert.AreEqual("subproperty1 value", obj.test.subprop1);
            Assert.AreEqual(2, obj.number);
        }
        protected class TestExpando
        {
            public string Property1 {get;set;}
            public string Field1;
        }
        [Test]
        public void Extend()
        {
            var test = new TestExpando();
            test.Field1 = "Value from Real Object";
            test.Property1 = "ValueFromProp";

            dynamic test2 = new ExpandoObject();
            test2.ExField1 = "Value from Expando";
            var exField2 = new string[] { "el1", "el2" };
            test2.ExField2 = exField2;

            dynamic target = CsQuery.Extend(null, test);
            Assert.AreEqual("Value from Real Object", target.Field1, "Appended a regular object field to an expando object");
            Assert.AreEqual("ValueFromProp", target.Property1, "Appended a regular object property to an expando object");

            CsQuery.Extend(target, test2);

            Assert.AreEqual("Value from Expando", target.ExField1, "Appended an expando object property to an expando object");
            Assert.AreEqual(exField2, target.ExField2, "Appended a regular object property to an expando object");

        }
    }
}