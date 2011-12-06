using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using Jtc.CsQuery.Utility.EquationParser;
using Jtc.CsQuery.Utility.EquationParser.Implementation;

namespace CsqueryTests.Csharp
{
    [TestFixture, TestClass, Description("Math Expression Parser")]
    public class Expressions
    {
        [Test, TestMethod]
        public void Utility_()
        {
            Assert.IsTrue(Utils.IsIntegralType<int>(), "Int is integral");
            Assert.IsTrue(Utils.IsIntegralType<Int64>(), "Int64 is integral");
            Assert.IsTrue(Utils.IsIntegralType<ushort>(), "ushort is integral");
            Assert.IsTrue(Utils.IsIntegralType<bool>(), "Bool is integral");
            Assert.IsTrue(Utils.IsIntegralType<char>(), "Char is integral");
            Assert.IsFalse(Utils.IsIntegralType<double>(), "Double is not integral");
            Assert.IsFalse(Utils.IsIntegralType<string>(), "String is not integral");
            Assert.IsFalse(Utils.IsIntegralType<DateTime>(), "DateTime is not integral");
            
            Assert.IsTrue(Utils.IsIntegralValue(10), "10 is integral");
            Assert.IsFalse(Utils.IsIntegralValue(10.2), "10.2 is not integral");
            Assert.IsTrue(Utils.IsIntegralValue(10.0), "10.0 is integral");
            Assert.IsTrue(Utils.IsIntegralValue((float)10.0), "10.0 is integral");
            Assert.IsTrue(Utils.IsIntegralValue(false), "boolean value is integral");
            
        }
        [Test, TestMethod]
        public void Literal_()
        {
            // test implplicit overload
            Literal<int> literal = 10;

            Assert.AreEqual(10, literal.Value, "Int literal");
            Assert.AreEqual(true, literal.IsInteger, "Reports being an integer");

            ILiteral litI = literal;
            litI.Set(10.5);
            Assert.AreEqual(10, literal.Value, "Int literal converts doublt to int");

            litI = new Literal<double>();

            litI.Set(10.5);
            Assert.AreEqual(10.5, litI.Value, "Double literal returns double");
            Assert.AreEqual(false, litI.IsInteger, "Double doesn't report being an integer");

            Assert.AreEqual("10.5", litI.ToString(), "ToString works");
        }

        protected void GetVariableValue(object sender, VariableReadEventArgs e)
        {
            e.Value = xVal;
        }
        protected int xVal = 0;
        
        [Test,TestMethod]
        public void Variable_()
        {
            IVariable<int> variable = new Variable<int>("x");

            try
            {
                var x = variable.Value;
                Assert.Fail("Can't get value from unbound variable");
            }
            catch { }

            variable.OnGetValue += new EventHandler<VariableReadEventArgs>(GetVariableValue);
            xVal = 22;

            Assert.AreEqual(22, variable.Value, "Variable got correct value");
            xVal = 123;
            Assert.AreEqual(22, variable.Value, "Variable caches value");
            variable.Clear();
            Assert.AreEqual(123, variable.Value, "Variable got correct value after clearing cache");

            Assert.AreEqual("x", variable.ToString(), "ToString works");
        }

        [Test, TestMethod]
        public void Clause_()
        {
            Literal<int> op1 =25;
            Literal<double> op2 = 55.2;

            var exp1 = new Clause<double>(op1, op2, "+");
            Assert.AreEqual(80.2, exp1.Value, "Double expression evaluated properly");
            Assert.AreEqual("25+55.2", exp1.ToString(), "Non-associative ToString works");

            var exp2 = new Clause<int>(op2, op1, "-");
            Assert.AreEqual(30, exp2.Value, "Int expression evaluated properly");

            var exp3 = new Clause<int>(12, 20, "*");
            Assert.AreEqual(exp3.Value, 240, "Implicit creating of clause and multiplication worked");
            Assert.AreEqual("12*20", exp3.ToString(), "Associative ToString works");

            var exp4 = new Clause<int>(92, exp2, new Operator("/"));

            Assert.AreEqual(3, exp4.Value, "Nesting clauses and division work");
            Assert.AreEqual("92/(55.2-25)",exp4.ToString(), "Nested ToString works");
            var exp5 = new Clause<double>(exp2, 2.5, "*");
            Assert.AreEqual(75, exp5.Value, "Another nesting test");

            

        }
    }
}
