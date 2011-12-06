using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery.Utility.EquationParser
{
    public interface IOperator
    {
        void Set(string value);
        bool TrySet(string value);
        Operation Operation { get; }
        bool IsAssociative { get; }
    }

}
