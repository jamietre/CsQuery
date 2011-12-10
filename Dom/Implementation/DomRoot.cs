using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jtc.CsQuery.ExtensionMethods;
using Jtc.CsQuery.Utility;

namespace Jtc.CsQuery.Implementation
{

    /// <summary>
    /// Special node type to represent the DOM.
    /// </summary>
    public class DomRoot : DomContainer<DomRoot>, IDomRoot
    {

        public DomRoot()
            : base()
        {
        }
        public DomRoot(IEnumerable<IDomObject> elements): base()
        {
            ChildNodes.AddRange(elements);
        }
        public DomRoot(char[] html)
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
                throw new Exception("Cannot set parent for a DOM root node.");
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

        protected CsQuery _Owner = null;

        public IDomElement GetElementById(string id)
        {
            CsQuerySelectors selectors = new CsQuerySelectors("#" + id);
            return (IDomElement)selectors.Select(Document).FirstOrDefault();
        }
        public IDomElement GetElementByTagName(string tagName)
        {
            return GetElementsByTagName(tagName).FirstOrDefault();
        }
        public List<IDomElement> GetElementsByTagName(string tagName)
        {
            CsQuerySelectors selectors = new CsQuerySelectors(tagName);
            return new List<IDomElement>(OnlyElements(selectors.Select(Document)));
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
        
        public override IDomRoot Document
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
        public DomDocumentType DocTypeNode
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
        public override DomRoot Clone()
        {
            DomRoot clone = new DomRoot();
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
    }
    
}
