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
            return EnsureCsQuery(elements).InsertAtOffset(this, 1);
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
            return InsertAtOffset(EnsureCsQuery(target), offset, out ignore);
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

        protected CQ InsertAtOffset(CQ target, int offset, out CQ insertedElements)
        {
            
            bool isTargetDisconnected = target.CsQueryParent != null && target.Document != target.CsQueryParent.Document;

            bool isFirst = true;

            // Copy the target list: it could change otherwise
            List<IDomObject> targets = new List<IDomObject>(target);
            insertedElements = NewInstance();
            

            // bind the source to the target's document if it was itself a CsQuery object, and update its selection set to reflect the 
            // current document.

            Document = target.Document;

                if (isTargetDisconnected)
                {
                    // When the target is disconnected, just append elements to the selection set, not to the DOM
                    
                    CQ result = New();
                    result.CsQueryParent = this.CsQueryParent;

                    if (offset == 0)
                    {
                        result.AddSelection(Selection);
                    }
                    result.AddSelection(target);
                    if (offset == 1)
                    {
                        result.AddSelection(Selection);
                    }
                    result.SelectionSet.OutputOrder = SelectionSetOrder.OrderAdded;
                    insertedElements.AddSelection(Selection);
                    return result;
                    // selection set will be messed up if document was changed; rebuild it
                    //SelectionSet.Clear();
                    //SelectionSet.AddRange(insertedElements);
                }
                else
                {

                    foreach (var el in targets)
                    {
                    
                    
                        if (isFirst)
                        {
                            insertedElements.AddSelection(SelectionSet);
                            InsertAtOffset(el, offset);
                            isFirst = false;
                            // selection set will be messed up if document was changed; rebuild it
                            //SelectionSet.Clear();
                            //SelectionSet.AddRange(insertedElements);
                        }
                        else
                        {
                            var clone = this.Clone();
                            clone.InsertAtOffset(el, offset);
                            insertedElements.AddSelection(clone);
                        }
                    
                    }
                }



            return target;
        }
        #endregion
    }
}
