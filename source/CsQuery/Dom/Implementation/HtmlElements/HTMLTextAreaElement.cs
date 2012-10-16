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

    public class HTMLTextAreaElement : DomElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>

        public HTMLTextAreaElement(): base(HtmlData.tagTEXTAREA)
        {

        }

        /// <summary>
        /// The value of the HTMLRawInnerTextElementBase's contents
        /// </summary>

        public override string Value
        {
            get
            {
                var formatter = new Output.FormatDefault();
                StringWriter sw = new StringWriter();

                formatter.RenderChildren(this, sw);
                return sw.ToString();
            }
            set
            {
                ChildNodes.Clear();
                ChildNodes.Add(Document.CreateTextNode(value));
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

    }
}
