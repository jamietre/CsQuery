using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.Utility;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.Engine;
using CsQuery.Implementation;

namespace CsQuery
{
    public partial class CQ
    {
        #region public methods

        /// <summary>
        /// Insert content, specified by the parameter, after each element in the set of matched elements.
        /// </summary>
        ///
        /// <param name="selector">
        /// A CSS selector that determines the elements to insert.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/after/
        /// </url>

        public CQ After(string selector)
        {
            return After(Select(selector));
        }

        /// <summary>
        /// Insert an element, specified by the parameter, after each element in the set of matched
        /// elements.
        /// </summary>
        ///
        /// <param name="element">
        /// The element to insert.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/after/
        /// </url>

        public CQ After(IDomObject element)
        {
            return After(Objects.Enumerate(element));
        }

        /// <summary>
        /// Insert elements, specified by the parameter, after each element in the set of matched
        /// elements.
        /// </summary>
        ///
        /// <param name="elements">
        /// The elements to insert.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/after/
        /// </url>

        public CQ After(IEnumerable<IDomObject> elements)
        {
            EnsureCsQuery(elements).InsertAtOffset(this, 1);
            return this;
        }
        #endregion

        #region private methods

        /// <summary>
        /// Inserts an element at the specified offset from a target. Helper method for Before and After.
        /// </summary>
        ///
        /// <param name="target">
        /// Target for the.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        ///
        /// <returns>
        /// .
        /// </returns>

        protected CQ InsertAtOffset(IEnumerable<IDomObject> target, int offset)
        {
            CQ ignore;
            return InsertAtOffset(target, offset, out ignore);
        }

        /// <summary>
        /// Insert every element in the selection at or after the index of each target (adding offset to
        /// the index). If there is more than one target, the a clone is made of the selection for the
        /// 2nd and later targets.
        /// </summary>
        ///
        /// <exception cref="InvalidOperationException">
        /// Thrown if attempting to add elements to a disconnected (parentless) sequence
        /// </exception>
        ///
        /// <param name="target">
        /// The target element
        /// </param>
        /// <param name="offset">
        /// The offset from the target at which to begin inserting
        /// </param>
        /// <param name="insertedElements">
        /// [out] The inserted elements.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object
        /// </returns>

        protected CQ InsertAtOffset(IEnumerable<IDomObject> target, int offset, out CQ insertedElements)
        {
            CQ cq = target as CQ;
            SelectionSet<IDomObject> sel = null;

            if (cq!=null) {
                sel = cq.SelectionSet;
            }

            bool isTargetCsQuery = cq != null;
            
            bool isFirst = true;

            // Copy the target list: it could change otherwise
            List<IDomObject> targets = new List<IDomObject>(target);
            insertedElements = new CQ();
            bool isEmptyTarget = sel.Count == 0;

            // bind the source to the target's document if it was itself a CsQuery object, and update its selection set to reflect the 
            // current document.

            if (isTargetCsQuery)
            {
                Document = cq.Document;
            }

            if (isTargetCsQuery && isEmptyTarget)
            {
                // If appending items to an empty selection, just add them to the selection set
                sel.AddRange(SelectionSet);
                insertedElements.AddSelection(SelectionSet);

                // selection set will be messed up if document was changed; rebuild it
                SelectionSet.Clear();
                SelectionSet.AddRange(insertedElements);
            }
            else
            {
                foreach (var el in targets)
                {
                    if (el.IsDisconnected)
                    {
                        // Disconnected items are added to the selection set (if that's the target)
                        if (!isTargetCsQuery)
                        {
                            throw new InvalidOperationException("You can't add elements to a disconnected element list, it must be in a selection set");
                        }
                        int index = sel.IndexOf(el);

                        sel.OutputOrder = SelectionSetOrder.OrderAdded;
        
                        foreach (var item in SelectionSet)
                        {
                            sel.Insert(index + offset, item);
                        }
                        insertedElements.AddSelection(SelectionSet);

                        // selection set will be messed up if document was changed; rebuild it
                        SelectionSet.Clear();
                        SelectionSet.AddRange(insertedElements);
                    }
                    else
                    {
                        if (isFirst)
                        {
                            insertedElements.AddSelection(SelectionSet);
                            InsertAtOffset(el, offset);
                            isFirst = false;
                            // selection set will be messed up if document was changed; rebuild it
                            SelectionSet.Clear();
                            SelectionSet.AddRange(insertedElements);
                        }
                        else
                        {
                            var clone = Clone();
                            clone.InsertAtOffset(el, offset);
                            insertedElements.AddSelection(clone);
                        }
                    }
                }
            }



            return this;
        }
        #endregion
    }
}
