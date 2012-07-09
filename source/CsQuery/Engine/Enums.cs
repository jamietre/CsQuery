﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Engine
{
    [Flags]
    public enum SelectorType
    {
        All = 1,
        Tag = 2,
        ID = 4,
        Class = 8,
        AttributeValue = 32,
        PseudoClass = 128,
        Elements = 256,
        HTML = 512,
        None = 1024   // returns no values ever

    }
    public enum AttributeSelectorType
    {
        Exists = 1,
        Equals = 2,
        StartsWith = 3,
        Contains = 4,
        NotExists = 5,
        ContainsWord = 6,
        EndsWith = 7,
        NotEquals = 8,
        StartsWithOrHyphen=9 // for lang
    }

    public enum CombinatorType
    {
        And = 1,       // This selector clause  and the prior one must match
        Chained = 2,   // The selector clause is applied to the results of the prior one
        Root = 3       // The selector clause is applied to the root context of this selector
    }
    public enum TraversalType
    {
        Incomplete=0,
        All = 1,
        Filter = 2,
        Descendent = 3,
        Child = 4,
        Adjacent = 5,
        Sibling = 6
        //,Inherited = 7
    }

    /// <summary>
    /// Position-type selectors match one or more element children of another element. The selection engine can either access all 
    /// matching children, or test a particular element for matching a selector
    /// </summary>
    public enum PseudoClassType
    {
        All = 1,

        // jQuery pseudoclass selectors
        // 
        Even = 2, 
        Odd = 3,
        First = 4,
        Last = 5,
        IndexEquals = 6,
        IndexLessThan = 7,
        IndexGreaterThan = 8,
        Parent = 9,
        Visible = 10,
        Hidden = 11,
        Header = 12,
        Has = 13,
        Not = 14, 
        
        // CSS pseudoclass selectors
        
        FirstChild = 20,
        LastChild = 21,
        NthChild = 22,
        FirstOfType = 23,
        LastOfType = 24,
        NthOfType = 25,
        NthLastChild = 26,
        NthLastOfType = 27,
        OnlyChild = 28,
        OnlyOfType = 29,
        Empty = 30
        
    }


}
