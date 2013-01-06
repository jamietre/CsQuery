using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CsQuerySite;
using CsQuerySite.Helpers.XmlDoc;

namespace CsQuerySiteTests
{
    [TestClass]
    public class MethodInfoExtensionsTest
    {
       

        [TestMethod]
        public void Methods()
        {
            Type t = typeof(DocTestClass);
            MethodInfo mi = t.GetMethod("ReturnlessMethod");

            Assert.AreEqual("public void ReturnlessMethod()",mi.GetSignature());

            mi = t.GetMethod("StaticMethod");
            Assert.AreEqual("public static string StaticMethod()", mi.GetSignature());

            mi = t.GetMethod("GenericMethod");
            Assert.AreEqual("public T GenericMethod<T>(int parameter, T genericParameter)", mi.GetSignature());

            mi = t.GetMethod("ComplexGenericMethod");
            Assert.AreEqual("public Action<T> ComplexGenericMethod<T>(int param1, params object[] parmArray)", mi.GetSignature());

            mi = t.GetMethod("OptionalArgs");
            Assert.AreEqual("public void OptionalArgs(double required, double? optional = 2)", mi.GetSignature());

            mi = t.GetMethod("ReallyOdd");
            Assert.AreEqual("public Func<T,U>[] ReallyOdd<T, U>(short?[] nullableShortArray, U[] genericArray = null, params Func<U>[] optionals) where U: class, new()", mi.GetSignature());

            mi = t.GetMethod("ReallyOdd2");
            Assert.AreEqual("public ulong ReallyOdd2<T>(T input, out ITestInterface output) where T: ITestInterface, new()", mi.GetSignature());

        }
    }
}
