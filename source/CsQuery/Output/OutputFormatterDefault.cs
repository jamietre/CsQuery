using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CsQuery.HtmlParser;

namespace CsQuery.Output
{
    /// <summary>
    /// Default output formatter.
    /// </summary>

    public class OutputFormatterDefault: IOutputFormatter
    {
        /// <summary>
        /// Abstract base class constructor.
        /// </summary>
        ///
        /// <param name="options">
        /// Options for controlling the operation.
        /// </param>
        /// <param name="encoder">
        /// The encoder.
        /// </param>

        public OutputFormatterDefault(DomRenderingOptions options, IHtmlEncoder encoder)
        {
            DomRenderingOptions = options;
            MergeDefaultOptions();
            HtmlEncoder = encoder ?? HtmlEncoders.Default;
        }

        /// <summary>
        /// Creates the default OutputFormatter using default DomRenderingOption values and default HtmlEncoder
        /// </summary>

        public OutputFormatterDefault()
            : this(DomRenderingOptions.Default, HtmlEncoders.Default)
        { }

        private DomRenderingOptions DomRenderingOptions;
        private IHtmlEncoder HtmlEncoder;

        /// <summary>
        /// Renders the object to the textwriter.
        /// </summary>
        ///
        /// <exception cref="NotImplementedException">
        /// Thrown when the requested operation is unimplemented.
        /// </exception>
        ///
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="writer">
        /// The writer to which output is written.
        /// </param>

        public void Render(IDomObject node, TextWriter writer)
        {

            switch (node.NodeType) {
                case NodeType.ELEMENT_NODE:
                    RenderElement(node,writer,true);
                    break;
                case NodeType.DOCUMENT_FRAGMENT_NODE:
                case NodeType.DOCUMENT_NODE:
                    RenderElements(node.ChildNodes,writer);
                    break;
                case NodeType.TEXT_NODE:
                    RenderTextNode(node, writer,false);
                    break;
                case NodeType.CDATA_SECTION_NODE:
                    RenderCdataNode(node, writer);
                    break;
                case NodeType.COMMENT_NODE:
                    RenderCommentNode(node, writer);
                    break;
                case NodeType.DOCUMENT_TYPE_NODE:
                    RenderDocTypeNode(node, writer);
                    break;
                default:
                    throw new NotImplementedException("An unknown node type was found while rendering the CsQuery document.");
            }
        }

        /// <summary>
        /// Renders the object to a string.
        /// </summary>
        ///
        /// <param name="node">
        /// The node.
        /// </param>
        ///
        /// <returns>
        /// A string.
        /// </returns>

        public virtual string Render(IDomObject node)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            Render(node, sw);
            return sb.ToString();
        }

        /// <summary>
        /// Renders the sequence of elements.
        /// </summary>
        ///
        /// <param name="elements">
        /// The elements.
        /// </param>
        /// <param name="writer">
        /// The writer to which output is written.
        /// </param>

        protected virtual void RenderElements(IEnumerable<IDomObject> elements,TextWriter writer)
        {
            foreach (var item in elements)
            {
                Render(item, writer);
            }
        }

        /// <summary>
        /// Gets the HTML representation of this element and its children
        /// </summary>
        ///
        /// <param name="options">
        /// Options for how to render the HTML.
        /// </param>
        /// <param name="sb">
        /// A StringBuilder object to which append the output.
        /// </param>
        /// <param name="includeChildren">
        /// true to include, false to exclude the children.
        /// </param>

        public virtual void RenderElement(IDomObject el, TextWriter writer,bool includeChildren)
        {
            bool quoteAll = DomRenderingOptions.HasFlag(DomRenderingOptions.QuoteAllAttributes);

            writer.Write("<");
            string nodeName = el.NodeName.ToLower();
            writer.Write(nodeName);
            
            if (el.HasAttributes)
            {
                foreach (var kvp in el.Attributes)
                {
                    writer.Write(" ");
                    RenderAttribute(writer,kvp.Key, kvp.Value, quoteAll);
                }
            }
            if (el.InnerHtmlAllowed || el.InnerTextAllowed)
            {
                writer.Write(">");
                if (includeChildren)
                {
                    RenderChildren(el, writer);
                }
                else
                {
                    writer.Write(el.HasChildren ?
                            "..." :
                            String.Empty);
                }
                writer.Write("</");
                writer.Write(nodeName);
                writer.Write(">");
            }
            else
            {

                if ((el.Document == null ? CQ.DefaultDocType : el.Document.DocType) == DocType.XHTML)
                {
                    writer.Write(" />");
                }
                else
                {
                    writer.Write(">");
                }
            }
        }

