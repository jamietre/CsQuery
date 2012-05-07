using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.Utility.EquationParser.Implementation;

namespace CsQuery.Utility.EquationParser
{
    public interface IVariable : IOperand, IVariableContainer
    {
        /// <summary>
        /// The name of this variable
        /// </summary>
        string Name { get; }
       // void Clear();
        event EventHandler<VariableReadEventArgs> OnGetValue;
        new IVariable Clone();
    }
    public interface IVariable<T> : IOperand<T>, IVariable where T : IConvertible
    {
        Type Type { get; }
        /// <summary>
        /// Release stored value for this variable, causing it to be re-read from the owner
        /// </summary>
        new IVariable<T> Clone();
        //IVariable<U> CloneAs<U>() where U : IConvertible;
        new T Value { get; set; }
    }
   
}
