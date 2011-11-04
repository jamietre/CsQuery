using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery
{
    [Flags]
    public enum DomRenderingOptions
    {
        RemoveMismatchedCloseTags = 1,
        RemoveComments = 2
    }

    public interface IDomRoot : IDomContainer
    {
        DocType DocType { get; set; }
        DomRenderingOptions DomRenderingOptions { get; set; }
        IDomElement GetElementById(string id);
    }


    /// <summary>
    /// Special node type to represent the DOM.
    /// </summary>
    public class DomRoot : DomContainer<DomRoot>, IDomRoot
    {
        public DomRoot()
            : base()
        {
        }
        public DomRoot(IEnumerable<IDomObject> elements)
            : base(elements)
        {

        }
        /// <summary>
        /// This is NOT INDEXED and should only be used for testing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IDomElement GetElementById(string id)
        {
            return GetElementById(Elements, id);
        }
        protected IDomElement GetElementById(IEnumerable<IDomElement> elements, string id)
        {
            foreach (IDomElement el in elements)
            {
                if (el.ID == id)
                {
                    return el;
                }
                if (el.ChildNodes.Count > 0)
                {
                    var childEl = GetElementById(el.Elements, id);
                    if (childEl != null)
                    {
                        return childEl;
                    }
                }
            }
            return null;
        }


        // Store for all text node content (to avoid overhead of allocation when cloning large numbers of objects)
        public List<string> TextContent = new List<string>();

        public DomRenderingOptions DomRenderingOptions
        {
            get
            {
                return _DomRenderingOptions;
            }
            set
            {
                _DomRenderingOptions = value;
            }
        }

        protected DomRenderingOptions _DomRenderingOptions = 0;
        public override DomRoot Dom
        {
            get
            {
                return this;
            }
        }
        public override NodeType NodeType
        {
            get { return NodeType.DOCUMENT_NODE; }
        }
        public DomDocumentType DocTypeNode
        {
            get
            {
                foreach (IDomObject obj in Dom.ChildNodes)
                {
                    if (obj.NodeType == NodeType.DOCUMENT_TYPE_NODE)
                    {
                        return (DomDocumentType)obj;
                    }
                }
                return null;
            }
        }
        /// <summary>
        /// Gets the DocType for this node. This can be changed through the DomRoot
        /// </summary>
        public DocType DocType
        {
            get
            {
                if (_DocType == 0)
                {
                    DomDocumentType docType = DocTypeNode;
                    if (docType == null)
                    {
                        _DocType = DocType.XHTML;
                    }
                    else
                    {
                        _DocType = docType.DocType;
                    }
                }
                return _DocType;
            }
            set
            {
                // Keep synchronized with DocTypeNode
                if (_settingDocType) return;
                _settingDocType = true;
                _DocType = value;
                DomDocumentType docType = DocTypeNode;
                if (docType != null)
                {
                    DocTypeNode.DocType = value;
                }
                _settingDocType = false;

            }
        }
        private bool _settingDocType = false;
        protected DocType _DocType = 0;
        public RangeSortedDictionary<DomElement> SelectorXref = new RangeSortedDictionary<DomElement>();
        public override bool InnerHtmlAllowed
        {
            get { return true; }
        }
        public override bool Complete
        {
            get { return true; }
        }
        public override string ToString()
        {
            return "DOM Root (" + DocType.ToString() + ", " + DescendantCount().ToString() + " elements)";
        }
        public override DomRoot Clone()
        {
            DomRoot clone = base.Clone();
            clone.TextContent = new List<string>(TextContent);
            return clone;
        }
        public override IEnumerable<IDomObject> CloneChildren()
        {
            if (HasChildren)
            {
                foreach (IDomObject obj in ChildNodes)
                {
                    yield return obj.Clone();
                }
            }
            yield break;
        }
    }
    
}
