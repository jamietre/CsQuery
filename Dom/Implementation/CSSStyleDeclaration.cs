using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using Jtc.CsQuery.Utility;
using Jtc.CsQuery.ExtensionMethods;
using System.Xml;

namespace Jtc.CsQuery.Implementation
{
     
    public class CSSStyleDeclaration : IDictionary<string, string>, IEnumerable<KeyValuePair<string, string>>, ICSSStyleDeclaration
    {

        /// <summary>
        /// Warning: Do not attempt to access this value directly to determine whether or not there are styles, since it also
        /// depends on QuickSetStyles. Use HasStyles method instead.
        /// </summary>
        private IDictionary<ushort, string> _Styles;

        protected IDictionary<ushort, string> Styles
        {
            get
            {
                if (_Styles == null)
                {
                    _Styles = new SmallDictionary<ushort, string>();
                    if (QuickSetValue != null)
                    {
                        AddStyle(QuickSetValue, false);
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
        protected string QuickSetValue;

        public CSSStyleDeclaration Clone()
        {
            CSSStyleDeclaration clone = new CSSStyleDeclaration();

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
        /// Sets all the styles from a single CSS style string. This method is also used during DOM creation.
        /// </summary>
        /// <param name="styles"></param>
        /// <param name="strict"></param>
        public void SetStyles(string styles, bool strict)
        {
            _Styles = null;

            if (!strict)
            {
                QuickSetValue = styles;
            }
            else
            {
                AddStyle(styles, strict);
            }
        }
        public void AddStyle(string styles, bool strict)
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
        protected bool HasStyles
        {
            get
            {
                return QuickSetValue != null || (_Styles != null && Styles.Count > 0);
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

        public bool Remove(string name)
        {
            return Styles.Remove(DomData.TokenID(name,true));
        }
        public void Add(string name, string value)
        {
            this[name] = value;
        }

        public string this[string name]
        {
            get
            {
                return Get(name);
            }
            set
            {
                Set(name,value,true);
            }
        }
        public string this[string name, bool strict]
        {
            set
            {
                Set(name,value,strict);
            }
        }
        /// <summary>
        /// Sets style setting with no parsing
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetRaw(string name, string value)
        {
            Styles[DomData.TokenID(name,true)] = value;
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

        protected void Set(string name, string value, bool strict) {
            name = name.ToLower();
            if (value == null)
            {
                Remove(name);
                return;
            }
            
            value = value.Trim().Replace(";", String.Empty);
            name = name.Trim();
            CssStyle style = null;
            if (!HtmlDom.StyleDefs.TryGetValue(name, out style))
            {
                if (strict)
                {
                    throw new Exception("No attribute named '" + name + "' exists (strict mode)");
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
                                value = ValidateUnitString(value);
                            }
                            catch
                            {
                                throw new Exception("No valid unit data or option provided for attribue '" 
                                    + name + "'. Valid options are: " + OptionList(style));
                            }
                        }
                        break;
                    case CssStyleType.Option:
                        if (!style.Options.Contains(value))
                        {
                           throw new Exception("The value '" + value + "' is not allowed for attribute '"
                               + name + "'. Valid options are: " + OptionList(style));
                        }
                        break;
                    case CssStyleType.Unit:
                        value = ValidateUnitString(value);
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
        protected string ValidateUnitString(string value)
        {
            int pos = 0;
            value = value.Trim();
            StringBuilder outVal = new StringBuilder();
            string type = "px";
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
                    throw new Exception("Invalid unit data type for attribute, data: '" + value + "'");
                }
            }
            if (outVal.Length == 0)
            {
                throw new Exception("No data provided for attribute, data: '" + value + "'");
            }
            outVal.Append(type);
            return outVal.ToString();
        }
        

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return stylesEnumerable().GetEnumerator();
        }

        protected IEnumerable<KeyValuePair<string, string>> stylesEnumerable()
        {
            foreach (var kvp in Styles)
            {
                yield return new KeyValuePair<string,string>(DomData.TokenName(kvp.Key),kvp.Value);

            }
            yield break;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        bool IDictionary<string,string>.ContainsKey(string key)
        {
            return Styles.ContainsKey(DomData.TokenID(key,true));
        }
        public bool HasStyle(string styleName)
        {
            return Styles.ContainsKey(DomData.TokenID(styleName, true));
        }

        public ICollection<string> Keys
        {
            get {
                List<string> keys = new List<string>();
                foreach (var kvp in Styles)
                {
                    keys.Add(DomData.TokenName(kvp.Key));
                }
                return keys;
            }
        }

        public bool TryGetValue(string key, out string value)
        {
            return Styles.TryGetValue(DomData.TokenID(key,true), out value);
        }

        public ICollection<string> Values
        {
            get { return Styles.Values; }
        }


        public void Clear()
        {
            if (_Styles != null)
            {
                Styles.Clear();
            }
            QuickSetValue=null;
        }

        public int Count
        {
            get { return !HasStyles ? 0 : Styles.Count; }
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
            return Styles.Contains(new KeyValuePair<ushort, string>(DomData.TokenID(item.Key), item.Value));
        }

        void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            array = new KeyValuePair<string, string>[Styles.Count];
            int index = 0;
            foreach (var kvp in Styles)
            {
                array[index++] = new KeyValuePair<string,string>(DomData.TokenName(kvp.Key),kvp.Value);
            }
        }

        bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
        {
            var kvp=new KeyValuePair<ushort,string>(DomData.TokenID(item.Key),item.Value);
            return Styles.Remove(kvp);
        }

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
