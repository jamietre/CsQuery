using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.Utility;
using CsQuery.Engine;
using CsQuery.HtmlParser;

namespace CsQuery.Implementation
{

    /// <summary>
    /// Special node type to represent the DOM.
    /// </summary>
    public class DomDocument : DomContainer<DomDocument>, IDomDocument, IDomIndex
    {
        #region constructors

        /// <summary>
        /// Create a new, empty DOM document
        /// </summary>
        public DomDocument()
            : base()
        {
            InitializeDomDocument();
        }

        /// <summary>
        /// Create a new document from a sequence of elements
        /// </summary>
        /// <param name="elements"></param>
        public DomDocument(IEnumerable<IDomObject> elements): base()
        {
            InitializeDomDocument();
            Populate(elements);
        }

        
        /// <summary>
        /// Create a new document from a character array of html
        /// </summary>
        /// <param name="html"></param>
        public DomDocument(char[] html, HtmlParsingMode htmlParsingMode)
        {
            InitializeDomDocument();
            Populate(html, htmlParsingMode);
        }

        private void Populate(char[] html, HtmlParsingMode htmlParsingMode)
        {

            if (html != null && html.Length > 0)
            {
                SourceHtml = html;
            }

            HtmlElementFactory factory = new HtmlParser.HtmlElementFactory(this);
            Populate(factory.Parse(htmlParsingMode));

        }

        private void Populate(IEnumerable<IDomObject> elements)
        {
            foreach (var item in elements)
            {
                ChildNodesInternal.AddAlways(item);
            }
  
        }

        private void InitializeDomDocument()
        {
            ChildNodes.Clear();
            SelectorXref.Clear();
            OriginalStrings = new List<Tuple<int, int>>();
        }

        #endregion

        #region private properties

        private List<Tuple<int, int>> OriginalStrings;
        private bool _settingDocType;
        private IList<ICSSStyleSheet> _StyleSheets;
        private IDictionary<string, object> _Data;
        

        protected CQ _Owner;
        protected DocType _DocType;
        protected RangeSortedDictionary<IDomObject> _SelectorXref;

        #endregion

        #region public properties

        public IList<ICSSStyleSheet> StyleSheets
        {
            get
            {
                if (_StyleSheets == null)
                {
                    _StyleSheets = new List<ICSSStyleSheet>();
                }
                return _StyleSheets;
            }
        }

        /// <summary>
        /// Exposes the Document as an IDomIndex object
        /// </summary>

        public IDomIndex DocumentIndex
        {
            get
            {
                return (IDomIndex)this;
            }
        }

