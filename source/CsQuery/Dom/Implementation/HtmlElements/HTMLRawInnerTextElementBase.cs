using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// The value of the TEXTAREA's contents
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
        /// For TEXTAREA elements, InnerText doesn't actually do anything, whereas Value is the InnerText.
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

        protected override void RenderChildTextNode(IDomObject textNode, StringBuilder sb)
        {
            sb.Append(textNode.NodeValue);
        }



    }
}
