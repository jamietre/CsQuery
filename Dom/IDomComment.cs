using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery
{
    public interface IDomComment : IDomSpecialElement
    {
        bool IsQuoted { get; set; }
    }
}
