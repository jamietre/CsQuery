using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using Jtc.CsQuery.Utility;
using System.Xml;

namespace Jtc.CsQuery
{


    public enum CssStyleType
    {
        Unit = 1,
        Option = 2,
        UnitOption=3,
        Composite = 4,
        Color = 5,
        Font = 6,
        Url=7,
        String=8
    }
    public interface ICssStyle
    {
        string Name { get; set; }
        CssStyleType Type { get; set; }
        string Format { get; set; }
        HashSet<string> Options { get; set; }
        string Description { get; set; }
    }
    public class CssStyle : ICssStyle
    {
        public string Name { get; set; }
        public CssStyleType Type { get; set; }
        public string Format { get; set; }
        public string Description { get; set; }
        public HashSet<string> Options { get; set; }
    }

    public class CSSStyleDeclaration : IDictionary<string,string>, IEnumerable<KeyValuePair<string,string>>
    {
        public CSSStyleDeclaration()
        {
            GetStyleString = DefaultGetStyleString;
            SetStyleString = DefaultSetStyleString;
        }
        public CSSStyleDeclaration(Func<string> getStyle, Action<string> setStyle)
        {
            GetStyleString = getStyle;
            SetStyleString = setStyle;
        }
        /// <summary>
        /// In order to maintain consistency with the attribute model, we want to keep the style string as an attribute.
        /// However we also  want to be able to provide decent access to the individual styles for the purposes of this model.
        /// So the delegates here are linked to the "GetAttribute" / "SetAttribute" methods in the owner DOMElement object.
        /// Additionally, the same owner object binds a delegate in the Attributes object that is called when the style
        /// attribute changes, which in turn tells this object to clear its cached dictionary. This should provide very good
        /// performance: no work done during HTML parsing, styles parsed on first access, and cached unless changed otherwise.
        /// </summary>
        public Func<string> GetStyleString;
        public Action<string> SetStyleString;
        protected string DefaultGetStyleString()
        {
            return _Style;
        }
        protected void DefaultSetStyleString(string style)
        {
            _Style = style;
        }
        protected string _Style = String.Empty;
        protected Dictionary<string, string> _Styles=null;
        
        private bool Cached { get; set; }
        protected Dictionary<string, string> Styles
        {
            get
            {
                if (_Styles == null)
                {
                    _Styles = new Dictionary<string, string>();
                    Cached = false;
                }
                if (!Cached)
                {
                    Cached = true;
                    SetStylesImpl(GetStyleString(),false);
                }
                return _Styles;
            }
        }
        public void SetStyles(string styles, bool strict)
        {
             if (Cached)
            {
                Styles.Clear();
            }
            SetStylesImpl(styles, strict);
        }
        private void SetStylesImpl(string styles, bool strict)
        {
            string[] stList = GetStyleString().Trim().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
 
            foreach (string val in stList)
            {
                AddStyle(val, strict);
            }
        }
        public void AddStyle(string style, bool strict)
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
                    Add(stName,stValue);
                }
            }
        }
        public override string ToString()
        {
            string style = String.Empty;
            foreach (var kvp in Styles)
            {
                style += (style == String.Empty ? String.Empty : " ") + kvp.Key + ": " + kvp.Value + ";";
            }
            return style;
        }
        public void ClearCache()
        {
            Styles.Clear();
            Cached = false;
        }
        public bool Remove(string name)
        {
            return Styles.Remove(name);
        }
        public void Add(string name, string value)
        {
            this[name] = value;
        }
        //public CSSStyleDeclaration Clone()
        //{
        //    CSSStyleDeclaration clone = new CSSStyleDeclaration();
        //    clone.Styles = new Dictionary<string, string>();
        //    foreach (var kvp in Styles)
        //    {
        //        clone.Styles[kvp.Key] = kvp.Value;
        //    }
        //    return clone;
        //}
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
            Styles[name.ToLower()] = value;
        }
        protected string Get(string name)
        {
            string value;
            if (Styles.TryGetValue(name, out value))
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
            //string parsedVal;
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
            Styles[name] = value;
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

        HashSet<char> numberChars = new HashSet<char>("-+0123456789.,");
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
                if (!numberChars.Contains(cur)) break;
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

                if (Units.Contains(remainder))
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
        protected List<string> Units = new List<string>(new string[] { "%", "in", "cm", "mm", "em", "ex", "pt", "pc", "px" });

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return Styles.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public bool ContainsKey(string key)
        {
            return Styles.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return Styles.Keys; }
        }

        public bool TryGetValue(string key, out string value)
        {
            return Styles.TryGetValue(key, out value);
        }

        public ICollection<string> Values
        {
            get { return Styles.Values; }
        }


        public void Clear()
        {
            Styles.Clear();
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
            get { return Styles.Count; }
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
            return  ContainsKey(item.Key) && Styles[item.Key]== item.Value;
        }

        void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            array = new KeyValuePair<string, string>[Styles.Count];
            int index = 0;
            foreach (var kvp in Styles)
            {
                array[index++] = kvp;
            }
        }

        bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
        {
            if (ContainsKey(item.Key) && Styles[item.Key] == item.Value)
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
