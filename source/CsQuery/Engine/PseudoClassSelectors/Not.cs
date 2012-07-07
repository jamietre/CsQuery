using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Engine.PseudoClassSelectors
{
    /// <summary>
    /// Return only the last element from a selection
    /// </summary>

    public class Not : PseudoSelector, IPseudoSelectorFilter 
    {

        public IEnumerable<IDomObject> Filter(IEnumerable<IDomObject> selection)
        {
         
            var selector = SubSelector();

            var first = selection.FirstOrDefault();
            if (first != null)
            {
                foreach (var item in selector.Except(first.Document, selection))
                {
                    if (item.NodeType == NodeType.ELEMENT_NODE)
                    {
                        yield return (IDomElement)item;
                    }
                }
            }

        }

        protected Selector SubSelector()
        {
           return new Selector(String.Join(",", Parameters))
                .SetTraversalType(TraversalType.Filter);
            
        }

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
