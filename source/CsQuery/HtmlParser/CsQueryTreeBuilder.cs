using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using HtmlParserSharp.Common;
using HtmlParserSharp.Core;
using CsQuery;
using CsQuery.Implementation;

namespace CsQuery.HtmlParser
{
    /// <summary>
    /// The tree builder glue for building a tree through the public DOM APIs.
    /// </summary>
    class CsQueryTreeBuilder : CoalescingTreeBuilder<IDomObject>
    {
        
        /// <summary>
        /// The current doc.
        /// </summary>
        private DomDocument document;

        /// <summary>
        /// This is a fragment
        /// </summary>

        private bool isFragment;

        override protected void AddAttributesToElement(IDomObject element, HtmlAttributes attributes)
        {
            for (int i = 0; i < attributes.Length; i++)
            {
                string attributeName = AttributeName(attributes.GetLocalName(i), attributes.GetURI(i));
                if (!element.HasAttribute(attributeName))
                {
                    element.SetAttribute(attributeName, attributes.GetValue(i));
                }
            }
        }


        override protected void AppendCharacters(IDomObject parent, string text)
        {

            IDomObject lastChild = parent.LastChild;
            if (lastChild == null || lastChild.NodeType != NodeType.TEXT_NODE)
            {
                lastChild = document.CreateTextNode(text);
                parent.AppendChild(lastChild);
            } else {
                ((IDomText)lastChild).NodeValue += text;
            }
        }

        override protected void AppendChildrenToNewParent(IDomObject oldParent, IDomObject newParent)
        {
            while (oldParent.HasChildren)
            {
                newParent.AppendChild(oldParent.FirstChild);
            }
        }

        protected override void AppendDoctypeToDocument(string name, string publicIdentifier, string systemIdentifier)
        {
            var doctype = document.CreateDocumentType(name,publicIdentifier,systemIdentifier);

            document.AppendChild(doctype);
        }

        override protected void AppendComment(IDomObject parent, String comment)
        {
            //parent.AppendChild(document.CreateComment(comment));
            parent.AppendChild(new DomComment(comment));
        }

        override protected void AppendCommentToDocument(String comment)
        {
            document.AppendChild(document.CreateComment(comment));
        }

        override protected IDomObject CreateElement(string ns, string name, HtmlAttributes attributes)
        {
            // ns is not used
            IDomElement rv = document.CreateElement(name);
            for (int i = 0; i < attributes.Length; i++)
            {

                string attributeName = AttributeName(attributes.GetLocalName(i), attributes.GetURI(i));
                rv.SetAttribute(attributeName, attributes.GetValue(i));
                //if (attributes.GetType(i) == "ID")
                //{
                    //rv.setIdAttributeNS(null, attributes.GetLocalName(i), true); // FIXME
                //}
            }
            return rv;
        }

        override protected IDomObject CreateHtmlElementSetAsRoot(HtmlAttributes attributes)
        {
            if (!isFragment)
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
            else
            {
                return document;
            }
        }

        override protected void AppendElement(IDomObject child, IDomObject newParent)
        {
            newParent.AppendChild(child);
        }

        override protected bool HasChildren(IDomObject element)
        {
            return element.HasChildren;
        }

        override protected IDomObject CreateElement(string ns, string name, HtmlAttributes attributes, IDomObject form)
        {
            IDomObject rv = CreateElement(ns, name, attributes);
            //rv.setUserData("nu.validator.form-pointer", form, null); // TODO
            return rv;
        }

        override protected void Start(bool fragment)
        {
            isFragment = fragment;
            document = fragment ?
                new DomFragment() :
                new DomDocument();
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
        //internal IDomFragment getDocumentFragment()
        //{
            //IDomFragment rv = new DomFragment();
            //IDomObject rootElt = document;
            //while (rootElt.HasChildren)
            //{
            //    rv.AppendChild(rootElt.FirstChild);
            //}
            //document = null;
            //return rv;
        //}

        override protected void InsertFosterParentedCharacters(string text, IDomObject table, IDomObject stackParent)
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
            if (lastChild != null && lastChild.NodeType == NodeType.TEXT_NODE)
            {
                IDomText lastAsText = (IDomText)lastChild;
                lastAsText.NodeValue += text;
                return;
            }
            stackParent.AppendChild(document.CreateTextNode(text));
        }

        override protected void InsertFosterParentedChild(IDomObject child, IDomObject table, IDomObject stackParent)
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

        override protected void DetachFromParent(IDomObject element)
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
