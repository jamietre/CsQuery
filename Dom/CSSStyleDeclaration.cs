using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using Jtc.CsQuery.Utility;
using Jtc.CsQuery.ExtensionMethods;
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
    public interface ICSSStyleDeclaration
    {
        bool HasStyle(string styleName);
    }

    public class CSSStyleDeclaration : IDictionary<string, string>, IEnumerable<KeyValuePair<string, string>>, ICSSStyleDeclaration
    {
        public CSSStyleDeclaration()
        {
         
        }
        
        protected Dictionary<string, string> _Styles=null;
        
        protected Dictionary<string, string> Styles
        {
            get
            {
                if (_Styles == null)
                {
                    _Styles = new Dictionary<string, string>();
                }
                return _Styles;
            }
        }
        public CSSStyleDeclaration Clone()
        {
            CSSStyleDeclaration clone = new CSSStyleDeclaration();
            foreach (KeyValuePair<string, string> kvp in Styles)
            {
                clone.Add(kvp.Key, kvp.Value);
            }
            return clone;

        }
        /// <summary>
        /// Sets all the styles from a single CSS style sttrin
        /// </summary>
        /// <param name="styles"></param>
        /// <param name="strict"></param>
        public void SetStyles(string styles, bool strict)
        {

            Styles.Clear();
            AddStyle(styles, strict);
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
        public override string ToString()
        {
            string style = String.Empty;
            foreach (var kvp in Styles)
            {
                style += kvp.Key + ":" + kvp.Value + ";";
            }
            return style;
        }
        //public void ClearCache()
        //{
        //    Styles.Clear();
        //    //Cached = false;
        //}
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
            if (Styles.TryGetValue(name.ToLower(), out value))
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

        
        bool IDictionary<string,string>.ContainsKey(string key)
        {
            return Styles.ContainsKey(key);
        }
        public bool HasStyle(string styleName)
        {
            return Styles.ContainsKey(styleName);
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
            return  Styles.ContainsKey(item.Key) && Styles[item.Key]== item.Value;
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
            if (Styles.ContainsKey(item.Key) && Styles[item.Key] == item.Value)
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
