using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Utility.EquationParser
{
    public interface ILiteral: IOperand
    {
        void Set(IConvertible value);
        new ILiteral Clone();
    }
    public interface ILiteral<T> : IOperand<T>, ILiteral where T : IConvertible
    {
        void Set(T value);
        new ILiteral<T> Clone();
    }
}
