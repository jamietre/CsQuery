using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.HtmlParser;

namespace CsQuery.Implementation
{
    /// <summary>
    /// An HTML progress element.
    /// </summary>

    public class HTMLProgressElement : DomElement, IHTMLProgressElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>

        public HTMLProgressElement()
            : base(HtmlData.tagPROGRESS)
        {
        }

        public new int Value
        {
            get
            {
                return IntOrZero(GetAttribute(HtmlData.ValueAttrId));
            }
            set
            {
                SetAttribute(HtmlData.ValueAttrId, value.ToString());
            }
        }

        public double Max
        {
            get
            {
                return DoubleOrZero(GetAttribute("max"));
            }
            set
            {
                SetAttribute("max", value.ToString());
            }
        }

        ///  <summary>
        /// If the progress bar is an indeterminate progress bar, then the position IDL attribute must
        /// return −1. Otherwise, it must return the result of dividing the current value by the maximum
        /// value.
        /// </summary>

        public double Position
        {
            get
            {
                if (!HasAttribute("value"))
                {
                    return -1;
                }
                else
                {
                    return Value / Max;

                }
            }
          
        }

        /// <summary>
        /// A NodeList of all LABEL elements within this Progress element
        /// </summary>

        public INodeList<IDomElement> Labels
        {
            get {
                return new NodeList<IDomElement>(ChildElementsOfTag<IDomElement>(HtmlData.tagLABEL));
            }
        }

        private double DoubleOrZero(string value)
        {
            var val = GetAttribute("value");
            double dblVal;
            if (double.TryParse(value, out dblVal))
            {
                return dblVal;
            }
            else
            {
                return 0;
            }
        }
        private int IntOrZero(string value)
        {
            var val = GetAttribute("value");
            int intVal;
            if (int.TryParse(value, out intVal))
            {
                return intVal;
            }
            else
            {
                return 0;
            }
        }

    }
}
