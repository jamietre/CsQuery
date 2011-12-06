using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery.Utility.EquationParser
{
    public interface ILiteral: IOperand
    {
        void Set(IConvertible value);
    }
    public interface ILiteral<T>: IOperand<T>, ILiteral where T: IConvertible
    {
        void Set(T value);
    }
}
