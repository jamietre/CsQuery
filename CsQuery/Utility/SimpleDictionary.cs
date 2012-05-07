using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace CsQuery.Utility
{
    /// <summary>
    /// Just convers a NameValueCollection to a disctionary with few methods
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleDictionary<T> where T : class
    {
        public SimpleDictionary(NameValueCollection dataSource)
        {
            DataSource = dataSource;

        }
        protected NameValueCollection DataSource;
        protected Dictionary<string, T> InnerDict
        {
            get
            {
                if (_InnerDict == null)
                {
                    _InnerDict = new Dictionary<string, T>();
                }
                return _InnerDict;
            }
        } protected Dictionary<string, T> _InnerDict = new Dictionary<string, T>();
        
        public bool TryGetValue(string key, out T value)
        {
            T storedValue;
            if (InnerDict.TryGetValue(key, out storedValue))
            {
                value = storedValue;
                return true;
            }
            else
            {
                string sourceValue = DataSource[key];
                if (sourceValue != null)
                {
                    _InnerDict.Add(key, sourceValue as T);
                    value = sourceValue as T;
                    return true;
                }
            }
            value = default(T);
            return false;
        }
        public T GetValueOrDefault(string key)
        {
            return GetValueOrDefault(key, default(T));
        }
        public T GetValueOrDefault(string key, T defaultValue)
        {
            T value;
            if (TryGetValue(key, out value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
