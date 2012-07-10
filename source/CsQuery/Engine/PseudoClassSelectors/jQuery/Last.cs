using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Engine.PseudoClassSelectors
{
    /// <summary>
    /// Return only the last element from a selection
    /// </summary>

    public class Last : PseudoSelector, IPseudoSelectorFilter
    {
        public IEnumerable<IDomObject> Filter(IEnumerable<IDomObject> selection)
        {
            var last = selection.LastOrDefault();
            if (last != null)
            {
                yield return last;
            } 
        }
    }
}
