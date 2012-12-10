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
    /// Ensure correct handling when mapping creating JSON from a dictionary
    /// </summary>
    [TestFixture, TestClass]
    public class FromDictionary
    {
        /// <summary>
        /// When mapping from a dictionary, the keys can be numeric. Make sure these values get quoted (per bug 6/5)
        /// </summary>
        [Test, TestMethod]
        public void StringProperties()
        {
            IDictionary<string, object> dict = new Dictionary<string, object>();

            dict["intprop"] = 1;
            string json = JSON.ToJSON(dict);
            Assert.AreEqual("{\"intprop\":1}",json);

            dict.Clear();
            dict["stringprop"] = "foo";
            json = JSON.ToJSON(dict);
            Assert.AreEqual("{\"stringprop\":\"foo\"}", json);

            dict.Clear();
            dict["arrayprop"] = new int[] { 1, 2, 3 };
            json = JSON.ToJSON(dict);
            Assert.AreEqual("{\"arrayprop\":[1,2,3]}", json);
            
            dict.Clear();
            dict["1"] = "bar";
            json = JSON.ToJSON(dict);
            Assert.AreEqual("{\"1\":\"bar\"}", json);

            dict.Clear();
            dict["false"] = "foobar";
            json = JSON.ToJSON(dict);
            Assert.AreEqual("{\"false\":\"foobar\"}", json);
        }

        [Test, TestMethod]
        public void NumericProperties()
        {
            IDictionary<int, object> dict = new Dictionary<int, object>();


            dict.Clear();
            dict[1] = "bar";
            string json = JSON.ToJSON(dict);
            Assert.AreEqual("{\"1\":\"bar\"}", json);

        }


    }
}