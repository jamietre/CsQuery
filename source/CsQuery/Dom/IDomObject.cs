using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.Implementation;

namespace CsQuery
{
    /// <summary>
    /// An node that appears directly in the DOM. This is essentially synonymous with a Node, but it does
    /// not include attributes.
    /// 
    /// All properties of Element nodes are implemented in IDomObject even though many are only applicable to
    /// Elements. Attempting to read a property that doesn't exist on the node type will generally return 'null'
    /// whereas attempting to write will throw an exception. This is intended to make coding against this model
    /// the same as coding against the actual DOM, where accessing nonexistent properties is acceptable. Because
    /// some javascript code actually uses this in logic we allow the same kind of access. It also eliminates the
    /// need to cast frequently, for example, when accessing the results of a jQuery object by index.
    /// </summary>
    public interface IDomObject: IDomNode
    {
        // To simulate the way the real DOM works, most properties/methods of things directly in the DOM
        // are part of a common interface, even if they do not apply.

        /// <summary>
        /// The HTML document to which this element belongs
        /// </summary>
        IDomDocument Document { get; }

        /// <summary>
        /// The direct parent of this node
        /// </summary>
        IDomContainer ParentNode { get; }

        /// <summary>
        /// The child node at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IDomObject this[int index] { get; }

        /// <summary>
        /// Get or set the value of the named attribute
        /// </summary>
        /// <param name="attribute">The attribute name</param>
        /// <returns>An attribute value</returns>
        /// <returntype>string</returntype>
        string this[string attribute] { get; set; }

        /// <summary>
        /// Get or set value of the id attribute
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// An interface to access the attributes collection of this element
        /// </summary>
        IAttributeCollection Attributes { get; }

        /// <summary>
        /// An object encapsulating the Styles associated with this element
        /// </summary>
        CSSStyleDeclaration Style { get; }

        /// <summary>
        /// gets and sets the value of the class attribute of the specified element.
        /// </summary>
        /// <href>https://developer.mozilla.org/en/DOM/element.className</href>
        string ClassName { get; set; }


        /// <summary>
        /// A sequence of all the unique class names applied to this object
        /// </summary>
        IEnumerable<string> Classes { get; }

        /// <summary>
        /// For input elements, the "value" property of this element. Returns null for other element types.
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// The value of an input element, or the text of a textarea element. 
        /// </summary
        string DefaultValue { get; set; }

        /// <summary>
        /// Sets or gets the HTML of an elements descendants
        /// </summary>
        /// <href>https://developer.mozilla.org/en/DOM/element.innerHTML</href>
        string InnerHTML { get; set; }

        /// <summary>
        /// Gets or sets the text content of a node and its descendants.
        /// </summary>
        /// <href>https://developer.mozilla.org/en/DOM/Node.textContent</href>
        string InnerText { get; set; }

        /// <summary>
        /// Adds a node to the end of the list of children of a specified parent node. 
        /// If the node already exists it is removed from current parent node, then added to new parent node.
        /// </summary>
        /// <param name="element"></param>
        /// <href>https://developer.mozilla.org/en/DOM/Node.appendChild</href>
        void AppendChild(IDomObject element);
        
        /// <summary>
        /// Removes a child node from the DOM. Returns removed node.
        /// </summary>
        /// <param name="element"></param>
        /// <href>https://developer.mozilla.org/En/DOM/Node.removeChild</href>
        void RemoveChild(IDomObject element);

        /// <summary>
        /// Inserts the specified node before a reference element as a child of the current node.
        /// </summary>
        /// <param name="newNode"></param>
        /// <param name="referenceNode"></param>
        /// <href>https://developer.mozilla.org/en/DOM/Node.insertBefore</href>
        void InsertBefore(IDomObject newNode, IDomObject referenceNode);

        /// <summary>
        /// Inserts the specified node after a reference element as a child of the current node.
        /// 
        /// </summary>
        /// <remarks>
        /// This method is not part of the core DOM spec.
        /// </remarks>
        /// <param name="newNode"></param>
        /// <param name="referenceNode"></param>
        /// <href>https://developer.mozilla.org/en/DOM/Node.insertBefore</href>
        void InsertAfter(IDomObject newNode, IDomObject referenceNode);


        IDomObject FirstChild { get; }
        IDomElement FirstElementChild { get; }
        IDomObject LastChild { get; }
        IDomElement LastElementChild { get; }
        IDomObject NextSibling { get; }
        IDomObject PreviousSibling { get; }
        IDomElement NextElementSibling { get; }
        IDomElement PreviousElementSibling { get; }

        void SetAttribute(string name);
        void SetAttribute(string name, string value);
        string GetAttribute(string name);
        string GetAttribute(string name, string defaultValue);
        bool TryGetAttribute(string name, out string value);
        bool HasAttribute(string name);
        bool RemoveAttribute(string name);

        bool HasClass(string className);
        bool AddClass(string className);
        bool RemoveClass(string className);

        bool HasStyle(string styleName);
        void AddStyle(string styleString);
        bool RemoveStyle(string name);

        /// <summary>
        /// Returns true if this node has any attributes
        /// </summary>
        bool HasAttributes { get; }

        /// <summary>
        /// Returns true if this node has CSS classes
        /// </summary>
        bool HasClasses { get; }

        /// <summary>
        /// Returns true if this node has styles
        /// </summary>
        bool HasStyles { get; }

        bool Selected { get; }
        bool Checked { get; set; }
        bool ReadOnly { get; set; }

        /// <summary>
        /// The type of attribute
        /// </summary>
        string Type { get; set; }

        /// <summary>
        /// For input elements, the name
        /// </summary>
        string Name { get; set; }

        // Nonstandard elements
        bool InnerHtmlAllowed { get; }
        bool InnerTextAllowed { get; }

        int DescendantCount();
        int Depth { get; }
        int Index { get; }
        string PathID { get; }
        string Path { get; }

        // Wrap this node in a CQ object
        CQ Cq();
        new IDomObject Clone();

        ushort NodeNameID { get; }
    }

    public interface IDomObject<out T> : IDomObject
    {
        new T Clone();
    }
}
