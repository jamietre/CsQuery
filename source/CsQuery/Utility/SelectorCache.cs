using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.Implementation;
using CsQuery.Engine;

namespace CsQuery.Utility
{
    /// <summary>
    /// Class to cache selectors on a DOM
    /// </summary>

    public class SelectorCache
    {
        public SelectorCache(CQ cqSource)
        {
            CqSource = cqSource;
        }


        private CQ  CqSource;

        private IDictionary<Selector, IList<IDomObject>> _SelectionCache;
        protected IDictionary<Selector, IList<IDomObject>> SelectionCache
        {
            get
            {
                if (_SelectionCache == null)
                {
                    _SelectionCache = new Dictionary<Selector, IList<IDomObject>>();
                }
                return _SelectionCache;

            }
        }

        public CQ Select(string selector)
        {
            IList<IDomObject> selection;

            var sel = new Selector(selector);
            if (SelectionCache.TryGetValue(sel, out selection)) {
                return new CQ(selection);
            } else {
                var result = CqSource.Select(sel);
                SelectionCache.Add(sel, result.Selection.ToList());
                return result;
            }   


        }
    }
}
