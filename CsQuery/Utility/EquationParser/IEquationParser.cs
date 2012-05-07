using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Utility.EquationParser
{
    public interface IEquationParser
    {
        bool TryParse(string text, out IOperand operand);
        IOperand Parse(string text);
        IOperand Parse<T>(string text) where T : IConvertible;
        string Error { get; }
    }
    public interface IEquationParser<T> : IEquationParser where T : IConvertible
    {
        bool TryParse(string text, out IOperand<T> operand);
        new IOperand<T> Parse(string text);
    }
}
