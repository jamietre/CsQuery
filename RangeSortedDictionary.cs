using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery
{
    /// <summary>
    /// A sorted dictionary that allows lookup by range.
    /// </summary>
    interface IRangeSortedDictionary<TValue>: IDictionary<string, TValue>
    {
        IEnumerable<string> GetRangeKeys(string subKey);
        IEnumerable<TValue> GetRange(string subKey);

    }
    /*
     * .class/body/div
     * .class/body/a/span/element
     * #id/body/div/span/element
     * 
     * search for .class#id
     * 1) return all .class elements
     * 2) serach #id/body/div
     */
    public class RangeSortedDictionary<TValue> : IRangeSortedDictionary<TValue>
    {
        public RangeSortedDictionary()
        {
            Keys = new SortedSet<string>(StringComparer.Ordinal);
        }
        protected SortedSet<string> Keys;
        protected Dictionary<string,TValue> Index = new Dictionary<string,TValue>();
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

        public IEnumerable<TValue> GetRange(string subKey)
        {
            foreach (var key in GetRangeKeys(subKey))
            {
                yield return Index[key];
            }
        }
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
            return false;
        }

        public bool TryGetValue(string key, out TValue value)
        {
            return Index.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get { throw new NotImplementedException(); }
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
