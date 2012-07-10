using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.Utility;

namespace CsQuery.Implementation
{
    /// <summary>
    /// Not included in the project - experiment to see how DOM performance is affected by implementing the dictionary differently
    /// </summary>
    public class DomAttributes : IDictionary<string, string>, IEnumerable<KeyValuePair<string, string>>
    {
        #region constructors
        //public DomAttributes(IDomElement owner)
        //{
        //    Owner = owner;
        //}
        public DomAttributes()
        {

        }
        #endregion

        #region private properties

        //protected IDomElement Owner;

        // private IDictionary<ushort, string> _Attributes;
        private List<ushort> _Keys;
        private List<string> _Values;

        //protected IDictionary<ushort, string> Attributes 
        //{
        //     get {
        //         if (_Attributes == null)
        //         {
        //             _Attributes = new Dictionary<ushort, string>();
        //         }
        //         return _Attributes;
        //     }
        //}
        protected void InitializeKvp()
        {
            _Keys = new List<ushort>();
            _Values = new List<string>();
        }
        protected List<ushort> InnerKeys
        {
            get
            {
                if (_Keys == null)
                {
                    InitializeKvp();
                }
                return _Keys;
            }
        }
        protected List<string> InnerValues
        {
            get
            {
                if (_Keys == null)
                {
                    InitializeKvp();
                }
                return _Values;
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
                return _Keys != null && Keys.Any();
            }
        }

        public int Count
        {
            //get { return Attributes.Count; }
            get { return InnerKeys.Count; }
        }

        #endregion

        #region public methods

        public void Clear()
        {
            //Attributes.Clear();
            InnerKeys.Clear();
            InnerValues.Clear();
        }

        public DomAttributes Clone()
        {
            DomAttributes clone = new DomAttributes();

            if (HasAttributes)
            {
                //foreach (string kvp in Attributes)
                //{
                //    clone.Attributes.Add(kvp,_Attributes[kvp]);
                //}
                for (int i = 0; i < InnerKeys.Count; i++)
                {
                    clone.InnerKeys.Add(InnerKeys[i]);
                    clone.InnerValues.Add(InnerValues[i]);
                }

                //clone.Attributes.Add(kvp.Key, kvp.Value);
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
            //return Attributes.ContainsKey(DomData.TokenID(key, true));
            return InnerKeys.Contains(DomData.TokenID(key, true));

        }
        public bool ContainsKey(ushort tokenId)
        {
            //return Attributes.ContainsKey(tokenId);
            return InnerKeys.Contains(tokenId);
        }
        public ICollection<string> Keys
        {
            get
            {
                List<string> keys = new List<string>();
                //foreach (var id in Attributes.Keys)
                //{
                //    keys.Add(DomData.TokenName(id).ToLower());
                //}
                //return keys;

                foreach (var id in InnerKeys)
                {
                    keys.Add(DomData.TokenName(id).ToLower());
                }
                return keys;
            }
        }
        public ICollection<string> Values
        {
            //get { return Attributes.Values; }
            get { return InnerValues; }
        }
        public bool TryGetValue(string key, out string value)
        {
            // do not use trygetvalue from dictionary. We need default handling in Get
            value = Get(key);
            return value != null ||
                //Attributes.ContainsKey(DomData.TokenID(key, true));
                InnerKeys.Contains(DomData.TokenID(key, true));
        }
        public bool TryGetValue(ushort key, out string value)
        {
            // do not use trygetvalue from dictionary. We need default handling in Get
            value = Get(key);
            return value != null ||
                //Attributes.ContainsKey(key);
                InnerKeys.Contains(key);
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
            return Get(DomData.TokenID(name, true));
        }
        protected string Get(ushort tokenId)
        {
            //string value;

            // if (Attributes.TryGetValue(tokenId, out value))
            //if (Attributes.TryGetValue(tokenId, out value))
            //{
            //    return value;
            //}
            //else
            //{
            //    return null;
            //}
            int index = InnerKeys.IndexOf(tokenId);
            return index < 0 ? null : InnerValues[index];


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
            //switch (tokenId)
            //{
            //    case DomData.IDAttrId:
            //        Owner.Id = value;
            //        return;
            //    case DomData.StyleNodeId:
            //        Owner.Style.SetStyles(value, false);
            //        return;
            //    case DomData.ClassAttrId:
            //        Owner.ClassName = value;
            //        return;
            //    case DomData.ValueAttrId:
            //        SetRaw(tokenId,value.CleanUp());
            //        break;
            //    default:
            //        // Uncheck any other radio buttons
            //        if (tokenId == DomData.CheckedAttrId
            //            && Owner.NodeName == "INPUT" 
            //            && Owner.Type == "radio" 
            //            && !String.IsNullOrEmpty(Owner["name"])
            //            && value!=null
            //            && Owner.Document != null)
            //        {
            //            var radios = Owner.Document.QuerySelectorAll("input[type='radio'][name='" + Owner["name"] + "']:checked");
            //            foreach (var item in radios)
            //            {
            //                item.Checked = false;
            //            }
            //        }

            //        SetRaw(tokenId,value);
            //        break;
            //}
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
                //Attributes[tokenId] = value;
                int index = InnerKeys.IndexOf(tokenId);
                if (index < 0)
                {
                    index = InnerKeys.Count;
                    InnerKeys.Add(tokenId);
                    InnerValues.Add(value);
                }
                else
                {
                    InnerValues[index] = value;
                }
            }
        }
        /// <summary>
        /// Sets a boolean only attribute having no value
        /// </summary>
        /// <param name="name"></param>
        public void SetBoolean(string name)
        {
            ushort tokenId = DomData.TokenID(name, true);

            SetBoolean(tokenId);
        }
        public void SetBoolean(ushort tokenId)
        {
            //Attributes[tokenId] = null;

            int index = InnerKeys.IndexOf(tokenId);
            if (index < 0)
            {
                index = InnerKeys.Count;
                InnerKeys.Add(tokenId);
                InnerValues.Add(null);
            }
            else
            {
                InnerValues[index] = null;
            }
        }
        /// <summary>
        /// Removing an attribute implementation
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Unset(string name)
        {
            return Unset(DomData.TokenID(name, true));
        }
        public bool Unset(ushort tokenId)
        {
            //return Attributes.Remove(tokenId);
            int index = InnerKeys.IndexOf(tokenId);
            if (index >= 0)
            {
                InnerKeys.RemoveAt(index);
                InnerValues.RemoveAt(index);
                return true;
            }
            return false;

        }


