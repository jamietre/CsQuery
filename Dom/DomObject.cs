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
        DOCUMENT_FRAGMENT_NODE = 11,
        //NOTATION_NODE  =12
    }

    public interface IDomObject
    {

        IDomContainer ParentNode { get; }
        NodeType NodeType { get; }

        //CsQuery Owner { get; }
        
        CsQuery Csq();
        IDomRoot Document { get; }
        string Render();
        void Render(StringBuilder sb);

        void Remove();
        IDomObject Clone();
        
        string InnerHTML { get; set; }
        string InnerText { get; set; }

        //? These are really only part of IDomContainer. However, to avoid awful typecasting all the time, they are part of the interface
        // for objects.

        string ID { get; set; }
        CSSStyleDeclaration Style { get; }
        DomAttributes Attributes { get; }
        string ClassName { get; set; }
        string Value { get; set; }

        string NodeName { get; set; }
        string NodeValue { get; set; }
        IEnumerable<IDomElement> ChildElements { get; }
        NodeList ChildNodes { get; }
        IDomObject FirstChild { get; }
        IDomObject LastChild { get; }
        IDomObject NextSibling { get; }
        IDomObject PreviousSibling { get; }
        IDomElement NextElementSibling { get; }
        IDomElement PreviousElementSibling { get; }
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

        // Nonstandard elements
        bool InnerHtmlAllowed { get; }
        bool InnerTextAllowed { get; }
        bool Complete { get; }
        int DescendantCount();

        int Index { get; }
        string PathID { get; }
        string Path { get; }
        bool IsDisconnected { get; }
        IEnumerable<string> IndexKeys();
        //int Ordinal { get; }
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
        public abstract NodeType NodeType { get; }
        public virtual NodeList ChildNodes
        {
            get
            {
                return null;
            }
        }
        public abstract bool HasChildren
        { get; }

        public virtual IEnumerable<string> IndexKeys()
        {
            return Objects.EmptyEnumerable<string>();
        }
      

        //protected string _Path = null;

        public virtual IDomContainer ParentNode
        {
            get
            {
                return _ParentNode;
            }
            internal set
            {
                _ParentNode = value;
            }
        }
        private IDomContainer _ParentNode = null;

        /// <summary>
        /// The element is not associated with an IDomRoot
        /// </summary>
        public bool IsDisconnected
        {
            get
            {
                return Document == null;
            }
        }
        public abstract int Index { get; internal set; }
        
        /// <summary>
        /// Unique ID assigned when added to a dom. This is not the full path but just the ID at this level. The full
        /// path is never stored with each node to prevent having to regenerate if node trees are moved. 
        /// </summary>
        public virtual string PathID
        {
            get
            {
                // Don't actually store paths with non-element nodes as they aren't indexed and don't have children.
                // Fast read access is less important than not having to reset them when moved.
                return PathEncode(Index);
            }
        } 
        protected string _PathID = null;


        public virtual IDomRoot Document
        {
            get
            {
                return ParentNode == null ? null : ParentNode.Document;
            }
        }
        protected IDomRoot _Document;
        internal int _Index = -1;

        private static char[] chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToArray();
        protected string PathEncode(int number)
        {
            return number.ToString().PadLeft(3, '0');
        }
        
        protected string Base62Code(int number)
        {
            int ks_len = chars.Length;
            string sc_result = "";
            int num_to_encode = number;
            int i = 0;
            do
            {
                i++;
                sc_result = chars[(num_to_encode % ks_len)] + sc_result;
                num_to_encode = ((num_to_encode - (num_to_encode % ks_len)) / ks_len);
            }
            while (num_to_encode != 0);
            return sc_result.PadLeft(3, '0');
        }
        
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
        
        //public DomObject(CsQuery owner)
        //{
        //    _Owner = owner;
        //}
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
            //if (NodeType != NodeType.DOCUMENT_NODE)
            //{
            //    DomRoot cloneRoot = new DomDetached(Owner.Dom);
            //    clone.ParentNode = cloneRoot;
            //}
            
            return clone;
        }
        /// <summary>
        /// The element's absolute index among its siblings
        /// </summary>
        /// <returns></returns>
        public override int Index
        {
            get
            {
                if (_Index<0) {
                    if (ParentNode != null)
                    {
                        _Index = ParentNode.ChildNodes.IndexOf(this);
                    }
                    else
                    {
                        _Index = 0;
                    }
                }
                return _Index;
            }
            internal set
            {
                _Index = value;
                _PathID = null;
            }
        }

        /// <summary>
        /// The full path to this node. This is calculated by requesting the parent path and adding its own ID.
        /// </summary>
        public virtual string Path
        {
            get
            {
                return ParentNode==null ? "_" + PathID : ParentNode.Path + "/" + PathID;            
            }
        }

        public abstract bool InnerHtmlAllowed { get; }
        public virtual bool InnerTextAllowed
        {
            get { return InnerHtmlAllowed; }
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
            protected set
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
        protected short nodeNameID = -1;

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
        public virtual string InnerHTML
        {
            get
            {
                throw new Exception("Accessing InnerHtml is not valid for this element type.");
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
                throw new Exception("Accessing InnerText is not valid for this element type.");
            }
            set
            {
                throw new Exception("Assigning InnerText is not valid for this element type.");
            }
        }
        // Owner can be null (this is an unbound element)
        // if so create an arbitrary one.

        
        public abstract bool Complete { get; }
        public abstract string Render();
        public virtual void Render(StringBuilder sb)
        {
            sb.Append(Render());
        }

        protected int IDCount = 0;

      
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

        public virtual int DescendantCount()
        {
            return 0;
        }

        public virtual IEnumerable<IDomElement> ChildElements
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

        public IDomObject NextSibling
        {
            get
            {
                return ParentNode!= null && ParentNode.ChildNodes.Count-1 > Index ?
                    ParentNode.ChildNodes[Index + 1] :
                    null;
            }
        }
        public IDomObject PreviousSibling
        {
            get
            {
                return ParentNode != null && Index>0 ?
                    ParentNode.ChildNodes[Index - 1] :
                    null;
            }
        }
        public IDomElement NextElementSibling
        {
            get
            {
                int curIndex = Index;
                var elements = ParentNode.ChildNodes;
                while (++curIndex < elements.Count)
                {
                    if (elements[curIndex].NodeType == NodeType.ELEMENT_NODE)
                    {
                        return (IDomElement)elements[curIndex];
                    }
                }
                return null;
            }
        }
        public IDomElement PreviousElementSibling
        {
            get
            {
                int curIndex = Index;
                var elements = ParentNode.ChildNodes;
                while (--curIndex >=0)
                {
                    if (elements[curIndex].NodeType == NodeType.ELEMENT_NODE)
                    {
                        return (IDomElement)elements[curIndex];
                    }
                }
                return null;
            }
        }
        IDomObject IDomObject.Clone()
        {
            return Clone();
        }

    }
    
}
