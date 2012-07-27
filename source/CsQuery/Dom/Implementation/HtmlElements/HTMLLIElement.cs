using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.HtmlParser;
using CsQuery.Utility;

namespace CsQuery.Implementation
{
    /// <summary>
    /// An HTML LI element.
    /// </summary>

    public class HTMLLIElement : DomElement, IHTMLLIElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>

        public HTMLLIElement()
            : base(HtmlData.tagLI)
        {
        }

        public new int Value
        {
            get
            {
                return Support.IntOrZero(GetAttribute(HtmlData.ValueAttrId));
            }
            set
            {
                SetAttribute(HtmlData.ValueAttrId, value.ToString());
            }
        }

    

       
    }
}
