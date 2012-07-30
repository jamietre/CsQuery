using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.HtmlParser;
using CsQuery.Utility;

namespace CsQuery.Implementation
{
    /// <summary>
    /// An Anchor (A) element.
    /// </summary>
    ///
    /// <url>
    /// http://dev.w3.org/html5/spec/single-page.html#the-a-element
    /// </url>
    public class HtmlAnchorElement : DomElement, IHTMLAnchorElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>

        public HtmlAnchorElement()
            : base(HtmlData.tagA)
        {
        }

        /// <summary>
        /// A name or keyword giving a browsing context for UAs to use when following the hyperlink.
        /// </summary>

        public string Target
        {
            get
            {
                return GetAttribute("target");
            }
            set
            {
                SetAttribute("target", value);
            }
        }

        /// <summary>
        /// A URL that provides the destination of the hyperlink. If the href attribute is not specified,
        /// the element represents a placeholder hyperlink.
        /// </summary>

        public string Href
        {
            get
            {
                return GetAttribute("href");
            }
            set
            {
                SetAttribute("href",value);
            }
        }

        public RelAnchor Rel
        {
            get
            {
                return Support.AttributeToEnum<RelAnchor>(GetAttribute("rel"));
            }
            set
            {
                SetAttribute("rel", Support.EnumToAttribute(value));
            }
        }

        public string Hreflang
        {
            get
            {
                return GetAttribute("hreflang");
            }
            set
            {
                SetAttribute("hreflang", value);
            }
        }

        public string Media
        {
            get
            {
                return GetAttribute("media");
            }
            set
            {
                SetAttribute("media", value);
            }
        }
    }
}
