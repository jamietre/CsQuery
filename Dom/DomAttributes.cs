using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jtc.CsQuery.ExtensionMethods;

namespace Jtc.CsQuery
{
    public class DomAttributes: IDictionary<string, string>, IEnumerable<KeyValuePair<string, string>>
    {
        public DomAttributes(IDomElement owner)
        {
            Owner = owner;
        }
        public Action<string> SetStyle = null;
        public Action<string> SetClass = null;

        protected IDomElement Owner;
        //protected Dictionary<string, string> Attributes = new Dictionary<string, string>();
        protected Dictionary<short, string> Attributes = new Dictionary<short, string>();
        //public override string ToString()
        //{
        //    string attr = String.Empty;
        //    foreach (var kvp in Attributes)
        //    {
        //        attr += (style == String.Empty ? String.Empty : " ") + kvp.Key + ": " + kvp.Value + ";";
        //    }
        //    return style;
        //}

        public bool Remove(string name)
        {
            //return Attributes.Remove(name);
            return Attributes.Remove(DomData.TokenID(name));
        }
        public void Add(string name, string value)
        {
            this[name] = value;
        }
        public DomAttributes Clone(IDomElement owner)
        {
            DomAttributes clone = new DomAttributes(owner);
           //clone.Attributes = new Dictionary<string, string>();
            clone.Attributes = new Dictionary<short, string>();
            foreach (var kvp in Attributes)
            {
                clone.Attributes.Add(kvp.Key, kvp.Value);
            }
            return clone;
        }
        public string this[string name]
        {
            get
            {
                return Get(name);
            }
            set
            {
                Set(name, value);
            }
        }
        public string this[short nodeId]
        {
            get
            {
                return Get(nodeId);
            }
            set
            {
                Set(nodeId, value);
            }
        }
        private static HashSet<string> _emptyDefaults = new HashSet<string>(new string[] { "input", "option", "select" });
        protected string Get(string name)
        {
            return Get(DomData.TokenID(name));
        }
        protected string Get(short tokenID)
        {
            string value;
            //name = name.ToLower();
            //if (Attributes.TryGetValue(name, out value))
            if (tokenID == DomData.ClassAttrID || tokenID == DomData.StyleNodeId)
            {
                throw new Exception("You cannot access class or style as attributes, use className & Style");
            }
            if (Attributes.TryGetValue(tokenID, out value))
            {
                return value;
            }
            else
            {
                string defaultValue = null;

                //if (name.Equals("value", StringComparison.CurrentCultureIgnoreCase) &&
                //        (Owner.NodeName.IsOneOf("input", "option", "select")))
                //{
                //    defaultValue = String.Empty;
                //}
                return defaultValue;
            }
        }

        protected void Set(string name, string value)
        {
            Set(DomData.TokenID(name), value);

        }
        protected void Set(short tokenId, string value)
        {
            switch (tokenId)
            {
                case DomData.StyleNodeId:
                    SetStyle(value.CleanUp());
                    break;
                case DomData.ClassAttrID:
                    SetClass(value.CleanUp());
                    break;
                case DomData.ValueNodeId:
                    Attributes[tokenId] = value.CleanUp();
                    break;
                default:
                    Attributes[tokenId] = value;
                    break;
            }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _attributes().GetEnumerator();
        }
        protected IEnumerable<KeyValuePair<string, string>> _attributes()
        {
            //array = new KeyValuePair<string, string>[Attributes.Count];
            //int index = 0;
            foreach (var kvp in Attributes)
            {
                yield return new KeyValuePair<string, string>(DomData.TokenName(kvp.Key), kvp.Value);
            }

        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public bool ContainsKey(string key)
        {
            return Attributes.ContainsKey(DomData.TokenID(key));
        }

        public ICollection<string> Keys
        {
            get { 
                List<string> keys = new List<string>();
                foreach (var id in Attributes.Keys)
                {
                    keys.Add(DomData.TokenName(id));
                }
                return keys;
                //return Attributes.Keys; 
            }
        }

        public bool TryGetValue(string key, out string value)
        {
            // do not use trygetvalue from dictionary. We need default handling in Get
            value = Get(key);
            return value !=null ||
                Attributes.ContainsKey(DomData.TokenID(key));
        }
        public bool TryGetValue(short key, out string value)
        {
            // do not use trygetvalue from dictionary. We need default handling in Get
            value = Get(key);
            return value != null ||
                Attributes.ContainsKey(key);
        }
        public ICollection<string> Values
        {
            get { return Attributes.Values; }
        }


        public void Clear()
        {
            Attributes.Clear();
        }

        //public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        //{
        //    array = new KeyValuePair<string, string>[Styles.Count];
        //    int index = 0;
        //    foreach (var kvp in Styles)
        //    {
        //        array[index++] = kvp;
        //    }
        //}

        public int Count
        {
            get { return Attributes.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item)
        {
            Add(item.Key, item.Value);
        }


        bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item)
        {
            return ContainsKey(item.Key) 
                && Attributes[DomData.TokenID(item.Key)] == item.Value;
        }

        void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            array = new KeyValuePair<string, string>[Attributes.Count];
            int index = 0;
            foreach (var kvp in Attributes)
            {
                array[index++] = new KeyValuePair<string, string>(DomData.TokenName(kvp.Key), kvp.Value);
            }
        }

        bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
        {
            if (ContainsKey(item.Key) 
                &&  Attributes[DomData.TokenID(item.Key)] == item.Value)
            {
                return Remove(item.Key);
            }
            else
            {
                return false;
            }
        }

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