        /// <summary>
        /// Renders all the children of the passed node.
        /// </summary>
        ///
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="writer">
        /// The writer to which output is written.
        /// </param>

        public virtual void RenderChildren(IDomObject element, TextWriter writer)
        {
            if (element.HasChildren)
            {
                bool isRaw = HtmlData.HtmlChildrenNotAllowed(element.NodeNameID);

                foreach (IDomObject e in element.ChildNodes)
                {
                    if (e.NodeType == NodeType.TEXT_NODE)
                    {
                        RenderTextNode(e, writer, isRaw);
                    }
                    else
                    {
                        Render(e, writer);
                    }
                }
            }
        }

        /// <summary>
        /// Renders the text node.
        /// </summary>
        ///
        /// <param name="textNode">
        /// The text node.
        /// </param>
        /// <param name="writer">
        /// The writer to which output is written.
        /// </param>
        /// <param name="raw">
        /// true to raw.
        /// </param>

        protected virtual void RenderTextNode(IDomObject textNode, TextWriter writer, bool raw)
        {
            if (raw)
            {
                writer.Write(textNode.NodeValue);
            }
            else
            {
                HtmlEncoder.Encode(textNode.NodeValue, writer);
            }
        }

        /// <summary>
        /// Renders a CDATA node.
        /// </summary>
        ///
        /// <param name="parameter1">
        /// The first parameter.
        /// </param>
        /// <param name="parameter2">
        /// The second parameter.
        /// </param>

        protected void RenderCdataNode(IDomObject element, TextWriter writer)
        {
            writer.Write("<![CDATA[" + element.NodeValue + ">");
        }

        /// <summary>
        /// Renders the comment node.
        /// </summary>
        ///
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="writer">
        /// The writer to which output is written.
        /// </param>

        protected void RenderCommentNode(IDomObject element, TextWriter writer)
        {
            if (DomRenderingOptions.HasFlag(DomRenderingOptions.RemoveComments))
            {
                return;
            }
            else
            {
                writer.Write("<!--" + element.NodeValue + "-->");
            }
        }

        protected void RenderDocTypeNode(IDomObject element, TextWriter writer)
        {

            writer.Write("<!DOCTYPE " + ((IDomSpecialElement)element).NonAttributeData + ">");
        }

        /// <summary>
        /// Render an attribute
        /// </summary>
        ///
        /// <param name="sb">
        /// A StringBuilder to append the attributes to
        /// </param>
        /// <param name="name">
        /// The name of the attribute
        /// </param>
        /// <param name="value">
        /// The attribute value
        /// </param>
        /// <param name="quoteAll">
        /// true to require quotes around the attribute value, false to use quotes only if needed.
        /// </param>

        protected void RenderAttribute(TextWriter writer, string name, string value, bool quoteAll)
        {
            // validator.nu: as it turns out "" and missing are synonymous
            // don't ever render attr=""

            if (value != null && value != "")
            {
                string quoteChar;
                string attrText = HtmlData.AttributeEncode(value,
                    quoteAll,
                    out quoteChar);
                writer.Write(name.ToLower());
                writer.Write("=");
                writer.Write(quoteChar);
                writer.Write(attrText);
                writer.Write(quoteChar);
            }
            else
            {
                writer.Write(name);
            }
        }

                /// <summary>
        /// Merge options with defaults when needed
        /// </summary>
        ///
        /// <param name="options">
        /// (optional) options for controlling the operation.
        /// </param>

        protected void MergeDefaultOptions()
        {
            if (DomRenderingOptions.HasFlag(DomRenderingOptions.Default))
            {
                DomRenderingOptions = CsQuery.Config.DomRenderingOptions | DomRenderingOptions & ~(DomRenderingOptions.Default);
            }
        }



    }
}
