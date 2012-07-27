using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.HtmlParser;

namespace CsQuery.Implementation
{
    public class HtmlAnchorElement : DomElement, IHTMLAnchorElement
    {
        public HtmlAnchorElement()
            : base(HtmlData.tagA)
        {
        }


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

        public string Rel
        {
            get
            {
                return GetAttribute("rel");
            }
            set
            {
                SetAttribute("rel", value);
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
