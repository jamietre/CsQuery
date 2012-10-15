using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery
{
 
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
