using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery.EquationParser;
using CsQuery.EquationParser.Implementation;
using CsQuery.EquationParser.Implementation.Functions;

namespace CsQuery.Tests.EquationParser
{
    [TestFixture, TestClass, Description("Math Expression Parser")]
    public class Utility
    {
        [Test, TestMethod]
        public void IsIntegralType()
        {
            Assert.IsTrue(Utils.IsIntegralType<int>(), "Int is integral");
            Assert.IsTrue(Utils.IsIntegralType<Int64>(), "Int64 is integral");
            Assert.IsTrue(Utils.IsIntegralType<ushort>(), "ushort is integral");
            Assert.IsTrue(Utils.IsIntegralType<bool>(), "Bool is integral");
            Assert.IsTrue(Utils.IsIntegralType<char>(), "Char is integral");
            Assert.IsFalse(Utils.IsIntegralType<double>(), "Double is not integral");
            Assert.IsFalse(Utils.IsIntegralType<string>(), "String is not integral");
            Assert.IsFalse(Utils.IsIntegralType<DateTime>(), "DateTime is not integral");
        }
        [Test, TestMethod]
        public void IsIntegralValue()
        {
            Assert.IsTrue(Utils.IsIntegralValue(10), "10 is integral");
            Assert.IsFalse(Utils.IsIntegralValue(10.2), "10.2 is not integral");
            Assert.IsTrue(Utils.IsIntegralValue(10.0), "10.0 is integral");
            Assert.IsTrue(Utils.IsIntegralValue((float)10.0), "10.0 is integral");
            Assert.IsTrue(Utils.IsIntegralValue(false), "boolean value is integral");
            
        }
        
    }
}

