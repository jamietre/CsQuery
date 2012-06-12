using System;
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
        Attribute = 16,
        Contains = 32,
        PseudoClass = 64,
        Elements = 128,
        HTML = 256,
        SubSelectorHas = 512,
        SubSelectorNot = 1024
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
        Cumulative = 1,
        Chained = 2,
        Root = 3
    }
    public enum TraversalType
    {
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
        Even = 2, // jquery
        Odd = 3, // jquery
        First = 4, // jquery
        Last = 5, // jquery
        FirstChild = 6,
        LastChild = 7,
        NthChild = 8,
        IndexEquals = 9, // jquery
        IndexLessThan = 10, // jquery
        IndexGreaterThan = 11, // jquery
        FirstOfType=12,
        LastOfType=13,
        NthOfType= 14,
        OnlyChild=15,
        OnlyOfType = 16,
        Empty = 17,
        Parent = 18, // jquery
        Visible = 19, // jquery
        Hidden = 20, // jquery
        Header = 21 // jquery
    }


}
