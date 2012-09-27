using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using CsQuery.HtmlParser;

namespace CsQuery.Implementation
{
    /// <summary>
    /// Used for literal text (not part of a tag)
    /// </summary>
    public class DomText : DomObject<DomText>, IDomText
    {
        public DomText()
        {

        }

        public DomText(string nodeValue)
            : base()
        {
            NodeValue = nodeValue;
        }

        public override string NodeName
        {
            get
            {
                return "#text";
            }
        }

        public override NodeType NodeType
        {
            get { return NodeType.TEXT_NODE; }
        }

        protected string nodeValue;
        
        public override string NodeValue
        {
            get
            {
                return nodeValue;
            }
            set
            {
                nodeValue = value;
            }
        }

        /// <summary>
        /// Renders this text node using document default rendering options
        /// </summary>
        ///
        /// <returns>
        /// A string of HTML-encoded text
        /// </returns>

        public override string Render(DomRenderingOptions options = DomRenderingOptions.Default)
        {
            return HtmlData.HtmlEncode(NodeValue,
                options.HasFlag(DomRenderingOptions.HtmlEncodingNone) ? 
                    HtmlEncodingMethod.HtmlEncodingNone :
                    options.HasFlag(DomRenderingOptions.HtmlEncodingMinimum) ?
                        HtmlEncodingMethod.HtmlEncodingMinimum :
                        HtmlEncodingMethod.HtmlEncodingFull);
        }
        
        /// <summary>
        /// Renders this text node using document default rendering options to the provided StringBuilder
        /// </summary>
        ///
        /// <param name="sb">
        /// A StringBuilder object
        /// </param>

        public override void Render(StringBuilder sb, DomRenderingOptions options = DomRenderingOptions.Default)
        {
            sb.Append(Render(options));
        }
        public override DomText Clone()
        {
            return new DomText(nodeValue);
        }

        public override bool InnerHtmlAllowed
        {
            get { return false; }
        }
        public override bool HasChildren
        {
            get { return false; }
        }
        public override string ToString()
        {
            return NodeValue;
        }

    }
}
