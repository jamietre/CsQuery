using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jtc.CsQuery.Utility;

namespace Jtc.CsQuery.Utility.EquationParser.Implementation
{
    public static class Literal
    {
        public static ILiteral Create(IConvertible value)
        {
            if (Utils.IsText(value))
            {
                return new Literal<int>(value.ToString());
            }
            if (Utils.IsIntegralType(value))
            {
                return new Literal<int>(Convert.ToInt32(value));
            }
            else
            {
                return new Literal<double>(Convert.ToDouble(value));
            }
        }
    }

    public class Literal<T>: Operand<T>, ILiteral<T> where T: IConvertible 
    {
        public Literal(): base()
        {

        }
        public Literal(IConvertible value): base()
        {
            SetConvert(this,value);
        }

        public static implicit operator Literal<T>(int value)
        {
            return new Literal<T>(value);
        }
        public static implicit operator Literal<T>(double value)
        {
            return new Literal<T>(value);
        }
        public static implicit operator Literal<T>(string value)
        {
            return new Literal<T>(value);
        }
        public override string ToString()
        {
            return Value.ToString();
        }

        public void Set(T value)
        {
            Value = value;
        }

        private static void SetConvert(ILiteral literal, IConvertible value)
        {
            literal.Set((T)Convert.ChangeType(value, typeof(T)));
        }

        void ILiteral.Set(IConvertible value)
        {
            Set((T)Convert.ChangeType(value, typeof(T)));
        }
    }
}
