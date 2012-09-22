using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using HtmlParserSharp.Common;
using HtmlParserSharp.Core;
using CsQuery;
using CsQuery.Implementation;

namespace HtmlParserSharp
{
    /// <summary>
    /// The tree builder glue for building a tree through the public DOM APIs.
    /// </summary>
    class DomTreeBuilder : CoalescingTreeBuilder<IDomElement>
    {
        /// <summary>
        /// The current doc.
        /// </summary>
        private DomDocument document;

        override protected void AddAttributesToElement(IDomElement element, HtmlAttributes attributes)
        {
            for (int i = 0; i < attributes.Length; i++)
            {  
                string attributeName = AttributeName(attributes.GetLocalName(i),attributes.GetURI(i));
                if (!element.HasAttribute(attributeName))
                {
                    element.SetAttribute(attributeName,
                            attributes.GetValue(i));
                }
            }
        }

        override protected void AppendCharacters(IDomElement parent, string text)
        {
            //IDomObject lastChild = parent.LastChild;
            //if (lastChild != null && lastChild.NodeType == NodeType.TEXT_NODE)
            //{
            //    IDomText lastAsText = (IDomText)lastChild;
            //    lastAsText.NodeValue += text;
            //    return;
            //}
            parent.AppendChild(document.CreateTextNode(text));
        }

        override protected void AppendChildrenToNewParent(IDomElement oldParent, IDomElement newParent)
        {
            while (oldParent.HasChildren)
            {
                newParent.AppendChild(oldParent.FirstChild);
            }
        }

        protected override void AppendDoctypeToDocument(string name, string publicIdentifier, string systemIdentifier)
        {
            // TODO: this method was not there originally. is it correct?

            if (publicIdentifier == String.Empty)
                publicIdentifier = null;
            if (systemIdentifier == String.Empty)
                systemIdentifier = null;

            var doctype = new DomDocumentType(DocType.HTML5);

            document.AppendChild(doctype);
        }

        override protected void AppendComment(IDomElement parent, String comment)
        {
            //parent.AppendChild(document.CreateComment(comment));
            parent.AppendChild(new DomComment(comment));
        }

        override protected void AppendCommentToDocument(String comment)
        {
            document.AppendChild(document.CreateComment(comment));
        }

        override protected IDomElement CreateElement(string ns, string name, HtmlAttributes attributes)
        {
            // ns is not used
            IDomElement rv = document.CreateElement(name);
            for (int i = 0; i < attributes.Length; i++)
            {

                string attributeName = AttributeName(attributes.GetLocalName(i), attributes.GetURI(i));
                rv.SetAttribute(attributeName, attributes.GetValue(i));
                if (attributes.GetType(i) == "ID")
                {
                    //rv.setIdAttributeNS(null, attributes.GetLocalName(i), true); // FIXME
                }
            }
            return rv;
        }

        override protected IDomElement CreateHtmlElementSetAsRoot(HtmlAttributes attributes)
        {
            IDomElement rv = document.CreateElement("html");
            for (int i = 0; i < attributes.Length; i++)
            {
                string attributeName = AttributeName(attributes.GetLocalName(i), attributes.GetURI(i));
                rv.SetAttribute(attributeName, attributes.GetValue(i));
            }
            document.AppendChild(rv);
            return rv;
        }

        override protected void AppendElement(IDomElement child, IDomElement newParent)
        {
            newParent.AppendChild(child);
        }

        override protected bool HasChildren(IDomElement element)
        {
            return element.HasChildren;
        }

        override protected IDomElement CreateElement(string ns, string name, HtmlAttributes attributes, IDomElement form)
        {
            IDomElement rv = CreateElement(ns, name, attributes);
            //rv.setUserData("nu.validator.form-pointer", form, null); // TODO
            return rv;
        }

        override protected void Start(bool fragment)
        {
            document = new DomDocument(); // implementation.createDocument(null, null, null);
            // TODO: fragment?
        }

        protected override void ReceiveDocumentMode(DocumentMode mode, String publicIdentifier,
                String systemIdentifier, bool html4SpecificAdditionalErrorChecks)
        {
            //document.setUserData("nu.validator.document-mode", mode, null); // TODO
        }

        /// <summary>
        /// Returns the document.
        /// </summary>
        /// <returns>The document</returns>
        internal IDomDocument Document
        {
            get
            {
                return document;
            }
        }

        /// <summary>
        /// Return the document fragment.
        /// </summary>
        /// <returns>The document fragment</returns>
        internal IDomFragment getDocumentFragment()
        {
            IDomFragment rv = new DomFragment();
            //IDomElement rootElt = document.FirstChild;
            //while (rootElt.HasChildNodes)
            //{
            //    rv.AppendChild(rootElt.FirstChild);
            //}
            //document = null;
            return rv;
        }

        override protected void InsertFosterParentedCharacters(string text, IDomElement table, IDomElement stackParent)
        {
            IDomObject parent = table.ParentNode;
            if (parent != null)
            { // always an element if not null
                IDomObject previousSibling = table.PreviousSibling;
                if (previousSibling != null
                        && previousSibling.NodeType == NodeType.TEXT_NODE)
                {
                    IDomText lastAsText = (IDomText)previousSibling;
                    lastAsText.NodeValue += text;
                    return;
                }
                parent.InsertBefore(document.CreateTextNode(text), table);
                return;
            }
            IDomObject lastChild = stackParent.LastChild;
            if (lastChild != null && lastChild.NodeType ==  NodeType.TEXT_NODE)
            {
                IDomText lastAsText = (IDomText)lastChild;
                lastAsText.NodeValue += text;
                return;
            }
            stackParent.AppendChild(document.CreateTextNode(text));
        }

        override protected void InsertFosterParentedChild(IDomElement child, IDomElement table, IDomElement stackParent)
        {
            IDomObject parent = table.ParentNode;
            if (parent != null)
            { // always an element if not null
                parent.InsertBefore(child, table);
            }
            else
            {
                stackParent.AppendChild(child);
            }
        }

        override protected void DetachFromParent(IDomElement element)
        {
            IDomObject parent = element.ParentNode;
            if (parent != null)
            {
                parent.RemoveChild(element);
            }
        }

        private string AttributeName(string localName, string uri)
        {
           
            string attributeName = localName;
           
            return String.IsNullOrEmpty(uri) ?
                attributeName :
                attributeName += ":" + uri;
         }
    }
}
