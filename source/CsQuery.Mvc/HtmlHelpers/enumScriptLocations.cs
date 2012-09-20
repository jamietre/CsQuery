using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Mvc
{
    /// <summary>
    /// Values that represent a placement for a script
    /// </summary>

    public enum ScriptLocations
    {
        /// <summary>
        /// The script should remain at the location it was found in the markup.
        /// </summary>
        Inline=1,
        /// <summary>
        /// The script should be moved to HEAD.
        /// </summary>
        Head=2
    }
}
