using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Utility
{
    /// <summary>
    /// A sorted dictionary that allows lookup by range.
    /// </summary>
    interface IRangeSortedDictionary<TValue> : IDictionary<string, TValue>
    {
        IEnumerable<string> GetRangeKeys(string subKey);
        IEnumerable<TValue> GetRange(string subKey);

    }
}
