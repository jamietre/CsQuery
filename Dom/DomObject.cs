using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery
{
    public enum NodeType
    {
        ELEMENT_NODE = 1,
        //ATTRIBUTE_NODE =2,
        TEXT_NODE = 3,
        CDATA_SECTION_NODE = 4,
        //ENTITY_REFERENCE_NODE = 5,
        //ENTITY_NODE=  6,
        //PROCESSING_INSTRUCTION_NODE =7,
        COMMENT_NODE = 8,
        DOCUMENT_NODE = 9,
        DOCUMENT_TYPE_NODE = 10,
        //DOCUMENT_FRAGMENT_NODE = 11,
        //NOTATION_NODE  =12
    }

    public interface IDomObject
    {

        IDomContainer ParentNode { get; set; }
        NodeType NodeType { get; }
        string PathID { get; }
        string Path { get; }
        CsQuery Owner { get; set; }
        CsQuery Csq();
        DomRoot Dom { get; }
        string Render();
        void AddToIndex();
        void RemoveFromIndex();
        void Remove();
        IDomObject Clone();
        bool InnerHtmlAllowed { get; }
        bool InnerTextAllowed { get; }
        string InnerHtml { get; set; }
        string InnerText { get; set; }
        bool Complete { get; }
        int DescendantCount();

        //? These are really only part of IDomContainer. However, to avoid awful typecasting all the time, they are part of the interface
        // for objects.

        string ID { get; set; }
        CSSStyleDeclaration Style { get; }
        DomAttributes Attributes { get; }
        string ClassName { get; set; }
        string Value { get; set; }

        string NodeName { get; set; }
        string NodeValue { get; set; }
        IEnumerable<IDomElement> Elements { get; }
        NodeList ChildNodes { get; }
        IDomObject FirstChild { get; }
        IDomObject LastChild { get; }
        void AppendChild(IDomObject element);
        void RemoveChild(IDomObject element);

        void SetAttribute(string name);
        void SetAttribute(string name, string value);
        string GetAttribute(string name);
        string GetAttribute(string name, string defaultValue);
        bool TryGetAttribute(string name, out string value);
        bool HasAttribute(string name);
        bool RemoveAttribute(string name);
        bool HasChildren { get; }
        bool Selected { get; }
        bool Checked { get; set; }
        bool ReadOnly { get; set; }
    }

    public interface IDomSpecialElement : IDomObject
    {
        string NonAttributeData { get; set; }

    }
    /// <summary>
    /// Base abstract class, used only internally
    /// </summary>
    public abstract class DomObject
    {

        public virtual NodeList ChildNodes
        {
            get
            {
                return null;
            }
        }
        public abstract bool HasChildren
        { get; }
        // Unique ID assigned when added to a dom
        public string PathID
        {
            get
            {
                if (_PathID == null)
                {

                    _PathID = (ParentNode == null ? String.Empty : ParentNode.GetNextChildID());
                }
                return _PathID;
            }

        } protected string _PathID = null;
        public string Path
        {
            get
            {
                if (_Path != null)
                {
                    return _Path;
                }
                return (ParentNode == null ? String.Empty : ParentNode.Path + "/") + PathID;
            }
        }
        protected string _Path = null;

        public IDomContainer ParentNode
        {
            get
            {
                return _Parent;
            }
            set
            {
                ResetPath();
                _Parent = value;
            }
        }
        /// <summary>
        /// Erase stored path information. This must be done whenever a node is added to a new DOM.
        /// </summary>
        protected void ResetPath()
        {
            _Path = null;
            _PathID = null;
            // Also must clear values of child nodes
            if (HasChildren)
            {
                foreach (DomObject node in ChildNodes)
                {
                    node.ResetPath();
                }
            }
        }
        protected IDomContainer _Parent = null;
    }


    /// <summary>
    /// Base class for anything that exists in the DOM
    /// </summary>
    /// 
    public abstract class DomObject<T> : DomObject, IDomObject where T : IDomObject, new()
    {
        public DomObject()
        {

        }
        public DomObject(CsQuery owner)
        {
            _Owner = owner;
        }
        /// <summary>
        /// Wraps the element in a CsQuery object
        /// </summary>
        /// <returns></returns>
        public CsQuery Csq()
        {
            return new CsQuery(this);
        }

        public virtual T Clone()
        {
            T clone = new T();
            return clone;
        }

        public abstract bool InnerHtmlAllowed { get; }
        public virtual bool InnerTextAllowed
        {
            get { return InnerHtmlAllowed; }
        }
        public virtual CsQuery Owner
        {
            get
            {
                return _Owner;
            }
            set
            {
                _Owner = value;
                var container = this as IDomContainer;
                if (container != null)
                {
                    if (container.HasChildren)
                    {
                        foreach (IDomObject obj in container.ChildNodes)
                        {
                            obj.Owner = value;
                        }
                    }
                }
            }
        }
        protected CsQuery _Owner = null;
        public virtual DomRoot Dom
        {
            get
            {
                if (Owner != null)
                {
                    return Owner.Dom;
                }
                else
                {
                    return null;
                }
            }
        }

        public virtual string ID
        {
            get
            {
                return null;
            }
            set
            {
                throw new Exception("Cannot set ID for this node type.");
            }
        }
        public virtual string Value
        {
            get
            {
                return null;
            }
            set
            {
                throw new Exception("Cannot set value for this node type.");
            }
        }
        public virtual string ClassName
        {
            get
            {
                return null;
            }
            set
            {
                throw new Exception("ClassName is not applicable to this node type.");
            }
        }
        public virtual DomAttributes Attributes
        {
            get
            {
                throw new Exception("Attributes is not applicable to this node type.");
            }
            protected set
            {
                throw new Exception("Attributes is not applicable to this node type.");
            }
        }
        public virtual CSSStyleDeclaration Style
        {
            get
            {
                throw new Exception("Style is not applicable to this node type.");
            }
        }
        public virtual string NodeName
        {
            get
            {
                return null;
            }
            set
            {
                throw new Exception("You can't change the node name.");
            }
        }
        public virtual string NodeValue
        {
            get
            {
                return null;
            }
            set
            {
                throw new Exception("You can't set NodeValue for this node type.");
            }
        }
        public virtual string InnerHtml
        {
            get
            {
                return String.Empty;
            }
            set
            {
                throw new Exception("Assigning InnerHtml is not valid for this element type.");
            }
        }
        public virtual string InnerText
        {
            get
            {
                return String.Empty;
            }
            set
            {
                throw new Exception("Assigning InnerText is not valid for this element type.");
            }
        }
        // Owner can be null (this is an unbound element)
        // if so create an arbitrary one.

        public abstract NodeType NodeType { get; }
        public abstract bool Complete { get; }
        public abstract string Render();

        protected int IDCount = 0;

        protected IEnumerable<string> IndexKeys()
        {
            DomElement e = this as DomElement;
            if (e == null)
            {
                yield break;
            }
            if (!Complete)
            {
                throw new Exception("This element is incomplete and cannot be added to a DOM.");
            }
            // Add just the element to the index no matter what so we have an ordered representation of the dom traversal
            yield return IndexKey(String.Empty);
            yield return IndexKey(e.NodeName);
            if (!String.IsNullOrEmpty(e.ID))
            {
                yield return IndexKey("#" + e.ID);
            }
            foreach (string cls in e.Classes)
            {
                yield return IndexKey("." + cls);
            }
            //todo -add attributes?
        }
        protected int UniqueID = 0;
        /// <summary>
        /// Remove this element from the DOM
        /// </summary>
        public void Remove()
        {
            if (ParentNode == null)
            {
                throw new Exception("This element has no parent.");
            }
            ParentNode.ChildNodes.Remove(this);
        }
        public void AddToIndex()
        {
            if (Dom != null && this is IDomElement)
            {
                // Fix the path when it's added to the index.
                // This is a little confusing. Would rather that we can't access it until it's added to a DOM.

                _Path = Path;
                foreach (string key in IndexKeys())
                {
                    AddToIndex(key);
                }
                var container = this as IDomContainer;
                if (container != null && container.HasChildren)
                {
                    foreach (IDomObject child in container.ChildNodes)
                    {
                        // Move root in case this is coming from an unmapped or alternate DOM
                        child.Owner = Owner;
                        child.AddToIndex();
                    }
                }
            }
        }
        public void RemoveFromIndex()
        {
            if (Dom == null)
            {
                return;
            }
            var element = this as IDomElement;
            if (element != null)
            {
                var container = this as IDomContainer;
                if (container != null)
                {
                    foreach (DomElement child in container.Elements)
                    {
                        child.RemoveFromIndex();
                    }
                }
                foreach (string key in IndexKeys())
                {
                    RemoveFromIndex(key);
                }
            }
        }
        /// <summary>
        /// Remove only a single index, not the entire object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        public void RemoveFromIndex(string key)
        {
            if (Dom != null)
            {
                Dom.SelectorXref.Remove(key);
            }
        }
        public void AddToIndex(string key)
        {
            if (Dom != null)
            {
                Dom.SelectorXref.Add(key, this as DomElement);
            }
        }
        protected string IndexKey(string key)
        {
            return key + ">" + Path;
        }

        public virtual int DescendantCount()
        {
            return 0;
        }

        public virtual IEnumerable<IDomElement> Elements
        {
            get
            {
                yield break;
            }
        }

        public virtual IDomObject FirstChild
        {
            get { return null; }
        }
        public virtual IDomObject LastChild
        {
            get { return null; }
        }
        public virtual void AppendChild(IDomObject element)
        {
            throw new Exception("This type of element does not have children.");
        }
        public virtual void RemoveChild(IDomObject element)
        {
            throw new Exception("This type of element does not have children.");
        }


        public virtual void SetAttribute(string name)
        {
            return;
        }

        public virtual void SetAttribute(string name, string value)
        {
            return;
        }

        public virtual string GetAttribute(string name)
        {
            return null;
        }

        public virtual string GetAttribute(string name, string defaultValue)
        {
            return null;
        }

        public virtual bool TryGetAttribute(string name, out string value)
        {
            value = null;
            return false;
        }

        public virtual bool HasAttribute(string name)
        {
            return false;
        }

        public virtual bool RemoveAttribute(string name)
        {
            return false;
        }


        public virtual bool Selected
        {
            get { return false; }
        }

        public virtual bool Checked
        {
            get
            {
                return false;
            }
            set
            {
                throw new Exception("Not valid for this element type.");
            }
        }

        public virtual bool ReadOnly
        {
            get
            {
                return true;
            }
            set
            {
                throw new Exception("Not valid for this element type.");
            }
        }

        IDomObject IDomObject.Clone()
        {
            return Clone();
        }
    }
    
}
