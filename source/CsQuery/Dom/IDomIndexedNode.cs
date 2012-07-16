using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery
{
    /// <summary>
    /// A node that should be indexed
    /// </summary>
    public interface IDomIndexedNode: IDomNode
    {
        IEnumerable<string> IndexKeys();
        IDomObject IndexReference { get; }
    }
}
