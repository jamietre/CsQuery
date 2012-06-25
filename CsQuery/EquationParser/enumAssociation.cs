using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.EquationParser
{
    public enum AssociationType
    {
        Addition= 1,          // lowest level of assocation
        Multiplicaton =2,     // associate with others in group
        Power = 3,            // never associate, use adjacent operands
        Function = 4          // never associate, use parenthesized operands

    }
}