        protected IEnumerable<KeyValuePair<string, string>> GetAttributes()
        {
            //foreach (var kvp in Attributes)
            //{
            //    yield return new KeyValuePair<string, string>(DomData.TokenName(kvp.Key).ToLower(), kvp.Value);
            //}
            for (int i = 0; i < InnerKeys.Count; i++)
            {
                yield return new KeyValuePair<string, string>(DomData.TokenName(InnerKeys[i]).ToLower(), InnerValues[i]);
            }
        }
        internal IEnumerable<ushort> GetAttributeIds()
        {
            //return Attributes.Keys;
            return InnerKeys;
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
            //return ContainsKey(item.Key) 
            //&& Attributes[DomData.TokenID(item.Key,true)] == item.Value;

            int index = InnerKeys.IndexOf(DomData.TokenID(item.Key, true));
            if (index < 0)
            {
                return false;
            }
            else
            {
                return InnerValues[index] == item.Value;
            }
        }

        void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            //array = new KeyValuePair<string, string>[Attributes.Count];
            //int index = 0;
            //foreach (var kvp in Attributes)
            //{
            //    array[index++] = new KeyValuePair<string, string>(DomData.TokenName(kvp.Key), kvp.Value);
            //}

            array = new KeyValuePair<string, string>[InnerKeys.Count];
            for (int index = 0; index < InnerKeys.Count; index++)
            {

                array[index++] = new KeyValuePair<string, string>(DomData.TokenName(InnerKeys[index]), InnerValues[index]);
            }
        }

        bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
        {
            //if (ContainsKey(item.Key) 
            //    &&  Attributes[DomData.TokenID(item.Key,true)] == item.Value)
            //{
            //    return Remove(item.Key);
            //}
            //else
            //{
            //    return false;
            //}

            int index = InnerKeys.IndexOf(DomData.TokenID(item.Key, true));
            if (index < 0)
            {
                return false;
            }
            else
            {
                if (InnerValues[index] == item.Value)
                {
                    return Remove(item.Key);
                }
                else
                {
                    return false;
                }

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
