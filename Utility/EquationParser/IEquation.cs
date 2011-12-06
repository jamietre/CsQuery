using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jtc.CsQuery.Utility.EquationParser.Implementation;

namespace Jtc.CsQuery.Utility.EquationParser
{
    public interface IEquation: IOperand
    {
        void SetVariable(string name, IConvertible value);
        void SetVariable<T>(string name, T value) where T: IConvertible;
        IConvertible Calculate();
        IConvertible Calculate(params IConvertible[] values);
        bool TryCalculate(out object result);
        bool TryParse(string text);
        void Parse(string text);
    }
    public interface IEquation<T>: IOperand<T>, IEquation where T: IConvertible 
    {
        new T Calculate();
        new T Calculate(params IConvertible[] values);
        bool TryCalculate(out T result);
    }
}
