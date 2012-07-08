using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Engine.PseudoClassSelectors
{
    public class NthOfType: NthChildSelector
    {


        public override bool Matches(IDomElement element)
        {
            return element.NodeType != NodeType.ELEMENT_NODE ? false :
                NthC.IsNthChildOfType((IDomElement)element,Parameters[0],false);
        }

        public override IEnumerable<IDomObject> ChildMatches(IDomElement element)
        {
            return NthC.NthChildsOfTypeImpl(element,Parameters[0],false);
        }


    }
}
