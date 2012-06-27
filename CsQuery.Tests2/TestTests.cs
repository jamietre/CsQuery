using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute;
using MsTestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using TestContext = NUnit.Framework.TestContext;
using CsQuery;
using CsQuery.Utility;

namespace CsQuery.Tests
{
    /// <summary>
    /// Ensures that all methods are marked properly for both Visual Studio and NUnit. If anything is marked for one but not the other,
    /// the test will fail.
    /// </summary>
    [TestClass,TestFixture]
    public class _ValidateTestConfig
    {
        [Test, TestMethod]
        public void AnalyzeAndVerify()
        {
            var assem = Assembly.GetExecutingAssembly();
            foreach (var type in assem.GetTypes())
            {
                VerifyType(type);
            }
            Assert.AreEqual(0, Errors.Count, String.Join(System.Environment.NewLine, Errors));

        }

        // VS first, NUnit 2nd

        HashSet<Type> TestClassAttributes = new HashSet<Type> 
        {
            typeof(TestClassAttribute), typeof(TestFixtureAttribute)
        };

        HashSet<Type> TestMethodAttributes = new HashSet<Type> 
        {
            typeof(TestMethodAttribute), typeof(TestAttribute)
        };

        List<string> Errors = new List<string>();

        private void VerifyType(Type type)
        {
            if (VerifyClass(type))
            {
                VerifyMethods(type);
            }
        }

        /// <summary>
        /// Return true if this is a test class
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool VerifyClass(Type type)
        {
            HashSet<Type> hasAttributes = new HashSet<Type>();
            var typeAttributes = type.GetCustomAttributes(true).Select(item=>item.GetType());
            foreach (var attr in typeAttributes)
            {
                if (TestClassAttributes.Contains(attr))
                {
                    hasAttributes.Add(attr);
                }
            }

            if (hasAttributes.Count > 0 && hasAttributes.Count != 2)
            {
                bool ok = false;

                // The setup fixture needs to be marked as TestClass or MS will ignore the AssemblyInitialize, etc. methods. However 
                // it is not a regular NUnit test class either. So just ignore.
                if (typeAttributes.Contains(typeof(SetUpFixtureAttribute)))
                {
                    ok = true;
                }
                if (!ok)
                {
                    Errors.Add(String.Format("The class {0} had only one of two required attributes to identify a test class.", Support.TypePath(type)));
                }
            }
            return hasAttributes.Count == 2;
        }
        private void VerifyMethods(Type type)
        {

            foreach (var mi in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                VerifyMethod(mi);

            }

        }
        private void VerifyMethod(MethodInfo mi)
        {

            HashSet<Type> hasAttributes = new HashSet<Type>();
            foreach (var attrObj in mi.GetCustomAttributes(true))
            {
                Type attr = ((Attribute)attrObj).GetType();
                if (TestMethodAttributes.Contains(attr))
                {
                    hasAttributes.Add(attr);
                }
            }
            if (hasAttributes.Count > 0 && hasAttributes.Count != 2)
            {
                Errors.Add(String.Format("The method {0} had only one of two required attributes to identify a test method.", Support.MethodPath(mi)));
            }

        }
    }
}
