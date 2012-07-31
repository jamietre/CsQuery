using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Dom.Implementation
{

    /// <summary>
    /// Interface for icss rule.
    /// </summary>
    ///
    /// <url>
    /// http://www.w3.org/TR/DOM-Level-2-Style/css.html#CSS-CSSRule
    /// </url>

    public abstract class CSSRule: ICSSRule
    {
        public CSSRule(ICSSStyleSheet parentStyleSheet, ICSSRule parentRule)
        {
            ParentStyleSheet = parentStyleSheet;
            ParentRule = parentRule;
        }
        /// <summary>
        /// Gets the type of rule.
        /// </summary>

        public CSSRuleType Type
        {
            get;
            set;
        }

        /// <summary>
        /// The parsable textual representation of the rule. This reflects the current state of the rule
        /// and not its initial value.
        /// </summary>

        public abstract string CssText { get; set; }

        public ICSSStyleSheet ParentStyleSheet
        {
            get;
            protected set;
        }

        public ICSSRule ParentRule
        {
            get;
            protected set;
        }
    }
}
