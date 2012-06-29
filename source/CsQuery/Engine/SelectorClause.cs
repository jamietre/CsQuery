using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Engine
{
    /// <summary>
    /// A CSS selector parsed into it's component parts
    /// </summary>
    public class SelectorClause
    {

        #region constructors
       
        public SelectorClause()
        {
            Initialize();
        }
        protected void Initialize()
        {
            SelectorType = 0;
            AttributeSelectorType = AttributeSelectorType.Equals;
            CombinatorType = CombinatorType.Root;
            TraversalType = TraversalType.All;
            PseudoClassType = PseudoClassType.All;
        }

        #endregion

        #region private properties

        protected string _Tag;
        protected SelectorType _SelectorType;
        protected string _AttributeName;
        protected List<Selector> _SubSelectors;
        
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
        public PseudoClassType PseudoClassType { get; set; }
        public AttributeSelectorType AttributeSelectorType { get; set; }

        /// <summary>
        /// Selection tag name
        /// </summary>
        public string Tag
        {
            get
            {
                return _Tag;
            }
            set
            {
                _Tag = value == null ?
                    value:
                    value.ToUpper();
            }
        }
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
        /// This is really "parameters" and is used differently by different selectors. It's the criteria for attribute selectors;
        /// the node type for -of-type selectors, the equation for nth-child. For nth-of-type, its "type|equation"
        /// </summary>
        public string Criteria {get;set;}

        /// <summary>
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

        public bool IsDomPositionPseudoSelector
        {
            get
            {
                if (SelectorType != SelectorType.PseudoClass)
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
                if (SelectorType != SelectorType.PseudoClass)
                {
                    return false;
                }
                switch (PseudoClassType)
                {
                    case PseudoClassType.Even:
                    case PseudoClassType.Odd:
                    case PseudoClassType.Last:
                    case PseudoClassType.First:
                    case PseudoClassType.IndexEquals:
                    case PseudoClassType.IndexGreaterThan:
                    case PseudoClassType.IndexLessThan:
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
                switch (PseudoClassType)
                {
                    case PseudoClassType.IndexEquals:
                    case PseudoClassType.IndexGreaterThan:
                    case PseudoClassType.IndexLessThan:
                    case PseudoClassType.NthChild:
                    case PseudoClassType.NthLastChild:
                    case PseudoClassType.NthLastOfType:
                    case PseudoClassType.NthOfType:
                    case PseudoClassType.Has:
                    case PseudoClassType.Not:
                        return true;
                    default:
                        return false;
                }
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
            _SubSelectors = null;

            Initialize();
        }

        public SelectorClause Clone()
        {
            SelectorClause clone = new SelectorClause();

            clone.SelectorType = SelectorType;
            clone.TraversalType = TraversalType;
            clone.CombinatorType = CombinatorType;
            clone.PseudoClassType = PseudoClassType;
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

            return clone;
        }

        public override int GetHashCode()
        {
            return GetHash(SelectorType) + GetHash(TraversalType) + GetHash(CombinatorType) +
                GetHash(PseudoClassType) + GetHash(AttributeName) + GetHash(AttributeSelectorType) +
                GetHash(AttributeValue) + GetHash(Class) + GetHash(Criteria) + GetHash(Html) +
                GetHash(ID) + GetHash(NoIndex) + GetHash(PositionIndex) + GetHash(SelectElements) +
                GetHash(Tag);
        }
        
        public override bool Equals(object obj)
        {
            SelectorClause other = obj as SelectorClause;
            return other != null &&
                other.SelectorType == SelectorType &&
                other.TraversalType == TraversalType &&
                other.CombinatorType == CombinatorType &&
                other.PseudoClassType == PseudoClassType &&
                other.AttributeName == AttributeName &&
                other.AttributeSelectorType == AttributeSelectorType &&
                other.AttributeValue == AttributeValue &&
                other.ChildDepth == ChildDepth &&
                other.Class == Class &&
                other.Criteria == Criteria &&
                other.Html == Html &&
                other.ID == ID &&
                other.NoIndex == NoIndex &&
                other.PositionIndex == PositionIndex &&
                other.SelectElements == SelectElements &&
                other.Tag == Tag;
        }

        private int GetHash(object obj) {

            return obj == null ? 0 : obj.GetHashCode();
        }

        public override string ToString()
        {
            string output = "";
            switch (TraversalType)
            {
                case TraversalType.Child:
                    output += " > ";
                    break;
                case TraversalType.Descendent:
                    output += " ";
                    break;
                case TraversalType.Adjacent:
                    output += " + ";
                    break;
                case TraversalType.Sibling :
                    output += " ~ ";
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
            
            if (SelectorType.HasFlag(SelectorType.AttributeValue) || SelectorType.HasFlag(SelectorType.AttributeExists))
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
            if (SelectorType.HasFlag(SelectorType.PseudoClass))
            {
                output += ":" + PseudoClassType.ToString();
                if (IsFunction)
                {
                    output += "(" + Criteria + ")";
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
