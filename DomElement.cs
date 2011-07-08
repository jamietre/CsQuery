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
        public DomRoot Root;
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
    public abstract class DomContainer : DomObject
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
        public virtual void Add(DomObject element)
        {
            element.Parent = this;
            element.Root = this.Root;
            _Children.Add(element);
            if (Root != null)
            {
                Root.AddToIndex(element);
            }
 
        }
        /// <summary>
        /// Add all elements as children of this element
        /// </summary>
        /// <param name="elements"></param>
        public virtual void AddRange(IEnumerable<DomObject> elements)
        {
            foreach (DomObject e in elements)
            {
                Add(e);
            }
        }
        /// <summary>
        /// Adds a child element to the end of the list
        /// </summary>
        /// <param name="index"></param>
        /// <param name="element"></param>
        public void Insert(int index, DomObject element)
        {
            element.Parent = this;
            element.Root = this.Root;
            _Children.Insert(index, element);
            if (Root != null)
            {
                Root.AddToIndex(element);
            }
        }
   
        public void Remove(DomObject obj)
        {

            _Children.Remove(obj);
            if (Root != null)
            {
                Root.RemoveFromIndex(obj);
            }
            obj.Parent = null;
            obj.Root = null;
        }
 
        /// <summary>
        /// Removes all children
        /// </summary>
        public void RemoveChildren()
        {
            for (int i=_Children.Count-1;i>=0;i--)
            {
                Remove(_Children[i]);
            }
        }
    }

    public class DomRoot : DomContainer
    {
        public DomRoot()
            : base()
        {
        }
        public DomRoot(IEnumerable<DomObject> elements)
            : base(elements)
        {

        }
        public Dictionary<string, HashSet<DomElement>> SelectorXref = new Dictionary<string, HashSet<DomElement>>();
        protected IEnumerable<string> IndexKeys(DomElement e)
        {
            yield return e.Tag;
            if (!String.IsNullOrEmpty(e.ID)) {
                yield return "#"+e.ID;
            }
            foreach (string cls in e.Classes)
            {
                yield return "."+cls;
            }
            //todo -add attributes?
        }
        public void AddToIndex(DomObject obj)
        {
            if (obj is DomElement)
            {
                HashSet<DomElement> list;

                DomElement e = (DomElement)obj;
                // ENsure all children from unbound elements become part of this DOM
                if (e.Root == null)
                {
                    e.Root = this;
                }

                foreach (string key in IndexKeys(e))
                {
                    if (SelectorXref.TryGetValue(key, out list))
                    {
                        list.Add(e);
                    }
                    else
                    {
                        list= new HashSet<DomElement>();
                        list.Add(e);
                        SelectorXref[key] = list;
                    }
                }
                foreach (DomElement child in e.Elements)
                {
                    AddToIndex(child);
                }
            }
        }
        public void RemoveFromIndex(DomObject obj)
        {
            if (obj is DomElement) {
                HashSet<DomElement> list;
                DomElement e = (DomElement)obj;
                foreach (DomElement child in e.Elements)
                {
                    RemoveFromIndex(child);
                }
                foreach (string key in IndexKeys(e))
                {
                    if (SelectorXref.TryGetValue(key, out list))
                    {
                        list.Remove(e);
                    }
                }
            }
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

        /// <summary>
        /// Creates a deep clone of this
        /// </summary>
        /// <returns></returns>
        public DomElement Clone()
        {
            DomElement e = new DomElement();
            e.Tag = Tag;
            
            e._Styles = new Dictionary<string, string>(_Styles);
            e._Classes = new HashSet<string>(_Classes);
            foreach (var attr in _Attributes)
            {
                e.SetAttribute(attr.Key, attr.Value);
            }
            foreach (DomObject obj in _Children)
            {
                if (obj is DomElement)
                {
                    e.Add(((DomElement)obj).Clone());
                }
                else if (obj is DomLiteral)
                {
                    DomLiteral lit = new DomLiteral(((DomLiteral)obj).Html);
                    e.AppendChild(lit);
                } else {
                    throw new Exception("Unexpected element type while cloning a DomElement");
                }
            }
            return e;
        }

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
        public IEnumerable<KeyValuePair<string,string>> Styles
        {
            get
            {
                foreach (var kvp in _Styles)
                {
                    yield return kvp;
                }
            }
        } protected Dictionary<string, string> _Styles = new Dictionary<string, string>();
        public void AppendChild(DomObject obj) {
            obj.Parent = this;
            _Children.Add(obj);
        }
        public bool HasClass(string name)
        {
            return _Classes.Contains(name);
        }
        public bool AddClass(string name)
        {
            return _Classes.Add(name);
        }
        public bool RemoveClass(string name)
        {
            return _Classes.Remove(name);
        }
        public void AddStyle(string name, string value)
        {
            _Styles[name.Trim()] = value.Replace(";",String.Empty).Trim();
        }
        public void AddStyle(string style)
        {
            string[] kvp = style.Split(':');
            AddStyle(kvp[0], kvp[1]);
        }
        public bool RemoveStyle(string name)
        {
            return _Styles.Remove(name);
        }
        public string GetStyle(string name)
        {
            string value;
            if (_Styles.TryGetValue(name,out value)) {
                return value;
            }
            return null;
        }
                   // move to imp
            //            List<DomElement> list;
//            foreach (string c in element.
//            if (SelectorXref.TryGetValue(
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
                case "style":
                    _Styles.Clear();
                    string[] styles = value.Trim().Split(new char[] {';'},StringSplitOptions.RemoveEmptyEntries);
                    foreach (string val in styles)
                    {
                        if (val.IndexOf(":") > 0)
                        {
                            string[] kvps = val.Split(':');
                            _Styles[kvps[0]] = kvps[1];
                        }
                    }
                    break;
                default:
                    _Attributes[lowName] = value;
                    break;
            }
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
            return GetAttribute(name, null);
        }
        /// <summary>
        /// Returns the value of an attribute or a default value if it could not be found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetAttribute(string name, string defaultValue)
        {
            string value;
            switch (name.ToLower())
            {
                case "style":
                    return Style;
                case "class":
                    return Class;
            }
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
        
        public string Style
        {
            get
            {
                string style = String.Empty;
                foreach (var kvp in _Styles)
                {
                    style += (style==String.Empty?String.Empty:" ") + kvp.Key + ": " + kvp.Value + ";";
                }
                return style;
            }
        }
        public string Class
        {
            get
            {
                string cls = String.Empty;
                foreach (var val in _Classes)
                {
                    cls += (cls == String.Empty ? String.Empty : " ") + val;
                }
                return cls;
            }
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
        /// <summary>
        /// Returns the completel HTML for this element and its children
        /// </summary>
        public override string Html
        {
            get
            {
                return GetHtml(true);
            }
        }
        /// <summary>
        /// Returns the HTML for this element, ignoring children/innerHTML
        /// </summary>
        public string ElementHtml
        {
            get
            {
                return GetHtml(false);
            }
        }
        protected string GetHtml(bool includeChildren)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<" + Tag);
            if (_Classes.Count > 0)
            {
                sb.Append(" class=\"" + Class+"\"");
            }
            if (_Styles.Count > 0)
            {
                sb.Append(" style=\"" + Style+"\"");
            }
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
                sb.Append(String.Format(">{0}</" + Tag + ">", includeChildren ? InnerHtml : String.Empty));
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
