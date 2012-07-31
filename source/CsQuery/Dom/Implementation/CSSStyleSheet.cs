using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Dom.Implementation
{
    /// <summary>
    /// A CSS style sheet.
    /// </summary>
    ///
    /// <url>
    /// http://www.w3.org/TR/DOM-Level-2-Style/css.html#CSS-CSSStyleSheet
    /// </url>

    public class CSSStyleSheet: ICSSStyleSheet
    {
        #region constructors

        public CSSStyleSheet(IDomElement ownerNode)
        {
            OwnerNode = ownerNode;
        }

        #endregion

        #region private properties

        private IList<ICSSRule> _Rules;

        #endregion

        #region public properties

        /// <summary>
        /// Indicates whether the style sheet is applied to the document.
        /// </summary>

        public bool Disabled
        {
            get;set;
        }

        /// <summary>
        /// If the style sheet is a linked style sheet, the value of its attribute is its location. For
        /// inline style sheets, the value of this attribute is null.
        /// </summary>

        public string Href
        {
            get
            {
                return OwnerNode == null ? 
                    null : 
                    OwnerNode["href"];
            }
            set
            {
                if (OwnerNode == null)
                {
                    throw new InvalidOperationException("This CSSStyleSheet is not bound to an element node.");
                }
                OwnerNode["href"] = value;
            }
        }

        public IDomElement OwnerNode
        {
            get;
            protected set;
        }

        public string Type
        {
            get { return "text/css"; }
        }


        public IList<ICSSRule> CssRules
        {
            get {
                if (_Rules == null)
                {
                    _Rules = new List<ICSSRule>();
                }
                return _Rules;
            }
        }

        #endregion
    }
}
