 using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using Jtc.CsQuery.Utility;


namespace Jtc.CsQuery
{
    
    public class JsObjectUndefined
    {
    }
    public class JsObjectDefault
    {
     
    }
    public class JsObject  : DynamicObject, IDictionary<string, object>, IEnumerable<KeyValuePair<string, object>>, IEnumerable
    {

        public JsObject()
        {
            Initialize();
        }
        public JsObject(object missingPropertyValue)
        {
            Initialize();
            MissingPropertyValue = missingPropertyValue;
        }

        protected void Initialize()
        {
            AllowMissingProperties = true;
            MissingPropertyValue = null;
            IgnoreCase = true;
            InnerProperties = new Dictionary<string, object>();
        }
        public override string ToString()
        {
            return CsQuery.ToJSON(this);
        }
        public IEnumerable<T> Enumerate<T>()
        {
            return CsQuery.Enumerate<T>(this);
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

        protected IDictionary<string, string> NameXref
        {
            get
            {
                return _NameXref.Value;
            }
        }
        private Lazy<Dictionary<string, string>> _NameXref = new Lazy<Dictionary<string, string>>();
        
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
            name = getInnerName(name);
            bool success = String.IsNullOrEmpty(name) ? false : InnerProperties.TryGetValue(name, out value);
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
                    throw new Exception("There is no property named \"" + name + "\".");
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
        /// <summary>
        /// Translate name based on case-sensititity settings
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected string getInnerName(string name)
        {
            if (IgnoreCase)
            {
                string realName;
                if (!NameXref.TryGetValue(name.ToLower(), out realName))
                {
                    NameXref[name.ToLower()] = name;
                }
                else
                {
                    name = realName;
                }
            }
            return name;

        }
        protected bool TrySetMember(string name, object value)
        {
            try
            {
                name = getInnerName(name);
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
            return InnerProperties.ContainsKey(getInnerName(name));
        }
        public bool Delete(string name)
        {
            return InnerProperties.Remove(getInnerName(name));
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

        //public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        //{
        //    if (binder.Operation == System.Linq.Expressions.ExpressionType.Equal)
        //    {
        //        JsObject compare = arg.ToExpando();
        //        return this.Equals(arg);
        //    }
        //    else
        //    {
        //        return base.TryBinaryOperation(binder, arg, out result);
        //    }
            
        //}
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return InnerProperties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #region Static values
        public static JsObjectUndefined Undefined
        {
            get
            {
                return new JsObjectUndefined();
            }
        }
        public static JsObjectDefault Default
        {
            get
            {
                return new JsObjectDefault();
            }
        }
        #endregion
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
