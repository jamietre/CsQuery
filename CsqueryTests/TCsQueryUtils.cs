using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Dynamic;
using Jtc.CsQuery.Utility;
using NUnit.Framework;
using Jtc.CsQuery;

namespace CsqueryTests
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

            //dynamic obj = CsQuery.FromJSON("{\"test\": \"value\", \"number\": 2}");
            dynamic obj = CsQuery.ParseJSON("{test: 'value',number: 2}");
            
            Assert.AreEqual("value", obj.test);

            obj = CsQuery.ParseJSON("{'test':  {'subprop1': 'subproperty1 value', 'subprop2': null }, 'number': 2}");

            Assert.AreEqual("subproperty1 value", obj.test.subprop1);
            Assert.AreEqual(2, obj.number);

            var tc3 = new TestClass3();
            var expected = "{\"list\":[\"item1\",\"item2\"],"
                +"\"listMixed\":[3,\"hello\",{\"Prop1\":\"TC1 value1\",\"Prop2\":null},999],"
                +"\"floatingPointProp\":123.33,\"stringProp\":\"asdad\"}";

            string json = CsQuery.ToJSON(tc3);
            Assert.AreEqual(expected, json, "CsQuery.ToJson works");

            
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

            // Test "extending" regular objects (property copy)

            TestClass1 t1 = new TestClass1();
            t1.Prop1 = "value1";
            t1.Prop2 = "value2";

            TestClass2 t2 = new TestClass2();
            t2.Prop2 = "class2value2";
            t2.Prop3 = "class2vlaue3";

            CsQuery.Extend(t1, t2);

            Assert.AreEqual("value1", t1.Prop1, "Target prop1 unchanged");
            Assert.AreEqual("class2value2", t1.Prop2, "Target prop2 updated");

        }
        [Test]
        public void Extend2()
        {
            dynamic test = "{ prop1: 'val1', prop2: 'val2',prop3: 'original'}".FromJSON();
            dynamic test2 = "{ prop1: 'from_enum1'}".FromJSON();
            dynamic test3 = "{ prop2: 'from_enum2'}".FromJSON();
            var enumer = new List<ExpandoObject>();
            enumer.Add(test2);
            enumer.Add(test3);
            var merged = CsQuery.Extend(null, test, enumer);

            Assert.AreEqual("{prop1: 'from_enum1', prop2: 'from_enum2', prop3: 'original'}".FromJSON(), merged,"Merged with an enumerable parameter");

            Assert.AreNotEqual("{prop1: 'from_enum1', prop2: 'from_enum2'}".FromJSON(), merged,"Sanity check");

        }
        [Test]
        public void Styles()
        {
            var styleDefs = HtmlDom.StyleDefs;
            CssStyle style = styleDefs["padding-left"];
            Assert.AreEqual(CssStyleType.Unit | CssStyleType.Option ,style.Type,  "Padding style is correct type");
            
            style = styleDefs["word-wrap"];
            HashSet<string> expectedOpts = new HashSet<string>(new string[] { "normal", "break-word" });
            Assert.AreEqual(expectedOpts, style.Options, "word-wrap has correct options");
        }
        protected class TestClass1
        {
            public string Prop1;
            public string Prop2;
        }
        protected class TestClass2
        {
            public string Prop2;
            public string Prop3;
        }
        protected class TestClass3
        {
            public List<string> list = new List<string>(new string[] {"item1","item2"});
            public List<object> listMixed= new List<object>();
            public TestClass3() {
                listMixed.Add(3);
                listMixed.Add("hello");
                TestClass1 tc1 = new TestClass1();
                tc1.Prop1="TC1 value1";
                tc1.Prop2 = null;
                listMixed.Add(tc1);
                listMixed.Add(999);

            }
            public double floatingPointProp = 123.33;
            public string stringProp = "asdad";
        }
    }
}