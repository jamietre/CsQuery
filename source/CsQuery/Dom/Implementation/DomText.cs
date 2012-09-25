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
        //protected int textIndex=-1;
        // for use during initial construction from char array
        //public void SetTextIndex(IDomDocument dom, int index)
        //{
        //    textIndex = index;
        //    // create a hard reference to the DOM from which we are mapping our string data. Otherwise if this
        //    // is moved to another dom, it will break
        //    stringRef = dom;
        //}

        protected IDomDocument stringRef = null;

        protected string unboundText=null;
        
        public override string NodeValue
        {
            get
            {
                //return textIndex >= 0 ?
                //    HtmlData.HtmlDecode(stringRef.DocumentIndex.GetTokenizedString(textIndex))
                //        : unboundText;
                return unboundText;
            }
            set
            {
               unboundText =value;
               //textIndex = -1;
            }
        }

        /// <summary>
        /// Renders this text node using document default rendering options
        /// </summary>
        ///
        /// <returns>
        /// A string of HTML-encoded text
        /// </returns>

        public override string Render()
        {
            var opts = DomRenderingOptions;

            return HtmlData.HtmlEncode(NodeValue, 
                opts.HasFlag(DomRenderingOptions.HtmlEncodingNone) ? 
                    HtmlEncodingMethod.HtmlEncodingNone :
                    opts.HasFlag(DomRenderingOptions.HtmlEncodingMinimum) ?
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

        public override void Render(StringBuilder sb)
        {
            sb.Append(Render());
        }
        public override DomText Clone()
        {
            DomText domText = new DomText();
            //domText.textIndex = textIndex;
            domText.unboundText = unboundText;
            domText.stringRef = stringRef;
            return domText;
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
