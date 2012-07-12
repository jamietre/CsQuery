using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Engine
{
    public enum QuotingRule
    {
        NeverQuoted = 1,
        AlwaysQuoted = 2,
        OptionallyQuoted = 3
    }
}
