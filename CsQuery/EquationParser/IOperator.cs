using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.EquationParser
{
    public interface IOperator: ICloneable 
    {
        void Set(string value);
        bool TrySet(string value);
        OperationType OperationType { get; }
        AssociationType AssociationType { get; }
        bool IsInverted { get; }
        new IOperator Clone();

        IOperation GetFunction();
    }
}
