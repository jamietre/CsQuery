using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Engine.PseudoClassSelectors
{
    /// <summary>
    /// Base class for an Element pseudoselector.
    /// </summary>

    public abstract class PseudoSelectorElement: PseudoSelector, IPseudoSelectorElement
    {
        /// <summary>
        /// Test whether an element matches this selector.
        /// </summary>
        ///
        /// <param name="element">
        /// The element to test.
        /// </param>
        ///
        /// <returns>
        /// true if it matches, false if not.
        /// </returns>

        public abstract bool Matches(IDomElement element);

        /// <summary>
        /// Basic implementation of ChildMatches, runs the Matches method against each child. This should
        /// be overridden with something more efficient if possible. For example, selectors that inspect
        /// the element's index could get their results more easily by picking the correct results from
        /// the list of children rather than testing each one.
        /// </summary>
        ///
        /// <param name="element">
        /// The parent element.
        /// </param>
        ///
        /// <returns>
        /// A sequence of children that match.
        /// </returns>

        public virtual IEnumerable<IDomObject> ChildMatches(IDomElement element)
        {
            return element.ChildElements.Where(item => Matches(item));
        }
    }

}
