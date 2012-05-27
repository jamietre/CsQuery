using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Xml;
using CsQuery.Utility;
using CsQuery.Utility.StringScanner;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.Implementation
{
     
    public class CSSStyleDeclaration : IDictionary<string, string>, IEnumerable<KeyValuePair<string, string>>, ICSSStyleDeclaration
    {
        #region constructors
        public CSSStyleDeclaration(IDomElement owner)
        {
            Owner = owner;
        }
        #endregion
        
        #region private properties
        protected IDomElement Owner;
        /// <summary>
        /// Warning: Do not attempt to access _Styles directly from this class or any subclass to determine whether or 
        /// not there are styles, since it also depends on QuickSetStyles. Use HasStyles method instead.
        /// </summary>
        private IDictionary<ushort, string> _Styles;
        protected string _QuickSetValue;

        protected IDictionary<ushort, string> Styles
        {
            get
            {
                if (_Styles == null)
                {
                    _Styles = new SmallDictionary<ushort, string>();
                    if (QuickSetValue != null)
                    {
                        AddStyles(QuickSetValue, false);
                        QuickSetValue = null;
                    }
                }
                return _Styles;
            }
        }
        /// <summary>
        /// For fast DOM creation - since styles are not indexed or validated.
        /// If they are ever accessed by style name, they will be parsed on demand.
        /// </summary>
        protected string QuickSetValue
        {
            get
            {
                return _QuickSetValue;
            }
            set
            {
                bool hadStyles = HasStyles;
                _QuickSetValue = value;
                UpdateIndex(hadStyles);
            }
        }
        #endregion

        #region public properties
        /// <summary>
        /// Create a clone of this CSSStyleDeclaration object bound to the owner passed
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public CSSStyleDeclaration Clone(IDomElement owner)
        {
            CSSStyleDeclaration clone = new CSSStyleDeclaration(owner);

            if (QuickSetValue != null)
            {
                clone.QuickSetValue = QuickSetValue;
            }
            else
            {
                foreach (KeyValuePair<ushort, string> kvp in Styles)
                {
                    clone.Styles.Add(kvp);
                }
            }
            return clone;
        }
        /// <summary>
        /// True if there is at least one style.
        /// </summary>
        protected bool HasStyles
        {
            get
            {
                return QuickSetValue != null || (_Styles != null && Styles.Count > 0);
            }
        }
        public int Count
        {
            get { return !HasStyles ? 0 : Styles.Count; }
        }
        public bool IsReadOnly
        {
            get { return false; }
        }
        public ICollection<string> Keys
        {
            get
            {
                List<string> keys = new List<string>();
                foreach (var kvp in Styles)
                {
                    keys.Add(DomData.TokenName(kvp.Key));
                }
                return keys;
            }
        }      
        public ICollection<string> Values
        {
            get { return Styles.Values; }
        }
        /// <summary>
        /// Get or set the named style
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string this[string name]
        {
            get
            {
                return Get(name);
            }
            set
            {
                Set(name, value, true);
            }
        }
        /// <summary>
        /// Get or set the named style, optionally enabling strict mode
        /// </summary>
        /// <param name="name"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        public string this[string name, bool strict]
        {
            set
            {
                Set(name, value, strict);
            }
        }
        public string Height
        {
            get
            {
                return Get("height");
            }
            set
            {
                Set("height", value,true);
            }
        }
        public string Width
        {
            get
            {
                return Get("width");
            }
            set
            {
                Set("width", value, true);
            }
        }
        #endregion

        #region public methods
        /// <summary>
        /// Sets all the styles from a single CSS style string. Any existing styles will be erased.
        /// This method is used by DomElementFactory (not in strict mode).
        /// </summary>
        /// <param name="styles">A legal HTML style string</param>
        /// <param name="strict">When true, the styles will be validated and an error thrown if any are not valid</param>
        public void SetStyles(string styles, bool strict)
        {
            _Styles = null;

            if (!strict)
            {
                QuickSetValue = styles;
            }
            else
            {
                AddStyles(styles, strict);
            }
        }
        /// <summary>
        /// Add one or more styles to this element. Unlike SetStyle, existing styles are not affected, except
        /// for existing styles of the same name.
        /// </summary>
        /// <param name="styles"></param>
        /// <param name="strict"></param>
        public void AddStyles(string styles, bool strict)
        {
            foreach (string style in styles.SplitClean(';'))
            {
                int index = style.IndexOf(":");
                string stName;
                string stValue;
                if (index > 0)
                {
                    stName = style.Substring(0, index).Trim();
                    stValue = style.Substring(index + 1).Trim();
                    if (!strict)
                    {
                        SetRaw(stName, stValue);
                    }
                    else
                    {
                        Add(stName, stValue);
                    }
                }
            }
        }
        /// <summary>
        /// Remove a single named style
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Remove(string name)
        {
            return Styles.Remove(DomData.TokenID(name, true));
        }
        /// <summary>
        /// Add a single style
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Add(string name, string value)
        {
            Set(name, value, true);
        }
        /// <summary>
        /// Remove all styles
        /// </summary>
        public void Clear()
        {
            if (_Styles != null)
            {
                Styles.Clear();
            }
            QuickSetValue = null;
        }
        /// <summary>
        /// Returns true if the named style is defined
        /// </summary>
        /// <param name="styleName"></param>
        /// <returns></returns>
        public bool HasStyle(string styleName)
        {
            return Styles.ContainsKey(DomData.TokenID(styleName, true));
        }
        /// <summary>
        /// Sets style setting with no parsing
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetRaw(string name, string value)
        {
            bool hadStyles = HasStyles;
            Styles[DomData.TokenID(name, true)] = value;
            UpdateIndex(hadStyles);
        }
        public bool TryGetValue(string key, out string value)
        {
            return Styles.TryGetValue(DomData.TokenID(key, true), out value);
        }

        public double? NumberPart(string style)
        {
            string st = Get(style);
            if (st == null)
            {
                return null;
            }
            else
            {
                IStringScanner scanner = Scanner.Create(st);
                string numString;
                if (scanner.TryGet(MatchFunctions.Number(),out numString)) {
                    double num;
                    if (double.TryParse(numString, out num))
                    {
                        return num;
                    }
                }
                return null;
            }
        }
        public override string ToString()
        {
            string style = String.Empty;
            if (HasStyles)
            {
                foreach (var kvp in Styles)
                {
                    style += DomData.TokenName(kvp.Key) + ":" + kvp.Value + ";";
                }
            }
            return style;
        }
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return stylesEnumerable().GetEnumerator();
        }
       
        #endregion

        #region private methods
        /// <summary>
        /// Adds, removes, or does nothing to the index depending on whether a change is needed
        /// </summary>
        /// <param name="previouslyHadStyles"></param>
        protected void UpdateIndex(bool previouslyHadStyles)
        {
            if (HasStyles && !previouslyHadStyles)
            {
                AddToIndex();
            }
            else if (previouslyHadStyles && !HasStyles)
            {
                RemoveFromIndex();
            }
        }
        protected void AddToIndex()
        {
            if (!Owner.IsDisconnected)
            {
                Owner.Document.AddToIndex(Owner.Attributes.IndexKey(DomData.StyleNodeId), Owner);
            }
        }

        protected void RemoveFromIndex()
        {
            if (!Owner.IsDisconnected)
            {
                Owner.Document.RemoveFromIndex(Owner.Attributes.IndexKey(DomData.StyleNodeId));
            }
        }


        protected string Get(string name)
        {
            string value;
            if (Styles.TryGetValue(DomData.TokenID(name, true), out value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        protected void Set(string name, string value, bool strict)
        {
            name = ParseCamelCase(name);
            if (value == null)
            {
                Remove(name);
                return;
            }

            value = value.Trim().Replace(";", String.Empty);
            name = name.Trim();
            CssStyle style = null;
            if (!DomStyles.StyleDefs.TryGetValue(name, out style))
            {
                if (strict)
                {
                    throw new ArgumentException("The style '" + name + "' is not valid (strict mode)");
                }
            }
            else
            {
                switch (style.Type)
                {
                    case CssStyleType.UnitOption:
                        if (!style.Options.Contains(value))
                        {
                            try
                            {
                                value = ValidateUnitString(name,value);
                            }
                            catch
                            {
                                throw new ArgumentException("No valid unit data or option provided for attribue '"
                                    + name + "'. Valid options are: " + OptionList(style));
                            }
                        }
                        break;
                    case CssStyleType.Option:
                        if (!style.Options.Contains(value))
                        {
                            throw new ArgumentException("The value '" + value + "' is not allowed for attribute '"
                                + name + "'. Valid options are: " + OptionList(style));
                        }
                        break;
                    case CssStyleType.Unit:
                        value = ValidateUnitString(name,value);
                        break;
                    default:
                        // TODO: other formatting verification
                        break;
                }
            }
            SetRaw(name, value);
        }

        protected string OptionList(CssStyle style)
        {

            string list = "";
            foreach (string item in style.Options)
            {
                list += (list == String.Empty ? String.Empty : ",") + "'" + item + "'";
            }
            return list;

        }

        /// <summary>
        /// Cleans/validates a CSS units string, or throws an error if not possible
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string ValidateUnitString(string name,string value)
        {
            int pos = 0;
            value = value.Trim();
            StringBuilder outVal = new StringBuilder();
            //TODO this can't be comprehensive.
            string type = name=="opacity" ? "" : "px";

            if (String.IsNullOrEmpty(value))
            {
                return null;
            }
            int len = value.Length;
            char cur;
            while (pos < len)
            {
                cur = value[pos];
                if (!DomData.NumberChars.Contains(cur)) break;
                outVal.Append(cur);
                pos++;
            }
            while (pos > len)
            {
                if (value[pos] != ' ') break;
                pos++;
            }
            string remainder = value.Substring(pos).Trim();
            if (remainder != String.Empty)
            {

                if (DomData.Units.Contains(remainder))
                {
                    type = remainder;
                }
                else
                {
                    // unknown unit type - this is invalid, and we're in strict mode
                    throw new ArgumentException("Invalid unit data type for attribute, data: '" + value + "'");
                }
            }
            if (outVal.Length == 0)
            {
                throw new ArgumentException("No data provided for attribute, data: '" + value + "'");
            }
            outVal.Append(type);
            return outVal.ToString();
        }

        protected IEnumerable<KeyValuePair<string, string>> stylesEnumerable()
        {
            foreach (var kvp in Styles)
            {
                yield return new KeyValuePair<string, string>(DomData.TokenName(kvp.Key), kvp.Value);

            }
            yield break;
        }
        /// <summary>
        /// Convert camelcased CSS attributes to correct value
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected string ParseCamelCase(string name)
        {
            int pos = 0;
            while (pos < name.Length)
            {
                if (name[pos] >= 'A' && name[pos] <= 'Z')
                {
                    name = name.Substring(0, pos) + "-" + name.Substring(pos, 1).ToLower() + name.Substring(pos + 1);
                    pos++;
                }
                pos++;
            }
            return name;

        }
        
        #endregion

        #region interface members
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        bool IDictionary<string, string>.ContainsKey(string key)
        {
            return Styles.ContainsKey(DomData.TokenID(key, true));
        }
        void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item)
        {
            Add(item.Key, item.Value);
        }


        bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item)
        {
            return Styles.Contains(new KeyValuePair<ushort, string>(DomData.TokenID(item.Key), item.Value));
        }

        void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            array = new KeyValuePair<string, string>[Styles.Count];
            int index = 0;
            foreach (var kvp in Styles)
            {
                array[index++] = new KeyValuePair<string, string>(DomData.TokenName(kvp.Key), kvp.Value);
            }
        }

        bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
        {
            var kvp = new KeyValuePair<ushort, string>(DomData.TokenID(item.Key), item.Value);
            return Styles.Remove(kvp);
        }

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion        
    }
}
