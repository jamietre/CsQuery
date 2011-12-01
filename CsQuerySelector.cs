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
        SubSelector = 512
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
        IndexEquals = 8,
        IndexLessThan = 9,
        IndexGreaterThan = 10
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
        public IEnumerable<IDomObject> SelectElements;
        public List<CsQuerySelectors> SubSelectors = new List<CsQuerySelectors>();
    }
}
