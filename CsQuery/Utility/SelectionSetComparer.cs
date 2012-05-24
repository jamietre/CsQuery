using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Utility
{
    /// <summary>
    /// A comparer to ensure that items are returned from a selection set in DOM order, e.g. by comparing their
    /// internal paths.
    /// </summary>
    public class SelectionSetComparer : IComparer<IDomObject>
    {
        public SelectionSetComparer(SelectionSetOrder order)
        {
            if (order != SelectionSetOrder.Ascending && order != SelectionSetOrder.Descending)
            {
                throw new InvalidOperationException("This comparer can only be used to sort.");
            }
            Order = order;
        }
        protected SelectionSetOrder Order;
        public int Compare(IDomObject x, IDomObject y)
        {
            return Order == SelectionSetOrder.Ascending ?
                String.CompareOrdinal(x.Path, y.Path) :
                String.CompareOrdinal(y.Path, x.Path);
        }
    }
}
