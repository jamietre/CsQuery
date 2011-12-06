using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery.Utility.EquationParser
{
    public interface IClause: IOperand
    {
        IOperand OperandA { get; set; }
        IOperand OperandB { get; set; }
        IOperator Operator { get; set; }
        IClause Chain(IOperand operandB, IOperator op);
    }
    public interface IClause<T> : IOperand<T>, IClause where T: IConvertible
    {
        new IClause<T> Chain(IOperand operandB, IOperator op);
    }
}
