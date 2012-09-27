using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.Utility;
using CsQuery.HtmlParser;

namespace CsQuery
{
    /// <summary>
    /// An interface to a DOM Document, the high-level representation of an HTML document. This is
    /// analagous to the web browser "document" object.
    /// </summary>

    public interface IDomDocument : IDomContainer
    {

        /// <summary>
        /// An interface to the internal indexing methods. You generally should not use this.
        /// </summary>

        IDomIndex DocumentIndex { get; }

        /// <summary>
        /// Gets the document type node for this document, or null if none exists.
        /// </summary>

        IDomDocumentType DocTypeNode { get; set; }

        /// <summary>
        /// Returns the document type of this document. If no DOCTYPE node exists, this will return the default
        /// document type defined through the CsQuery.Options variable.
        /// </summary>

        DocType DocType { get; }

        IList<ICSSStyleSheet> StyleSheets { get; }

        /// <summary>
        /// Deprecated: options will be specificed as a Render parameter
        /// </summary>

        [Obsolete]
        DomRenderingOptions DomRenderingOptions { get; set; }

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

        IDomElement GetElementById(string id);

        /// <summary>
        /// Gets an element by identifier, and return a strongly-typed interface
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

        T GetElementById<T>(string id) where T : IDomElement;

        /// <summary>
        /// Creates the specified HTML element.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// Name of the node.
        /// </param>
        ///
        /// <returns>
        /// The new element.
        /// </returns>
        ///
        /// <url>
        /// https://developer.mozilla.org/en/DOM/document.createElement
        /// </url>

        IDomElement CreateElement(string nodeName);

        /// <summary>
        /// Creates a new Text node.
        /// </summary>
        ///
        /// <param name="text">
        /// The text.
        /// </param>
        ///
        /// <returns>
        /// The new text node.
        /// </returns>
        ///
        /// <url>
        /// https://developer.mozilla.org/en/DOM/document.createTextNode
        /// </url>

        IDomText CreateTextNode(string text);

        /// <summary>
        /// Creates a new comment.
        /// </summary>
        ///
        /// <param name="comment">
        /// The comment.
        /// </param>
        ///
        /// <returns>
        /// The new comment.
        /// </returns>
        ///
        /// <url>
        /// https://developer.mozilla.org/en/DOM/document.createComment
        /// </url>

        IDomComment CreateComment(string comment);

        /// <summary>
        /// Creates a document type node.
        /// </summary>
        ///
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="publicIdentifier">
        /// The public type identifier.
        /// </param>
        /// <param name="systemIdentifier">
        /// The system type identifier.
        /// </param>
        ///
        /// <returns>
        /// The new document type.
        /// </returns>

        IDomDocumentType CreateDocumentType(string type, string publicIdentifier, string systemIdentifier);


        /// <summary>
        /// Returns the first element within the document (using depth-first pre-order traversal of the
        /// document's nodes) that matches the specified group of selectors.
        /// </summary>
        ///
        /// <param name="selector">
        /// The selector.
        /// </param>
        ///
        /// <returns>
        /// An element, the first that matches the selector.
        /// </returns>
        ///
        /// <url>
        /// https://developer.mozilla.org/En/DOM/Document.querySelector
        /// </url>

        IDomElement QuerySelector(string selector);

        /// <summary>
        /// Returns a list of the elements within the document (using depth-first pre-order traversal of
        /// the document's nodes) that match the specified group of selectors.
        /// </summary>
        ///
        /// <param name="selector">
        /// The selector.
        /// </param>
        ///
        /// <returns>
        /// A sequence of elements matching the selector.
        /// </returns>
        ///
        /// <url>
        /// https://developer.mozilla.org/en/DOM/Document.querySelectorAll
        /// </url>


        IList<IDomElement> QuerySelectorAll(string selector);

        /// <summary>
        /// Returns a list of elements with the given tag name. The subtree underneath the specified
        /// element is searched, excluding the element itself.
        /// </summary>
        ///
        /// <remarks>
        /// Unlike the browser DOM version, this list is not live; it will represent the selection at the
        /// time the query was run.
        /// </remarks>
        ///
        /// <param name="tagName">
        /// Name of the tag.
        /// </param>
        ///
        /// <returns>
        /// The element by tag name.
        /// </returns>
        ///
        /// <url>
        /// https://developer.mozilla.org/en/DOM/element.getElementsByTagName
        /// </url>

        INodeList<IDomElement> GetElementsByTagName(string tagName);

        /// <summary>
        /// Return the body element for this Document.
        /// </summary>

        IDomElement Body { get; }

        /// <summary>
        /// Creates an IDomDocument that is derived from this one. The new type can also be a derived
        /// type, such as IDomFragment. The new object will inherit DomRenderingOptions from this one.
        /// </summary>
        ///
        /// <typeparam name="T">
        /// The type of object to create that is IDomDocument
        /// </typeparam>
        ///
        /// <returns>
        /// A new, empty concrete class that is represented by the interface T, configured with the same
        /// options as the current object.
        /// </returns>

        IDomDocument CreateNew<T>() where T : IDomDocument;

        /// <summary>
        /// Creates an IDomDocument that is derived from this one. The new type can also be a derived
        /// type, such as IDomFragment. The new object will inherit DomRenderingOptions from this one.
        /// </summary>
        ///
        /// <returns>
        /// The new Document.
        /// </returns>

        IDomDocument CreateNew();

    }
}
