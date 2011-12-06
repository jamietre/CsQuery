using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jtc.CsQuery.Utility.EquationParser.Implementation;

namespace Jtc.CsQuery.Utility.EquationParser
{
    public interface IVariable: IOperand
    {
        /// <summary>
        /// The name of this variable
        /// </summary>
        string Name { get; }
        void Clear();
        event EventHandler<VariableReadEventArgs> OnGetValue;
    }
    public interface IVariable<T> : IOperand<T>, IVariable where T: IConvertible
    {
        Type Type { get; }
        /// <summary>
        /// Release stored value for this variable, causing it to be re-read from the owner
        /// </summary>
        
    }
   
}
