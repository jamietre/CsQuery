using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Engine.PseudoClassSelectors
{
    public abstract class NthChildSelector: PseudoSelector, IPseudoSelectorElement
    {
        private NthChildMatcher _NthC;
        protected NthChildMatcher NthC
        {
            get
            {
                if (_NthC == null)
                {
                    _NthC = new NthChildMatcher();
                }
                return _NthC;
            }

        }

        public override int MinimumParameterCount
        {
            get
            {
                return 1;
            }
        }

        public override int MaximumParameterCount
        {
            get
            {
                return 1;
            }
        }
        
        public abstract bool Matches(IDomElement element);

        public abstract IEnumerable<IDomObject> ChildMatches(IDomElement element);
    }
}
