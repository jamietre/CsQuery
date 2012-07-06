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
    ///
    /// </url>

    public class Contains : PseudoSelector, IPseudoSelectorFilter
    {


        public IEnumerable<IDomObject> Filter(IEnumerable<IDomObject> selection)
        {
            foreach (IDomObject el in selection)
            {
                if (ContainsText((IDomElement)el, Arguments))
                {
                    yield return el;
                }
            }
        
        }

        protected bool ContainsText(IDomElement source, string text)
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
        public override bool? ParameterQuoted(int index)
        {
            return null;
        }
    }
}
