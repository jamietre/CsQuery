using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.Utility;
using CsQuery.Engine;

namespace CsQuery.Implementation
{

    /// <summary>
    /// Special node type to represent the DOM.
    /// </summary>
    public class DomDocument : DomContainer<DomDocument>, IDomDocument
    {

        public DomDocument()
            : base()
        {
        }
        public DomDocument(IEnumerable<IDomObject> elements): base()
        {
            ChildNodes.AddRange(elements);
        }
        public DomDocument(char[] html)
        {
            SourceHtml = html;
        }


        public void AddToIndex(IDomIndexedNode element)
        {
            foreach (string key in element.IndexKeys())
            {
                AddToIndex(key, element);
            }

            if (element.HasChildren)
            {
                foreach (DomElement child in ((IDomContainer)element).ChildElements)
                {
                    AddToIndex(child);
                }
            }
        }

        public void AddToIndex(string key, IDomIndexedNode element)
        {
            SelectorXref.Add(key, element.IndexReference);
        }
        public void RemoveFromIndex(IDomIndexedNode element)
        {
            if (element.HasChildren)
            {
                foreach (IDomElement child in ((IDomContainer)element).ChildElements)
                {
                    if (child.IsIndexed)
                    {
                        RemoveFromIndex(child);
                    }
                }
            }

            foreach (string key in element.IndexKeys())
            {
                RemoveFromIndex(key);
            }
        }
        public void RemoveFromIndex(string key)
        {
            SelectorXref.Remove(key);
        }
        public IEnumerable<IDomObject> QueryIndex(string subKey, int depth, bool includeDescendants)
        {
            return SelectorXref.GetRange(subKey, depth, includeDescendants);
        }
        public IEnumerable<IDomObject> QueryIndex(string subKey)
        {
            return SelectorXref.GetRange(subKey);
        }
        public override IDomContainer ParentNode
        {
            get
            {
                return null;
            }
            internal set
            {
                throw new InvalidOperationException("Cannot set parent for a DOM root node.");
            }
        }
        public override string Path
        {
            get
            {
                return "";
            }
        }
        public override int Depth
        {
            get
            {
                return 0;
            }
        }
        public  char[] SourceHtml
        {
            get;
            protected set;
        }
        private List<Tuple<int,int>> OriginalStrings = new List<Tuple<int,int>>();

        public virtual string GetTokenizedString(int index)
        {
            var range = OriginalStrings[index];
            return SourceHtml.Substring(range.Item1, range.Item2);
        }

        public int TokenizeString(int start, int length)
        {
            OriginalStrings.Add(new Tuple<int,int>(start,length));
            return OriginalStrings.Count - 1;
        }

        protected CQ _Owner = null;

        public IDomElement GetElementById(string id)
        {
            SelectorChain selectors = new SelectorChain("#" + id);
            return (IDomElement)selectors.Select(Document).FirstOrDefault();
        }
        public IDomElement GetElementByTagName(string tagName)
        {
            return GetElementsByTagName(tagName).FirstOrDefault();
        }
        public IList<IDomElement> GetElementsByTagName(string tagName)
        {
            SelectorChain selectors = new SelectorChain(tagName);
            return (new List<IDomElement>(OnlyElements(selectors.Select(Document)))).AsReadOnly();
        }
        public IList<IDomElement> QuerySelectorAll(string selector)
        {
            SelectorChain selectors = new SelectorChain(selector);
            return (new List<IDomElement>(OnlyElements(selectors.Select(Document)))).AsReadOnly();
        }
        protected IEnumerable<IDomElement> OnlyElements(IEnumerable<IDomObject> objectList)
        {
            foreach (IDomObject obj in objectList)
            {
                if (obj.NodeType == NodeType.ELEMENT_NODE)
                {
                    yield return (IDomElement)obj;
                }
            }
            yield break;
        }
        public IDomElement CreateElement(string nodeName) {
            return new DomElement(nodeName);
        }

        public IDomText CreateTextNode(string text)
        {
            return new DomText(text);
        }
        public IDomComment CreateComment(string comment)
        {
            return new DomComment(comment);
        }

        // Store for all text node content (to avoid overhead of allocation when cloning large numbers of objects)
        //public List<string> TextContent = new List<string>();

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
        
        public override IDomDocument Document
        {
            get
            {
                return this;
            }
        }
        public override NodeType NodeType
        {
            get { return GetElementById("html")==null 
                ? NodeType.DOCUMENT_FRAGMENT_NODE :
                  NodeType.DOCUMENT_NODE; }
        }
        public IDomDocumentType DocTypeNode
        {
            get
            {
                foreach (IDomObject obj in Document.ChildNodes)
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
                // If explicitly set, return that value, otherwise get from DomDocument node

                IDomDocumentType docNode = DocTypeNode;
                if (_DocType != 0 && docNode==null)
                {
                    return _DocType;
                }
                if (docNode != null)
                {
                    return docNode.DocType;
                } else {
                    return CQ.DefaultDocType;
                }
            }
            set
            {
                // Keep synchronized with DocTypeNode
                if (_settingDocType) return;
                _settingDocType = true;
                _DocType = value;
                IDomDocumentType docType = DocTypeNode;
                if (docType != null)
                {
                    DocTypeNode.DocType = value;
                }
                else
                {
                    _DocType = value;
                }
                _settingDocType = false;

            }
        }
        private bool _settingDocType = false;
        protected DocType _DocType = 0;
        public RangeSortedDictionary<IDomObject> SelectorXref
        {
            get
            {
                return _SelectorXref.Value;
            }
        }
        protected Lazy<RangeSortedDictionary<IDomObject>> _SelectorXref =
            new Lazy<RangeSortedDictionary<IDomObject>>();

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
        public override DomDocument Clone()
        {
            DomDocument clone = new DomDocument();
            clone.SourceHtml = SourceHtml;
            clone.OriginalStrings = OriginalStrings;

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
        private IDictionary<string, object> _Data;
        public IDictionary<string, object> Data
        {
            get
            {
                if (_Data == null)
                {
                    _Data = new Dictionary<string, object>();
                }
                return _Data;
            }
            set
            {
                _Data = value;
            }
        }
    }
    
}
