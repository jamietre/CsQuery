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
    class CsQueryTreeBuilder : CoalescingTreeBuilder<DomObject>
    {
        
        /// <summary>
        /// The current doc.
        /// </summary>
        private DomDocument document;

        /// <summary>
        /// This is a fragment
        /// </summary>

        private bool isFragment;


        override protected void AddAttributesToElement(DomObject element, HtmlAttributes attributes)
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


        override protected void AppendCharacters(DomObject parent, string text)
        {
            IDomText lastChild = parent.LastChild as IDomText;
            if (lastChild != null)
            {
                ((IDomText)lastChild).NodeValue += text;
                
            } else {
                lastChild = document.CreateTextNode(text);
                parent.AppendChildUnsafe(lastChild);
            }
        }

        override protected void AppendChildrenToNewParent(DomObject oldParent, DomObject newParent)
        {
            while (oldParent.HasChildren)
            {
                // cannot use unsafe method here - this method specifically moves children
                newParent.AppendChild(oldParent.FirstChild);
            }
        }

        protected override void AppendDoctypeToDocument(string name, string publicIdentifier, string systemIdentifier)
        {
            var doctype = document.CreateDocumentType(name,publicIdentifier,systemIdentifier);

            document.AppendChildUnsafe(doctype);
        }

        override protected void AppendComment(DomObject parent, String comment)
        {
            parent.AppendChildUnsafe(new DomComment(comment));
        }

        override protected void AppendCommentToDocument(String comment)
        {
            document.AppendChildUnsafe(document.CreateComment(comment));
        }

        override protected DomObject CreateElement(string ns, string name, HtmlAttributes attributes)
        {
            // ns is not used
            DomElement rv = DomElement.Create(name);
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

        override protected DomObject CreateHtmlElementSetAsRoot(HtmlAttributes attributes)
        {
            if (!isFragment)
            {
                DomElement rv = DomElement.Create("html");
                for (int i = 0; i < attributes.Length; i++)
                {
                    string attributeName = AttributeName(attributes.GetLocalName(i), attributes.GetURI(i));
                    rv.SetAttribute(attributeName, attributes.GetValue(i));
                }
                document.AppendChildUnsafe(rv);
                return rv;
            }
            else
            {
                return document;
            }
        }

        override protected void AppendElement(DomObject child, DomObject newParent)
        {
           newParent.AppendChildUnsafe(child);
        }

        override protected bool HasChildren(DomObject element)
        {
            return element.HasChildren;
        }

        override protected DomObject CreateElement(string ns, string name, HtmlAttributes attributes, DomObject form)
        {
            DomObject rv = CreateElement(ns, name, attributes);
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

        override protected void InsertFosterParentedCharacters(string text, DomObject table, DomObject stackParent)
        {
            IDomObject parent = table.ParentNode;
            if (parent != null)
            { 
                // always an element if not null
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
            // fall through
            
            IDomText lastChild = stackParent.LastChild as IDomText;
            if (lastChild != null)
            {
                lastChild.NodeValue += text;
                return;
            }
            else
            {
                stackParent.AppendChildUnsafe(document.CreateTextNode(text));
            }
        }

        override protected void InsertFosterParentedChild(DomObject child, DomObject table, DomObject stackParent)
        {
            IDomObject parent = table.ParentNode;
            if (parent != null)
            { 
                // always an element if not null
                parent.InsertBefore(child, table);
            }
            else
            {
                stackParent.AppendChildUnsafe(child);
            }
        }

        override protected void DetachFromParent(DomObject element)
        {
            IDomObject parent = element.ParentNode;
            if (parent != null)
            {
                parent.RemoveChild(element);
            }
        }

        private string AttributeName(string localName, string uri)
        {
            return String.IsNullOrEmpty(uri) ?
                localName :
                localName += ":" + uri;
        }
    }
}
