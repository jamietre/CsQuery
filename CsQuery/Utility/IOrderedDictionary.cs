using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Utility
{
    public interface IOrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IList<KeyValuePair<TKey, TValue>>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>> 
    {
        int IndexOf(TKey key);
    }
}
