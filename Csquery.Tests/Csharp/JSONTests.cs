using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;
using CsQuery.ExtensionMethods;

namespace CsqueryTests.Csharp
{

    [TestFixture, TestClass, Description("CsQuery Utility Tests (Not from Jquery test suite)")]
    public class JSONTests
    {
        protected CQ csq;
        [TestFixtureSetUp,ClassInitialize]
        public static void Init(TestContext context)
        {
            //string html = Support.GetFile("Resources\\TestHtml.htm");
            //csq = CsQuery.Create(html);
        }
        [Test, TestMethod]
        public void JsonFromValues()
        {
            string quotes = @"""The quick brown ""fox"" jumps""";
            Assert.IsTrue(JSON.IsJsonString(quotes));
            Assert.AreEqual("The quick brown \"fox\" jumps", JSON.ParseJSONValue(quotes));

            string arr = @"[""one"",""two"",""three""]";
            Assert.IsTrue(JSON.IsJsonArray(arr));
            object arrConv = JSON.ParseJSONValue(arr);
            Assert.AreEqual(arrConv.GetType(), typeof(List<string>),"Array converted to correct type.");
            
            var arrList = (List<string>)arrConv;
            Assert.AreEqual("two", arrList[1], "Array element converted.");
            Assert.AreEqual(3, arrList.Count, "Arr has correct # elements");

            arr = @"[""one"",2,""three""]";
            arrConv = JSON.ParseJSONValue(arr);
            Assert.AreEqual(arrConv.GetType(), typeof(List<object>), "Mixed type array converted to correct type.");
            var arrListObject = (List<object>)arrConv;
            Assert.AreEqual("three", arrListObject[2], "Array element converted.");
            Assert.AreEqual(2, arrListObject[1], "Array element converted.");
            Assert.AreEqual(3, arrListObject.Count, "Arr has correct # elements");

            
        }

        [Test,TestMethod]
        public void Json()
        {

            //dynamic obj = CsQuery.FromJSON("{\"test\": \"value\", \"number\": 2}");
            dynamic obj = CQ.ParseJSON("{test: 'value',number: 2}");
            
            Assert.AreEqual("value", obj.test);

            obj = CQ.ParseJSON("{'test':  {'subprop1': 'subproperty1 value', 'subprop2': null }, 'number': 2}");

            Assert.AreEqual("subproperty1 value", obj.test.subprop1);
            Assert.AreEqual(2, obj.number);

            var tc3 = new TestClass3();
            var expected = "{\"list\":[\"item1\",\"item2\"],"
                +"\"listMixed\":[3,\"hello\",{\"Prop1\":\"TC1 value1\",\"Prop2\":null},999],"
                +"\"floatingPointProp\":123.33,\"stringProp\":\"asdad\"}";

            string json = CQ.ToJSON(tc3);
            Assert.AreEqual(expected, json, "CsQuery.ToJson works");
        }

        [Test, TestMethod]
        public void JsonValues()
        {
            dynamic obj = CQ.ParseJSON<int>("2");
            Assert.AreEqual(2, obj);

            Assert.AreEqual(2,CQ.ParseJSON("2"));
            Assert.AreEqual(typeof(JsObject),CQ.ParseJSON("{}").GetType());
            Assert.AreEqual("abc123", CQ.ParseJSON("\"abc123\""));
            Assert.AreEqual(1.2, CQ.ParseJSON("1.2"));
            Assert.AreEqual(false, CQ.ParseJSON("false"));
            Assert.AreEqual(true, CQ.ParseJSON("true"));
            Assert.AreEqual(null, CQ.ParseJSON("undefined"));
            Assert.AreEqual(null, CQ.ParseJSON(""));
            Assert.Throws<ArgumentException>(() =>
            {
                var x = CQ.ParseJSON("x");
            });

        }

        protected class TestExpando
        {
            public string Property1 {get;set;}
            public string Field1;
        }
        [Test,TestMethod]
        public void TestJsObject()
        {
            dynamic obj = new JsObject();
            obj.val1 = "hello";
            obj.val2 = new JsObject();
            obj.val2.subprop1 = 99;
            obj.val2.subprop2 = "yo";

            Assert.AreEqual(null, obj.val3, "Can test missing property, and it is null");
            Assert.AreEqual("yo", obj.val2.subprop2, "Subproperty exists");
            string json = CQ.ToJSON(obj);

            Assert.AreEqual("{\"val1\":\"hello\",\"val2\":{\"subprop1\":99,\"subprop2\":\"yo\"}}", CQ.ToJSON(obj));


        }
        [Test,TestMethod]
        public void Extend()
        {
            var test = new TestExpando();
            test.Field1 = "Value from Real Object";
            test.Property1 = "ValueFromProp";

            dynamic test2 = new ExpandoObject();
            test2.ExField1 = "Value from Expando";
            var exField2 = new string[] { "el1", "el2" };
            test2.ExField2 = exField2;

            dynamic target = CQ.Extend(null, test);
            Assert.AreEqual("Value from Real Object", target.Field1, "Appended a regular object field to an expando object");
            Assert.AreEqual("ValueFromProp", target.Property1, "Appended a regular object property to an expando object");

            CQ.Extend(target, test2);

            Assert.AreEqual("Value from Expando", target.ExField1, "Appended an expando object property to an expando object");
            Assert.AreEqual(exField2, target.ExField2, "Appended a regular object property to an expando object");

            // Test "extending" regular objects (property copy)

            TestClass1 t1 = new TestClass1();
            t1.Prop1 = "value1";
            t1.Prop2 = "value2";

            TestClass2 t2 = new TestClass2();
            t2.Prop2 = "class2value2";
            t2.Prop3 = "class2vlaue3";

            CQ.Extend(t1, t2);

            Assert.AreEqual("value1", t1.Prop1, "Target prop1 unchanged");
            Assert.AreEqual("class2value2", t1.Prop2, "Target prop2 updated");

        }
        [Test,TestMethod]
        public void Extend2()
        {
            dynamic test = "{ prop1: 'val1', prop2: 'val2',prop3: 'original'}".ParseJSON();
            dynamic test2 = "{ prop1: 'from_enum1'}".ParseJSON();
            dynamic test3 = "{ prop2: 'from_enum2'}".ParseJSON();
            // will not work -- regular enumerables treated like objects
            //var enumer = new List<IDynamicMetaObjectProvider>();
            var enumer = new object[2];
            enumer[0]= test2;
            enumer[1] =test3;
            var merged = CQ.Extend(null, test, enumer);

            Assert.AreEqual("{prop1: 'from_enum1', prop2: 'from_enum2', prop3: 'original'}".ParseJSON(), merged, "Merged with an enumerable parameter");

            Assert.AreNotEqual("{prop1: 'from_enum1', prop2: 'from_enum2'}".ParseJSON(), merged, "Sanity check");

            // TODO: Test copying from an object with properties that return errors
        }
        [Test,TestMethod]
        public void Styles()
        {
            var styleDefs = DomStyles.StyleDefs;
            ICssStyle style = styleDefs["padding-left"];
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