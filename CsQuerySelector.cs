using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery
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
        SubSelectorNot= 1024
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
        NthChild=8,
        IndexEquals = 9,
        IndexLessThan = 10,
        IndexGreaterThan = 11
    }

    public class CsQuerySelector
    {
        public CsQuerySelector()
        {
            SelectorType = 0;
            AttributeSelectorType = AttributeSelectorType.Equals;
            CombinatorType = CombinatorType.Root;
            TraversalType = TraversalType.All;
            PositionType = PositionType.All;
        }
       
        public SelectorType SelectorType { get; set; }
        public AttributeSelectorType AttributeSelectorType { get; set; }
        public CombinatorType CombinatorType { get; set; }
        /// <summary>
        /// Indicates that a position type selector refers to the result list, not the DOM position
        /// </summary>
        /// <returns></returns>
        public bool IsResultListPosition()
        {
            if (SelectorType != SelectorType.Position)
            {
                return false;
            }
            switch (PositionType)
            {
                case PositionType.Last:
                case PositionType.First:
                case PositionType.IndexEquals:
                case PositionType.IndexGreaterThan:
                case PositionType.IndexLessThan:
                    return true;
                default:
                    return false;
            }
        }
        public bool IsDomIndexPosition()
        {
            if (SelectorType != SelectorType.Position)
            {
                return false;
            }
            return !IsResultListPosition();
        }
        public bool IsOrdinalIndexPosition()
        {
            if (SelectorType != SelectorType.Position)
            {
                return false;
            }
            return PositionType == PositionType.IndexEquals || PositionType == PositionType.IndexGreaterThan || PositionType == PositionType.IndexLessThan;
        }
        public bool IsComplete
        {
            get
            {
                return SelectorType != 0;
            }
        }
        public string Html = null;
        //public bool AllowHtmlTextNodes { get; set; }

        /// <summary>
        /// Selection tag name
        /// </summary>
        public string Tag
        {
            get;
            set;
        }
        public TraversalType TraversalType
        { get; set; }
        public PositionType PositionType
        { get; set; }
        /// <summary>
        /// Selection criteria for attibute selector functions
        /// </summary>
        public string Criteria
        {
            get
            {
                return _Criteria;
            }
            set
            {
                _Criteria = value;
            }
        } protected string _Criteria = null;
        /// <summary>
        /// For Position selectors, the position. Negative numbers start from the end.
        /// </summary>
        public int PositionIndex
        { get; set; }
        /// <summary>
        /// For Child selectors, the depth of the child.
        /// </summary>
        public int ChildDepth
        { get; set; }
        public string AttributeName
        {
            get
            {
                return _AttributeName;
            }
            set
            {
                _AttributeName = (value == null ? value : value.ToLower());
            }
        } protected string _AttributeName = null;
        public string AttributeValue = null;
        public string Class = null;
        public string ID = null;
        public bool NoIndex = false;

        public IEnumerable<IDomObject> SelectElements;
        /// <summary>
        /// A list of subselectors. The results of this are used as criteria for a primary selector, e.g. has or not.
        /// </summary>
        public List<CsQuerySelectors> SubSelectors 
        {
            get {
                if (_SubSelectors == null)
                {
                    _SubSelectors = new List<CsQuerySelectors>();
                }
                return _SubSelectors;
            }
        }
        protected List<CsQuerySelectors> _SubSelectors= null;


        public override string ToString()
        {
            string output = "";
            switch (TraversalType)
            { 
                case TraversalType.All:
                    output="";
                    break;
                case TraversalType.Child:
                    output += " > ";
                    break;
                case TraversalType.Descendent:
                    output += " ";
                    break;
            }
            if (SelectorType.HasFlag(SelectorType.Elements))
            {
                output += "<ElementList[" + SelectElements.Count() + "]> ";
            }
            if (SelectorType.HasFlag(SelectorType.HTML))
            {
                output += "<HTML[" + Html.Length + "]> ";
            }
            if (SelectorType.HasFlag(SelectorType.Tag))
            {
                output += Tag;
            }
            if (SelectorType.HasFlag(SelectorType.ID))
            {
                output += "#" + ID;
            }
            if (SelectorType.HasFlag(SelectorType.Attribute))
            {
                output += "[" + AttributeName;
                if (!String.IsNullOrEmpty(AttributeValue)) {
                    output += "." + AttributeSelectorType.ToString() + ".'" + AttributeValue + "'";
                }
                output += "]";
            }
            if (SelectorType.HasFlag(SelectorType.Class))
            {
                output += "." + Class;
            }
            if (SelectorType.HasFlag(SelectorType.All)) {
                output += "*";
            }
            if (SelectorType.HasFlag(SelectorType.Position)) {
                output += ":" + PositionType.ToString();
                if (IsOrdinalIndexPosition())
                {
                    output += "(" + PositionIndex + ")";
                } else if (SubSelectors.Count>0) {
                    output+= SubSelectors.ToString();
                }
            }
            if (SelectorType.HasFlag(SelectorType.Contains))
            {
                output += ":contains(" + Criteria + ")";
            }


            return output;
        }             
    }
}
