using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery
{
    /// <summary>
    /// Interface for a special element type (CDATA, Comment, DocType). These are elements that have
    /// data in the tag itself that does not conform to the standard attribute/value construct.
    /// </summary>

    public interface IDomSpecialElement : IDomObject
    {
        /// <summary>
        /// The non-attribute data.
        /// </summary>

        string NonAttributeData { get; set; }

    }
}
