using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.HtmlParser;

namespace CsQuery.Implementation
{
    public class AttributeCollection : IDictionary<string, string>, IEnumerable<KeyValuePair<string, string>>
    {
        #region constructors

        public AttributeCollection()
        {

        }
        #endregion

        #region private properties


        private IDictionary<ushort, string> _Attributes;

        protected IDictionary<ushort, string> Attributes 
        {
            get
            {
                if (_Attributes == null)
                {
                    _Attributes = new Dictionary<ushort, string>();
                }
                return _Attributes;
            }
        }

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

        #endregion

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
            Attributes.Clear();
        }

        public AttributeCollection Clone()
        {
            AttributeCollection clone = new AttributeCollection();
           
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
        public bool Remove(ushort tokenId)
        {
            return Unset(tokenId);
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
            return Attributes.ContainsKey(HtmlData.TokenID(key));
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
                    keys.Add(HtmlData.TokenName(id).ToLower());
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
                Attributes.ContainsKey(HtmlData.TokenID(key));
        }
        public bool TryGetValue(ushort key, out string value)
        {
            // do not use trygetvalue from dictionary. We need default handling in Get
            value = Get(key);
            return value != null ||
                Attributes.ContainsKey(key);
        }

        #endregion

        #region private methods


        protected string Get(string name)
        {
            name = name.CleanUp();
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            return Get(HtmlData.TokenID(name));
        }
        protected string Get(ushort tokenId)
        {
            string value;

            if (Attributes.TryGetValue(tokenId, out value))
            {
                return value;
            }
            else
            {
                return null;
            }

        }
        /// <summary>
        /// Adding an attribute implementation
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        protected void Set(string name, string value)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Cannot set an attribute with no name.");
            }
            name = name.CleanUp();
            Set(HtmlData.TokenID(name), value);
        }
        /// <summary>
        /// Second to last line of defense -- will call back to owning Element for attempts to set class, style, or ID, which are 
        /// managed by Element.
        /// </summary>
        /// <param name="tokenId"></param>
        /// <param name="value"></param>
        protected void Set(ushort tokenId, string value)
        {
            SetRaw(tokenId, value);
        }
        /// <summary>
        /// Used by DomElement to (finally) set the ID value
        /// </summary>
        /// <param name="tokenId"></param>
        /// <param name="value"></param>
        internal void SetRaw(ushort tokenId, string value)
        {
            if (value == null)
            {
                Unset(tokenId);
            }
            else
            {
                Attributes[tokenId] = value;
            }
        }
        /// <summary>
        /// Sets a boolean only attribute having no value
        /// </summary>
        /// <param name="name"></param>
        public void SetBoolean(string name)
        {
            ushort tokenId = HtmlData.TokenID(name);

            SetBoolean(tokenId);
        }
        public void SetBoolean(ushort tokenId)
        {
            Attributes[tokenId] = null;
        }
        /// <summary>
        /// Removing an attribute implementation
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Unset(string name)
        {
            return Unset(HtmlData.TokenID(name));
        }
        public bool Unset(ushort tokenId)
        {
            bool result = Attributes.Remove(tokenId);
            //if (result)
            //{
            //    RemoveFromIndex(tokenId);
            //}
            return result;
        }


        protected IEnumerable<KeyValuePair<string, string>> GetAttributes()
        {
            foreach (var kvp in Attributes)
            {
                yield return new KeyValuePair<string, string>(HtmlData.TokenName(kvp.Key).ToLower(), kvp.Value);
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
                && Attributes[HtmlData.TokenID(item.Key)] == item.Value;
        }

        void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            array = new KeyValuePair<string, string>[Attributes.Count];
            int index = 0;
            foreach (var kvp in Attributes)
            {
                array[index++] = new KeyValuePair<string, string>(HtmlData.TokenName(kvp.Key), kvp.Value);
            }
        }

        bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
        {
            if (ContainsKey(item.Key)
                && Attributes[HtmlData.TokenID(item.Key)] == item.Value)
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
