using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery
{
    /// <summary>
    /// Interface for a fragment. A fragment is similar to a Document node, except that there is no
    /// expectation that it represents a complete DOM.
    /// </summary>

    public interface IDomFragment : IDomDocument
    {

    }

}
