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
    [TestFixture, TestClass]
    public class JsObject_ : CsQueryTest
    {
        private static IDictionary<string, object> testDict = null;

        public override void FixtureSetUp()
        {
            base.FixtureSetUp();
            testDict = new Dictionary<string, object>();
            testDict.Add("Prop1", "value1");
            testDict.Add("Prop2", 999);
            DateTime timeNow = DateTime.Now;
            testDict.Add("Prop3", timeNow);
            IDictionary<string, object> subDict = new Dictionary<string, object>();
            testDict.Add("Prop4", subDict);
            subDict.Add("subProp1", "sub value 1");
            subDict.Add("subprop2", null);
        }

        /// <summary>
        /// This is not possible yet. I leave this here so hopefully some future version of the C# compiler
        /// will permit a DynamicObject to be told about the target type upon gets.
        /// </summary>
        [Test, TestMethod]
        public void CoerceTypes()
        {
            //dynamic test = new JsObject();
            //test.stringVal = "12";
            //int intVal = test.stringVal;
        }
        [Test,TestMethod]
        public void Dict2Expando()
        {
            dynamic obj = Objects.Dict2Dynamic<JsObject>(testDict);
            Assert.AreEqual("value1", obj.Prop1, "Value 1 is equal");
            Assert.AreEqual("value1", obj.PROP1, "Case insensitivity works");
            Assert.IsTrue(obj.Prop4.GetType() == typeof(JsObject), "Sub property is a JsObject");
            Assert.AreEqual("sub value 1", obj.prop4.subprop1, "Subproperty is accessible and correct");

            string json = JSON.ToJSON(obj);
            dynamic back = json.ParseJSON();
            Assert.AreEqual("sub value 1", back.prop4.subprop1, "Subproperty is accessible and correct");
            TimeSpan diff =(((DateTime)obj.prop3).Subtract((DateTime)back.prop3));

            Assert.IsTrue(Math.Abs(diff.Milliseconds)<1000, "Time was the same");
        }
        [Test,TestMethod]
        public void JsObjectConversion()
        {
            dynamic obj = Objects.Dict2Dynamic<JsObject>(testDict);

            obj.listprop = new JsObject();
            obj.listprop.val1 = 1;
            obj.listprop.val2 = 10;
            obj.listprop.val3 = 93;
            obj.listprop.valx = 192;

            List<long> expected = new List<long>(new long[] {1,10,93,192});
            List<long> actual = new List<long>(obj.listprop.Enumerate<long>());
            Assert.AreEqual(expected, actual, "Enumerate works");

            //JsObject subObj = obj.listprop;
            //actual = new List<long>(Objects.Enumerate<long>(subObj.Enumerate<long>(TestUseFunc));

            //Assert.AreEqual(expected, actual, "Enumerate works using a function to generate the data");

        }
        [Test,TestMethod]
        public void JsObjectProperties()
        {
            JsObject jsObj = new JsObject();
            dynamic obj = jsObj;
            obj.someProp = "test";

            Assert.AreEqual(null, obj.missingProp, "Accessing missing property returns null");

            object outval;
            bool hasValue = ((IDictionary<string, object>)jsObj).TryGetValue("someOtherProp", out outval);
            // Bug 12-15-2011 - using JsObject as generic IDict problems
            Assert.IsFalse(hasValue, "TryGetValue (casting as IDictionary) still knows that property doesn't exist");

        }
        protected long TestUseFunc(string key, object value)
        {
            return System.Convert.ToInt64(value);
        }
        
    }
}