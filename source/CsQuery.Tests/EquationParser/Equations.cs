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
    public class Equations_
    {

        [Test,TestMethod]
        public void Basic()
        {


            var eq = Equations.CreateEquation<int>("2+abs(x)");
            Assert.AreEqual("2+abs(x)", eq.ToString(), "Equation looks like it got parsed");
            Assert.AreEqual(6, eq.GetValue(3.5));

            var eq2 = Equations.CreateEquation<double>("2+abs(x)");
            Assert.AreEqual(5.5, eq2.GetValue(3.5));
        }

        [Test, TestMethod]
        public void LeadingSigns()
        {


            var eq = Equations.CreateEquation<int>("+2");
            Assert.AreEqual("2", eq.ToString(), "Equation looks like it got parsed");
            Assert.AreEqual(2, eq.GetValue());

            eq = Equations.CreateEquation<int>("-n+1");
            Assert.AreEqual("-1*n+1", eq.ToString(), "Equation looks like it got parsed");
            Assert.AreEqual(-2, eq.GetValue(3));

            eq = Equations.CreateEquation<int>("n+1");
            Assert.AreEqual("n+1", eq.ToString(), "Equation looks like it got parsed");
            Assert.AreEqual(4, eq.GetValue(3));

            eq = Equations.CreateEquation<int>("-1n+1");
            Assert.AreEqual("-1*n+1", eq.ToString(), "Equation looks like it got parsed");
            Assert.AreEqual(-2, eq.GetValue(3));
        }

        [Test, TestMethod]
        public void Clone_()
        {

            //var variable = Equations.CreateVariable<int>("x");
            var variable = Equations.CreateVariable("x");

            var lit = Equations.CreateLiteral<int>("120");
            //var clause = Equations.CreateClause<int>(lit, variable, "-");
            var clause = new Difference(lit, variable);

            var exp = Equations.CreateEquation<int>(clause);
            exp.SetVariable("x", 5);
            Assert.AreEqual(115, exp.Value, "Built an equation by hand and it worked");

            //clause.Operator = Equations.CreateOperator("*");

            //Assert.AreEqual(600, exp.Value, "Built an equation by hand and it worked");

            var litClone = lit.Clone();
            Assert.AreEqual(120, litClone.Value, "Literal cloned ok");
            var clauseClone = clause.Clone();
            var cloneVars = clauseClone.Variables.FirstOrDefault();

            Assert.IsTrue(cloneVars != null, "Clone has variables");

            // 120 - x + 3
            clauseClone.AddOperand(3);
            var newExp = Equations.CreateEquation<int>(clauseClone);
            newExp.SetVariable("x", 2);

            Assert.AreEqual(121, newExp.Value, "Cloned & chained worked");
            Assert.AreEqual(115, exp.Value, "Original unaffected");

            var exp3 = Equations.CreateEquation<double>("10/(2*x+3-y*1.5*(3+5))+22.5");
            Assert.AreEqual(10 / (2 * 5 + 3 - 3 * 1.5 * (3 + 5)) + 22.5, exp3.GetValue(5, 3), "long equation");
        }
    }
}



