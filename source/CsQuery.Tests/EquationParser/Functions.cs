using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using CollectionAssert = NUnit.Framework.CollectionAssert;
using StringAssert = NUnit.Framework.StringAssert;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;
using CsQuery.ExtensionMethods;
using CsQuery.StringScanner;
using CsQuery.EquationParser;
using CsQuery.EquationParser.Implementation;
using CsQuery.EquationParser.Implementation.Functions;

namespace CsQuery.Tests.EquationParser
{
    [TestClass,TestFixture]
    public class Functions
    {
        [Test,TestMethod]
        public void Power()
        {

            var square= Equations.CreateEquation<int>("x^2");
            Assert.AreEqual(9, square.GetValue(3));

            square = Equations.CreateEquation<int>("(x+1)^2");
            Assert.AreEqual(4, square.GetValue(1));

            square = Equations.CreateEquation<int>("3*(x+2)^2");
            Assert.AreEqual(48, square.GetValue(2));
            
        }

        [Test, TestMethod]
        public void Abs()
        {


            var abs= Equations.CreateEquation<int>("abs(x)");
            Assert.AreEqual(3, abs.GetValue(-3));
            Assert.AreEqual(3, abs.GetValue(3));



        }
       
    }
}



