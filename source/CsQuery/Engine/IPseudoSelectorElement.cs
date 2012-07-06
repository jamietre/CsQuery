using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Engine
{
    /// <summary>
    /// A pseudo-selector that depends only on an actual element's properties and/or it's
    /// relationship to other elements within the DOM. All CSS pseudoselectors fall within this
    /// category.
    /// </summary>

    public interface IPseudoSelectorElement : IPseudoSelector
    {
        /// <summary>
        /// Test whether this element matches the selector implementation.
        /// </summary>
        ///
        /// <param name="element">
        /// The object.
        /// </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>

        bool Matches(IDomElement element);

        /// <summary>
        /// Return a sequence of all children matching the selector implementation
        /// </summary>
        ///
        /// <param name="element">
        /// The parent element.
        /// </param>
        ///
        /// <returns>
        /// A sequence of children that match
        /// </returns>

        IEnumerable<IDomObject> ChildMatches(IDomElement element);
    }
}
