using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Engine.PseudoClassSelectors
{
    public class NthLastOfType: NthChildSelector
    {

        public override bool Matches(IDomObject element)
        {
            return element.NodeType != NodeType.ELEMENT_NODE ? false :
                NthC.IsNthChildOfType((IDomElement)element,Parameters[0],true);
        }

        public override IEnumerable<IDomObject> ChildMatches(IDomContainer element)
        {
            return NthC.NthChildsOfTypeImpl(element,Parameters[0],true);
        }

     

    }
}
