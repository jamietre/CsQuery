using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Engine.PseudoClassSelectors
{
    /// <summary>
    /// Test whether an element appears before the specified position with the list.
    /// </summary>

    public class Lt : Indexed
    {

        public override IEnumerable<IDomObject> Filter(IEnumerable<IDomObject> selection)
        {
            return IndexLessThan(selection, Index);
        }

        private IEnumerable<IDomObject> IndexLessThan(IEnumerable<IDomObject> list, int position)
        {
            int index = 0;
            foreach (IDomObject obj in list)
            {
                if (index++ < position)
                {
                    yield return obj;
                }
            }
        }
      
    }
}
