using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.Engine.PseudoClassSelectors
{
    /// <summary>
    /// Mathches elements that have children containing the specified text
    /// </summary>
    ///
    /// <url>
    /// http://api.jquery.com/contains-selector/
    /// </url>

    public class Contains : PseudoSelectorFilter
    {
        public override IEnumerable<IDomObject> Filter(IEnumerable<IDomObject> selection)
        {
            foreach (IDomObject el in selection)
            {
                if (ContainsText((IDomElement)el, Parameters[0]))
                {
                    yield return el;
                }
            }
        }

        public override bool Matches(IDomObject element)
        {
            return element is IDomContainer ?
                ContainsText(element, Parameters[0]) :
                false;
        }

        protected bool ContainsText(IDomObject source, string text)
        {
            foreach (IDomObject e in source.ChildNodes)
            {
                if (e.NodeType == NodeType.TEXT_NODE)
                {
                    if (((IDomText)e).NodeValue.IndexOf(text) >= 0)
                    {
                        return true;
                    }
                }
                else if (e.NodeType == NodeType.ELEMENT_NODE)
                {
                    if (ContainsText((IDomElement)e, text))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override int MaximumParameterCount
        {
            get
            {
                return 1;
            }
        }
        public override int MinimumParameterCount
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// A value to determine how to parse the string for a parameter at a specific index.
        /// </summary>
        ///
        /// <param name="index">
        /// Zero-based index of the parameter.
        /// </param>
        ///
        /// <returns>
        /// NeverQuoted to treat quotes as any other character; AlwaysQuoted to require that a quote
        /// character bounds the parameter; or OptionallyQuoted to accept a string that can (but does not
        /// have to be) quoted.
        /// </returns>

        protected override QuotingRule ParameterQuoted(int index)
        {
            return QuotingRule.OptionallyQuoted;
        }
        
    }
}
