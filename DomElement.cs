using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jtc.ExtensionMethods;

namespace Jtc.CsQuery
{
    /// <summary>
    /// Base class for anything that exists in the DOM
    /// </summary>
    public abstract class DomObject
    {
        public DomContainer Parent;

        public abstract string Html
        { get;  }
    }

    /// <summary>
    /// Used for literal text (not part of a tag)
    /// </summary>
    public class DomLiteral : DomObject
    {
        public DomLiteral()
        {
        }
        public DomLiteral(string html)
        {
            _Html = html;
        }

        public override string Html
        {
            get
            {
                return _Html;
            }
        }
        protected string _Html=String.Empty;
        //public override int GetHashCode()
        //{
        //    return _Html.GetHashCode();
        //}
        //public override bool Equals(object obj)
        //{
        //    return this == obj;
        //}
    }
    /// <summary>
    /// Base class for Dom object that contain other elements
    /// </summary>
    public abstract class DomContainer : DomObject, IEnumerable<DomElement>
    {
        public DomContainer()
        {

        }

        public DomContainer(IEnumerable<DomObject> elements)
        {
            _Children.AddRange(elements);   
        }
        /// <summary>
        /// Returns all children (including inner HTML as objects);
        /// </summary>
        public IEnumerable<DomObject> Children
        {
            get
            {
               
                return _Children;
            }
        }
        internal List<DomObject> _Children
        {
            get
            {
                if (__Children == null)
                {
                    __Children = new List<DomObject>();
                }
                return __Children;
            }
        } 
        protected List<DomObject> __Children = null;
        /// <summary>
        /// Returns all elements
        /// </summary>
        public IEnumerable<DomElement> Elements
        {
            get
            {
                foreach (DomObject elm in Children)
                {
                    if (elm is DomElement)
                    {
                        yield return (DomElement)elm;
                    }
                }
                yield break;
            }
        }
        public DomObject this[int index]
        {
            get
            {
                return _Children[index];
            }

        }
        public int Count
        {
            get
            {
                return _Children.Count;
            }
        }
        public void Clear()
        {
            _Children.Clear();
        }
        public override string Html
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (DomObject e in Children )
                {
                    sb.Append(e.Html);
                }
                return (sb.ToString());
            }
        } 

        /// <summary>
        /// Add a child to this element 
        /// </summary>
        /// <param name="element"></param>
        public void Add(DomObject element)
        {
            element.Parent = this;
            _Children.Add(element);
        }
        /// <summary>
        /// Add all elements as children of this element
        /// </summary>
        /// <param name="elements"></param>
        public void Add(IEnumerable<DomObject> elements)
        {
            foreach (DomObject e in elements)
            {
                e.Parent = this;
                _Children.Add(e);
            }
        }
        public void Insert(int index, DomObject element)
        {
            element.Parent = this;
            _Children.Insert(index, element);
        }
        public void Remove(DomObject obj)
        {
            _Children.Remove(obj);
        }
 
        /// <summary>
        /// Removes all children
        /// </summary>
        public void RemoveChildren()
        {
            _Children.Clear();
        }


        #region IEnumerable<DomElement> Members

        public IEnumerator<DomElement> GetEnumerator()
        {
            return Elements.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

    public class DomRoot : DomContainer
    {
        public DomRoot(IEnumerable<DomObject> elements)
            : base(elements)
        {

        }


    }
    public enum ElementType
    {
        Normal=1,
        Informational=2
    }
    /// <summary>
    /// HTML elements
    /// </summary>
    public class DomElement : DomContainer
    {
        public DomElement()
        {
        }
        public ElementType ElementType = ElementType.Normal;


        public IEnumerable<string> Classes
        {
            get
            {
                foreach (string val in _Classes)
                {
                    yield return val;
                }
            }
        } protected HashSet<string> _Classes = new HashSet<string>();

        public bool HasClass(string name)
        {
            return _Classes.Contains(name);
        }
        public void SetAttribute(string name, string value)
        {
            string lowName = name.ToLower();
            // TODO this is not right, should be able to set Class attribute, seaprate this handling
            switch (lowName)
            {
                case "class":
                    _Classes.Clear();
                    foreach (string val in value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        _Classes.Add(val);
                    }
                    break;
            }
            _Attributes[lowName] = value;
        }
        /// <summary>
        /// Sets an attribute with no value
        /// </summary>
        /// <param name="name"></param>
        public void SetAttribute(string name)
        {
            SetAttribute(name, String.Empty);
        }
        public bool RemoveAttribute(string name)
        {
            return _Attributes.Remove(name);
        }
        /// <summary>
        /// Gets an attribute value, or returns null if the value is missing. If a valueless attribute is found, this will also return null. HasAttribute should be used
        /// to test for such attributes. Attributes with an empty string value will return String.Empty.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetAttribute(string name)
        {
            string value = null;
            if (_Attributes.TryGetValue(name, out value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Returns the value of an attribute or a default value if it could not be found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetAttribute(string name, string defaultValue)
        {
            string value;
            if (_Attributes.TryGetValue(name.ToLower(), out value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
        public bool TryGetAttribute(string name, out string value)
        {
            return _Attributes.TryGetValue(name.ToLower(),out  value);

        }
        public IEnumerable<KeyValuePair<string, string>> Attributes
        {
            get
            {
                foreach (string key in _Attributes.Keys)
                {
                    yield return new KeyValuePair<string, string>(key.ToLower(), _Attributes[key]);
                }
            }
        }
        public bool HasAttribute(string name)
        {
            string value;
            if (_Attributes.TryGetValue(name.ToLower(), out value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string Tag
        {
            get
            {
                return _Tag;
            }
            set
            {
                _Tag = value.ToLower();
            }
        } protected string _Tag = String.Empty;
        public string Type
        {
            get
            {
                return GetAttribute("type", String.Empty);
            }
            set
            {
                _Attributes["type"] = value;
            }
        } protected string _Type = String.Empty;
        protected Dictionary<string, string> _Attributes = new Dictionary<string, string>();


        public IEnumerable<string> Errors
        {
            get
            {
                if (__Errors == null)
                {
                    yield break;
                }
                else
                {
                    foreach (string error in _Errors)
                    {
                        yield return error;
                    }
                }
            }
        }
        protected List<string> _Errors
        {

            get
            {
                if (__Errors == null)
                {
                    __Errors = new List<string>();
                }
                return __Errors;
            }
        } private List<string> __Errors = null;
        public string ID
        {
            get
            {
                return GetAttribute("id",String.Empty);
            }
            set
            {
                _Attributes["id"] = value;
            }
        }
        public string Name
        {
            get
            {
                return GetAttribute("name",String.Empty);
            }
            set
            {
                _Attributes["name"] = value;
            }

        }
        /// <summary>
        /// For special tag types, like !DOCTYPE or comments, any data that is not really a tag.
        /// </summary>
        public string NonAttributeData { get; set; }
        
        /// <summary>
        /// Returns text of the inner HTMl
        /// </summary>
        public string InnerHtml
        {
            get
            {
                if (Children.IsNullOrEmpty())
                {
                    return String.Empty;
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (DomObject elm in Children)
                    {
                        sb.Append(elm.Html);
                    }
                    return sb.ToString();
                }
            }
        }

        public override string Html
        {
            get
            {
                return GetHtml();
            }
        } 
        protected string GetHtml()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<" + Tag);
            foreach (var kvp in _Attributes)
            {
                string val = kvp.Value;
                if (val != String.Empty)
                {
                    sb.Append(" " + kvp.Key + "=\"" + kvp.Value + "\"");
                }
                else
                {
                    sb.Append(" " + kvp.Key);
                }
            }
            if (ElementType == ElementType.Informational)
            {
                sb.Append(" " + NonAttributeData.Trim());
            }
            if (InnerHtmlAllowed)
            {
                sb.Append(String.Format(">{0}</" + Tag + ">", InnerHtml));
            }
            else
            {
                sb.Append(" />");
            }
            return sb.ToString();
        }
     

        /// <summary>
        /// This object type can have inner HTML.
        /// </summary>
        /// <returns></returns>
        public bool InnerHtmlAllowed
        {
            get
            {
                switch (Tag.ToLower())
                {
                    case "base":
                    case "basefont":
                    case "frame":
                    case "link":
                    case "meta":
                    case "area":
                    case "col":
                    case "hr":
                    case "param":
                    case "input":
                    case "img":
                    case "br":
                    case "!doctype":
                    case "!--":
                        return false;
                    default:
                        return true;
                }
            }
        }
        //public override int GetHashCode()
        //{
        //    return Tag.GetHashCode() + _Attributes.GetHashCode();
        //}
        //public override bool Equals(object obj)
        //{
        //    return obj == this;
        //}
    }
}
