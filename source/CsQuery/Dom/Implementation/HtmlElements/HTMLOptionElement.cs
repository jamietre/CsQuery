using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.HtmlParser;

namespace CsQuery.Implementation
{
    /// <summary>
    /// An HTML option element.
    /// </summary>

    public class HTMLOptionElement : DomElement, IHTMLOptionElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>

        public HTMLOptionElement()
            : base(HtmlData.tagOPTION)
        {
        }

        /// <summary>
        /// The value of the OPTIOn element, or empty string if none specified.
        /// </summary>

        public override string Value
        {
            get
            {
                return GetAttribute(HtmlData.ValueAttrId, "");
            }
            set
            {
                base.Value = value;
            }
        }


        public bool Disabled
        {
            get
            {
                if (HasAttribute(HtmlData.attrDISABLED)) {
                    return true;
                } else {
                    if (ParentNode.NodeNameID == HtmlData.tagOPTION || ParentNode.NodeNameID == HtmlData.tagOPTGROUP)
                    {
                        var disabled = ((DomElement)ParentNode).HasAttribute(HtmlData.attrDISABLED);
                        if (disabled)
                        {
                            return true;
                        }
                        else
                        {
                            return ParentNode.ParentNode.NodeNameID == HtmlData.tagOPTGROUP &&
                                ((DomElement)ParentNode.ParentNode).HasAttribute(HtmlData.attrDISABLED);                   
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            set
            {
                SetProp(HtmlData.attrDISABLED, value);
            }
        }

        public IDomElement Form
        {
            get { return Closest(HtmlData.tagFORM); }
        }

        public string Label
        {
            get
            {
                return GetAttribute(HtmlData.tagLABEL);
            }
            set
            {
                SetAttribute(HtmlData.tagLABEL,value);
            }
        }

    }
}
