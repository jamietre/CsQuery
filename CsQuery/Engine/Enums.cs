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
        Position = 64,
        Elements = 128,
        HTML = 256,
        SubSelectorHas = 512,
        SubSelectorNot = 1024,
        Other=2048
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
        NotEquals = 8
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
        Child = 4
    }

    public enum PositionType
    {
        All = 1,
        Even = 2,
        Odd = 3,
        First = 4,
        Last = 5,
        FirstChild = 6,
        LastChild = 7,
        NthChild = 8,
        IndexEquals = 9,
        IndexLessThan = 10,
        IndexGreaterThan = 11
    }
    public enum OtherType
    {
        Visible = 1
    }

}
