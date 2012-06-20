using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.EquationParser
{
    public interface IOperation: IFunction
    {
        IList<OperationType> Operators { get; }
        void AddOperand(IConvertible operand, bool invert);
        /// <summary>
        /// Replaces the last item 
        /// </summary>
        /// <param name="operand"></param>
        
        void ReplaceLastOperand(IOperand operand);
    }

}
