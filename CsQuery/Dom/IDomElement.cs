using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery
{   
    /// <summary>
    /// A regular DOM element
    /// </summary>
    public interface IDomElement : IDomContainer, IDomIndexedNode
    {

        bool HasClass(string className);
        bool AddClass(string className);
        bool RemoveClass(string className);

        bool HasStyle(string styleName);
        void AddStyle(string styleString);
        bool RemoveStyle(string name);
        bool IsBlock { get; }

        // iterator to access classes. Style and Attributes are enumerable directly.
        IEnumerable<string> Classes { get; }
        string ElementHtml();
        int ElementIndex { get; }
    }
}
