using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.Utility;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.Engine;
using CsQuery.Implementation;

namespace CsQuery
{
    public partial class CQ
    {
        #region public methods

        /// <summary>
        /// Get the combined text contents of each element in the set of matched elements, including
        /// their descendants.
        /// </summary>
        ///
        /// <returns>
        /// A string containing the text contents of the selection.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/text/#text1
        /// </url>

        public string Text()
        {
            StringBuilder sb = new StringBuilder();

            Text(sb, SelectionSet);

            return sb.ToString();
        }

        /// <summary>
        /// Set the content of each element in the set of matched elements to the specified text.
        /// </summary>
        ///
        /// <param name="value">
        /// A string of text.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/text/#text2
        /// </url>

        public CQ Text(string value)
        {
            foreach (IDomElement obj in Elements)
            {
                SetChildText(obj, value);
            }
            return this;
        }

        /// <summary>
        /// Set the content of each element in the set of matched elements to the text returned by the
        /// specified function delegate.
        /// </summary>
        ///
        /// <param name="func">
        /// A delegate to a function that returns an HTML string to insert at the end of each element in
        /// the set of matched elements. Receives the index position of the element in the set and the
        /// old HTML value of the element as arguments. The function can return any data type, if it is not
        /// a string, it's ToString() method will be used to convert it to a string.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/text/#text2
        /// </url>

        public CQ Text(Func<int, string, object> func)
        {

            int count = 0;
            StringBuilder sb = new StringBuilder();
            foreach (IDomElement obj in Elements)
            {
                sb.Clear();
                Text(sb, obj);
                string newText = func(count, sb.ToString()).ToString();
                if (sb.ToString() != newText)
                {
                    SetChildText(obj, newText);
                }
                count++;
            }
            return this;
        }

        #endregion

        #region private methods

        /// <summary>
        /// Helper for public Text() function to act recursively.
        /// </summary>
        ///
        /// <param name="sb">
        /// .
        /// </param>
        /// <param name="elements">
        /// .
        /// </param>

        private void Text(StringBuilder sb, IEnumerable<IDomObject> elements)
        {
            IDomObject lastElement = null;
            foreach (IDomObject obj in elements)
            {
                //string text = Text(obj);
                
                int len = sb.Length;
                
                Text(sb, obj);
                
                if (lastElement != null && obj.Index > 0
                   && obj.PreviousSibling != lastElement
                    && sb.Length > len)
                {
                    sb.Append(" ");
                }

                lastElement = obj;

            }
        }

        /// <summary>
        /// Get the combined text contents of this and all child elements.
        /// </summary>
        ///
        /// <param name="sb">
        /// The StribgBuilder object to write to
        /// </param>
        /// <param name="obj">
        /// The object.
        /// </param>
        
        private void Text(StringBuilder sb, IDomObject obj)
        {
            switch (obj.NodeType)
            {
                case NodeType.TEXT_NODE:
                case NodeType.CDATA_SECTION_NODE:
                case NodeType.COMMENT_NODE:
                    sb.Append(obj.NodeValue);
                    break;
                case NodeType.ELEMENT_NODE:
                case NodeType.DOCUMENT_FRAGMENT_NODE:
                case NodeType.DOCUMENT_NODE:
                    Text(sb, obj.ChildNodes);
                    break;
                case NodeType.DOCUMENT_TYPE_NODE:
                default:
                    break;
            }
        }


        /// <summary>
        /// Sets a child text for this element, using the text node type appropriate for this element's type
        /// </summary>
        ///
        /// <param name="el">
        /// The element to add text to
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>

        private void SetChildText(IDomElement el, string text)
        {
            if (el.ChildrenAllowed)
            {
                el.ChildNodes.Clear();

                // Element types that cannot have HTML contents should not have the value encoded.
                // use DomInnerText node for those node types to preserve the raw text value

                //IDomText textEl = el.InnerHtmlAllowed ?
                //    new DomText(text) :
                //    new DomInnerText(text);
                IDomText textEl = new DomText(text);
                el.ChildNodes.Add(textEl);
            }

        }

        #endregion

    }
}
