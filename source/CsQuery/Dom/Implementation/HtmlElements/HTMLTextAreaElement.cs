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

    public class HTMLTextAreaElement : HTMLRawInnerTextElementBase
    {
       
        public HTMLTextAreaElement(): base(HtmlData.tagTEXTAREA)
        {

        }

    }
}
