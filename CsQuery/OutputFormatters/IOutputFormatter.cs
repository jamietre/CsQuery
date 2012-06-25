using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery
{
    public interface IOutputFormatter
    {
        string Format(CQ selection);
    }
}
