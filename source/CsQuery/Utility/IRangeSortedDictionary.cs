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
        /// <summary>
        /// Return all keys starting with subKey
        /// </summary>
        /// <param name="subKey">The substring to match</param>
        /// <returns></returns>
        IEnumerable<string> GetRangeKeys(string subKey);

        /// <summary>
        /// Return all values having keys beginning with subKey
        /// </summary>
        /// <param name="subKey"></param>
        /// <returns></returns>
        IEnumerable<TValue> GetRange(string subKey);
    }
}
