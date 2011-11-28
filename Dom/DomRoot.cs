using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jtc.CsQuery.ExtensionMethods;

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
        //RangeSortedDictionary<IDomObject> SelectorXref { get; }
        void AddToIndex(string key, IDomElement element);
        void AddToIndex(IDomElement element);
        void RemoveFromIndex(string key);
        void RemoveFromIndex(IDomElement element);
        IEnumerable<IDomObject> QueryIndex(string subKey, int depth, bool includeDescendants);
        IEnumerable<IDomObject> QueryIndex(string subKey);
        
        DocType DocType { get; set; }
        DomRenderingOptions DomRenderingOptions { get; set; }
        IDomElement GetElementById(string id);
        IDomElement CreateElement(string nodeName);
        IDomText CreateTextNode(string text);
        IDomComment CreateComment(string comment);
        IEnumerable<IDomElement> GetElementsByTagName(string tagName);
       // void SetOwner(CsQuery owner);
        int TokenizeString(int startIndex, int length);
        string GetTokenizedString(int index);
        char[] SourceHtml { get; }
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
        public DomRoot(IEnumerable<IDomObject> elements): base()
        {
            ChildNodes.AddRange(elements);
        }
        public DomRoot(char[] html)
        {
            SourceHtml = html;
        }

        
        public void AddToIndex(IDomElement element)
        {
            AddToIndexImpl(element, element.IsDisconnected,true);
        }
        /// <summary>
        /// Pass "disconnected" info for all children to improve performance. When calling recursively,
        /// never reindex children -- their relative positions will not change.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="isDisconnected"></param>
        /// <param name="reIndex"></param>
        protected void AddToIndexImpl(IDomElement element, bool isDisconnected, bool reIndex)
        {
            if (isDisconnected)
            {
                RemoveFromIndex(element);
                if (reIndex)
                {
                    element.Reindex();
                }
            }
            DomRoot document = (DomRoot)Document;
            foreach (string key in element.IndexKeys())
            {
                AddToIndex(key, element);
            }

            if (element.HasChildren)
            {
                foreach (DomElement child in element.ChildElements)
                {
                    AddToIndexImpl(child,isDisconnected,false);
                }
            }

        }
        public void AddToIndex(string key, IDomElement element)
        {
            SelectorXref.Add(key, element);
        }
        public void RemoveFromIndex(IDomElement element)
        {
            if (element.HasChildren)
            {
                foreach (IDomElement child in element.ChildElements)
                {
                    RemoveFromIndex(child);
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
        public  char[] SourceHtml
        {
            get;
            protected set;
        }
        private List<Tuple<int,int>> OriginalStrings = new List<Tuple<int,int>>();
        private List<string> Strings;

        public int AddString(string text) {
            if (Strings==null) {
                Strings = new List<string>();
            }
            Strings.Add(text);
            return OriginalStrings.Count + Strings.Count-1;
        }
        public virtual string GetTokenizedString(int index)
        {
            if (index < OriginalStrings.Count)
            {
                var range = OriginalStrings[index];
                return SourceHtml.Substring(range.Item1, range.Item2);
            }
            else
            {
                return Strings[OriginalStrings.Count-index];
            }
        }
        //internal void SetOriginalString(char[] originalString)
        //{
        //    SourceHtml = originalString;
        //}
        public int TokenizeString(int start, int length)
        {
            OriginalStrings.Add(new Tuple<int,int>(start,length));
            return OriginalStrings.Count - 1;
        }
        //public void SetOwner(CsQuery owner)
        //{
        //    _Owner = owner;
        //}
        protected CsQuery _Owner = null;
        /// <summary>
        /// This is NOT INDEXED and should only be used for testing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IDomElement GetElementById(string id)
        {
            CsQuerySelectors selectors = new CsQuerySelectors("#" + id);
            return (IDomElement)selectors.Select(Document).FirstOrDefault();
        }
        //protected I(DomElement GetElementById(IEnumerable<IDomElement> elements, string id)
        //{


        //    //foreach (IDomElement el in elements)
        //    //{
        //    //    if (el.ID == id)
        //    //    {
        //    //        return el;
        //    //    }
        //    //    if (el.ChildNodes.Count > 0)
        //    //    {
        //    //        var childEl = GetElementById(el.ChildElements, id);
        //    //        if (childEl != null)
        //    //        {
        //    //            return childEl;
        //    //        }
        //    //    }
        //    //}
        //   // return null;
        //}
        public IEnumerable<IDomElement> GetElementsByTagName(string tagName)
        {
            CsQuerySelectors selectors = new CsQuerySelectors(tagName);
            return OnlyElements(selectors.Select(Document));
         
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
        public RangeSortedDictionary<IDomElement> SelectorXref {
            get
            {
                return _SelectorXref.Value;
            }
        }
        protected Lazy<RangeSortedDictionary<IDomElement>> _SelectorXref = 
            new Lazy<RangeSortedDictionary<IDomElement>>();

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
