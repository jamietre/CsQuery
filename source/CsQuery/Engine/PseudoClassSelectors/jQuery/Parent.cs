using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Engine.PseudoClassSelectors
{
    /// <summary>
    /// Determines whether the target is a parent.
    /// </summary>

    public class Parent: PseudoSelectorFilter
    {
        public override bool Matches(IDomObject element)
        {

            return element.HasChildren ?
                !Empty.IsEmpty(element) : 
                false;
        }


    }
}
