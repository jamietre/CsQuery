using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.Utility.EquationParser.Implementation;

namespace CsQuery.Utility.EquationParser
{
    //TODO - unimplemented
    //Add/subtract/multiply etc should just be functions with two arguments. Operator should just
    // return a function.
    public interface IFunction: IOperand, IVariableContainer
    {
        /// <summary>
        /// The name of this variable
        /// </summary>
        string Name { get; }
        AssociationType AssociationType { get; }
        int RequiredParmCount { get; }
        int MaxParmCount { get; }
        IList<IOperand> Operands { get; }
        void AddOperand(IConvertible operand);
        void Compile();
    }
    /// <summary>
    /// T is the output type of the function.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFunction<T> : IOperand<T>, IFunction where T : IConvertible
    {
       
    }

}
