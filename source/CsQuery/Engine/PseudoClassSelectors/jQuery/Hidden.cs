﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.HtmlParser;

namespace CsQuery.Engine.PseudoClassSelectors
{
    /// <summary>
    /// A pseudoselector that returns elements that are hidden. Visibility is defined by CSS: a
    /// nonzero opacity, a display that is not "hidden", and the absence of zero-valued width &amp;
    /// heights. Additionally, input elements of type "hidden" are always considered not visible.
    /// </summary>

    public class Hidden: PseudoSelectorFilter
    {
        public override bool Matches(IDomObject element)
        {
            return !Visible.IsVisible(element);
        }

      
    }
}
