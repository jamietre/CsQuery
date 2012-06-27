using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Reflection;
using NUnit.Framework;

namespace CsQuery.Tests
{

    public static class AssertEx
    {

        public static void ObjectPropertiesAreEqual(object actual, object expected, string message=null)
        {
            if (!ObjectPropertiesAreEqual(actual, expected))
            {
                Assert.Fail(message);
            }
        }

        public static void ObjectPropertiesAreNotEqual(object actual, object expected, string message = null)
        {
            if (ObjectPropertiesAreEqual(actual,expected)) {
                Assert.Fail(message);
            }
        }

        private static bool ObjectPropertiesAreEqual(object actual, object expected)
        {
            IDictionary<string, object> actualDict = actual as IDictionary<string, object>;
            IDictionary<string, object> expectedDict = expected as IDictionary<string, object>;

            if (actualDict == null)
            {
                actualDict = CQ.ToExpando(actual);
            }
            if (expectedDict == null)
            {
                expectedDict = CQ.ToExpando(expected);
            }
            try
            {
                CollectionAssert.AreEqual(actualDict, expectedDict);
            }
            catch(AssertionException)
            {
                return false;
            }
            return true;
        }
        //public static void PropertyValuesAreEquals(object actual, object expected)
        //{
        //    PropertyInfo[] properties = expected.GetType().GetProperties();
        //    foreach (PropertyInfo property in properties)
        //    {
        //        object expectedValue = property.GetValue(expected, null);
        //        object actualValue = property.GetValue(actual, null);

        //        if (actualValue is IList)
        //            AssertListsAreEquals(property, (IList)actualValue, (IList)expectedValue);
        //        else if (!Equals(expectedValue, actualValue))
        //            Assert.Fail("Property {0}.{1} does not match. Expected: {2} but was: {3}", property.DeclaringType.Name, property.Name, expectedValue, actualValue);
        //    }
        //}

        private static void AssertListsAreEquals(PropertyInfo property, IList actualList, IList expectedList)
        {
            if (actualList.Count != expectedList.Count)
                Assert.Fail("Property {0}.{1} does not match. Expected IList containing {2} elements but was IList containing {3} elements", property.PropertyType.Name, property.Name, expectedList.Count, actualList.Count);

            for (int i = 0; i < actualList.Count; i++)
                if (!Equals(actualList[i], expectedList[i]))
                    Assert.Fail("Property {0}.{1} does not match. Expected IList with element {1} equals to {2} but was IList with element {1} equals to {3}", property.PropertyType.Name, property.Name, expectedList[i], actualList[i]);
        }
    }
    
}