        /// <summary>
        /// The index
        /// </summary>
        public RangeSortedDictionary<IDomObject> SelectorXref
        {
            get
            {
                if (_SelectorXref == null)
                {
                    _SelectorXref = new RangeSortedDictionary<IDomObject>();
                }
                return _SelectorXref;
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
                return NodeType.DOCUMENT_NODE;
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

        public override bool IsIndexed
        {
            get
            {
                return true;
            }
        }

        public override bool IsFragment
        {
            get
            {
                return false;
            }
        }
        public override bool IsDisconnected
        {
            get
            {
                return false;
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

        /// <summary>
        /// Returns a reference to the element by its ID.
        /// </summary>
        ///
        /// <param name="id">
        /// The identifier.
        /// </param>
        ///
        /// <returns>
        /// The element by identifier.
        /// </returns>
        ///
        /// <url>
        /// https://developer.mozilla.org/en/DOM/document.getElementById
        /// </url>

        public IDomElement GetElementById(string id)
        {

            return GetElementById<IDomElement>(id);
        }

        /// <summary>
        /// Gets an element by identifier, and return a strongly-typed interface.
        /// </summary>
        ///
        /// <typeparam name="T">
        /// Generic type parameter.
        /// </typeparam>
        /// <param name="id">
        /// The identifier.
        /// </param>
        ///
        /// <returns>
        /// The element by id&lt; t&gt;
        /// </returns>

        public T GetElementById<T>(string id) where T: IDomElement
        {

            // construct the selector manually so there's no syntax checking as if it were a general-purpose selector

            SelectorClause selector = new SelectorClause();
            selector.SelectorType = SelectorType.ID;
            selector.ID = id;

            Selector selectors = new Selector(selector);
            return (T)selectors.Select(Document).FirstOrDefault();
        }

        public IDomElement GetElementByTagName(string tagName)
        {
            Selector selectors = new Selector(tagName);
            return (IDomElement)selectors.Select(Document).FirstOrDefault();
        }

        public INodeList<IDomElement> GetElementsByTagName(string tagName)
        {
            Selector selectors = new Selector(tagName);
            return new NodeList<IDomElement>(new List<IDomElement>(OnlyElements(selectors.Select(Document))));
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

        public IDomElement CreateElement(string nodeName) 
        {
            return DomElement.Create(nodeName);
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


        public IDomDocument CreateNew<T>() where T : IDomDocument
        {
            return CreateNew(typeof(T));
        }

        private IDomDocument CreateNew(Type t) 
        {
            IDomDocument newDoc;
            if (t == typeof(IDomDocument))
            {
                newDoc = new DomDocument();

            }
            else if (t == typeof(IDomFragment))
            {
                newDoc = new DomFragment();
            }
            
            else
            {
                throw new ArgumentException(String.Format("I don't know about an IDomDocument subclass \"{1}\"",
                    t.ToString()));
            }

            FinishConfiguringNew(newDoc);
            return newDoc;
        }

        public virtual IDomDocument CreateNew()
        {
            return CreateNew<IDomDocument>();
        }

        /// <summary>
        /// Creates an IDomDocument that is derived from this one. The new type can also be a derived
        /// type, such as IDomFragment. The new object will inherit DomRenderingOptions from this one.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when one or more arguments have unsupported or illegal values.
        /// </exception>
        ///
        /// <typeparam name="T">
        /// The type of object to create that is IDomDocument.
        /// </typeparam>
        /// <param name="html">
        /// The HTML source for the new document.
        /// </param>
        ///
        /// <returns>
        /// A new, empty concrete class that is represented by the interface T, configured with the same
        /// options as the current object.
        /// </returns>

  
         public IDomDocument CreateNew<T>(char[] html, HtmlParsingMode htmlParsingMode) where T : IDomDocument
         {
            IDomDocument newDoc;
            if (typeof(T) == typeof(IDomDocument))
            {
                newDoc = new DomDocument(html, htmlParsingMode);

            }
            else if (typeof(T) == typeof(IDomFragment))
            {
                newDoc = new DomFragment(html, htmlParsingMode);
            }
           
            else
            {
                throw new ArgumentException(String.Format("I don't know about an IDomDocument subclass \"{1}\"",
                    typeof(T).ToString()));
            }

            FinishConfiguringNew(newDoc);
            return newDoc;
        }

        /// <summary>
        /// Creates an IDomDocument that is derived from this one. The new type can also be a derived
        /// type, such as IDomFragment. The new object will inherit DomRenderingOptions from this one.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when one or more arguments have unsupported or illegal values.
        /// </exception>
        ///
        /// <typeparam name="T">
        /// The type of object to create that is IDomDocument.
        /// </typeparam>
        /// <param name="elements">
        /// The elements that are the source for the new document.
        /// </param>
        ///
        /// <returns>
        /// A new, empty concrete class that is represented by the interface T, configured with the same
        /// options as the current object.
        /// </returns>

        public IDomDocument CreateNew<T>(IEnumerable<IDomObject> elements) where T : IDomDocument
        {
            IDomDocument newDoc;
            if (typeof(T) == typeof(IDomDocument))
            {
                newDoc = new DomDocument(elements);

            }
            else if (typeof(T) == typeof(IDomFragment))
            {
                newDoc = new DomFragment(elements);
            }
           
            else
            {
                throw new ArgumentException(String.Format("I don't know about an IDomDocument subclass \"{1}\"",
                    typeof(T).ToString()));
            }

            FinishConfiguringNew(newDoc);
            return newDoc;
        }

        private void FinishConfiguringNew(IDomDocument newDoc)
        {
            newDoc.DomRenderingOptions = DomRenderingOptions;
        }
    }
    
}
