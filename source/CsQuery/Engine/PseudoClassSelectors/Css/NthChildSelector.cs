using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Engine.PseudoClassSelectors
{
    public abstract class NthChildSelector: PseudoSelector, IPseudoSelectorChild
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

        public abstract bool Matches(IDomObject element);

        public abstract IEnumerable<IDomObject> ChildMatches(IDomContainer element);

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

    }
}
