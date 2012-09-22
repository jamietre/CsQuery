using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;

using CsQuery.ExtensionMethods.Internal;
using CsQuery.Utility;
using CsQuery.Engine;
using CsQuery.Web;
using CsQuery.Promises;
using CsQuery.HtmlParser;
using CsQuery.Implementation;

namespace CsQuery
{
    public partial class CQ
    {
        /// <summary>
        /// Create an empty CQ object.
        /// </summary>
        ///
        /// <returns>
        /// A new CQ object
        /// </returns>

        public static CQ Create()
        {
            return new CQ();
        }

        /// <summary>
        /// Create a new CQ object from an HTML character array.
        /// </summary>
        ///
        /// <param name="html">
        /// The HTML source for the document
        /// </param>
        ///
        /// <returns>
        /// A new CQ object
        /// </returns>

        public static CQ Create(char[] html)
        {
            return new CQ(html);
        }

        /// <summary>
        /// Create a new CQ object from a single element. Unlike the constructor method <see cref="CsQuery.CQ"/>
        /// this new objet is not bound to any context from the element.
        /// </summary>
        ///
        /// <param name="element">
        /// The element to wrap
        /// </param>
        ///
        /// <returns>
        /// A new CQ object
        /// </returns>
        
        public static CQ Create(IDomObject element)
        {
            CQ csq = new CQ();
            if (element is IDomDocument) {
                csq.Document = (IDomDocument)element;
                csq.AddSelection(csq.Document.ChildNodes);
                csq.FinishCreatingNewDocument();
            } else {
                csq.CreateNewFragment(Objects.Enumerate(element));
            }
            return csq;
        }

        /// <summary>
        /// Creeate a new CQ object from an HTML string.
        /// </summary>
        ///
        /// <param name="html">
        /// A string containing HTML
        /// </param>
        ///
        /// <returns>
        /// A new CQ object
        /// </returns>

        public static CQ Create(string html)
        {
            return new CQ(html);
        }

        /// <summary>
        /// Create a new CQ from an HTML fragment, and use quickSet to create attributes (and/or css)
        /// </summary>
        ///
        /// <param name="html">
        /// A string of HTML.
        /// </param>
        /// <param name="quickSet">
        /// an object containing CSS properties and attributes to be applied to the resulting fragment.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object
        /// </returns>

        public static CQ Create(string html, object quickSet)
        {
            CQ csq = CQ.CreateFragment(html);
            return csq.AttrSet(quickSet, true);
        }

        /// <summary>
        /// Creeate a new CQ object from a squence of elements, or another CQ object.
        /// </summary>
        ///
        /// <param name="elements">
        /// A sequence of elements.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object
        /// </returns>

        public static CQ Create(IEnumerable<IDomObject> elements)
        {
            CQ csq = new CQ();
            csq.CreateNewFragment(elements);
            return csq;
        }

        /// <summary>
        /// Create a new CQ object from a stream of HTML, treating the HTML as a content document.
        /// </summary>
        ///
        /// <param name="stream">
        /// An open Stream
        /// </param>
        ///
        /// <returns>
        /// A new CQ object
        /// </returns>

        public static CQ Create(Stream stream)
        {
            return Create(Support.StreamToCharArray(stream));
        }

        /// <summary>
        /// Creeate a new fragment from HTML text.
        /// </summary>
        ///
        /// <param name="html">
        /// A string of HTML.
        /// </param>
        /// <param name="elementContext">
        /// (optional) context for the element; this determines how parsing rules are applied. If null,
        /// it will be considered in a legal context.
        /// </param>
        ///
        /// <returns>
        /// The new fragment.
        /// </returns>

        public static CQ CreateFragment(string html, string elementContext=null)
        {
            return CQ.CreateFragment(Support.StringToCharArray(html));
        }

        /// <summary>
        /// Create a new fragment from HTML text.
        /// </summary>
        ///
        /// <param name="html">
        /// A character array containing HTML
        /// </param>
        ///
        /// <returns>
        /// The new fragment.
        /// </returns>

        public static CQ CreateFragment(char[] html)
        {
            CQ csq = new CQ();
            //csq.LoadFragment(html);
            csq.CreateNewFragment(html.AsString(), HtmlParsingMode.Fragment);
            return csq;
        }

        /// <summary>
        /// Create a new fragment from a stream of HTML text.
        /// </summary>
        ///
        /// <param name="html">
        /// An open Stream 
        /// </param>
        ///
        /// <returns>
        /// The new fragment.
        /// </returns>

        public static CQ CreateFragment(Stream html)
        {
            return CreateFragment(Support.StreamToCharArray(html));
        }

        /// <summary>
        /// Creeate a new CQ object from a squence of elements, or another CQ object.
        /// </summary>
        ///
        /// <param name="elements">
        /// A sequence of elements.
        /// </param>
        ///
        /// <returns>
        /// The new fragment.
        /// </returns>

        public static CQ CreateFragment(IEnumerable<IDomObject> elements)
        {
            // this is synonymous with the Create method of the same sig because we definitely
            // would never autogenerate elements from a sequence of elements

            return Create(elements);
        }

        /// <summary>
        /// Creeate a new DOM from HTML text using full HTML5 tag generation.
        /// </summary>
        ///
        /// <param name="html">
        /// A string of HTML
        /// </param>
        ///
        /// <returns>
        /// The new document.
        /// </returns>

        public static CQ CreateDocument(string html)
        {
            return CreateDocument(Support.StringToCharArray(html));
        }

        /// <summary>
        /// Creeate a new DOM from HTML text using full HTML5 tag generation.
        /// </summary>
        ///
        /// <param name="html">
        /// A character array containing HTML
        /// </param>
        ///
        /// <returns>
        /// The new document.
        /// </returns>

        public static CQ CreateDocument(char[] html)
        {
            CQ csq = new CQ();
            csq.CreateNewDocument(html.AsString(), HtmlParsingMode.Document);
            
            return csq;
        }

        /// <summary>
        /// Creates a new DOM from a stream containing HTML
        /// </summary>
        ///
        /// <param name="html">
        /// A n open Stream
        /// </param>
        ///
        /// <returns>
        /// The new document.
        /// </returns>

        public static CQ CreateDocument(Stream html)
        {
            return CreateDocument(Support.StreamToCharArray(html));
        }

        /// <summary>
        /// Creates a new DOM from an HTML file.
        /// </summary>
        ///
        /// <param name="htmlFile">
        /// The full path to the file
        /// </param>
        ///
        /// <returns>
        /// The new document from file.
        /// </returns>

        public static CQ CreateDocumentFromFile(string htmlFile)
        {
            using (Stream strm = Support.GetFileStream(htmlFile))
            {
                return CQ.CreateDocument(strm);
            }
        }

        /// <summary>
        /// Creates a new DOM from an HTML file.
        /// </summary>
        ///
        /// <param name="htmlFile">
        /// The full path to the file
        /// </param>
        ///
        /// <returns>
        /// The new from file.
        /// </returns>

        public static CQ CreateFromFile(string htmlFile)
        {
            using (Stream strm = Support.GetFileStream(htmlFile))
            {
                return CQ.Create(strm);
            }
        }

    }
}
