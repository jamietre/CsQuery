 using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using CsQuery.Utility;


namespace CsQuery
{
 
    /// <summary>
    /// A dynamic object implementation that differs from ExpandoObject in two ways:
    /// 
    /// 1) Missing property values always return null (or a specified value)
    /// 2) Allows case-insensitivity
    /// 
    /// </summary>
    public class JsObject  : DynamicObject, IDictionary<string, object>, IEnumerable<KeyValuePair<string, object>>, IEnumerable
    {
        public JsObject()
        {
            Initialize(null, null);
        }
        public JsObject(StringComparer comparer = null,object missingPropertyValue=null)
        {
            Initialize(comparer,missingPropertyValue);

        }

        protected void Initialize(StringComparer comparer, object missingPropertyValue)
        {
            AllowMissingProperties = true;
            MissingPropertyValue = missingPropertyValue;
            InnerProperties = new Dictionary<string, object>(comparer ?? StringComparer.OrdinalIgnoreCase);
        }
        public override string ToString()
        {
            return CQ.ToJSON(this);
        }
        public IEnumerable<T> Enumerate<T>()
        {
            return CQ.Enumerate<T>(this);
        }
        protected bool AllowMissingProperties
        {
            get;
            set;
        }
        protected object MissingPropertyValue
        {
            get;
            set;
        }
        public bool IgnoreCase
        {
            get;
            set;
        }
        public IDictionary<string, object> InnerProperties
        {
            get;
            protected set;
        }
        
        public object this[string name]
        {
            get
            {
                object result;
                TryGetMember(name, typeof(object), out result);
                return result;
            }
            set
            {
                TrySetMember(name, value);
            }
        }
        public T Get<T>(string name)
        {
            object value;
            TryGetMember(name, typeof(T), out value);
            return (T)value;
        }
        /// <summary>
        /// Try to return a li
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<T> GetList<T>(string name)
        {
            IEnumerable list = Get(name) as IEnumerable;
            if (list != null)
            {
                foreach (object item in list)
                {
                    yield return (T)item;
                }
            }
            else
            {
                throw new ArgumentException("The property '" + name + "' is not an array.");
            }
        }
        public object Get(string name)
        {
            object value;
            TryGetMember(name, typeof(object), out value);
            return value;

        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return TryGetMember(binder.Name, binder.ReturnType, out result);
        }
        protected bool TryGetMember(string name, Type type, out object result)
        {
            object value = null;
            bool success = String.IsNullOrEmpty(name) ?
                false : 
                InnerProperties.TryGetValue(name, out value);

            if (!success)
            {
                if (AllowMissingProperties)
                {
                    if (type == typeof(object))
                    {
                        result = MissingPropertyValue;
                    }
                    else
                    {
                        result = Objects.DefaultValue(type);
                    }
                    success = true;
                }
                else
                {
                    throw new KeyNotFoundException("There is no property named \"" + name + "\".");
                }
            }
            else
            {
                if (type == typeof(object))
                {
                    result = value;
                }
                else
                {
                    result = Objects.Convert(value, type);
                }

            }
            return success;
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return TrySetMember(binder.Name, value);
            
        }

        protected bool TrySetMember(string name, object value)
        {
            try
            {
                if (String.IsNullOrEmpty(name))
                {
                    return false;
                }

                if (value is IDictionary<string, object> && !(value is JsObject))
                {
                    InnerProperties[name] = ToJsObject((IDictionary<string, object>)value);
                }
                else
                {
                    InnerProperties[name] = value;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        public bool HasProperty(string name)
        {
            return InnerProperties.ContainsKey(name);
        }
        public bool Delete(string name)
        {
            return InnerProperties.Remove(name);
        }

        protected JsObject ToJsObject(IDictionary<string, object> value)
        {
            JsObject obj = new JsObject();
            foreach (KeyValuePair<string, object> kvp in value)
            {
                obj[kvp.Key] = kvp.Value;
            }
            return obj;
        }
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return InnerProperties.Keys;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return InnerProperties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        #region explicit interface members

        void IDictionary<string, object>.Add(string key, object value)
        {
            TrySetMember(key, value);
        }

        bool IDictionary<string, object>.ContainsKey(string key)
        {
            return InnerProperties.ContainsKey(key);
        }

        ICollection<string> IDictionary<string, object>.Keys
        {
            get { return InnerProperties.Keys; }
        }

        bool IDictionary<string, object>.Remove(string key)
        {
            return InnerProperties.Remove(key);
        }

        bool IDictionary<string, object>.TryGetValue(string key, out object value)
        {
            if (HasProperty(key))
            {
                return TryGetMember(key, typeof(object), out value);
            }
            else
            {
                value = null;
                return false;
            }
        }

        ICollection<object> IDictionary<string, object>.Values
        {
            get { return InnerProperties.Values; }
        }

        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
        {
            TrySetMember(item.Key, item.Value);
        }

        void ICollection<KeyValuePair<string, object>>.Clear()
        {
            InnerProperties.Clear();
        }

        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
        {
            return InnerProperties.Contains(item);
        }

        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            InnerProperties.CopyTo(array, arrayIndex);
        }

        int ICollection<KeyValuePair<string, object>>.Count
        {
            get { return InnerProperties.Count; }
        }

        bool ICollection<KeyValuePair<string, object>>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
        {
            return InnerProperties.Remove(item);
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

    }
}
