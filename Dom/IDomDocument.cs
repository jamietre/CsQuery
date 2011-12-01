using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery
{
    public interface IDomRoot : IDomContainer
    {
        //RangeSortedDictionary<IDomObject> SelectorXref { get; }
        void AddToIndex(string key, IDomIndexedNode element);
        void AddToIndex(IDomIndexedNode element);
        void RemoveFromIndex(string key);
        void RemoveFromIndex(IDomIndexedNode element);
        IEnumerable<IDomObject> QueryIndex(string subKey, int depth, bool includeDescendants);
        IEnumerable<IDomObject> QueryIndex(string subKey);

        DocType DocType { get; set; }
        DomRenderingOptions DomRenderingOptions { get; set; }
        IDomElement GetElementById(string id);
        IDomElement CreateElement(string nodeName);
        IDomText CreateTextNode(string text);
        IDomComment CreateComment(string comment);
        IDomElement GetElementByTagName(string tagName);
        List<IDomElement> GetElementsByTagName(string tagName);
        // void SetOwner(CsQuery owner);
        int TokenizeString(int startIndex, int length);
        string GetTokenizedString(int index);
        char[] SourceHtml { get; }
    }
}
