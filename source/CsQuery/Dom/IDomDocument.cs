using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.Utility;

namespace CsQuery
{
    /// <summary>
    /// An interface to a Document that represents an HTML document.
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

        IDomDocumentType DocTypeNode {get;}

        /// <summary>
        /// Returns the document type of this document. If no DOCTYPE node exists, this will return the default
        /// document type defined through the CsQuery.Options variable.
        /// </summary>

        DocType DocType { get; set; }

        /// <summary>
        /// Gets or sets options for controlling how the output is rendered. All options are flags so
        /// multiple values can be set with "option1 | option2 ...".
        /// </summary>

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

        IList<IDomElement> GetElementsByTagName(string tagName);

        /// <summary>
        /// Return the body element for this Document.
        /// </summary>

        IDomElement Body { get; }

    }
}
