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

namespace CsQuery.Tests.Utility
{

    /// <summary>
    /// Basic JSON class tests
    /// </summary>
    [TestFixture, TestClass]
    public class JSON_
    {
        [Test, TestMethod]
        public void TestMethods()
        {
            string str = @"""The quick brown ""fox"" jumps""";
            string arr = @"[""one"",""two"",""three""]";
            string obj = "{ \"a\": b }";

            Assert.IsTrue(JSON.IsJsonString(str));
            Assert.IsFalse(JSON.IsJsonString(""));
            Assert.IsFalse(JSON.IsJsonString("a"));
            Assert.IsFalse(JSON.IsJsonString(arr));
            Assert.IsFalse(JSON.IsJsonString(obj));
            
            Assert.IsTrue(JSON.IsJsonArray(arr));
            Assert.IsFalse(JSON.IsJsonArray(""));
            Assert.IsFalse(JSON.IsJsonArray("a"));
            Assert.IsFalse(JSON.IsJsonArray(obj));
            Assert.IsFalse(JSON.IsJsonArray(str));

            Assert.IsTrue(JSON.IsJsonObject(obj));
            Assert.IsTrue(JSON.IsJsonObject("{}"));
            Assert.IsFalse(JSON.IsJsonObject("{"));
            Assert.IsFalse(JSON.IsJsonObject(""));
            Assert.IsFalse(JSON.IsJsonObject("a"));
            Assert.IsFalse(JSON.IsJsonObject(arr));
            Assert.IsFalse(JSON.IsJsonObject(str));
        }



        [Test, TestMethod]
        public void FromValues()
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

        /// <summary>
        /// Test handling of nonstandard JSON formats (using apostrophes and no-quotes for identifiers)
        /// </summary>
        [Test,TestMethod]
        public void NonStandard()
        {

            //dynamic obj = CsQuery.FromJSON("{\"test\": \"value\", \"number\": 2}");
            dynamic obj = CQ.ParseJSON("{test: 'value',number: 2}");
            
            Assert.AreEqual("value", obj.test, "Could parse JSON with no quotes");

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

        /// <summary>
        /// Test parsing single values
        /// </summary>
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


        

        #region Test data structures

        protected class TestExpando
        {
            public string Property1 { get; set; }
            public string Field1;
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

        #endregion
    }
}