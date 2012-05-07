using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Engine
{
    /// <summary>
    /// A CSS selector parsed into it's component parts
    /// </summary>
    public class Selector
    {
        #region constructors
       
        public Selector()
        {
            Initialize();
        }
        protected void Initialize()
        {
            SelectorType = 0;
            AttributeSelectorType = AttributeSelectorType.Equals;
            CombinatorType = CombinatorType.Root;
            TraversalType = TraversalType.All;
            PositionType = PositionType.All;
        }

        #endregion

        #region private properties

        protected SelectorType _SelectorType;
        protected string _AttributeName;
        protected List<SelectorChain> _SubSelectors;
        
        #endregion

        #region public properties

        public SelectorType SelectorType
        {
            get
            {
                return _SelectorType;
            }
            set
            {
                _SelectorType = value;
            }
        }
        public CombinatorType CombinatorType { get; set; }
        public TraversalType TraversalType { get; set; }
        public PositionType PositionType { get; set; }
        public AttributeSelectorType AttributeSelectorType { get; set; }
        public OtherType OtherType { get; set; }

        /// <summary>
        /// Selection tag name
        /// </summary>
        public string Tag { get; set; }
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
        }
        /// <summary>
        /// Selection criteria for attibute selector functions
        /// </summary>
        public string Criteria {get;set;}
        /// <summary>
        /// For Position selectors, the position. Negative numbers start from the end.
        /// </summary>
        public int PositionIndex { get; set; }
        /// <summary>
        /// For Child selectors, the depth of the child.
        /// </summary>
        public int ChildDepth { get; set; }
        public string AttributeValue { get; set; }
        public string Class { get; set; }
        public string ID { get; set; }
        public string Html { get; set; }
        /// <summary>
        /// The list of elements that should be matched, for elements selectors
        /// </summary>
        public IEnumerable<IDomObject> SelectElements { get; set; }
        /// <summary>
        /// A list of subselectors. The results of this are used as criteria for a primary selector, e.g. has or not.
        /// </summary>
        public List<SelectorChain> SubSelectors
        {
            get
            {
                if (_SubSelectors == null)
                {
                    _SubSelectors = new List<SelectorChain>();
                }
                return _SubSelectors;
            }
        }

        public bool IsDomIndexPosition
        {
            get
            {
                if (SelectorType != SelectorType.Position)
                {
                    return false;
                }
                return !IsResultListPosition;
            }
        }
        
        /// <summary>
        /// Indicates that a position type selector refers to the result list, not the DOM position
        /// </summary>
        /// <returns></returns>
        public bool IsResultListPosition
        {
            get
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
        }
        public bool IsFunction
        {
            get
            {
                switch (PositionType)
                {
                    case PositionType.IndexEquals:
                    case PositionType.IndexGreaterThan:
                    case PositionType.IndexLessThan:
                    case PositionType.NthChild:
                        return true;
                    default:
                        return false;
                }
            }
        }
        public bool HasSubSelectors
        {
            get
            {
                return _SubSelectors != null && SubSelectors.Count > 0;
            }
        }

        public bool IsNew
        {
            get
            {
                return SelectorType == 0
                    && ChildDepth == 0
                    && TraversalType == TraversalType.All
                    && CombinatorType == CombinatorType.Root;
            }
        }
        public bool IsComplete
        {
            get
            {
                return SelectorType != 0;
            }
        }
        /// <summary>
        /// When true do not attempt to use the index to obtain a result from this selector. Used for automatically 
        /// generated filters
        /// </summary>
        public bool NoIndex { get; set; }
        
        #endregion

        #region public methods
        
        public void Clear()
        {
            AttributeName = null;
            AttributeSelectorType = 0;
            AttributeValue = null;
            ChildDepth = 0;
            Class = null;
            Criteria = null;
            Html = null;
            ID = null;
            NoIndex = false;
            PositionIndex = 0;
            SelectElements = null;
            Tag = null;
            OtherType = 0;
            _SubSelectors = null;

            Initialize();
        }

        public Selector Clone()
        {
            Selector clone = new Selector();

            clone.SelectorType = SelectorType;
            clone.TraversalType = TraversalType;
            clone.CombinatorType = CombinatorType;

            clone.PositionType = PositionType;
            clone.AttributeName = AttributeName;
            clone.AttributeSelectorType = AttributeSelectorType;
            clone.AttributeValue = AttributeValue;
            clone.ChildDepth = ChildDepth;
            clone.Class = Class;
            clone.Criteria = Criteria;
            clone.Html = Html;
            clone.ID = ID;
            clone.NoIndex = NoIndex;
            clone.PositionIndex = PositionIndex;
            clone.SelectElements = SelectElements;
            clone.Tag = Tag;
            clone.OtherType = OtherType;
            
            if (HasSubSelectors)
            {
                foreach (var selector in SubSelectors)
                {
                    clone.SubSelectors.Add(selector.Clone());
                }
            }
            return clone;
        }

        public override string ToString()
        {
            string output = "";
            switch (TraversalType)
            {
                case TraversalType.All:
                    output = "";
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
                if (!String.IsNullOrEmpty(AttributeValue))
                {
                    output += "." + AttributeSelectorType.ToString() + ".'" + AttributeValue + "'";
                }
                output += "]";
            }
            if (SelectorType.HasFlag(SelectorType.Class))
            {
                output += "." + Class;
            }
            if (SelectorType.HasFlag(SelectorType.All))
            {
                output += "*";
            }
            if (SelectorType.HasFlag(SelectorType.Position))
            {
                output += ":" + PositionType.ToString();
                if (IsFunction)
                {
                    output += "(" + PositionIndex + ")";
                }
                else if (SubSelectors.Count > 0)
                {
                    output += SubSelectors.ToString();
                }
            }
            if (SelectorType.HasFlag(SelectorType.Contains))
            {
                output += ":contains(" + Criteria + ")";
            }


            return output;
        }
        #endregion
                    
    }
}
