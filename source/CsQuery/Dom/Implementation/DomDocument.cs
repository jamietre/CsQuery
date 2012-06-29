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
        #region constructors

        /// <summary>
        /// Create a new, empty DOM document
        /// </summary>
        public DomDocument()
            : base()
        {
        }

        /// <summary>
        /// Create a new document from a sequence of elements
        /// </summary>
        /// <param name="elements"></param>
        public DomDocument(IEnumerable<IDomObject> elements): base()
        {
            ChildNodes.AddRange(elements);
        }

        /// <summary>
        /// Create a new document from a character array of html
        /// </summary>
        /// <param name="html"></param>
        public DomDocument(char[] html)
        {
            if (html != null && html.Length > 0)
            {
                SourceHtml = html;
            }
        }

        #endregion

        #region private properties

        private List<Tuple<int, int>> OriginalStrings = new List<Tuple<int, int>>();
        private bool _settingDocType = false;
        private IDictionary<string, object> _Data;

        protected CQ _Owner = null;
        protected DocType _DocType = 0;
        protected Lazy<RangeSortedDictionary<IDomObject>> _SelectorXref =
            new Lazy<RangeSortedDictionary<IDomObject>>();

        #endregion

        #region public properties

        /// <summary>
        /// The index
        /// </summary>
        public RangeSortedDictionary<IDomObject> SelectorXref
        {
            get
            {
                return _SelectorXref.Value;
            }
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

        /// <summary>
        /// The depth in the node tree at which this node occurs. This is always 0 for the DomDocument.
        /// </summary>
        public override int Depth
        {
            get
            {
                return 0;
            }
        }

        public char[] SourceHtml
        {
            get;
            protected set;
        }

        // Store for all text node content (to avoid overhead of allocation when cloning large numbers of objects)
        //public List<string> TextContent = new List<string>();

        public DomRenderingOptions DomRenderingOptions
        {
            get; set;
        }

        public override IDomDocument Document
        {
            get
            {
                return this;
            }
        }

        public override NodeType NodeType
        {
            get
            {
                return GetElementById("html") == null
                    ? NodeType.DOCUMENT_FRAGMENT_NODE :
                NodeType.DOCUMENT_NODE;
            }
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
        /// Gets the DocType for this node. This can be changed through the DomDocument
        /// </summary>
        public DocType DocType
        {
            get
            {
                // If explicitly set, return that value, otherwise get from DomDocument node

                IDomDocumentType docNode = DocTypeNode;
                if (_DocType != 0 && docNode == null)
                {
                    return _DocType;
                }
                if (docNode != null)
                {
                    return docNode.DocType;
                }
                else
                {
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

        public override bool InnerHtmlAllowed
        {
            get { return true; }
        }
        
        public override bool Complete
        {
            get { return true; }
        }

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

        public IDomElement Body
        {
            get
            {
                return this.QuerySelectorAll("body").FirstOrDefault();
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Add an element to the index using the default keys for this element
        /// </summary>
        /// <param name="element"></param>
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

        /// <summary>
        /// Add an element to the index using a specified index key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="element"></param>
        public void AddToIndex(string key, IDomIndexedNode element)
        {
            SelectorXref.Add(key, element.IndexReference);
        }
        
        /// <summary>
        /// Remove an element from the index
        /// </summary>
        /// <param name="element"></param>
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

        /// <summary>
        /// Remove an element from the index using its key
        /// </summary>
        /// <param name="key"></param>
        public void RemoveFromIndex(string key)
        {
            SelectorXref.Remove(key);
        }

        /// <summary>
        /// Query the document's index for a subkey up to a specific depth, optionally including descendants that match the selector
        /// </summary>
        /// <param name="subKey"></param>
        /// <param name="depth">The zero-based depth to which searches should be limited</param>
        /// <param name="includeDescendants"></param>
        /// <returns></returns>
        public IEnumerable<IDomObject> QueryIndex(string subKey, int depth, bool includeDescendants)
        {
            return SelectorXref.GetRange(subKey, depth, includeDescendants);
        }

        /// <summary>
        /// Query the document's index for a subkey
        /// </summary>
        /// <param name="subKey"></param>
        /// <returns></returns>
        public IEnumerable<IDomObject> QueryIndex(string subKey)
        {
            return SelectorXref.GetRange(subKey);
        }

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

        public IDomElement GetElementById(string id)
        {
            // construct the selector manually so there's no syntax checking

            SelectorClause selector = new SelectorClause();
            selector.SelectorType = SelectorType.ID;
            selector.ID = id;

            Selector selectors = new Selector(selector);
            return (IDomElement)selectors.Select(Document).FirstOrDefault();
        }

        public IDomElement GetElementByTagName(string tagName)
        {
            Selector selectors = new Selector(tagName);
            return (IDomElement)selectors.Select(Document).FirstOrDefault();
        }

        public IList<IDomElement> GetElementsByTagName(string tagName)
        {
            Selector selectors = new Selector(tagName);
            return (new List<IDomElement>(OnlyElements(selectors.Select(Document)))).AsReadOnly();
        }

        public IDomElement QuerySelector(string selector)
        {
            Selector selectors = new Selector(selector);
            return OnlyElements(selectors.Select(Document)).FirstOrDefault();
        }

        public IList<IDomElement> QuerySelectorAll(string selector)
        {
            Selector selectors = new Selector(selector);
            return (new List<IDomElement>(OnlyElements(selectors.Select(Document)))).AsReadOnly();
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

        public override DomDocument Clone()
        {
            DomDocument clone = new DomDocument();
            clone.SourceHtml = SourceHtml;
            clone.OriginalStrings = OriginalStrings;

            return clone;
        }

        public override string ToString()
        {
            return "DOM Root (" + DocType.ToString() + ", " + DescendantCount().ToString() + " elements)";
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

        #endregion

        #region private methods

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

        #endregion

    }
    
}
