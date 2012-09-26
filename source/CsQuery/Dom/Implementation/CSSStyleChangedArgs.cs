using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Implementation
{
    /// <summary>
    /// Arguments for when a style is changed.
    /// </summary>

    public class CSSStyleChangedArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        ///
        /// <param name="hasStyles">
        /// A value indicating whether this object has styles following the change.
        /// </param>

        public CSSStyleChangedArgs(bool hasStyles)
        {
            HasStyles = hasStyles;
        }
        /// <summary>
        /// Gets a value indicating whether this object has styles following the change.
        /// </summary>

        public bool HasStyles
        {
            get;
            protected set;
        }
    }
}
