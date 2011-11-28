using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Jtc.CsQuery.ExtensionMethods;

namespace Jtc.CsQuery
{

    /// <summary>
    /// A visual element
    /// </summary>
    public interface IDomElement : IDomContainer
    {
        IEnumerable<string> Classes { get; }

        bool HasClass(string className);
        bool AddClass(string className);
        bool RemoveClass(string className);
        void AddStyle(string styleString);
        bool RemoveStyle(string name);
        bool IsBlock { get; }

        string this[string attribute] { get; set; }

        string ElementHtml();

        void Reindex();
    }
  
    /// <summary>
    /// HTML elements
    /// </summary>
    public class DomElement : DomContainer<DomElement>, IDomElement
    {
        //private const string needsQuoting = " '\"";
        
        protected DomAttributes _Attributes = null;
        protected CSSStyleDeclaration _Style = null;
        protected List<short> _Classes = null;

        public DomElement()
        {

        }
        public DomElement(string tag)
            : base()
        {
            NodeName = tag;
        }
       
        public void Reindex()
        {
            _PathID = null;
            _Index = -1;
        }
        //public DomElement(string tag,CsQuery owner)
        //{
        //    NodeName = tag;
        //    _Owner = owner;
        //}
        public override NodeType NodeType
        {
            get { return NodeType.ELEMENT_NODE; }
        }
      
        /// <summary>
        /// Assigning parent node should not be done e
        /// </summary>
        public override IDomContainer ParentNode
        {
            get
            {
                return base.ParentNode;
            }
            internal set
            {
                base.ParentNode = value;
            }
        }
        public override string PathID
        {
            get
            {
                if (_PathID == null)
                {
                    _PathID = PathEncode(Index);
                }
                return _PathID;
            }
        }
        protected string IndexKey(string key)
        {
            return IndexKey(key, Path);
        }
        protected string IndexKey(string key, string path)
        {
            return key + ">" + path;
        }
        public override IEnumerable<string> IndexKeys()
        {
            //if (!Complete)
            //{
            //    throw new Exception("This element is incomplete and cannot be added to a DOM.");
            //}
            // Add just the element to the index no matter what so we can query on subsets
            string path = Path;
            yield return IndexKey(String.Empty, path);
            yield return IndexKey(NodeName, path);
            string id = ID;
            if (!String.IsNullOrEmpty(id))
            {
                yield return IndexKey("#" + id, path);
            }

            foreach (string cls in Classes)
            {
                yield return IndexKey("." + cls, path);
            }

            //todo -add attributes?
        }
        //internal void AddToIndex()
        //{
        //    //if (Document != null)
        //    //{
        //    DomRoot document = (DomRoot)Document;
        //        foreach (string key in IndexKeys())
        //        {
        //            document.SelectorXref.Add(key, this);
        //        }

        //        if (_ChildNodes != null)
        //        {
        //            foreach (DomElement child in ChildElements)
        //            {
        //                child.AddToIndex();
        //            }
        //        }

        //    //}
           
        //}
        //internal void RemoveFromIndex()
        //{
        //    //if (Document != null)
        //    //{
        //        if (_ChildNodes != null)
        //        {
        //            foreach (DomElement child in ChildElements)
        //            {
        //                child.RemoveFromIndex();
        //            }
        //        }

        //        foreach (string key in IndexKeys())
        //        {
        //            Document.SelectorXref.Remove(key);
        //        }
        //        _PathID = null;
        //        _Index = -1;
        //    //}
        //}


        /// <summary>
        /// Creates a deep clone of this
        /// </summary>
        /// <returns></returns>
        public override DomElement Clone()
        {
            return CloneImpl(base.Clone());

        }


        protected DomElement CloneImpl(DomElement e)
        {
            e.nodeNameID = nodeNameID;

            if (_Attributes != null)
            {
                e.Attributes = Attributes.Clone(this);
            }
            if (_Classes != null)
            {
                e._Classes = new List<short>(_Classes);
            }
            if (_Style != null)
            {
                e.Style = Style.Clone();
            }
            // will not create ChildNotes list object unless results are returned (don't use AddRange)
            foreach (IDomObject child in CloneChildren())
            {
                e.ChildNodes.Add(child);
            }

            return e;
        }
        public override IEnumerable<IDomObject> CloneChildren()
        {
            if (_ChildNodes!=null)
            {
                foreach (IDomObject obj in ChildNodes)
                {
                    yield return obj.Clone();
                }
            }
            yield break;
        }

