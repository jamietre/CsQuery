using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Utility.EquationParser
{
    public enum AssociationType
    {
        Addition= 1,          // lowest level of assocation
        Multiplicaton =2,     // associate with others in group
        Function = 3            // never associate

    }
}
