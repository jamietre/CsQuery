using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery.Engine.FormulaParser
{
    public class IClause
    {
        IOperand OperandA;
        IOperand OperandB;
        IOperator Operator;
        object Calculate;
    }
    public class IClause<T>: IClause
    {
        T Calculate;
    }
}
