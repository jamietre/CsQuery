using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.Utility.EquationParser.Implementation;

namespace CsQuery.Utility.EquationParser
{
    public interface IEquation : IOperand, IVariableContainer
    {
        IOrderedDictionary<string, IConvertible> VariableValues { get; }
        void SetVariable(string name, IConvertible value);
        void SetVariable<U>(string name, U value) where U : IConvertible;
        IConvertible GetValue(params IConvertible[] values);
        bool TryGetValue(out IConvertible result, params IConvertible[] values);
        bool TryGetValue(out IConvertible result);
        new IEquation Clone();
        IOperand Operand { get; set; }
        void Compile();
    }
    public interface IEquation<T> : IOperand<T>, IEquation where T : IConvertible
    {
        new T GetValue(params IConvertible[] values);
        bool TryGetValue(out T result);
        bool TryGetValue(out T result, params IConvertible[] values);
        new IEquation<T> Clone();
        IEquation<U> CloneAs<U>() where U : IConvertible;
    }
}
