using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jtc.CsQuery.Utility;
using Jtc.CsQuery.Utility.EquationParser.Implementation;

namespace Jtc.CsQuery.Utility.EquationParser
{
    public static class Equations
    {
        public static Equation<T> CreateEquation<T>() where T : IConvertible
        {
            return new Equation<T>();
        }

        public static Equation<T> CreateEquation<T>(string text) where T : IConvertible
        {
            var equation = new Equation<T>();
            equation.Parse(text);
            return equation;
        }
        public static IOperand CreateOperand(IConvertible value)
        {
            if (value is IOperand)
            {
                return (IOperand)value;
            } else {
                return Literal.Create(value);
            }
        }
    }
}
