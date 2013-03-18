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
    public class Extend_
    {

        [Test,TestMethod]
        public void ExtendExpando()
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
        }

        [Test, TestMethod]
        public void ExtendPoco()
        {

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


        [Test, TestMethod]
        public void ExtendPocoFromDynamic()
        {

            TestClass1 t1 = new TestClass1();
            t1.Prop1 = "value1";
            t1.Prop2 = "value2";

            dynamic t2 = new JsObject();
            t2.Prop2 = "class2value2";
            t2.Prop3 = "class2vlaue3";

            CQ.Extend(t1, t2);

            Assert.AreEqual("value1", t1.Prop1, "Target prop1 unchanged");
            Assert.AreEqual("class2value2", t1.Prop2, "Target prop2 updated");

        }
        
        [Test,TestMethod]
        public void ExtendEnumerable()
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


        #region Test data structures

        protected class TestExpando
        {
            public string Property1 { get; set; }
            public string Field1;
        }

        protected class TestClass1
        {
            public string Prop1 { get; set; }
            public string Prop2 { get; set; }
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