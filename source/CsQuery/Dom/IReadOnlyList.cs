using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace CsQuery
{
    public interface IReadOnlyList<T> : IReadOnlyCollection<T>, IEnumerable<T>, IEnumerable
    {
        T this[int index] { get; }
    }
}
