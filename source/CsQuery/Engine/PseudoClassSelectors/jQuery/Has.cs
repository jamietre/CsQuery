using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Engine.PseudoClassSelectors
{
    /// <summary>
    /// Return only the last element from a selection
    /// </summary>

    
    public class Has : PseudoSelector, IPseudoSelectorFilter
    {

        public override string Arguments
        {
            get
            {
                return base.Arguments;
            }
            set
            {
                base.Arguments = value;
                ChildSelector = new Selector(String.Join(",", Parameters));
            }
        }

        protected Selector ChildSelector;

        //protected HashSet<IDomObject> Cache;

        public IEnumerable<IDomObject> Filter(IEnumerable<IDomObject> selection)
        {
            ChildSelector.SetTraversalType(TraversalType.Descendent);

            var first = selection.FirstOrDefault();
            if (first!=null) {
                foreach (var item in selection) {
                    if (ChildSelector.Select(first.Document, item).Any())
                    {
                        yield return item;
                    }
                }
            }
        }

        /// <summary>
        /// Adds a matched element to the cache. I'm not quite sure if this makes sense. The idea is that
        /// you will often be checking children of something you just checked; probably we would have to
        /// iterate through the results of the first subselector and add all the node trees to the root
        /// for each result to the cache. Might not be worth it.
        /// </summary>
        ///
        /// <value>
        /// The number of maximum parameters.
        /// </value>
        ///
        /// ### <param name="item">
        /// The item that contains the child selector.
        /// </param>

        //protected void AddToCache(IDomObject item)
        //{
        //    IDomObject next= item;
        //    while (next != item.Document && next != null)
        //    {
        //        Cache.Add(next);
        //        next = next.ParentNode;
        //    }
        //}
        

        public override int MaximumParameterCount
        {
            get
            {
                return 1;
            }
        }
        public override int MinimumParameterCount
        {
            get
            {
                return 1;
            }
        }

     
    }
}
