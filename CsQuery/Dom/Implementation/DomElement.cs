using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CsQuery.Utility;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.Implementation
{

    /// <summary>
    /// HTML elements
    /// </summary>
    public class DomElement : DomContainer<DomElement>, IDomElement
    {
        #region private fields
        protected DomAttributes _Attributes;
        protected CSSStyleDeclaration _Style;
        protected List<ushort> _Classes;
        #endregion

        #region constructors
        public DomElement()
        {

        }
        public DomElement(string tag)
            : base()
        {
            NodeName = tag;
        }
        #endregion
        
        #region public properties
        public override CSSStyleDeclaration Style
        {
            get
            {
                if (_Style == null)
                {
                    _Style = new CSSStyleDeclaration(this);
                }
                return _Style;
            }
            protected set
            {
                _Style = value;
            }
        }
        public override DomAttributes Attributes
        {
            get
            {
                if (_Attributes == null)
                {
                    _Attributes = new DomAttributes(this);
                }
                return _Attributes;
            }
            protected set
            {
                _Attributes = value;
            }
        }
        public override string ClassName
        {
            get
            {
                if (HasClasses)
                {
                    string className = "";
                    foreach (ushort clsId in _Classes)
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
        public override string Id
        {
            get
            {
                return GetAttribute(DomData.IDAttrId, String.Empty);
            }
            set
            {
                // ID is stored as an attribute internally so the index is OK for attribute searches
                if (Attributes.ContainsKey(DomData.IDAttrId) && !IsDisconnected)
                {
                    Document.RemoveFromIndex(IndexKey("#", Attributes[DomData.IDAttrId]));
                }
                Attributes.SetRaw(DomData.IDAttrId, value);
                if (!IsDisconnected)
                {
                    if (value != null)
                    {
                        Document.AddToIndex(IndexKey("#", value), this);

                        // Must index the attributes for search just on attribute too
                        //Document.AddToIndex(Attributes.IndexKey(DomData.IDAttrId), this);
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
                if (nodeNameID < 1)
                {
                    nodeNameID = DomData.TokenID(value, true);
                }
                else
                {
                    throw new InvalidOperationException("You can't change the tag of an element once it has been created.");
                }
            }
        }
        public override string DefaultValue
        {
            get
            {
                return hasDefaultValue() ?
                    (NodeName == "textarea" ? InnerText : Attributes["value"]) :
                    base.DefaultValue;
            }
            set
            {
                if (!hasDefaultValue())
                {
                    base.DefaultValue = value;
                }
                else
                {
                    if (NodeName == "textarea")
                    {
                        InnerText = value;
                    }
                    else
                    {
                        Attributes["value"] = value;
                    }
                }
            }
        }

        
        /// <summary>
        /// Value property for some node types (input,textarea)
        /// </summary>
        public override string Value
        {
            get
            {
                return DomData.InputNodeId == nodeNameID &&
                    HasAttribute(DomData.ValueAttrId) ?
                    Attributes[DomData.ValueAttrId] :
                    null;
            }
            set
            {
                SetAttribute(DomData.ValueAttrId, value);
            }
        }
        public override NodeType NodeType
        {
            get { return NodeType.ELEMENT_NODE; }
        }
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

        public override bool HasAttributes
        {
            get
            {
                return _Attributes != null && _Attributes.HasAttributes;
            }
        }
        public override bool HasStyles
        {
            get
            {
                return _Style != null && _Style.Count > 0;
            }
        }
        public override bool HasClasses
        {
            get
            {
                return _Classes != null && _Classes.Count > 0;
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
        public override bool IsIndexed
        {
            get
            {
                return true;
            }
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
        /// <summary>
        /// True if this element is valid (it needs a tag only)
        /// </summary>
        public override bool Complete
        {
            get { return nodeNameID >= 0; }
        }
        /// <summary>
        /// Returns the value of the named attribute
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string this[string attribute]
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
        public override IDomObject this[int index]
        {
            get
            {
                return ChildNodes[index];
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
                SetAttribute(DomData.CheckedAttrId, value ? "" : null);
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
                SetAttribute(DomData.ReadonlyAttrId, value ? "" : null);
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
                    base.Render(sb, Document == null ? CQ.DefaultDomRenderingOptions : Document.DomRenderingOptions);
                    return sb.ToString();
                }
            }
            set
            {
                if (!InnerHtmlAllowed)
                {
                    throw new InvalidOperationException(String.Format("You can't set the innerHTML for a {0} element.", NodeName));
                }
                ChildNodes.Clear();

                CQ csq = CQ.Create(value);
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
                if (!InnerTextAllowed)
                {
                    throw new InvalidOperationException(String.Format("You can't set the innerHTML for a {0} element.", NodeName));
                }
                IDomText text;
                if (!InnerHtmlAllowed)
                {
                    text = new DomInnerText(value);
                }
                else
                { 
                    text = new DomText(value);
                }
                ChildNodes.Clear();
                ChildNodes.Add(text);
            }
        }
        
        /// <summary>
        /// The index excluding text nodes
        /// </summary>
        public int ElementIndex
        {
            get
            {
                int index = -1;
                IDomElement el = this;
                while (el != null)
                {
                    el = el.PreviousElementSibling;
                    index++;
                }
                return index;
            }
        }

        /// <summary>
        /// The object to which this index refers
        /// </summary>
        public IDomObject IndexReference
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Returns true if this element is a block-trpe element
        /// </summary>
        public bool IsBlock
        {
            get
            {
                return DomData.IsBlock(nodeNameID);
            }
        }

        /// <summary>
        /// All class names present for this element
        /// </summary>
        public IEnumerable<string> Classes
        {
            get
            {
                foreach (var id in _Classes)
                {
                    yield return DomData.TokenName(id);
                }
            }
        }
        #endregion
        
        #region public methods
        public void Reindex()
        {
            PathID = null;
            Index = 0;
        }

        /// <summary>
        /// Returns the completel HTML for this element and its children
        /// </summary>
        public override void Render(StringBuilder sb, DomRenderingOptions options)
        {
            GetHtml(options, sb, true);
        }
        /// <summary>
        /// Returns the HTML for this element, but ignoring children/innerHTML
        /// </summary>
        public string ElementHtml()
        {
            StringBuilder sb = new StringBuilder();
            GetHtml(Document == null ? CQ.DefaultDomRenderingOptions : Document.DomRenderingOptions, sb, false);
            return sb.ToString();
        }

        /// <summary>
        /// Returns all the keys that should be in the index for this item (keys for class, tag, attributes, and id)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> IndexKeys()
        {
            string path = Path;
            yield return ""+DomData.indexSeparator+path;
            yield return IndexKey("+",nodeNameID, path);
            string id = Id;
            if (!String.IsNullOrEmpty(id))
            {
                yield return IndexKey("#" ,DomData.TokenID(id), path);
            }
            if (HasClasses)
            {
                foreach (ushort clsId in _Classes)
                {
                    yield return IndexKey(".", clsId, path);
                }
            }
            if (HasAttributes)
            {
                foreach (ushort attrId in Attributes.GetAttributeIds())
                {
                    yield return IndexKey("!", attrId, path);
                }
            }
        }
       
        public override DomElement Clone()
        {
            var clone = new DomElement();
            clone.nodeNameID = nodeNameID;

            if (HasAttributes)
            {
                clone.Attributes = Attributes.Clone(clone);
            }
            if (HasClasses)
            {
                clone._Classes = new List<ushort>(_Classes);
            }
            if (HasStyles)
            {
                clone.Style = Style.Clone(clone);
            }
            // will not create ChildNodes lazy object unless results are returned (this is why we don't use AddRange)
            foreach (IDomObject child in CloneChildren())
            {
                clone.ChildNodes.Add(child);
            }

            return clone;
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
        public bool HasStyle(string name)
        {
            return HasStyles &&
                Style.HasStyle(name);
        }
        public bool HasClass(string name)
        {
            return HasClasses 
                && _Classes.Contains(DomData.TokenID(name));
        }
        public bool AddClass(string name)
        {
            bool result=false;
            bool addedFirstClass = false;

            foreach (string cls in name.SplitClean())
            {
                if (!HasClass(cls))
                {
                    if (_Classes == null)
                    {
                        _Classes = new List<ushort>();
                    }
                    ushort tokenId = DomData.TokenID(cls);
                    addedFirstClass = !HasClasses;
                    _Classes.Add(tokenId);
                    if (!IsDisconnected)
                    {
                        Document.AddToIndex(IndexKey(".",tokenId), this);
                    }
                    
                    result = true;
                }
                if (addedFirstClass && !IsDisconnected)
                {
                    // Must index the attributes for search just on attribute too
                    Document.AddToIndex(Attributes.IndexKey(DomData.ClassAttrId),this);
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
                    ushort tokenId = DomData.TokenID(cls);
                    _Classes.Remove(tokenId);
                    if (!IsDisconnected)
                    {
                        Document.RemoveFromIndex(IndexKey(".",tokenId));
                        if (!HasClasses)
                        {
                            Document.RemoveFromIndex(Attributes.IndexKey(DomData.ClassAttrId));
                        }
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
            Style.AddStyles(style, strict);
        }
        public bool RemoveStyle(string name)
        {
            return _Style != null ? Style.Remove(name) : false;
        }
        protected bool HasAttribute(ushort nodeId)
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
        public override void SetAttribute(string name, string value)
        {
            Attributes[name] = value;
        }

        protected void SetAttribute(ushort tokenId, string value)
        {
            if (tokenId == DomData.ClassAttrId)
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
            Attributes.SetBooleanAttribute(name);
            
        }
        public void SetAttribute(ushort tokenId)
        {
            SetAttribute(tokenId, String.Empty);
        }
        public override bool RemoveAttribute(string name)
        {
            if (_Attributes == null)
            {
                return false;
            }
            ushort tokenId = DomData.TokenID(name,true);
            if (tokenId == DomData.ClassAttrId)
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
            return GetAttribute(DomData.TokenID(name,true),defaultValue);
        }
        protected string GetAttribute(ushort tokenId, string defaultValue)
        {
            string value = null;
            if (tokenId == DomData.ClassAttrId)
            {
                return ClassName;
            }

            if (_Attributes != null
                && Attributes.TryGetValue(tokenId, out value))
            {
                //IMPORTANT: Even though we need to distinguish between null and empty string values internally to
                // render the same way it was brought over (e.g. either "checked" or "checked=''") --- accessing the
                // attribute value is never null for attributes that exist.
                return value ?? "";
            }
            else
            {
                return defaultValue;
            }
        }
        public bool TryGetAttribute(ushort tokenId, out string value)
        {
            //value = GetNonDictionaryAttribute(name);
            //bool result = (GetNonDictionaryAttribute(name) != null) 
            //     || Attributes.TryGetValue(name.ToLower(), out  value);
            if (tokenId == DomData.ClassAttrId)
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
            return TryGetAttribute(DomData.TokenID(name,true), out value);
        }

        public override string ToString()
        {
            return ElementHtml();
        }

        #endregion

        #region private methods
        protected void setClassName(string className)
        {
            {
                if (string.IsNullOrEmpty(className))
                {
                    _Classes = null;
                }
                else
                {
                    _Classes = new List<ushort>();
                    foreach (var cls in className.SplitClean(' '))
                    {
                        AddClass(cls);
                    }
                }
            }
        }
        protected bool hasDefaultValue()
        {
            return NodeName == "input" || NodeName == "textarea";
        }
        internal string IndexKey(string prefix, ushort keyTokenId)
        {
            return IndexKey(prefix, keyTokenId, Path);
        }
        internal string IndexKey(string prefix, string key)
        {
            return IndexKey(prefix, key, Path);
        }
        internal string IndexKey(string prefix, string key, string path)
        {
#if DEBUG_PATH
            return prefix + key + DomData.indexSeparator + path;
#else
            return IndexKey(prefix, DomData.TokenID(key), path);
#endif
        }
        internal string IndexKey(string prefix, ushort keyTokenId, string path)
        {
#if DEBUG_PATH
            return prefix + DomData.TokenName(keyTokenId) + DomData.indexSeparator + path;
#else
            return prefix + (char)keyTokenId + DomData.indexSeparator + path;
#endif
        }
    
        protected void GetHtml(DomRenderingOptions options, StringBuilder sb, bool includeChildren)
        {
            bool quoteAll = options.HasFlag(DomRenderingOptions.QuoteAllAttributes);

            sb.Append("<");
            sb.Append(NodeName);
            // put ID first. Must use GetAttribute since the Id property defaults to ""
            string id = GetAttribute(DomData.IDAttrId,null);
            
            if (id != null)
            {
                sb.Append(" ");
                RenderAttribute(sb, "id", id, quoteAll);
            }
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
                    if (kvp.Key != "id")
                    {
                        sb.Append(" ");
                        RenderAttribute(sb, kvp.Key, kvp.Value, quoteAll);
                    }
                }
            }
            if (InnerHtmlAllowed || InnerTextAllowed )
            {
                sb.Append(">");
                if (includeChildren)
                {
                    base.Render(sb, options);
                }
                else
                {
                    sb.Append(HasChildren ?
                            "..." :
                            String.Empty);
                }
                sb.Append("</");
                sb.Append(NodeName);
                sb.Append(">");
            }
            else
            {

                if ((Document == null ? CQ.DefaultDocType : Document.DocType)== DocType.XHTML)
                {
                    sb.Append(" />");
                }
                else
                {
                    sb.Append(">");
                }
            }
        }
        /// <summary>
        /// TODO this really should be in Attributes
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected void RenderAttribute(StringBuilder sb, string name, string value, bool quoteAll)
        {
            if (value != null)
            {
                string quoteChar;
                string attrText = Objects.AttributeEncode(value,
                    quoteAll,
                    out quoteChar);
                sb.Append(name);
                sb.Append("=");
                sb.Append(quoteChar);
                sb.Append(attrText);
                sb.Append(quoteChar);
            }
            else
            {
                sb.Append(name);
            }
        }
        #endregion
    }
}
