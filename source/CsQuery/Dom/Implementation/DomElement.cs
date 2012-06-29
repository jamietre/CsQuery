using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CsQuery.StringScanner;
using CsQuery.HtmlParser;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.Implementation
{

    /// <summary>
    /// HTML elements
    /// </summary>
    public class DomElement : DomContainer<DomElement>, IDomElement, IAttributeCollection, ICSSStyleDeclaration
    {
        #region private fields
        private AttributeCollection _DomAttributes;
        protected CSSStyleDeclaration _Style;
        protected List<ushort> _Classes;

        protected AttributeCollection DomAttributes
        {
            get
            {
                if (_DomAttributes == null)
                {
                    _DomAttributes = new AttributeCollection();
                }
                return _DomAttributes;
            }
        }
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
        public DomElement(ushort tagId)
            : base()
        {
            _NodeNameID = tagId;
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

        /// <summary>
        /// Access via the IAttributeCollection interface to attributes. We don't actually refer to the inner AttributeCollection object
        /// here because we cannot allow users to set attributes directly in the object: they must use SetAttribute so that special
        /// handling for "class" and "style" as well as indexing can be performed. To avoid creating a wrapper object, simply pass bac
        /// a reference to ourselves.
        /// </summary>
        public override IAttributeCollection Attributes
        {
            get
            {
                return this;
            }
        }

        public override string ClassName
        {
            get
            {
                if (HasClasses)
                {
                    //return String.Join(" ", _Classes.Select(item=>DomData.TokenName(item)));
                    string className = "";
                    foreach (ushort clsId in _Classes)
                    {
                        className += (className == "" ? "" : " ") + HtmlData.TokenName(clsId);
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
                SetClassName(value);
            }
        }
        public override string Id
        {
            get
            {
                return GetAttribute(HtmlData.IDAttrId, String.Empty);
            }
            set
            {
                if (!IsDisconnected)
                {
                    if (DomAttributes.ContainsKey(HtmlData.IDAttrId))
                    {
                        Document.RemoveFromIndex(IndexKey("#", HtmlData.TokenIDCaseSensitive(Id), Path));
                    }
                    if (value != null)
                    {
                        Document.AddToIndex(IndexKey("#", HtmlData.TokenIDCaseSensitive(value), Path), this);
                    }
                }
                SetAttributeRaw(HtmlData.IDAttrId, value);
            }
        }

        /// <summary>
        /// The NodeName for the element (upper case).
        /// </summary>
        public override string NodeName
        {
            get
            {
                return HtmlData.TokenName(_NodeNameID).ToUpper();
            }
            set
            {
                if (_NodeNameID < 1)
                {
                    _NodeNameID = HtmlData.TokenID(value);
                }
                else
                {
                    throw new InvalidOperationException("You can't change the tag of an element once it has been created.");
                }
            }
        }
        /// <summary>
        /// TODO: in HTML5 type can be used on OL attributes (and maybe others?) and its value is
        /// case sensitive. The Type of input elements is always lower case, though. This behavior
        /// needs to be verified against the spec
        /// </summary>
        public override string Type
        {
            get
            {
                return NodeName=="INPUT" ?
                    GetAttribute("type","text").ToLower() :
                    GetAttribute("type");
            }
            set
            {
                SetAttribute("type", value);
            }
        }

        /// <summary>
        /// For certain elements, the Name. TODO: Verify attribute is applicable.
        /// </summary>
        public override string Name
        {
            get
            {
                return GetAttribute("name");
            }
            set
            {
                SetAttribute("name", value);
            }
        }


        public override string DefaultValue
        {
            get
            {
                return hasDefaultValue() ?
                    (NodeNameID == HtmlData.tagTEXTAREA ? 
                        InnerText : 
                        GetAttribute("value")) :
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
                    if (NodeNameID == HtmlData.tagTEXTAREA)
                    {
                        InnerText = value;
                    }
                    else
                    {
                        SetAttribute("value",value);
                    }
                }
            }
        }

        
        /// <summary>
        /// Value property for input node types
        /// </summary>
        public override string Value
        {
            get
            {
                return HtmlData.tagINPUT == _NodeNameID &&
                    HasAttribute(HtmlData.ValueAttrId) ?
                        GetAttribute(HtmlData.ValueAttrId) :
                        null;
            }
            set
            {
                SetAttribute(HtmlData.ValueAttrId, value);
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
                return _DomAttributes != null && DomAttributes.HasAttributes;
            }
        }
        public override bool HasStyles
        {
            get
            {
                return _Style != null && _Style.HasStyles;
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
                return !HtmlData.HtmlChildrenNotAllowed(_NodeNameID);

            }
        }
        public override bool InnerTextAllowed
        {
            get
            {
                return HtmlData.InnerTextAllowed(_NodeNameID);
            }
        }
        /// <summary>
        /// True if this element is valid (it needs a tag only)
        /// </summary>
        public override bool Complete
        {
            get { return _NodeNameID >= 0; }
        }

        /// <inheritdoc />
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
                return HasAttribute(HtmlData.SelectedAttrId);
            }
        }
        public override bool Checked
        {
            get
            {
                return HasAttribute(HtmlData.CheckedAttrId);
            }
            set
            {
                SetAttribute(HtmlData.CheckedAttrId, value ? "" : null);
            }
        }
        public override bool ReadOnly
        {
            get
            {
                return HasAttribute(HtmlData.ReadonlyAttrId);
            }
            set
            {
                SetAttribute(HtmlData.ReadonlyAttrId, value ? "" : null);
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

                CQ csq = CQ.CreateFragment(value);
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
                return HtmlData.IsBlock(_NodeNameID);
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
                    yield return HtmlData.TokenName(id);
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
            yield return "" + HtmlData.indexSeparator + path;
            yield return IndexKey("+",_NodeNameID, path);
            string id = Id;
            if (!String.IsNullOrEmpty(id))
            {
                yield return IndexKey("#", HtmlData.TokenIDCaseSensitive(id), path);
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
                foreach (ushort attrId in DomAttributes.GetAttributeIds())
                {
                    yield return IndexKey("!", attrId, path);
                }
            }
        }
       
        public override DomElement Clone()
        {
            var clone = new DomElement();
            clone._NodeNameID = _NodeNameID;

            if (HasAttributes)
            {
                clone._DomAttributes = DomAttributes.Clone();
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
                clone.ChildNodes.AddAlways(child);
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
        public override bool HasStyle(string name)
        {
            return HasStyles &&
                Style.HasStyle(name);
        }
        public override bool HasClass(string name)
        {
            return HasClasses
                && _Classes.Contains(HtmlData.TokenIDCaseSensitive(name));
        }

        public override bool AddClass(string name)
        {
            bool result=false;
            bool hadClasses = HasClasses;

            foreach (string cls in name.SplitClean(CharacterData.charsHtmlSpaceArray))
            {
                
                if (!HasClass(cls))
                {
                    if (_Classes == null)
                    {
                        _Classes = new List<ushort>();
                    }
                    ushort tokenId = HtmlData.TokenIDCaseSensitive(cls);
                    
                    _Classes.Add(tokenId);
                    if (!IsDisconnected)
                    {
                        Document.AddToIndex(IndexKey(".",tokenId), this);
                    }
                    
                    result = true;
                }
            }
            if (result && !hadClasses && !IsDisconnected)
            {
                // Must index the attributes for search just on attribute too
                Document.AddToIndex(AttributeIndexKey(HtmlData.ClassAttrId), this);
            }
            return result;
        }
        public override bool RemoveClass(string name)
        {
            bool result = false;
            bool hasClasses = HasClasses;
            foreach (string cls in name.SplitClean())
            {
                if (HasClass(cls))
                {
                    ushort tokenId = HtmlData.TokenIDCaseSensitive(cls);
                    _Classes.Remove(tokenId);
                    if (!IsDisconnected)
                    {
                        Document.RemoveFromIndex(IndexKey(".",tokenId));
                    }

                    result = true;
                }
            }
            if (!HasClasses && hasClasses && !IsDisconnected)
            {
                Document.RemoveFromIndex(AttributeIndexKey(HtmlData.ClassAttrId));
            }

            return result;
        }

       
        protected bool HasAttribute(ushort tokenId)
        {
            switch (tokenId)
            {
                case HtmlData.ClassAttrId:
                    return HasClasses;
                case HtmlData.tagSTYLE:
                    return HasStyles;
                default:
                    return _DomAttributes != null
                        && DomAttributes.ContainsKey(tokenId);
            }
        }
        public override bool HasAttribute(string name)
        {
            return HasAttribute(HtmlData.TokenID(name));
        }
        
        public override void SetAttribute(string name, string value)
        {
            SetAttribute(HtmlData.TokenID(name), value);
        }

        protected void SetAttribute(ushort tokenId, string value)
        {
            switch (tokenId)
            {
                case HtmlData.ClassAttrId:
                    ClassName = value;
                    return;
                case HtmlData.IDAttrId:
                    Id = value;
                    break;
                case HtmlData.tagSTYLE:
                    Style.SetStyles(value, false);
                    return;
                default:
                    // Uncheck any other radio buttons
                    if (tokenId == HtmlData.CheckedAttrId
                        && _NodeNameID == HtmlData.tagINPUT
                        && Type == "radio"
                        && !String.IsNullOrEmpty(Name)
                        && value != null
                        && Document != null)
                    {
                        var radios = Document.QuerySelectorAll("input[type='radio'][name='" + Name + "']:checked");
                        foreach (var item in radios)
                        {
                            item.Checked = false;
                        }
                    }
                    break;
            }

            SetAttributeRaw(tokenId, value);

        }
        /// <summary>
        /// Sets an attribute with no value
        /// </summary>
        /// <param name="name"></param>
        public override void SetAttribute(string name)
        {
            SetAttribute(HtmlData.TokenID(name));
            
        }
        public void SetAttribute(ushort tokenId)
        {
            AttributeAddToIndex(tokenId);
            DomAttributes.SetBoolean(tokenId);
        }

        /// <summary>
        /// Used by DomElement to (finally) set the ID value
        /// </summary>
        /// <param name="tokenId"></param>
        /// <param name="value"></param>
        protected void SetAttributeRaw(ushort tokenId, string value)
        {
            if (value == null)
            {
                DomAttributes.Unset(tokenId);
                AttributeRemoveFromIndex(tokenId);
            }
            else
            {
                AttributeAddToIndex(tokenId);
                DomAttributes[tokenId] = value;
            }
        }
        public override bool RemoveAttribute(string name)
        {
            return RemoveAttribute(HtmlData.TokenID(name));

        }
        protected bool RemoveAttribute(ushort tokenId)
        {
            switch (tokenId)
            {
                case HtmlData.ClassAttrId:
                    if (HasClasses)
                    {
                        SetClassName(null);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case HtmlData.IDAttrId:
                    if (DomAttributes.ContainsKey(HtmlData.IDAttrId))
                    {
                        Id = null;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case HtmlData.tagSTYLE:
                    if (HasStyles)
                    {
                        foreach (var style in Style.Keys)
                        {
                            Style.Remove(style);
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
            }

            if (_DomAttributes == null)
            {
                return false;
            }

            bool success = DomAttributes.Remove(tokenId);
            if (success)
            {
                AttributeRemoveFromIndex(tokenId);
            }
            return success;
            
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
        protected string GetAttribute(ushort tokenId)
        {
            return GetAttribute(tokenId, null);
        }
        /// <summary>
        /// Return an attribute value identified by name. If it doesn't exist, return the provided
        /// default value.
        /// </summary>
        /// <param name="name">The attribute name</param>
        /// <returns></returns>
        public override string GetAttribute(string name, string defaultValue)
        {
            return GetAttribute(HtmlData.TokenID(name), defaultValue);
        }

        /// <summary>
        /// Return an attribute value identified by a token ID. If it doesn't exist, return the provided
        /// default value.
        /// </summary>
        /// <param name="tokenId"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected string GetAttribute(ushort tokenId, string defaultValue)
        {

            string value = null;
            if (TryGetAttribute(tokenId, out value))
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
            switch (tokenId)
            {
                case HtmlData.ClassAttrId:
                    value = ClassName;
                    return true;
                case HtmlData.tagSTYLE:
                    value = Style.ToString();
                    return true;
                default:
                    if (_DomAttributes != null) {
                        return DomAttributes.TryGetValue(tokenId, out value);
                    }
                    value = null;
                    return false;
            }
        }
        public override bool TryGetAttribute(string name, out string value)
        {
            return TryGetAttribute(HtmlData.TokenID(name), out value);
        }

        public override string ToString()
        {
            return ElementHtml();
        }

        // ICSSStyleDeclations
        /// <summary>
        /// Add a single style in the form "styleName: value"
        /// </summary>
        /// <param name="style"></param>
        public override void AddStyle(string style)
        {
            AddStyle(style, true);
        }
        public void AddStyle(string style, bool strict)
        {
            Style.AddStyles(style, strict);
        }

        public override bool RemoveStyle(string name)
        {
            return _Style != null ? _Style.RemoveStyle(name) : false;
        }
        public void SetStyles(string styles)
        {
            SetStyles(styles, true);
        }
        public void SetStyles(string styles, bool strict)
        {
            Style.SetStyles(styles, strict);
        }

        public void SetStyle(string name, string value)
        {
            throw new NotImplementedException();
        }

        public void SetStyle(string name, string value, bool strict)
        {
            throw new NotImplementedException();
        }

        public string GetStyle(string name)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region private methods

        public string AttributeIndexKey(string attrName)
        {
            return AttributeIndexKey(HtmlData.TokenID(attrName));
        }
        public string AttributeIndexKey(ushort attrId)
        {
#if DEBUG_PATH
            return "!" + DomData.TokenName(attrId) + DomData.indexSeparator + Owner.Path;
#else
            return "!" + (char)attrId + HtmlData.indexSeparator + Path;
#endif
        }
        protected void AttributeRemoveFromIndex(ushort attrId)
        {
            if (!IsDisconnected)
            {
                Document.RemoveFromIndex(AttributeIndexKey(attrId));
            }
        }
        protected void AttributeAddToIndex(ushort attrId)
        {
            if (!IsDisconnected && !DomAttributes.ContainsKey(attrId))
            {
                
                Document.AddToIndex(AttributeIndexKey(attrId), this);
            }
        }

        protected void SetClassName(string className)
        {
            
            if (HasClasses) {
                foreach (var cls in Classes.ToList())
                {
                    RemoveClass(cls);
                }
            }
            if (!string.IsNullOrEmpty(className)) 
            {
                AddClass(className);
            }    
        }


        protected bool hasDefaultValue()
        {
            return NodeNameID == HtmlData.tagINPUT || NodeNameID == HtmlData.tagTEXTAREA;
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
            return IndexKey(prefix, HtmlData.TokenID(key), path);
#endif
        }
        internal string IndexKey(string prefix, ushort keyTokenId, string path)
        {
#if DEBUG_PATH
            return prefix + DomData.TokenName(keyTokenId) + DomData.indexSeparator + path;
#else
            return prefix + (char)keyTokenId + HtmlData.indexSeparator + path;
#endif
        }
    
        protected void GetHtml(DomRenderingOptions options, StringBuilder sb, bool includeChildren)
        {
            bool quoteAll = options.HasFlag(DomRenderingOptions.QuoteAllAttributes);

            sb.Append("<");
            string nodeName = NodeName.ToLower();
            sb.Append(nodeName);
            // put ID first. Must use GetAttribute since the Id property defaults to ""
            string id = GetAttribute(HtmlData.IDAttrId, null);
            
            if (id != null)
            {
                sb.Append(" ");
                RenderAttribute(sb, "id", id, quoteAll);
            }
            if (HasStyles)
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

            if (_DomAttributes != null)
            {
                foreach (var kvp in _DomAttributes)
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
                sb.Append(nodeName);
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
                string attrText = HtmlData.AttributeEncode(value,
                    quoteAll,
                    out quoteChar);
                sb.Append(name.ToLower());
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

        #region explicit members for IAttributesCollection

        string IAttributeCollection.this[string attributeName]
        {
            get
            {
                return GetAttribute(attributeName);
            }
            set
            {
                SetAttribute(attributeName, value);
            }
        }

        int IAttributeCollection.Length
        {
            get {
                int otherAttributes = (HasClasses ? 1 : 0) + (HasStyles ? 1 : 0);

                return otherAttributes + (!HasAttributes ? 0 :
                    DomAttributes.Count);
            }
        }

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            return AttributesCollection().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return AttributesCollection().GetEnumerator() ;
        }        

        /// <summary>
        /// Enumerate the attributes + class & style
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<KeyValuePair<string, string>> AttributesCollection()
        {
            if (HasClasses)
            {
                yield return new KeyValuePair<string, string>("class", ClassName);
            }
            if (HasStyles)
            {
                yield return new KeyValuePair<string, string>("style", Style.ToString());
            }
            foreach (var kvp in DomAttributes)
            {
                yield return kvp;
            }
        }
        
        #endregion



    }
}
