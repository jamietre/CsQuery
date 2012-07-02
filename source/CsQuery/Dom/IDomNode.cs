using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery
{
    public interface IDomNode : ICloneable 
    {
        NodeType NodeType { get; }

        /// <summary>
        /// The node (tag) name, in upper case.
        /// </summary>
        ///
        /// <url>
        /// https://developer.mozilla.org/en/DOM/Node.nodeName
        /// </url>

        string NodeName { get; set; }

        /// <summary>
        /// Gets or sets the value of this node.
        /// </summary>
        ///
        /// <remarks>
        /// For the document itself, nodeValue returns null. For text, comment, and CDATA nodes,
        /// nodeValue returns the content of the node.
        /// </remarks>
        ///
        /// <url>
        /// https://developer.mozilla.org/en/DOM/Node.nodeValue
        /// </url>

        string NodeValue { get; set; }

        /// <summary>
        /// Gets a value indicating whether this object has any children. For node types that cannot have
        /// children, it will always return false. To determine if a node is allowed to have children,
        /// use the ChildrenAllowed property.
        /// </summary>
        ///
        /// <seealso cref="ChildrenAllowed"/>

        bool HasChildren { get; }

        /// <summary>
        /// Return an INodeList of the direct children of this node.
        /// </summary>

        INodeList ChildNodes { get; }

        /// <summary>
        /// Return a sequence containing only the element children of this node (e.g. no text, cdata, comments)
        /// </summary>

        IEnumerable<IDomElement> ChildElements { get; }

        /// <summary>
        /// Gets a value indicating whether this node is complete. This is for internal use. During DOM
        /// construction it is possible for a node object to exist but not be properly configured.
        /// </summary>

        bool Complete { get; }

        /// <summary>
        /// Renders the complete HTML for this element, including its children.
        /// </summary>
        ///
        /// <returns>
        /// a string of HTML
        /// </returns>

        string Render();

        /// <summary>
        /// Renders the complete HTML for this element to a StringBuilder
        /// </summary>
        ///
        /// <param name="sb">
        /// An existing StringBuilder instance to append this element's HTML.
        /// </param>

        void Render(StringBuilder sb);

        /// <summary>
        /// Renders the complete HTML for this element to a StringBuilder using specified options.
        /// </summary>
        ///
        /// <param name="sb">
        /// An existing StringBuilder instance to append this element's HTML.
        /// </param>
        /// <param name="options">
        /// Options for controlling the operation.
        /// </param>

        void Render(StringBuilder sb, DomRenderingOptions options);

        /// <summary>
        /// Removes this object from it's parent, and consequently the Document, if any, to which it belongs.
        /// </summary>

        void Remove();

        /// <summary>
        /// Gets a value indicating whether this node should be is indexed. Generally, this is true for IDomElement
        /// nodes that are within an IDomDocument and false otherwise.
        /// </summary>

        bool IsIndexed { get; }

        /// <summary>
        /// Gets a value indicating whether this object belongs to a Document or not.
        /// </summary>
        ///
        /// <remarks>
        /// When an object is cloned, it becomes disconnected, and is not bound to any IDomDocument or
        /// IDomFragment. The only substantive difference between disconnected elements and elements
        /// belonging to a Document is that they are not indexed.
        /// </remarks>

        bool IsDisconnected { get; }

        /// <summary>
        /// Makes a deep copy of this object.
        /// </summary>
        ///
        /// <returns>
        /// A copy of this object.
        /// </returns>

        new IDomNode Clone();
    }

}
