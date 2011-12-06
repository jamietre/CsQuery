using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery.Utility.EquationParser
{
    public interface IEquationParser
    {
        bool TryParse(string text);
        void Parse(string text);
        IEnumerable<IVariable> Variables { get; }
        IOperand Equation { get; }
        string Error { get; }
    }
    public interface IEquationParser<T>: IEquationParser where T: IConvertible 
    {
        new IOperand<T> Equation { get; }
    }
}
