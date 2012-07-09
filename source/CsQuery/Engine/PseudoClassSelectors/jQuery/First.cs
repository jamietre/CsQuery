using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Engine.PseudoClassSelectors
{
    /// <summary>
    /// Return only odd-numbered elements from the selection
    /// </summary>

    public class First : PseudoSelector, IPseudoSelectorFilter
    {
        public IEnumerable<IDomObject> Filter(IEnumerable<IDomObject> selection)
        {
            var first = selection.FirstOrDefault();
            if (first != null)
            {
                yield return first;
            } 
        }
    }
}
