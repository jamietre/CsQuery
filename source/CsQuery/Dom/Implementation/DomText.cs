using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using CsQuery.HtmlParser;
using CsQuery.Output;

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
