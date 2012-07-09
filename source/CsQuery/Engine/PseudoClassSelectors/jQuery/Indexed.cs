using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Engine.PseudoClassSelectors
{
    /// <summary>
    /// Test whether an element is appears at the specified position with the list.
    /// </summary>

    public abstract class Indexed : PseudoSelector, IPseudoSelectorFilter
    {
        private int _Index;
        private bool IndexParsed;
        protected int Index
        {
            get
            {
                if (!IndexParsed)
                {
                    if (!int.TryParse(Parameters[0], out _Index))
                    {
                        throw new ArgumentException(String.Format("The {0} selector requires a single integer parameter.", Name));
                    }
                    IndexParsed = true;
                }
                return _Index;
            }
        }


        public override int  MaximumParameterCount
        {
	        get 
	        { 
		         return 1;
	        }
        }
        public override int  MinimumParameterCount
        {
	        get 
	        { 
		         return 1;
	        }
        }

        public abstract IEnumerable<IDomObject> Filter(IEnumerable<IDomObject> selection);
    }
}
