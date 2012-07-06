using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Engine.PseudoClassSelectors
{
    public class Header: PseudoSelectorElement
    {
        public override bool Matches(IDomElement element)
        {
            var nodeName = element.NodeName;
            return nodeName[0] == 'H'
                && nodeName.Length == 2
                && nodeName[1] >= '0'
                && nodeName[1] <= '6';
        }
    }
}
