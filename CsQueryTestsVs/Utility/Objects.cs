using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jtc.CsQuery;
using Jtc.CsQuery.Utility;

namespace Jtc.CsQuery.Tests
{
    [TestClass]
    public class _Objects
    {
        [TestMethod]
        public void IsTruthy()
        {
            Assert.IsTrue("x".IsTruthy(), "String is truthy");
            Assert.IsFalse("".IsTruthy(), "Empty string is not truthy");
            Assert.IsTrue(Objects.IsTruthy(1), "Positive number is truthy");
            Assert.IsTrue(Objects.IsTruthy(-1), "Negative number is truthy");
            Assert.IsFalse(Objects.IsTruthy(0), "Zero is not truthy");
            Assert.IsTrue(true.IsTruthy(), "True is truthy");
            Assert.IsFalse(false.IsTruthy(), "True is truthy");
            Assert.IsTrue(Objects.IsTruthy(new Object()), "Objects are truthy");
            Assert.IsTrue(Objects.IsTruthy(new string[] {}),"Arrays are truthy");
            Assert.IsFalse(Objects.IsTruthy(null), "Null is not truthy");
        }
    }
}
