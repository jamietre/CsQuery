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
            csq.CreateNewFragment(Objects.Enumerate(element));
            return csq;
        }

        /// <summary>
        /// Create a new CQ from a single element
        /// </summary>
        ///
        /// <param name="element">
        /// The element to wrap.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        ///
        /// <returns>
        ///  A new CQ object.
        /// </returns>

        public static CQ Create(IDomObject element, CQ context)
        {
            return new CQ(element,context);
        }

        /// <summary>
        /// Creeate a new CQ object from an HTML string
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static CQ Create(string html)
        {
            return new CQ(html);
        }



        /// <summary>
        /// Create a new CQ from an HTML fragment, and use quickSet to create attributes (and/or css)
        /// </summary>
        /// <param name="html">A string of HTML</param>
        /// <param name="quickSet">an object containing CSS properties and attributes to be applied to the resulting fragment</param>
        /// <returns></returns>
        public static CQ Create(string html, object quickSet)
        {
            CQ csq = CQ.CreateFragment(html);
            return csq.AttrSet(quickSet, true);
        }

        /// <summary>
        /// Create a new CQ object from an existing context, bound to the same domain.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="context"></param>
        private void Create(string selector, CQ context)
        {
             //when creating a new CsQuery from another, leave Document blank - it will be populated
             //automatically with the contents of the selector. 

            var csq = new CQ();
            CsQueryParent = context;

            if (!String.IsNullOrEmpty(selector))
            {
                Selector = new Selector(selector);
                AddSelection(Selector.Select(Document, context));
            }
        }



        /// <summary>
        /// Creeate a new CQ object from a squence of elements, or another CQ object
        /// </summary>
        /// <param name="elements">A sequence of elements</param>
        /// <returns></returns>
        public static CQ Create(IEnumerable<IDomObject> elements)
        {
            CQ csq = new CQ();
            //csq.LoadFragment(elements);
            csq.CreateNewFragment(elements);
            return csq;
        }

        /// <summary>
        /// Create a new CQ object from a stream of HTML, treating the HTML as a content document
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static CQ Create(Stream stream)
        {
            return Create(Support.StreamToCharArray(stream));
        }

        /// <summary>
        /// Creeate a new fragment from HTML text
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static CQ CreateFragment(string html)
        {
            return CQ.CreateFragment(Support.StringToCharArray(html));
        }


        /// <summary>
        /// Create a new fragment from HTML text
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static CQ CreateFragment(char[] html)
        {
            CQ csq = new CQ();
            //csq.LoadFragment(html);
            csq.CreateNewFragment(html, HtmlParsingMode.Fragment);
            return csq;
        }

        /// <summary>
        /// Create a new fragment from a stream of HTML text
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static CQ CreateFragment(Stream html)
        {
            return CreateFragment(Support.StreamToCharArray(html));
        }

        /// <summary>
        /// Creeate a new CQ object from a squence of elements, or another CQ object
        /// </summary>
        /// <param name="elements">A sequence of elements</param>
        /// <returns></returns>
        public static CQ CreateFragment(IEnumerable<IDomObject> elements)
        {
            // this is synonymous with the Create method of the same sig because we definitely
            // would never autogenerate elements from a sequence of elements

            return Create(elements);
        }

        /// <summary>
        /// Creeate a new DOM from HTML text using full HTML5 tag generation
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static CQ CreateDocument(string html)
        {

            return CreateDocument(Support.StringToCharArray(html));
        }

        /// <summary>
        /// Creeate a new DOM from HTML text using full HTML5 tag generation
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static CQ CreateDocument(char[] html)
        {
            CQ csq = new CQ();
            //html.csq.LoadDocument(html);
            csq.CreateNewDocument(html, HtmlParsingMode.Document);
            
            return csq;
        }


        /// <summary>
        /// UNTESTED!
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static CQ CreateDocument(Stream html)
        {
            return CreateDocument(Support.StreamToCharArray(html));
        }
        /// <summary>
        /// Creates a new DOM from an HTML file.
        /// </summary>
        /// <param name="htmlFile"></param>
        /// <returns></returns>
        public static CQ CreateDocumentFromFile(string htmlFile)
        {
            return CQ.CreateDocument(Support.GetFileStream(htmlFile));
        }

        /// <summary>
        /// Creates a new DOM from an HTML file.
        /// </summary>
        /// <param name="htmlFile"></param>
        /// <returns></returns>
        public static CQ CreateFromFile(string htmlFile)
        {
            return CQ.Create(Support.GetFileStream(htmlFile));
        }

    }
}
