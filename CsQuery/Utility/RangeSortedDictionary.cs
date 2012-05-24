using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.ExtensionMethods;

namespace CsQuery.Utility
{
    /// <summary>
    /// A dictionary that is substring-lookup capable. This is the data structure used to index HTML documents for selectors.
    /// A SortedSet of keys is used for the index because it allows fast access by substring. A list of keys obtained from the
    /// SortedSet for a selector is used to obtain the target references from a regular dictionary.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class RangeSortedDictionary<TValue> : IRangeSortedDictionary<TValue>
    {
        #region constructor

        public RangeSortedDictionary()
        {
            Keys = new SortedSet<string>(StringComparer.Ordinal);
        }

        #endregion

        #region private properties

        protected SortedSet<string> Keys;
        protected Dictionary<string,TValue> Index = new Dictionary<string,TValue>();

        #endregion

        #region public properties

        public IEnumerable<string> GetRangeKeys(string subkey)
        {
            if (string.IsNullOrEmpty(subkey)) {
                yield break;
            }
            string lastKey = subkey.Substring(0,subkey.Length - 1) + Convert.ToChar(Convert.ToInt32(subkey[subkey.Length - 1]) + 1);
            
            foreach (var key in Keys.GetViewBetween(subkey, lastKey))
            {
                if (key != lastKey)
                {
                    yield return key;
                }
            }
        }

        /// <summary>
        /// Return only keys at depth. Zero is the matching key.
        /// </summary>
        /// <param name="subKey"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public IEnumerable<TValue> GetRange(string subKey, int depth, bool descendants)
        {
            if (depth == 0 && !descendants)
            {
                if (Index.ContainsKey(subKey))
                {
                    yield return Index[subKey];
                }
                else
                {
                    yield break;
                }
            }
            else
            {
                int len = subKey.Length;
                int curDepth=0;
                foreach (var key in GetRangeKeys(subKey))
                {
                    if (key.Length > len)
                    {
                        {
#if DEBUG_PATH
                            curDepth = (key.Length - len) / DomData.pathIdLength;
#else
                            curDepth = key.Length - len;
#endif
                        }
                    }
                    if (curDepth == depth || (descendants && curDepth >= depth))
                    {
                        yield return Index[key];
                    }
                }
            }
        }

        public IEnumerable<TValue> GetRange(string subKey)
        {
            foreach (var key in GetRangeKeys(subKey))
            {
                yield return Index[key];
            }
        }

        #endregion

        #region IDictionary<string,TValue> Members

        public void Add(string key, TValue value)
        {
            if (Keys.Add(key))
            {
                Index.Add(key, value);
            }
        }

        public bool ContainsKey(string key)
        {
            return Keys.Contains(key);
        }

        ICollection<string> IDictionary<string, TValue>.Keys
        {
            get { return Keys; }
        }

        public bool Remove(string key)
        {
            if (Keys.Remove(key))
            {
                Index.Remove(key);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryGetValue(string key, out TValue value)
        {
            return Index.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get {
                return Values;
            }
        }

        public TValue this[string key]
        {
            get
            {
                return Index[key];
            }
            set
            {
                Index[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<string,TValue>> Members

        public void Add(KeyValuePair<string, TValue> item)
        {
            Add(item.Key, item.Value);
        }
        public void Clear()
        {
            Keys.Clear();
            Index.Clear();
        }

        public bool Contains(KeyValuePair<string, TValue> item)
        {
            return Index.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, TValue>[] array, int arrayIndex)
        {
            foreach (var kvp in this)
            {
                array[arrayIndex++] = kvp;
            }
        }

        public int Count
        {
            get { return Index.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, TValue> item)
        {
            return Remove(item.Key);
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,TValue>> Members

        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            // Don't use dictionary enumerator - return values in sorted order when enumerating over entire object
            foreach (var key in Keys)
            {
                yield return new KeyValuePair<string, TValue>(key, Index[key]);
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
 
    }
}
