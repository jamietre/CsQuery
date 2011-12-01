using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jtc.CsQuery.ExtensionMethods;

namespace Jtc.CsQuery.Implementation
{
    public class DomAttributes: IDictionary<string, string>, IEnumerable<KeyValuePair<string, string>>
    {
        public DomAttributes(IDomElement owner)
        {
            Owner = owner;
        }
        protected IDomElement Owner;

        protected IDictionary<ushort, string> _Attributes;

        protected IDictionary<ushort, string> Attributes 
        {
             get {

                 if (_Attributes == null)
                 {
                     _Attributes = new SmallDictionary<ushort, string>();
                 }
                 return _Attributes;
             }
        }
        #region public properties
        public bool HasAttributes
        {
            get
            {
                return _Attributes != null && Attributes.Count > 0;
            }
        }

        public int Count
        {
            get { return Attributes.Count; }
        }

        #endregion
        #region public methods
        public void Clear()
        {
            foreach (var attrId in GetAttributeIds())
            {
                RemoveFromIndex(attrId);
            }
            Attributes.Clear();
        }

        public DomAttributes Clone(IDomElement owner)
        {
            DomAttributes clone = new DomAttributes(owner);
           
            if (HasAttributes)
            {
                foreach (var kvp in Attributes)
                {
                    clone.Attributes.Add(kvp.Key, kvp.Value);
                }
            }
            return clone;
        }

        public void Add(string name, string value)
        {
            Set(name, value);
        }

        public bool Remove(string name)
        {
            return Unset(name);
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
        public bool ContainsKey(string key)
        {
            return Attributes.ContainsKey(DomData.TokenID(key, true));
        }
        public bool ContainsKey(ushort tokenId)
        {
            return Attributes.ContainsKey(tokenId);
        }
        public ICollection<string> Keys
        {
            get
            {
                List<string> keys = new List<string>();
                foreach (var id in Attributes.Keys)
                {
                    keys.Add(DomData.TokenName(id));
                }
                return keys;
            }
        }
        public ICollection<string> Values
        {
            get { return Attributes.Values; }
        }
        public bool TryGetValue(string key, out string value)
        {
            // do not use trygetvalue from dictionary. We need default handling in Get
            value = Get(key);
            return value != null ||
                Attributes.ContainsKey(DomData.TokenID(key, true));
        }
        public bool TryGetValue(ushort key, out string value)
        {
            // do not use trygetvalue from dictionary. We need default handling in Get
            value = Get(key);
            return value != null ||
                Attributes.ContainsKey(key);
        }

        #endregion
        #region internal

        internal string this[ushort nodeId]
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
        
        protected string Get(string name)
        {
            name = name.CleanUp();
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            return Get(DomData.TokenID(name,true));
        }
        protected string Get(ushort tokenId)
        {
            string value;
            switch (tokenId)
            {
                case DomData.StyleAttrId:
                    value = Owner.Style.ToString();
                    break;
                case DomData.ClassAttrId:
                    value = Owner.ClassName;
                    break;
                default:
                    if (!Attributes.TryGetValue(tokenId, out value))
                    {
                        value=null;
                    }
                        break;
            }
            return value;
        }
        /// <summary>
        /// Adding an attribute implementation
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        protected void Set(string name, string value)
        {
            name = name.CleanUp();
            if (String.IsNullOrEmpty(name))
            {
                throw new Exception("Cannot set an attribute with no name.");
            }
            Set(DomData.TokenID(name, true), value);
        }
        /// <summary>
        /// Second to last line of defense -- will call back to owning Element for attempts to set class, style, or ID, which are 
        /// managed by Element.
        /// </summary>
        /// <param name="tokenId"></param>
        /// <param name="value"></param>
        protected void Set(ushort tokenId, string value)
        {
            bool addToIndex = false;
            switch (tokenId)
            {
                case DomData.IDAttrId:
                    Owner.ID = value;
                    return;
                case DomData.StyleAttrId:
                    Owner.Style.SetStyles(value, false);
                    return;
                case DomData.ClassAttrId:
                    Owner.ClassName = value;
                    return;
                case DomData.ValueAttrId:
                    addToIndex = !Attributes.ContainsKey(tokenId);
                    SetRaw(tokenId,value.CleanUp());
                    break;
                default:
                    addToIndex = !Attributes.ContainsKey(tokenId);
                    SetRaw(tokenId,value);
                    break;
            }
            if (addToIndex)
            {
                AddToIndex(tokenId);
            }
        }
        /// <summary>
        /// Used by DomElement to (finally) set the ID value
        /// </summary>
        /// <param name="tokenId"></param>
        /// <param name="value"></param>
        internal void SetRaw(ushort tokenId, string value ) {
            Attributes[tokenId]=value;
        }
           
        /// <summary>
        /// Removing an attribute implementation
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected bool Unset(string name)
        {
            ushort tokenId = DomData.TokenID(name, true);
            bool result = Attributes.Remove(tokenId);
            if (result)
            {
                RemoveFromIndex(tokenId);
            }
            return result;
        }

        protected void RemoveFromIndex(ushort attrId)
        {
            if (!Owner.IsDisconnected)
            {
                Owner.Document.RemoveFromIndex(IndexKey(attrId));
            }
        }
        protected void AddToIndex(ushort attrId)
        {
            if (!Owner.IsDisconnected)
            {
                Owner.Document.AddToIndex(IndexKey(attrId), Owner);
            }
        }
        protected string IndexKey(ushort attrId)
        {
#if DEBUG_PATH
            return "!" + DomData.TokenName(attrId) + DomData.indexSeparator + Owner.Path;
#else
            return "!" + (char)attrId + DomData.indexSeparator + Owner.Path;
#endif
        }
        protected IEnumerable<KeyValuePair<string, string>> GetAttributes()
        {
            foreach (var kvp in Attributes)
            {
                yield return new KeyValuePair<string, string>(DomData.TokenName(kvp.Key), kvp.Value);
            }
        }
        internal IEnumerable<ushort> GetAttributeIds()
        {
            return Attributes.Keys;
        }

        #endregion
        #region interface implementation


        bool ICollection<KeyValuePair<string, string>>.IsReadOnly
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
                && Attributes[DomData.TokenID(item.Key,true)] == item.Value;
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
                &&  Attributes[DomData.TokenID(item.Key,true)] == item.Value)
            {
                return Remove(item.Key);
            }
            else
            {
                return false;
            }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return GetAttributes().GetEnumerator();
        }


        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
