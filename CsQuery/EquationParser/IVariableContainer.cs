using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.EquationParser
{
    public interface IVariableContainer
    {
        IEnumerable<IVariable> Variables {get;}
    }
}
