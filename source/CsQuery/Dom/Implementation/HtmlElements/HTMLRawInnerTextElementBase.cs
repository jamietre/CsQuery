using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CsQuery.HtmlParser;

namespace CsQuery.Implementation
{
    /// <summary>
    /// An HTML text area element.
    /// </summary>

    public abstract class HTMLRawInnerTextElementBase : DomElement
    {

        public HTMLRawInnerTextElementBase(ushort tagName) :
            base(tagName)
        {

        }

        /// <summary>
        /// The value of the HTMLRawInnerTextElementBase's contents
        /// </summary>

        public override string Value
        {
            get
            {
                return base.InnerText;
            }
            set
            {
                base.InnerText = value;
            }
        }

        /// <summary>
        /// For HTMLRawInnerTextElementBase elements, InnerText doesn't actually do anything, whereas Value is the InnerText.
        /// </summary>

        public new string InnerText
        {
            get
            {
                return "";
            }
            set
            {
                return;
            }
        }

        /// <summary>
        /// Renders the child text node. DomRenderingOptions are ignored when rendering
        /// HTMLRawInnerTextElementBase-derived objects.
        /// </summary>
        ///
        /// <param name="textNode">
        /// The text node.
        /// </param>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="options">
        /// This paremeter is ignored.
        /// </param>

        protected override void RenderChildTextNode(IDomObject textNode, TextWriter writer, DomRenderingOptions options)
        {
            writer.Write(textNode.NodeValue);
        }



    }
}
