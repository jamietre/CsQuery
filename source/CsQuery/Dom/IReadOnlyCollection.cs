using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace CsQuery
{
    public interface IReadOnlyCollection<T> : IEnumerable<T>, IEnumerable
    {
        int Count {get;}
    }
}
