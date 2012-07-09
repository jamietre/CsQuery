using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Implementation
{
    /// <summary>
    /// Orders in which the selection set can be arranged. Ascending and Descending refer to to the
    /// DOM element order.
    /// </summary>

    public enum SelectionSetOrder
    {
        OrderAdded = 1,
        Ascending = 2,
        Descending = 3
    }
}
