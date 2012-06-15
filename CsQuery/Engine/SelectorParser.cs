using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.Utility;
using CsQuery.StringScanner;
using CsQuery.StringScanner.Patterns;

namespace CsQuery.Engine
{
    /// <summary>
    /// A class to parse a CSS selector string into a sequence of Selector objects
    /// </summary>
    public class SelectorParser
    {
        #region private properties

        protected IStringScanner scanner;
        
        protected  Selector Selectors;
        private SelectorClause _Current;
        TraversalType NextTraversalType = TraversalType.All;
        CombinatorType NextCombinatorType = CombinatorType.Root;

        protected SelectorClause Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = new SelectorClause();
                }
                return _Current;
            }
        } 

        #endregion

        #region public methods

        /// <summary>
        /// Parse the string, and return a sequence of Selector objects
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public Selector Parse(string selector)
        {
            Selectors = new Selector();

            string sel = (selector ?? String.Empty).Trim();

            if (IsHtml(selector))
            {
                Current.Html = sel;
                Current.SelectorType = SelectorType.HTML;
                Selectors.Add(Current);
                return Selectors;
            }
            
            scanner = Scanner.Create(sel);

            while (!scanner.Finished)
            {
                switch (scanner.NextChar)
                {
                    case '*':
                        StartNewSelector(SelectorType.All);
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
                            case "input":
                                AddTagSelector("input");
                                AddTagSelector("textarea",true);
                                AddTagSelector("select",true);
                                AddInputSelector("button",true);
                                break;
                            case "checkbox":
                            case "radio":
                            case "button":
                            case "file":
                            case "text":
                            case "image":
                            case "reset":
                            case "submit":
                            case "password":
                                AddInputSelector(key);
                                break;
                            case "checked":
                            case "selected":
                            case "disabled":
                                StartNewSelector(SelectorType.AttributeExists);
                                Current.AttributeSelectorType = AttributeSelectorType.Exists;
                                Current.AttributeName = key;
                                break;
                            case "enabled":
                                StartNewSelector(SelectorType.AttributeValue);
                                Current.AttributeSelectorType = AttributeSelectorType.NotExists;
                                Current.AttributeName = "disabled";
                                break;
                            case "contains":

                                StartNewSelector(SelectorType.Contains);
                                IStringScanner inner = scanner.ExpectBoundedBy('(', true).ToNewScanner();
                                Current.Criteria = inner.Get(MatchFunctions.OptionallyQuoted);
                                break;
                            case "eq":
                            case "gt":
                            case "lt":
                                StartNewSelector(SelectorType.PseudoClass);
                                switch (key)
                                {
                                    case "eq": Current.PseudoClassType = PseudoClassType.IndexEquals; break;
                                    case "lt": Current.PseudoClassType = PseudoClassType.IndexLessThan; break;
                                    case "gt": Current.PseudoClassType = PseudoClassType.IndexGreaterThan; break;
                                }

                                scanner.ExpectChar('(');
                                Current.PositionIndex = Convert.ToInt32(scanner.GetNumber());
                                scanner.ExpectChar(')');

                                break;
                            case "even":
                                StartNewSelector(SelectorType.PseudoClass);
                                Current.PseudoClassType = PseudoClassType.Even;
                                break;
                            case "odd":
                                StartNewSelector(SelectorType.PseudoClass);
                                Current.PseudoClassType = PseudoClassType.Odd;
                                break;
                            case "first":
                                StartNewSelector(SelectorType.PseudoClass);
                                Current.PseudoClassType = PseudoClassType.First;
                                break;
                            case "last":
                                StartNewSelector(SelectorType.PseudoClass);
                                Current.PseudoClassType = PseudoClassType.Last;
                                break;
                            case "last-child":
                                StartNewSelector(SelectorType.PseudoClass);
                                Current.PseudoClassType = PseudoClassType.LastChild;
                                break;
                            case "first-child":
                                StartNewSelector(SelectorType.PseudoClass);
                                Current.PseudoClassType = PseudoClassType.FirstChild;
                                break;
                            case "first-of-type":
                                string type = Current.Tag;
                                StartNewSelector(SelectorType.PseudoClass);
                                Current.PseudoClassType = PseudoClassType.FirstOfType;
                                Current.Criteria = type;
                                break;
                            case "last-of-type":
                                type = Current.Tag;
                                StartNewSelector(SelectorType.PseudoClass);
                                Current.PseudoClassType = PseudoClassType.LastOfType;
                                Current.Criteria = type;
                                break;
                            case "only-child":
                                StartNewSelector(SelectorType.PseudoClass);
                                Current.PseudoClassType = PseudoClassType.OnlyChild;
                                break;                                
                            case "has":
                                StartNewSelector(SelectorType.PseudoClass);
                                Current.PseudoClassType = PseudoClassType.Has;
                                Current.Criteria = scanner.GetBoundedBy('(', true);
                                break;
                            case "not":
                                //StartNewSelector(key == "has" ? SelectorType.SubSelectorHas : SelectorType.SubSelectorNot);
                                //string criteria = Current.Criteria = scanner.GetBoundedBy('(', true);
                                StartNewSelector(SelectorType.PseudoClass);
                                Current.PseudoClassType = PseudoClassType.Not;
                                Current.Criteria = scanner.GetBoundedBy('(', true);
                                break;
                            case "visible":
                                StartNewSelector(SelectorType.PseudoClass);
                                Current.PseudoClassType = PseudoClassType.Visible;
                                break;
                            case "hidden":
                                StartNewSelector(SelectorType.PseudoClass);
                                Current.PseudoClassType = PseudoClassType.Hidden;
                                break;
                            case "empty":
                                StartNewSelector(SelectorType.PseudoClass);
                                Current.PseudoClassType = PseudoClassType.Empty;
                                break;
                            case "parent":
                                StartNewSelector(SelectorType.PseudoClass);
                                Current.PseudoClassType = PseudoClassType.Parent;
                                break;
                            case "only-of-type":
                                type = Current.Tag;
                                StartNewSelector(SelectorType.PseudoClass);
                                Current.PseudoClassType = PseudoClassType.OnlyOfType;
                                
                                // when it's not a filter, we are getting the only one of every type; skip criteria
                                if (Current.TraversalType == TraversalType.Filter)
                                {
                                    Current.Criteria = type;
                                }
                                break;
                            case "header":
                                StartNewSelector(SelectorType.PseudoClass);
                                Current.PseudoClassType = PseudoClassType.Header;
                                break;
                            case "nth-child":
                                StartNewSelector(SelectorType.PseudoClass);
                                Current.PseudoClassType = PseudoClassType.NthChild;
                                Current.Criteria = scanner.GetBoundedBy('(');
                                break;
                            case "nth-of-type":
                                type = Current.Tag;
                                StartNewSelector(SelectorType.PseudoClass);
                                Current.PseudoClassType = PseudoClassType.NthOfType;
                                
                                // when it's not a filter, we are getting the only one of every type; skip criteria

                                Current.Criteria = scanner.GetBoundedBy('(');
                                if (Current.TraversalType == TraversalType.Filter)
                                {
                                    Current.Criteria += "|"+ type;
                                }
                                break;
                            case "nth-last-child":
                                type = Current.Tag;
                                StartNewSelector(SelectorType.PseudoClass);
                                Current.PseudoClassType = PseudoClassType.NthLastChild;
                                
                                Current.Criteria = scanner.GetBoundedBy('(') + "|"+ type;

                                break;
                            case "nth-last-of-type":
                                type = Current.Tag;
                                StartNewSelector(SelectorType.PseudoClass);
                                Current.PseudoClassType = PseudoClassType.NthLastOfType;
                                Current.Criteria = scanner.GetBoundedBy('(');
                                if (Current.TraversalType == TraversalType.Filter)
                                {
                                    Current.Criteria += "|"+ type;
                                }
                                break;
                            case "lang":
                                // The problem with :lang is that it is based on an inherited property value. This messes  with the index since
                                // elements will be pre-filtered by an attribute selector. This could probably be implemented using a pseudoclass
                                // type construct instead, e.g. as "visible", but since this is a low priority it's excluded for now.

                                //StartNewSelector(SelectorType.Attribute);
                                //Current.AttributeSelectorType = AttributeSelectorType.StartsWithOrHyphen;
                                //Current.TraversalType = TraversalType.Inherited;
                                //Current.AttributeName = "lang";

                                //Current.Criteria = scanner.GetBoundedBy('(', false);
                                //break;
                                throw new NotImplementedException(":lang is not currently supported.");
                                

                            case "first-letter":
                            case "first-line":
                            case "before":
                            case "after":
                                throw new NotImplementedException("The CSS pseudoelement selectors are not implemented in CsQuery.");
                            case "target":
                            case "link":
                            case "hover":
                            case "active":
                            case "focus":
                            case "visited":
                                throw new NotImplementedException("Pseudoclasses that require a browser aren't implemented.");

                            default:
                                throw new ArgumentException("Unknown pseudo-class :\"" + key + "\". If this is a valid CSS or jQuery selector, please let us know.");
                        }
                        break;
                    case '.':
                        StartNewSelector(SelectorType.Class);
                        scanner.Next();
                        Current.Class = scanner.Get(MatchFunctions.CssClassName);
                        break;
                    case '#':

                        scanner.Next();
                        if (!scanner.Finished)
                        {
                            StartNewSelector(SelectorType.ID);
                            Current.ID = scanner.Get(MatchFunctions.HtmlIDValue());
                        }

                        break;
                    case '[':
                        StartNewSelector(SelectorType.AttributeExists);

                        IStringScanner innerScanner = scanner.ExpectBoundedBy('[', true).ToNewScanner();
                        
                        Current.AttributeName = innerScanner.Get(MatchFunctions.HTMLAttribute());
                        innerScanner.SkipWhitespace();

                        if (innerScanner.Finished)
                        {
                            Current.AttributeSelectorType = AttributeSelectorType.Exists;
                        }
                        else
                        {
                            string matchType = innerScanner.Get("=", "^=", "*=", "~=", "$=", "!=","|=");

                            // CSS allows [attr=] as a synonym for [attr]
                            if (innerScanner.Finished)
                            {
                                Current.AttributeSelectorType = AttributeSelectorType.Exists;
                            } 
                            else 
                            {
                                Current.AttributeValue=innerScanner.Get(expectsOptionallyQuotedValue());
                                switch (matchType)
                                {

                                    case "=":
                                        Current.SelectorType |= SelectorType.AttributeValue;
                                        Current.AttributeSelectorType = AttributeSelectorType.Equals;
                                        break;
                                    case "^=":
                                        Current.SelectorType |= SelectorType.AttributeValue;
                                        Current.AttributeSelectorType = AttributeSelectorType.StartsWith;
                                        // attributevalue starts with "" matches nothing
                                        if (Current.AttributeValue == "")
                                        {
                                            Current.AttributeValue = "" + (char)0;
                                        }
                                        break;
                                    case "*=":
                                        Current.SelectorType |= SelectorType.AttributeValue;
                                        Current.AttributeSelectorType = AttributeSelectorType.Contains;
                                        break;
                                    case "~=":
                                        Current.SelectorType |= SelectorType.AttributeValue;
                                        Current.AttributeSelectorType = AttributeSelectorType.ContainsWord;
                                        break;
                                    case "$=":
                                        Current.SelectorType |= SelectorType.AttributeValue;
                                        Current.AttributeSelectorType = AttributeSelectorType.EndsWith;
                                        break;
                                    case "!=":
                                        Current.SelectorType |= SelectorType.AttributeValue;
                                        Current.SelectorType &= ~SelectorType.AttributeExists;
                                        Current.AttributeSelectorType = AttributeSelectorType.NotEquals;
                                        // must matched manually - missing also validates as notEquals
                                        
                                        break;
                                    case "|=":
                                        Current.SelectorType |= SelectorType.AttributeValue;
                                        Current.AttributeSelectorType = AttributeSelectorType.StartsWithOrHyphen;

                                        break;
                                    default:
                                        throw new ArgumentException("Unknown attibute matching operator '" + matchType + "'");
                                }
                            }
                        }

                        break;
                    case ',':
                        FinishSelector();
                        NextCombinatorType = CombinatorType.Root;
                        NextTraversalType = TraversalType.All;
                        scanner.NextNonWhitespace();
                        break;
                    case '+':
                        StartNewSelector(TraversalType.Adjacent);
                        scanner.NextNonWhitespace();
                        break;
                    case '~':
                        StartNewSelector(TraversalType.Sibling);
                        scanner.NextNonWhitespace();
                        break;
                    case '>':
                        StartNewSelector(TraversalType.Child);
                        // This is a wierd thing because if you use the > selector against a set directly, the meaning is "filter" 
                        // whereas if it is used in a combination selector the meaning is "filter for 1st child"
                        //Current.ChildDepth = (Current.CombinatorType == CombinatorType.Root ? 0 : 1);
                        Current.ChildDepth = 1;
                        scanner.NextNonWhitespace();
                        break;
                    case ' ':
                        // if a ">" or "," is later found, it will be overridden.
                        scanner.NextNonWhitespace();
                        NextTraversalType = TraversalType.Descendent;
                        //StartNewSelector(TraversalType.Descendent);
                        break;
                    default:

                        string tag = "";
                        if (scanner.TryGet(MatchFunctions.HTMLTagSelectorName, out tag))
                        {
                            AddTagSelector(tag);
                        }
                        else
                        {
                            if (scanner.Pos == 0)
                            {
                                Current.Html = sel;
                                Current.SelectorType = SelectorType.HTML;
                                scanner.End();
                            }
                            else
                            {
                                throw new ArgumentException(scanner.LastError);
                            }

                        }

                        break;
                }
            }
            // Close any open selectors
            FinishSelector();
            if (Selectors.Count == 0)
            {
                var empty = new SelectorClause
                {
                    SelectorType = SelectorType.None,
                    TraversalType = TraversalType.Filter
                };
                Selectors.Add(empty);
                
            }
            return Selectors;
        }
        #endregion

        #region private methods

        /*
         * The "And" combinator is used to create groups of selectors that are kept in the context of an active subselector.
         * e.g. unlike just adding another clause with CombinatorType.Root (or a ","), this joins them but acting as a single
         * selector.
         * */

        private void AddTagSelector(string tagName, bool combineWithPrevious=false) 
        {
            if (!combineWithPrevious) {
                StartNewSelector(SelectorType.Tag);
            } else {
                 StartNewSelector(SelectorType.Tag,CombinatorType.And,Current.TraversalType);
            }
            Current.Tag = tagName;
        }

        private void AddInputSelector(string type, bool combineWithPrevious=false)
        {
            if (!combineWithPrevious)
            {
                StartNewSelector(SelectorType.AttributeExists);
            }
            else
            {
                StartNewSelector(SelectorType.AttributeExists, CombinatorType.And, Current.TraversalType);
            }
            Current.SelectorType |= SelectorType.AttributeValue;

            Current.AttributeSelectorType = AttributeSelectorType.Equals;
            Current.AttributeName = "type";
            Current.AttributeValue = type;

            if (type == "button" && !Current.SelectorType.HasFlag(SelectorType.Tag))
            {
                AddTagSelector("button",true);
            }

        }
        /// <summary>
        /// A pattern for the operand of an attribute selector
        /// </summary>
        /// <returns></returns>
        protected IExpectPattern expectsOptionallyQuotedValue()
        {
            var pattern = new OptionallyQuoted();
            pattern.Terminators = Objects.Enumerate(']');
            return pattern;
        }

        /// <summary>
        /// Start a new chained filter selector of the specified type
        /// </summary>
        /// <param name="positionType"></param>
        protected void StartNewSelector(SelectorType selectorType)
        {
            StartNewSelector(selectorType, NextCombinatorType, NextTraversalType);
        }

        /// <summary>
        /// Start a new selector that does not yet have a type specified
        /// </summary>
        /// <param name="combinatorType"></param>
        /// <param name="traversalType"></param>
        protected void StartNewSelector(CombinatorType combinatorType, TraversalType traversalType)
        {
            StartNewSelector(0, combinatorType, traversalType);
        }

        /// <summary>
        /// Start a new chained selector that does not yet have a type specified
        /// </summary>
        /// <param name="traversalType"></param>
        protected void StartNewSelector(TraversalType traversalType)
        {
            StartNewSelector(0, NextCombinatorType, traversalType);
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

            // if a selector was not finished, do not overwrite the existing combinator & traversal types,
            // as they could have been changed by a descendant or child selector. The exception is when
            // the new selector is an explicit "all" type; we always 

            // a new selector will not have been started if there was an explicit "*" creating an all. However, if there's
            // anything other than a filter, we do actually want 


            if (Current.IsComplete &&
                Current.SelectorType != SelectorType.All || traversalType != TraversalType.Filter)
            {
                    FinishSelector();
                    Current.CombinatorType = combinatorType;
                    Current.TraversalType = traversalType;
                
            }

            Current.SelectorType = selectorType;
        }

        /// <summary>
        /// Finishes any open selector and clears the current selector
        /// </summary>
        protected void FinishSelector()
        {
            if (Current.IsComplete)
            {
                var cur = Current.Clone();
                Selectors.Add(cur);
            }
            Current.Clear();
            NextTraversalType = TraversalType.Filter;
            NextCombinatorType = CombinatorType.Chained;
        }

        /// <summary>
        /// Clear the currently open selector
        /// </summary>
        protected void ClearCurrent()
        {
            _Current = null;
        }

        /// <summary>
        /// Return true of the text appears to be HTML (e.g. starts with a caret)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool IsHtml(string text)
        {
            return !String.IsNullOrEmpty(text) && text[0] == '<';
        }
        #endregion
    }
}
