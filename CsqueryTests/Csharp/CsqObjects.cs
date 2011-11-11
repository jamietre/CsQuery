using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Dynamic;
using Jtc.CsQuery;
using Jtc.CsQuery.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = Drintl.Testing.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;

namespace CsqueryTests.Csharp
{

    [TestFixture, TestClass, Description("CsQuery Tests (Not from Jquery test suite)")]
    public class CsqObjects : CsQueryTest
    {
        private static IDictionary<string, object> testDict = null;

        [TestFixtureSetUp,ClassInitialize]
        public static void Setup(TestContext context) {
            testDict = new Dictionary<string, object>();
            testDict.Add("Prop1", "value1");
            testDict.Add("Prop2", 999);
            DateTime timeNow = DateTime.Now;
            testDict.Add("Prop3", timeNow );
            IDictionary<string,object> subDict = new Dictionary<string,object>();
            testDict.Add("Prop4", subDict);
            subDict.Add("subProp1", "sub value 1");
            subDict.Add("subprop2", null);
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
        public void JsObject()
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
        protected long TestUseFunc(string key, object value)
        {
            return System.Convert.ToInt64(value);
        }
        
    }
}