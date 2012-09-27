using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CsQuery.Implementation
{
   
    /// <summary>
    /// Used for literal text (not part of a tag)
    /// </summary>
    [Obsolete]
    public class DomInnerText : DomText, IDomInnerText
    {
        public DomInnerText()
        {

        }

        public DomInnerText(string nodeValue)
            : base()
        {
            NodeValue = nodeValue;
        }
        public override string NodeValue
        {
            get
            {
                return nodeValue;
            }
        }
        public override string NodeName
        {
            get
            {
                return null;
            }
        }

        public override string Render(DomRenderingOptions options=DomRenderingOptions.Default)
        {
            // do NOT HtmlEncode unlike DomText
            return NodeValue;
        }


    }
}
