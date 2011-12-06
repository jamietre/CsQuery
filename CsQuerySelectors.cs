using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Jtc.CsQuery.ExtensionMethods;
using Jtc.CsQuery.Engine;
using Jtc.CsQuery.Utility;
using Jtc.CsQuery.Utility.StringScanner;

namespace Jtc.CsQuery
{
    public class CsQuerySelectors : IEnumerable<CsQuerySelector>
    { 
        public string Selector
        {
            get
            {
                return _Selector;
            }
            set
            {
                _Selector = value;
                ParseSelector(value);
            }
        } protected string _Selector = null;
        public CsQuerySelectors(string selector)   
        {
            Selector = selector;
        }
        public CsQuerySelectors(IEnumerable<IDomObject> elements ) {

            CsQuerySelector sel = new CsQuerySelector();
            sel.SelectorType = SelectorType.Elements;
            sel.SelectElements = elements;
            Selectors.Add(sel);
        }
        public CsQuerySelectors(IDomObject element)
        {

            CsQuerySelector sel = new CsQuerySelector();
            sel.SelectorType = SelectorType.Elements;
            sel.SelectElements = new List<IDomObject>();
            ((List<IDomObject>)sel.SelectElements).Add(element);
            Selectors.Add(sel);
        }

        protected CssSelectionEngine Engine
        {
            get
            {
                if (_Engine == null)
                {
                    _Engine = new CssSelectionEngine();
                    _Engine.Selectors = this;
                }
                return _Engine;
            }
        }
        protected CssSelectionEngine _Engine;
        protected CsQuerySelector Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = new CsQuerySelector();
                }
                return _Current;
            }

        } private CsQuerySelector _Current = null;

        /// <summary>
        /// Closes the currently active selector, destroying any partial selector composed so far.
        /// Returns true if a selector was added
        /// </summary>
        protected bool SaveCurrent() {
            bool result = false;
            if (Current.IsComplete)
            {
                Selectors.Add(Current);
                result = true;
            }
            return result;
        }
        protected void ClearCurrent()
        {
            _Current = null;
        }
        IStringScanner scanner;
        protected void ParseSelector(string selector)
        {
            string sel = ( _Selector ?? String.Empty).Trim();
            
            if (IsHtml)
            {
                Current.Html = sel;
                Current.SelectorType = SelectorType.HTML;
                Selectors.Add(Current);
                return;
            }
            scanner = Scanner.Create(sel);
            //scanner.StopChars = ;
            
            StartNewRootSelector();
            
            
            while (!scanner.Finished) {
                switch (scanner.NextChar)
                {
                    case '*':
                        Current.SelectorType = SelectorType.All;
                        scanner.Next();
                        break;
                    case '<':
                        // not selecting - creating html
                        Current.Html = sel;
                        scanner.End();
                        break;
                    case ':':
                        scanner.Next();
                        string key = scanner.Get(MatchFunctions.PseudoSelector);
                        switch (key)
                        {
                            case "checkbox":
                            case "radio":
                            case "button":
                            case "file":
                            case "text":
                                StartNewSelector(SelectorType.Attribute);

                                //Current.SelectorType |= SelectorType.Attribute;
                                Current.AttributeSelectorType = AttributeSelectorType.Equals;
                                Current.AttributeName = "type";
                                Current.AttributeValue = key;

                                if (key == "button" && !Current.SelectorType.HasFlag(SelectorType.Tag))
                                {
                                    //StartNewSelector(CombinatorType.Cumulative);
                                    StartNewSelector(SelectorType.Tag,CombinatorType.Cumulative,Current.TraversalType);
                                    //Current.SelectorType = SelectorType.Tag;
                                    Current.Tag = "button";
                                }
                                break;
                            case "checked":
                            case "selected":
                            case "disabled":
                                StartNewSelector(SelectorType.Attribute);
                                Current.AttributeSelectorType = AttributeSelectorType.Exists;
                                Current.AttributeName = key;
                                break;
                            case "enabled":
                                StartNewSelector(SelectorType.Attribute);
                                Current.AttributeSelectorType = AttributeSelectorType.NotExists;
                                Current.AttributeName = "disabled";
                                break;
                            case "visible":
                                throw new Exception("Not implemented.");
                            case "contains":

                                StartNewSelector(SelectorType.Contains);
                                IStringScanner inner = scanner.ExpectBoundedBy('(', true).ToNewScanner();
                                Current.Criteria = inner.Get(MatchFunctions.OptionallyQuoted);
                                break;
                            case "eq":
                            case "gt":
                            case "lt":
                                StartNewSelector(SelectorType.Position);
                                switch(key) {
                                    case "eq": Current.PositionType=PositionType.IndexEquals; break;
                                    case "lt": Current.PositionType = PositionType.IndexLessThan; break;
                                    case "gt": Current.PositionType = PositionType.IndexGreaterThan; break;
                                }
                                
                                scanner.ExpectChar('(');
                                Current.PositionIndex = Convert.ToInt32(scanner.GetNumber());
                                scanner.ExpectChar(')');

                                break;
                            case "even":
                                StartNewSelector(SelectorType.Position);
                                Current.PositionType = PositionType.Even;
                                break;
                            case "odd":
                                StartNewSelector(SelectorType.Position);
                                Current.PositionType = PositionType.Odd;
                                break;
                            case "first":
                                StartNewSelector(SelectorType.Position);
                                Current.PositionType = PositionType.First;
                                break;
                            case "last":
                                StartNewSelector(SelectorType.Position);
                                Current.PositionType = PositionType.Last;
                                break;
                            case "last-child":
                                StartNewSelector(SelectorType.Position);
                                Current.PositionType = PositionType.LastChild;
                                break;
                            case "first-child":
                                StartNewSelector(SelectorType.Position);
                                Current.PositionType = PositionType.FirstChild;
                                break;
                            case "nth-child":
                                StartNewSelector(SelectorType.Position);
                                Current.PositionType = PositionType.NthChild;
                                Current.Criteria=scanner.GetBoundedBy('(');
                                break;
                            case "has":
                            case "not":
                                StartNewSelector(key=="has"?SelectorType.SubSelectorHas: SelectorType.SubSelectorNot);
                                Current.TraversalType = TraversalType.Descendent;

                                string criteria = Current.Criteria = scanner.GetBoundedBy('(',true);
                                CsQuerySelectors subSelectors = new CsQuerySelectors(criteria);
                                Current.SubSelectors.Add(subSelectors);
                                break;

                            default:
                                throw new Exception("Unknown pseudoselector :\""+key+"\"");

                        }
                        break;
                    case '.':
                        StartNewSelector(SelectorType.Class);
                        scanner.Next();
                        Current.Class = scanner.Get(MatchFunctions.CssClass);
                        break;
                    case '#':
                        
                        scanner.Next();
                        if (!scanner.Finished)
                        {
                            StartNewSelector(SelectorType.ID);
                            Current.ID = scanner.Get(MatchFunctions.HTMLAttribute);
                        }
                        
                        break;
                    case '[':
                        StartNewSelector(SelectorType.Attribute);

                        IStringScanner innerScanner = scanner.ExpectBoundedBy('[', true).ToNewScanner();
                        Current.AttributeName = innerScanner.Get(MatchFunctions.HTMLAttribute);
                        innerScanner.SkipWhitespace();

                        if (innerScanner.Finished)
                        {
                            Current.AttributeSelectorType = AttributeSelectorType.Exists;
                        } else {
                            string matchType = innerScanner.Get("=", "^=", "*=", "~=", "$=", "!=");
                            Current.AttributeValue = innerScanner.Get(expectsOptionallyQuotedValue());
                            switch (matchType)
                            {
                                    
                                case "=":
                                    Current.AttributeSelectorType = AttributeSelectorType.Equals;
                                    break;
                                case "^=":
                                    Current.AttributeSelectorType = AttributeSelectorType.StartsWith;
                                    break;
                                case "*=":
                                    Current.AttributeSelectorType = AttributeSelectorType.Contains;
                                    break;
                                case "~=":
                                    Current.AttributeSelectorType = AttributeSelectorType.ContainsWord;
                                    break;
                                case "$=":
                                    Current.AttributeSelectorType = AttributeSelectorType.EndsWith;
                                    break;
                                case "!=":
                                    Current.AttributeSelectorType = AttributeSelectorType.NotEquals;
                                    break;
     
                                default:
                                    throw new Exception("Unknown attibute matching operator '" + matchType + "'");
                            }
                        }   
                            
                        break;
                    case ',':
                        if (Current.SelectorType != 0) {
                            SaveCurrent();
                            if (Selectors.Count==0) {
                                // , can only be after a complete selector
                                throw new Exception(", combinator found,  but the previous selector wasn't complete.");
                            }

                        }
                        ClearCurrent();
                        StartNewRootSelector();
                        scanner.NextNonWhitespace();
                        break;
                    case '>':
                        if (Current.SelectorType != 0)
                        {
                            SaveCurrent();
                            ClearCurrent();
                            StartNewSelector(TraversalType.Child);
                        }
                        else
                        {
                            Current.TraversalType = TraversalType.Child;
                        }
                        
                        // This is a wierd thing because if you use the > selector against a set directly, the meaning is "filter" 
                        // whereas if it is used in a combination selector the meaning is "filter for 1st child"
                        Current.ChildDepth = (Current.CombinatorType==CombinatorType.Root ? 0 : 1);
                        scanner.NextNonWhitespace();
                        break;
                    case ' ':
                        // if a ">" or "," is later found, it will be overridden.
                        scanner.NextNonWhitespace();
                        StartNewSelector(TraversalType.Descendent);
                        break;
                    default:
                        
                        string tag;
                        if (scanner.TryGet(MatchFunctions.HTMLTagName, out tag))
                        {
                            StartNewSelector(SelectorType.Tag);
                            Current.Tag = tag;
                        }

                        // When nothing was retrieved and it's the start of a selector, treat as text. Otherwise ignore the rest
                        // Quit either way
                        if (String.IsNullOrEmpty(Current.Tag))
                        {
                            if (scanner.Pos == 0)
                            {
                                Current.Html = sel;
                                Current.SelectorType = SelectorType.HTML;
                            }
                            scanner.End();
                        }
                        break;
                }
            }
            // Close any open selectors
            StartNewRootSelector();

        }
        protected IExpectPattern expectsOptionallyQuotedValue()
        {
            var pattern = new Jtc.CsQuery.Utility.StringScanner.Patterns.OptionallyQuoted();
            pattern.Terminators = Objects.Enumerate(']');
            return pattern;
        }

        protected void StartNewSelector(SelectorType positionType)
        {
            StartNewSelector(positionType, CombinatorType.Chained, TraversalType.Filter);
        }
        protected void StartNewSelector(CombinatorType combinatorType, TraversalType traversalType  )
        {
            StartNewSelector(0, combinatorType, traversalType);
        }
        protected void StartNewSelector(TraversalType traversalType)
        {
            StartNewSelector(0,CombinatorType.Chained,traversalType);
        }
        /// <summary>
        /// Close the currently active selector. If it's partial (e.g. a descendant/child marker) then merge its into into the 
        /// new selector created.
        /// </summary>
        /// <param name="selectorType"></param>
        /// <param name="combinatorType"></param>
        /// <param name="traversalType"></param>
        protected void StartNewSelector(SelectorType selectorType, 
            CombinatorType combinatorType, 
            TraversalType traversalType)
        {
            int childDepth = 0;
            if (_Current != null) 
            {
//                if (traversalType = TraversalType.Filter) 

                if (!SaveCurrent()) {
                    // this means " " or ">" was used, capture the traversal type. Skip if it was just a " " before a ">"
                    traversalType = Current.TraversalType;
                    combinatorType = Current.CombinatorType;
                    childDepth = Current.ChildDepth;
                }
                ClearCurrent();
            }
            Current.SelectorType = selectorType;
            Current.TraversalType = traversalType;
            Current.PositionType = PositionType.All;
            Current.CombinatorType = combinatorType;
            Current.ChildDepth = childDepth;
        }

        protected void StartNewRootSelector()
        {
            StartNewSelector(0,
                combinatorType:CombinatorType.Root,
                traversalType:TraversalType.All);

        }
        public bool IsHtml
        {
            get
            {
                return !String.IsNullOrEmpty(Selector) && Selector[0] == '<';
            }
        }
        public int Count
        {
            get
            {
                return Selectors.Count;
            }
        }
        protected List<CsQuerySelector> Selectors
        {
            get
            {
                if (_Selectors == null)
                {
                    _Selectors = new List<CsQuerySelector>();
                }
                return _Selectors;
            }
        } protected List<CsQuerySelector> _Selectors = null;
        public CsQuerySelector this[int index]
        {
            get
            {
                return Selectors[index];
            }
        }

        public IEnumerable<IDomObject> Is(IDomRoot root, IDomObject element)
        {
            List<IDomObject> list = new List<IDomObject>();
            list.Add(element);
            return Select(root, list);
        }
        public IEnumerable<IDomObject> Select(IDomRoot document)
        {
            return Select(document, (IEnumerable<IDomObject>)null);
        }
        public IEnumerable<IDomObject> Select(IDomRoot document, IDomObject context)
        {
            return Select(document, Objects.Enumerate(context));
        }
        public IEnumerable<IDomObject> Select(IDomRoot document, IEnumerable<IDomObject> context)
        {
            return Engine.Select(document, context);
        }
        public override string ToString()
        {
            string output = "";
            bool first=true;
            foreach (var selector in this)
            {
                if (!first && selector.CombinatorType == CombinatorType.Root) {
                    output+=", ";
                }
                output+=selector.ToString();
                first = false;
            }
            return output;
        }
        
        #region IEnumerable<CsQuerySelector> Members

        public IEnumerator<CsQuerySelector> GetEnumerator()
        {
            return Selectors.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Selectors.GetEnumerator();
        }

        #endregion
    }
    
  
}