        public IEnumerable<string> Classes
        {
            get
            {
                if (_Classes !=  null)
                {
                    foreach (short clsid in _Classes)
                    {
                        yield return DomData.TokenName(clsid);
                    }
                }
                yield break;
            }
        }
        protected bool HasClasses
        {
            get
            {
                return _Classes != null && _Classes.Count > 0;
            }
        }


        public bool HasClass(string name)
        {
            return HasClasses 
                && _Classes.Contains(DomData.TokenID(name,false));
        }
        public bool AddClass(string name)
        {
            bool result=false;
            foreach (string cls in name.SplitClean())
            {
                if (!HasClass(cls))
                {
                    if (_Classes == null)
                    {
                        _Classes = new List<short>();
                    }
                    _Classes.Add(DomData.TokenID(cls, false));
                    if (!IsDisconnected)
                    {
                        Document.AddToIndex(IndexKey("." + cls), this);
                    }
                    
                    result = true;
                }
            }
            return result;
        }
        public bool RemoveClass(string name)
        {
            bool result = false;
            foreach (string cls in name.SplitClean())
            {
                if (HasClass(cls))
                {
                    _Classes.Remove(DomData.TokenID(cls, false));
                    if (!IsDisconnected)
                    {
                        Document.RemoveFromIndex(IndexKey("." + cls));
                    }

                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// Add a single style in the form "styleName: value"
        /// </summary>
        /// <param name="style"></param>
        public void AddStyle(string style)
        {
            AddStyle(style,true);
        }
        public void AddStyle(string style,bool strict)
        {
            Style.AddStyle(style, strict);
        }
        public bool RemoveStyle(string name)
        {
            return _Style != null ? Style.Remove(name) : false;
        }
        protected bool HasAttribute(short nodeId)
        {
            string value;
            return _Attributes != null 
                && Attributes.TryGetValue(nodeId,out value);
        }
        public override bool HasAttribute(string name)
        {
            string value;
            return _Attributes != null 
                && Attributes.TryGetValue(name.ToLower(), out value);
        }
        public void SetStyles(string styles)
        {
            SetStyles(styles, true);
        }
        public void SetStyles(string styles, bool strict)
        {
            Style.SetStyles(styles, strict);
        }
        /// <summary>
        /// Set the value of an attribute to "value." 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public override void SetAttribute(string name, string value)
        {
            SetAttribute(DomData.TokenID(name), value);
        }
        protected void SetAttribute(short tokenId, string value)
        {
            if (tokenId == DomData.ClassAttrID)
            {
                ClassName = value;
            }
            else
            {
                Attributes[tokenId] = value;
            }
        }
        /// <summary>
        /// Sets an attribute with no value
        /// </summary>
        /// <param name="name"></param>
        public override void SetAttribute(string name)
        {
            SetAttribute(name, String.Empty);
        }
        public void SetAttribute(short tokenId)
        {
            SetAttribute(tokenId, String.Empty);
        }
        public override bool RemoveAttribute(string name)
        {
            if (_Attributes == null)
            {
                return false;
            }
            short tokenId = DomData.TokenID(name);
            if (tokenId == DomData.ClassAttrID)
            {
                _Classes = null;
                return true;
            }
            else
            {
                return Attributes.Remove(name.ToLower());
            }
        }
        /// <summary>
        /// Gets an attribute value, or returns null if the value is missing. If a valueless attribute is found, this will also return null. HasAttribute should be used
        /// to test for such attributes. Attributes with an empty string value will return String.Empty.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string GetAttribute(string name)
        {
            return GetAttribute(name, null);
        }
        /// <summary>
        /// Returns the value of an attribute or a default value if it could not be found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string GetAttribute(string name, string defaultValue)
        {
            return GetAttribute(DomData.TokenID(name),defaultValue);
        }
        protected string GetAttribute(short tokenId, string defaultValue)
        {
            string value = null;
            if (tokenId == DomData.ClassAttrID)
            {
                return ClassName;
            }

            if (_Attributes != null
                && Attributes.TryGetValue(tokenId, out value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
        public bool TryGetAttribute(short tokenId, out string value)
        {
            //value = GetNonDictionaryAttribute(name);
            //bool result = (GetNonDictionaryAttribute(name) != null) 
            //     || Attributes.TryGetValue(name.ToLower(), out  value);
            if (tokenId == DomData.ClassAttrID)
            {
                value= ClassName;
                return true;
            }
            if (_Attributes != null)
            {
                return Attributes.TryGetValue(tokenId, out value);
            }
            else
            {
                value = null;
                return false;
            }
        }
        public override bool TryGetAttribute(string name, out string value)
        {
            return TryGetAttribute(DomData.TokenID(name), out value);
        }

        public override CSSStyleDeclaration Style
        {
            get
            {
                if (_Style == null)
                {
                    _Style = new CSSStyleDeclaration();
                    if (_Attributes != null) {
                        setAttributesCallbacks();
                    }
                }
                return _Style;
            }
            protected set
            {
                _Style = value;
                if (_Attributes != null)
                {
                    setAttributesCallbacks();
                }
            }
        }
        // hooks for CSSStyleDeclaration
        protected string getStyle()
        {
            return _Style == null ?
                null :
                Style.ToString(); 
        }
        protected void setStyle(string style)
        {
            Style.SetStyles(style, false);
        }
        protected void setAttributesCallbacks()
        {
            _Attributes.SetStyle = setStyle;
            _Attributes.SetClass = setClassName;
        }
        
        public override DomAttributes Attributes
        {
            get
            {
                if (_Attributes == null)
                {
                    _Attributes = new DomAttributes(this);
                    setAttributesCallbacks();
                }
                return _Attributes;
            }
            protected set
            {
                _Attributes = value;
                setAttributesCallbacks();
            }
        }

        public override string ClassName
        {
            get
            {
                if (HasClasses)
                {
                    string className = "";
                    foreach (short clsId in _Classes)
                    {
                        className += (className == "" ? "" : " ") + DomData.TokenName(clsId);
                    }
                    return className;
                }
                else
                {
                    return "";
                }
            }
            set
            {
                setClassName(value);   
            }
        }
        protected void setClassName(string className)
        {
            {
                if (string.IsNullOrEmpty(className)) {
                    _Classes=null;
                } else {
                    _Classes = new List<short>();
                    foreach (var cls in className.SplitClean(' '))
                    {
                        AddClass(cls);
                    }
                }
            }
        }
        
        /// <summary>
        /// The NodeName for the element, in LOWER CASE.
        /// </summary>
        public override string NodeName
        {
            get
            {
                return DomData.TokenName(nodeNameID);
            }
            set
            {
                if (nodeNameID<0) 
                {
                    nodeNameID = DomData.TokenID(value);
                }
                else
                {
                    throw new Exception("You can't change the tag of an element once it has been created.");
                }
                
            }
        } 
        

        public override string ID
        {
            get
            {
                return GetAttribute(DomData.IDNodeId,String.Empty);
            }
            set
            {
                string id = Attributes[DomData.IDNodeId];
                if (!String.IsNullOrEmpty(id) && !IsDisconnected)
                {
                        Document.RemoveFromIndex(IndexKey("#" + id));
                }
                Attributes[DomData.IDNodeId] = value;
                if (!IsDisconnected)
                {
                    Document.AddToIndex(IndexKey("#" + value), this);
                }
            }
        }

        public override string Value
        {
            get
            {
                return DomData.InputNodeId == nodeNameID &&
                    HasAttribute(DomData.ValueNodeId) ? 
                    Attributes[DomData.ValueNodeId] :
                    null;
            }
            set
            {
                SetAttribute(DomData.ValueNodeId, value);
            }
        }
        /// <summary>
        /// Returns the value of the named attribute
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string this[string attribute]
        {
            get
            {
                return GetAttribute(attribute);
            }
            set
            {
                SetAttribute(attribute, value);
            }
        }
        /// <summary>
        /// Returns text of the inner HTML. When setting, any children will be removed.
        /// </summary>
        public override string InnerHTML
        {
            get
            {
                if (!HasChildren)
                {
                    return String.Empty;
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (IDomObject elm in ChildNodes)
                    {
                        // For node types that cannot have inner HTML, these should all be text nodes and 
                        // we want to return the literal text
                        if (!InnerHtmlAllowed && elm.NodeType == NodeType.TEXT_NODE)
                        {
                            sb.Append(elm.NodeValue);
                        }
                        else
                        {
                            elm.Render(sb);
                        }
                    }
                    return sb.ToString();
                }
            }
            set
            {
                ChildNodes.Clear();
             
                CsQuery csq = CsQuery.Create(value);
                ChildNodes.AddRange(csq.Document.ChildNodes);
            }
        }
        public override string InnerText
        {
            get
            {
                if (!HasChildren)
                {
                    return String.Empty;
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (IDomObject elm in ChildNodes)
                    {
                        if (elm.NodeType == NodeType.TEXT_NODE)
                        {
                            elm.Render(sb);
                        }
                    }
                    return sb.ToString();
                }
            }
            set
            {

                ChildNodes.Clear();

                DomText text = new DomText(System.Web.HttpUtility.HtmlEncode(value));
                ChildNodes.Add(text);
            }
        }

        public override bool Selected
        {
            get
            {
                return HasAttribute(DomData.SelectedAttrId);
            }
        }
        public override bool Checked
        {
            get
            {
                return HasAttribute(DomData.CheckedAttrId);
            }
            set
            {
                SetAttribute(DomData.CheckedAttrId);
            }
        }
        public override bool ReadOnly
        {
            get
            {
                return HasAttribute(DomData.ReadonlyAttrId);
            }
            set
            {
                SetAttribute(DomData.ReadonlyAttrId);
            }
        }
        /// <summary>
        /// Returns the completel HTML for this element and its children
        /// </summary>
        public override string Render()
        {
            StringBuilder sb = new StringBuilder();
            GetHtml(sb,true);
            return sb.ToString();
        }
        public override void Render(StringBuilder sb)
        {
            GetHtml(sb, true);
        }
        /// <summary>
        /// Returns the HTML for this element, ignoring children/innerHTML
        /// </summary>
        public string ElementHtml()
        {
            StringBuilder sb = new StringBuilder();
            GetHtml(sb, false);
            return sb.ToString();
        }
        protected DocType DocType
        {
            get
            {
                if (_DocType == 0 && !IsDisconnected)
                {
                    _DocType = Document.DocType;
                }
                return _DocType;
            }
        }
        private DocType _DocType;
        
        protected void GetHtml(StringBuilder sb, bool includeChildren)
        {
            sb.Append("<");
            sb.Append(NodeName);

            if (_Style != null && Style.Count > 0)
            {
                sb.Append(" style=\"");
                sb.Append(Style.ToString());
                sb.Append("\"");
            }
            if (HasClasses)
            {
                sb.Append(" class=\"");
                sb.Append(ClassName);
                sb.Append("\"");
            }

            if (_Attributes != null)
            {
                foreach (var kvp in _Attributes)
                {
                    if (!String.IsNullOrEmpty(kvp.Value))
                    {
                        //if (DocType== DocType.XHTML || val.IndexOfAny(needsQuoting) >=0) {
                        char quoteChar = kvp.Value.IndexOf("\"") >= 0 ? '\'' : '"';
                        sb.Append(" ");
                        sb.Append(kvp.Key);
                        sb.Append("=");
                        sb.Append(quoteChar);
                        sb.Append(kvp.Value);
                        sb.Append(quoteChar);

                        //} else {
                        //    sb.Append(" " + kvp.Key + "=" + val);
                        // }
                    }
                    else
                    {
                        sb.Append(" ");
                        sb.Append(kvp.Key);
                    }
                }
            }
            if (InnerHtmlAllowed || InnerTextAllowed )
            {
                sb.Append(">");
                sb.Append(includeChildren ?
                    InnerHTML :
                    (HasChildren ? 
                        "..." : 
                        String.Empty));
                sb.Append("</");
                sb.Append(NodeName);
                sb.Append(">");
            }
            else
            {
                if (DocType == DocType.XHTML)
                {
                    sb.Append(" />");
                }
                else
                {
                    sb.Append(" >");
                }
            }
        }
        
        public override string ToString()
        {
            return ElementHtml();
        }

        /// <summary>
        /// This object type can have inner HTML.
        /// </summary>
        /// <returns></returns>
        public override bool InnerHtmlAllowed
        {
            get
            {
                return !DomData.NoInnerHtmlAllowed(nodeNameID);

            }
        }
        public override bool InnerTextAllowed
        {
            get
            {
                return DomData.InnerTextAllowed(nodeNameID);
            }
        }
        public bool IsBlock
        {
            get
            {
                return DomData.IsBlock(nodeNameID);
            }
        }
        public override bool Complete
        {
            get { return nodeNameID >= 0; }
        }
    }
}
