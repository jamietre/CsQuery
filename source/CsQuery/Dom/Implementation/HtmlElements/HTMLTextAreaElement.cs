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

    public class HTMLTextAreaElement : DomElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>

        public HTMLTextAreaElement()
            : base(HtmlData.tagTEXTAREA)
        {
        }

        /// <summary>
        /// The value of the TEXTAREA's contents
        /// </summary>

        public override string Value
        {
            get
            {
                return InnerText;
            }
            set
            {
                base.Value = value;
            }
        }
    }
}
