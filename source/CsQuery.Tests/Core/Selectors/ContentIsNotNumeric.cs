using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CsQuery.Engine.PseudoClassSelectors
{
    /// <summary>
    /// By placing this class in the CsQuery.Engine.PseudoClassSelectors namspace, it should be
    /// registered automatically.
    /// </summary>

    public class ContentIsNotNumeric : PseudoSelectorFilter
    {
        public override string Name
        {
            get
            {
                return "isnotnumeric";
            }
        }

        public override bool Matches(IDomObject element)
        {
            if (element.HasChildren)
            {
                foreach (var item in element.ChildNodes)
                {
                    if (item.NodeType == NodeType.TEXT_NODE)
                    {
                        double val;
                        if (double.TryParse(item.NodeValue, out val))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}
