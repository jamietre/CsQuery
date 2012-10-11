using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery
{
    /// <summary>
    /// Interface for a strongly-typed object; this is used to provide a typed Clone method to each
    /// element type implementation.
    /// </summary>
    ///
    /// <typeparam name="T">
    /// Type of the derived object.
    /// </typeparam>

    public interface IDomObject<out T> : IDomObject
    {
        /// <summary>
        /// Clone this element.
        /// </summary>
        ///
        /// <returns>
        /// A copy of this element that is not bound to the original.
        /// </returns>
        
        new T Clone();
    }
    
}
