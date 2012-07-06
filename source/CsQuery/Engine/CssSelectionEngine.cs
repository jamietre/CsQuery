using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.HtmlParser;

namespace CsQuery.Engine
{
    public class CssSelectionEngine
    {
        #region constructor
        
        public CssSelectionEngine(IDomDocument document)
        {
            Document = document;
        }

        #endregion

        #region private properties


        protected IDomDocument Document;
        protected List<SelectorClause> ActiveSelectors;
        protected int activeSelectorId;

        #endregion

        #region public properties
        /// <summary>
        /// The current selection list being acted on
        /// </summary>

        public Selector Selectors { get; set; }
        #endregion

        #region public methods

        /// <summary>
        /// Select from the bound Document using index. First non-class/tag/id selector will result in this being passed off to GetMatches
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public IEnumerable<IDomObject> Select(IEnumerable<IDomObject> context)
        {

            if (Selectors == null )
            {
                throw new ArgumentException("No selectors provided.");
            }

            if (Selectors.Count == 0)
            {
                yield break;
            }

            ActiveSelectors = new List<SelectorClause>(Selectors);

            // First just check if we ended up here with an HTML selector; if so, had it off.
            var firstSelector = ActiveSelectors[0];
            if (firstSelector.SelectorType == SelectorType.HTML)
            {
                
                HtmlParser.HtmlElementFactory factory = new HtmlParser.HtmlElementFactory(firstSelector.Html);

                foreach (var obj in factory.Parse())
                {
                    yield return obj;
                }
                yield break;
            } 

            // this holds any results that carried over from the previous loop for chaining

            HashSet<IDomObject> lastResult = new HashSet<IDomObject>();

            // this holds the final output

            HashSet<IDomObject> output = new HashSet<IDomObject>();

            // this is the source  from which selections are made in a given iteration; it could be the DOM root, a context,
            // or the previous result set.
            
            IEnumerable<IDomObject> selectionSource=null;

            // Disable the index if there is no context (e.g. disconnected elements)


            bool useIndex = (context.IsNullOrEmpty() || !context.First().IsDisconnected) && Document.IsIndexed;



            for (activeSelectorId = 0; activeSelectorId < ActiveSelectors.Count; activeSelectorId++)
            {

                var selector = ActiveSelectors[activeSelectorId].Clone();

                // we will alter the selector during each iteration to remove the parts that have already been parsed,
                // so use a copy.
                // this is a selector that was chanined with the selector grouping combinator "," -- we always output the results so
                // far when beginning a new group.

                if (selector.CombinatorType == CombinatorType.Root && lastResult.Count>0)
                {
                    output.AddRange(lastResult);
                    lastResult.Clear();
                }

                // For "and" combinator types, we want to leave everything as it was -- the results of this selector should compound
                // with the prior.

                if (selector.CombinatorType != CombinatorType.And)
                {
                    selectionSource = GetSelectionSource(selector, context, lastResult);
                    lastResult.Clear();
                }

                

                string key = "";
                SelectorType removeSelectorType = 0;

                if (useIndex && !selector.NoIndex)
                {

#if DEBUG_PATH

                    if (type.HasFlag(SelectorType.AttributeExists))
                    {
                        key = "!" + selector.AttributeName;
                        removeSelectorType=SelectorType.AttributeExists;
                    } 
                    else if (type.HasFlag(SelectorType.Tag))
                    {
                        key = "+"+selector.Tag;
                        removeSelectorType=SelectorType.Tag;
                    }
                    else if (type.HasFlag(SelectorType.ID))
                    {
                        key = "#" + selector.ID;
                        removeSelectorType=SelectorType.ID;
                    }
                    else if (type.HasFlag(SelectorType.Class))
                    {
                        key = "." + selector.Class;
                        removeSelectorType=SelectorType.Class;
                    }

#else
                    if (selector.SelectorType.HasFlag(SelectorType.AttributeExists)) 
                    {
                        key = "!" + (char)HtmlData.TokenID(selector.AttributeName);

                        // AttributeValue must still be matched manually - so remove this flag only.
                        removeSelectorType=SelectorType.AttributeExists;
                    }
                    else if (selector.SelectorType.HasFlag(SelectorType.Tag))
                    {
                        key = "+" + (char)HtmlData.TokenID(selector.Tag);
                        removeSelectorType=SelectorType.Tag;
                    }
                    else if (selector.SelectorType.HasFlag(SelectorType.ID))
                    {
                        key = "#" + (char)HtmlData.TokenIDCaseSensitive(selector.ID);
                        removeSelectorType=SelectorType.ID;
                    }
                    else if (selector.SelectorType.HasFlag(SelectorType.Class))
                    {
                        key = "." + (char)HtmlData.TokenIDCaseSensitive(selector.Class);
                        removeSelectorType=SelectorType.Class;
                    }
#endif
                }

                // If part of the selector was indexed, key will not be empty. Return initial set from the
                // index. If any selectors remain after this they will be searched the hard way.

                IEnumerable<IDomObject> result = null;

                if (key != String.Empty)
                {
                    // This is the main index access point: if we have an index key, we'll get as much as we can from the index.
                    // Anything else will be handled manually.

                    int depth = 0;
                    bool descendants = true;
                    
                    switch (selector.TraversalType)
                    {
                        case TraversalType.Child:
                            depth = selector.ChildDepth;
                            descendants = false;
                            break;
                        case TraversalType.Filter:
                        case TraversalType.Adjacent:
                        case TraversalType.Sibling:
                            depth = 0;
                            descendants = false;
                            break;
                        case TraversalType.Descendent:
                            depth = 1;
                            descendants = true;
                            break;
                    }                    

                    if (selectionSource == null)
                    {
                        result = Document.DocumentIndex.QueryIndex(key + HtmlData.indexSeparator, depth, descendants);
                    }
                    else
                    {
                        HashSet<IDomObject> elementMatches = new HashSet<IDomObject>();
                        result = elementMatches;
                        foreach (IDomObject obj in selectionSource)
                        {
                            elementMatches.AddRange(Document.DocumentIndex.QueryIndex(key + HtmlData.indexSeparator + obj.Path,
                                    depth, descendants));
                        }

                    }
                    selector.SelectorType &= ~removeSelectorType;

                    // Special case for attribute selectors: when an Exists/Value attribute selector is present, we still need to filter
                    // for the correct value afterwards. But we need to change the traversal type ONLY if the primary match has already
                    // been done by the index; otherwise the couple cases where you need to match the value but can't select for "Exists" first
                    // won't work.

                    if (removeSelectorType == SelectorType.AttributeExists && selector.SelectorType.HasFlag(SelectorType.AttributeValue))
                    {
                        selector.TraversalType = TraversalType.Filter;
                    }
                    
                }
                else if (selector.SelectorType.HasFlag(SelectorType.Elements))
                {
                    HashSet<IDomObject> elementMatches = new HashSet<IDomObject>();
                    result = elementMatches;
                    foreach (IDomObject obj in GetAllChildOrDescendants(selector.TraversalType,selectionSource))
                    {

                        key = HtmlData.indexSeparator + obj.Path;
                        HashSet<IDomObject> srcKeys = new HashSet<IDomObject>(Document.DocumentIndex.QueryIndex(key));
                        foreach (IDomObject match in selector.SelectElements)
                        {
                            if (srcKeys.Contains(match))
                            {
                                elementMatches.Add(match);
                            }
                        }
                    }

                    selector.SelectorType &= ~SelectorType.Elements;
                }

                // If any selectors were not handled via the index, match them manually
                if (selector.SelectorType != 0)
                {
      
                    // if there are no temporary results (b/c there was no indexed selector) then use selection source instead
                    // (e.g. start from the same point that the index would have)

                    lastResult.AddRange(GetMatches(result ?? selectionSource ?? Document.ChildElements, selector));
                }
                else
                {
                    if (result != null)
                    {
                        lastResult.AddRange(result);
                    }
                }
            }

            // After the loop has finished, output any results from the last iteration.

            if (lastResult.Count>0)
            {
                output.AddRange(lastResult);
            }

            if (output.IsNullOrEmpty())
            {
                yield break;
            }
            else
            {
                // Selectors always return in DOM order. Selections may end up in a different order but
                // we always sort here.

                foreach (IDomObject item in output.OrderBy(item => item.Path, StringComparer.Ordinal))
                {
                    yield return item;
                }
            }
            ActiveSelectors.Clear();
        }

       
        #endregion

        #region selection matching main code


        /// <summary>
        /// Get the sequence that is the source for the current clause, based on the selector, prior results, and context.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="lastResult"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected IEnumerable<IDomObject> GetSelectionSource(SelectorClause selector, IEnumerable<IDomObject> context, IEnumerable<IDomObject> lastResult)
        {
            IEnumerable<IDomObject> selectionSource=null;
            switch (selector.CombinatorType)
            {
                case CombinatorType.Root:
                case CombinatorType.Chained:
                    selectionSource = null;
                    IEnumerable<IDomObject> interimSelectionSource = null;
                    if (selector.CombinatorType == CombinatorType.Root)
                    {
                        // if it's a root combinator type, then we need set the selection source to the context depending on the
                        // traversal type being applied.

                        if (context != null)
                        {
                            switch (selector.TraversalType)
                            {
                                case TraversalType.Adjacent:
                                case TraversalType.Sibling:
                                    //interimSelectionSource = GetChildElements(context);
                                    interimSelectionSource = context;
                                    break;
                                case TraversalType.Filter:
                                case TraversalType.Descendent:
                                    interimSelectionSource = context;
                                    break;
                                case TraversalType.All:
                                    selector.TraversalType = TraversalType.Descendent;
                                    interimSelectionSource = context;
                                    break;
                                case TraversalType.Child:
                                    interimSelectionSource = context;
                                    break;
                                default:
                                    throw new InvalidOperationException("The selector passed to FindImpl has an invalid traversal type for Find.");
                            }
                        } else {
                            interimSelectionSource = null;
                        }
                    }
                    else
                    {
                        // Must copy this because we will continue to add to lastResult in successive iterations

                        interimSelectionSource = lastResult.ToList();
                    }


                    // If the selector used the adjacent combinator, grab the next element for each
                    if (interimSelectionSource != null)
                    {
                        if (selector.TraversalType == TraversalType.Adjacent || selector.TraversalType == TraversalType.Sibling)
                        {
                            selectionSource = GetAdjacentOrSiblings(selector.TraversalType, interimSelectionSource);
                            selector.TraversalType = TraversalType.Filter;
                        }
                        else
                        {
                            selectionSource = interimSelectionSource;
                        }
                    }

                    break;
            }
            return selectionSource;
        }
        
        /// <summary>
        /// Return all elements matching a selector, within a domain baseList, starting from list.
        /// This function will traverse children, but it is expected that the source (e.g. from an Adjacent
        /// or Sibling selector) is correct.
        /// </summary>
        /// <param name="baseList"></param>
        /// <param name="list"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        protected IEnumerable<IDomObject> GetMatches(IEnumerable<IDomObject> list, SelectorClause selector)
        {
            // Maintain a hashset of every element already searched. Since result sets frequently contain items which are
            // children of other items in the list, we would end up searching the tree repeatedly
            HashSet<IDomObject> uniqueElements = null;

            Stack<MatchElement> stack = null;

            // map the list to adacent/siblings if needed. Descendant & child traversals are handled through
            // recursion.

            IEnumerable<IDomObject> curList = list;
            
            HashSet<IDomObject> temporaryResults = new HashSet<IDomObject>();

            // The unique list has to be reset for each sub-selector
            uniqueElements = new HashSet<IDomObject>();


            // For the jQuery extensions (which are mapped to the position in the output, not the DOM) we have to enumerate
            // the results first, rather than targeting specific child elements. Handle it here,

            if (selector.SelectorType.HasFlag(SelectorType.PseudoClass))
            {
                if (selector.IsResultListPosition) {

                    foreach (var obj in GetResultPositionMatches(curList, selector))
                    {
                        yield return obj;
                    }
                    yield break;
                } 
                //else if (selector.PseudoClassType == PseudoClassType.Not) {
                //    var subSelector = new Selector(selector.Criteria);
                //    foreach (var obj in subSelector.Except(Document,GetAllChildOrDescendants(selector.TraversalType,curList))) {
                //        yield return obj;
                //    }
                //    yield break;
                //}
                //else if (selector.PseudoClassType == PseudoClassType.Has)
                //{
                //    var subSelector = new Selector(selector.Criteria);
                //    foreach (var obj in GetAllChildOrDescendants(selector.TraversalType, curList))
                //    {
                //        if (Has(subSelector, obj))
                //        {
                //            yield return obj;
                //        }
                //    }
                //    yield break;
                //}
            }
            else if (selector.SelectorType.HasFlag(SelectorType.All))
            {
                // special case for all, just recurse
                foreach (var item in GetAllChildOrDescendants(selector.TraversalType,curList))
                {
                    yield return item;
                }
                yield break;
            } 

            // Otherwise, try to match each element individually
            
            stack = new Stack<MatchElement>();

            foreach (var obj in curList)
            {
                // We must check everything again when looking for specific depth of children
                // otherwise - no point - skip em
                IDomElement el = obj as IDomElement;
                if (el == null || selector.TraversalType != TraversalType.Child && uniqueElements.Contains(el))
                {
                    continue;
                }
                stack.Push(new MatchElement(el, 0));
                int matchIndex = 0;
                while (stack.Count != 0)
                {
                    var current = stack.Pop();

                    if (Matches(selector, current.Element, current.Depth))
                    {
                        temporaryResults.Add(current.Element);
                        matchIndex++;
                    }
                    // Add children to stack (in reverse order, so they are processed in the correct order when popped)

                    // Don't keep going to children if the target depth is < the depth. Though the match would still fail,
                    // stuff would end up the unique list which we might need to test later if it appears directly in the source list
                    // causing it to be ignored.

                    if (selector.TraversalType != TraversalType.Filter &&
                        (selector.TraversalType != TraversalType.Child || selector.ChildDepth > current.Depth))
                    {
                        SelectorType selectorType = selector.SelectorType;
                        IDomElement elm = current.Element;

                        if (selector.IsDomPositionPseudoSelector &&
                            ((selector.TraversalType == TraversalType.All) ||
                            (selector.TraversalType == TraversalType.Child && selector.ChildDepth == current.Depth + 1) ||
                            (selector.TraversalType == TraversalType.Descendent && selector.ChildDepth <= current.Depth + 1))) 
                        {
                            temporaryResults.AddRange(GetPseudoClassMatches(elm, selector));
                            selectorType &= ~SelectorType.PseudoClass;
                        }

                        if (selectorType == 0)
                        {
                            continue;
                        }

                        for (int j = elm.ChildNodes.Count - 1; j >= 0; j--)
                        {
                            IDomElement child = elm[j] as IDomElement;

                            if (child==null || !uniqueElements.Add(child))
                            {
                                continue;
                            }
                            if (child.NodeType == NodeType.ELEMENT_NODE)
                            {
                                stack.Push(new MatchElement(child, current.Depth + 1));
                            }
                        }
                    }
                }

            }

            foreach (var obj in temporaryResults)
            {
                yield return obj;
            }
            yield break;
        }



        /// <summary>
        /// Returns true if the element contains items matching the selector
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        //public bool Has(Selector selector, IDomObject element)
        //{
        //    selector[0].TraversalType = TraversalType.Descendent;
        //    return selector.Select(Document, element).Any();
        //}


        /// <summary>
        /// Return all elements that match the selector
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public IEnumerable<IDomObject> Matches(Selector selector, IEnumerable<IDomObject> elements)
        {
            HashSet<IDomObject> matches = new HashSet<IDomObject>(selector.Select(Document, elements));

            foreach (var item in elements)
            {
                if (matches.Contains(item))
                {
                    yield return item;
                }
            }
        }

        public IEnumerable<IDomObject> NotMatches(Selector selector, IEnumerable<IDomObject> elements)
        {
            HashSet<IDomObject> matches = new HashSet<IDomObject>(selector.Select(Document, elements));
            foreach (var item in elements)
            {
                if (!matches.Contains(item))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Return true if the element matches the selector. Anything other than "all" or filter-type selectors will return false.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool Matches(SelectorClause selector, IDomElement element)
        {
            switch (selector.TraversalType)
            {
                case TraversalType.All:
                    return true;
                case TraversalType.Filter:
                    return Matches(selector, element, 0);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Return true if an object matches a specific selector. If the selector has a desecendant or child traversal type, it must also
        /// match the specificed depth.
        /// </summary>
        /// <param name="selector">The jQuery/CSS selector</param>
        /// <param name="obj">The target object</param>
        /// <param name="depth">The depth at which the target must appear for descendant or child selectors</param>
        /// <returns></returns>
        protected bool Matches(SelectorClause selector, IDomElement obj, int depth)
        {
            switch (selector.TraversalType)
            {
                case TraversalType.Child:
                    if (selector.ChildDepth != depth)
                    {
                        return false;
                    }
                    break;
                case TraversalType.Descendent:
                    // Special case because this code is jacked up: when only "AttributeValue" it's ALWAYS a filter, it means
                    // the AttributeExists was handled previously by the index.

                    // This engine at some point should be reworked so that the "And" combinator is just a subselector, this logic has 
                    // become too brittle.

                   // if (depth == 0 && selector.SelectorType != SelectorType.AttributeValue)
                    if (depth == 0)
                    {
                        return false;
                    }
                    break;
            }

            if (selector.SelectorType.HasFlag(SelectorType.All))
            {
                return true;
            }

            if (selector.SelectorType.HasFlag(SelectorType.PseudoClass))
            {
                return //selector.TraversalType == TraversalType.Filter && 
                    MatchesPseudoClass(obj, selector);
            }

            if (obj.NodeType != NodeType.ELEMENT_NODE)
            {
                return false;
            }
            
            // Check each selector from easier/more specific to harder. e.g. ID is going to eliminate a lot of things.

            if (selector.SelectorType.HasFlag(SelectorType.ID) &&
                selector.ID != obj.Id)
            {
                return false;
            }
            if (selector.SelectorType.HasFlag(SelectorType.Class) &&
                !obj.HasClass(selector.Class))
            {
                return false;
            }
            if (selector.SelectorType.HasFlag(SelectorType.Tag) &&
                !String.Equals(obj.NodeName, selector.Tag, StringComparison.CurrentCultureIgnoreCase))
            {
                return false;
            }

            
            if ((selector.SelectorType & (SelectorType.AttributeExists | SelectorType.AttributeValue))>0)
            {
                return AttributeSelectors.MatchesAttribute(selector, (IDomElement)obj);
            }

           
            //if (selector.SelectorType.HasFlag(SelectorType.Contains) &&
            //    !ContainsText((IDomElement)obj, selector.Criteria))
            //{
            //    return false;
            //}

            if (selector.SelectorType == SelectorType.None)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Return all position-type matches. These are selectors that are keyed to the position within the selection
        /// set itself.
        /// </summary>
        /// <param name="sourceList"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        protected IEnumerable<IDomObject> GetResultPositionMatches(IEnumerable<IDomObject> list, SelectorClause selector)
        {
            // for sibling traversal types the mapping was done already by the Matches function

            var sourceList = GetAllChildOrDescendants(selector.TraversalType, list);

            switch (selector.PseudoClassType)
            {
                case PseudoClassType.Extension:
                    return ((IPseudoSelectorFilter)selector.PseudoSelector).Filter(sourceList);
                //case PseudoClassType.Odd:
                //    return PseudoSelectors.OddElements(sourceList);
                //case PseudoClassType.Even:
                //    return PseudoSelectors.EvenElements(sourceList);
                //case PseudoClassType.First:
                //    return PseudoSelectors.Enumerate(sourceList.FirstOrDefault());
                //case PseudoClassType.Last:
                //    return PseudoSelectors.Enumerate(sourceList.LastOrDefault());
                //case PseudoClassType.IndexEquals:
                //    return PseudoSelectors.Enumerate(PseudoSelectors.ElementAtIndex(sourceList, selector.PositionIndex));
                //case PseudoClassType.IndexGreaterThan:
                //    return PseudoSelectors.IndexGreaterThan(sourceList, selector.PositionIndex);
                //case PseudoClassType.IndexLessThan:
                //    return PseudoSelectors.IndexLessThan(sourceList, selector.PositionIndex);
                case PseudoClassType.All:
                    return sourceList;
                default:
                    throw new NotImplementedException("Unimplemented result position type selector");

            }
        }

        
        /// <summary>
        /// Return all child elements matching a DOM-position type selector
        /// </summary>
        /// <param name="elm"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        protected IEnumerable<IDomObject> GetPseudoClassMatches(IDomElement elm, SelectorClause selector)
        {
            IEnumerable<IDomObject> results;
            switch (selector.PseudoClassType)
            {
                case PseudoClassType.Extension:
                    results = ((IPseudoSelectorElement)selector.PseudoSelector).ChildMatches(elm);
                    break;
                //case PseudoClassType.Has:
                //    var subSelector = new Selector(selector.Criteria);
                //    results = Has(subSelector, elm.ChildElements);
                //    break;
                //case PseudoClassType.Not:
                //    subSelector = new Selector(selector.Criteria);
                //    results = NotMatches(subSelector, elm.ChildElements);
                //    break;
                //case PseudoClassType.NthChild:
                //case PseudoClassType.NthOfType:
                //    results= PseudoSelectors.NthChilds(elm,selector.Criteria);
                //    break;
                //case PseudoClassType.NthLastChild:
                //case PseudoClassType.NthLastOfType:
                //    results = PseudoSelectors.NthLastChilds(elm, selector.Criteria);
                //    break;
                //case PseudoClassType.FirstOfType:
                //    results=PseudoSelectors.FirstOfType(elm, selector.Criteria);
                //    break;
                //case PseudoClassType.LastOfType:
                //    results=PseudoSelectors.LastOfType(elm, selector.Criteria);
                //    break;
                //case PseudoClassType.FirstChild:
                //    results=PseudoSelectors.Enumerate(PseudoSelectors.FirstChild(elm));
                //    break;
                //case PseudoClassType.LastChild:
                //    results=PseudoSelectors.Enumerate(PseudoSelectors.LastChild(elm));
                //    break;
                //case PseudoClassType.OnlyChild:
                //    results = PseudoSelectors.Enumerate(PseudoSelectors.OnlyChild(elm));
                //    break;
                //case PseudoClassType.OnlyOfType:
                //    results = PseudoSelectors.OnlyOfType(elm, selector.Criteria);
                //    break;
                ////case PseudoClassType.Empty:
                //    results= PseudoSelectors.Empty(elm.ChildElements);
                //    break;
                //case PseudoClassType.Parent:
                //    results = PseudoSelectors.Parent(elm.ChildElements);
                //    break;
                case PseudoClassType.Visible:
                    results = PseudoSelectors.Visible(elm.ChildElements);
                    break;
                case PseudoClassType.Hidden:
                    results = PseudoSelectors.Hidden(elm.ChildElements);
                    break;
                //case PseudoClassType.Header:
                //    results = PseudoSelectors.Headers(elm.ChildElements);
                //    break;
                case PseudoClassType.All:
                    results=elm.ChildElements;
                    break;
                //case PseudoClassType.Extension:
                //    results =PseudoSelectors.Extension(elm, -1, selector.Criteria);
                //    break;
                default:
                    throw new NotImplementedException("Unimplemented position type selector");
            }

            foreach (var item in results)
            {
                yield return item;
            }

            // Traverse children if needed

            if (selector.TraversalType == TraversalType.Descendent || 
                selector.TraversalType == TraversalType.All)
            {
                foreach (var child in elm.ChildElements)
                {
                    foreach (var item in GetPseudoClassMatches(child, selector))
                    {
                        yield return item;
                    }
                }
            }
          
        }    

      
        /// <summary>
        /// Return true if an element matches a specific filter
        /// </summary>
        /// <param name="elm"></param>
        /// <param name="type"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        protected bool MatchesPseudoClass(IDomElement elm, SelectorClause selector)
        {
            switch (selector.PseudoClassType)
            {
                case PseudoClassType.Extension:
                    return ((IPseudoSelectorElement)selector.PseudoSelector).Matches(elm);
                //case PseudoClassType.Has:
                //    var subSelector = new Selector(selector.Criteria);
                //    return Has(subSelector, elm);
                //case PseudoClassType.Not:
                //    subSelector = new Selector(selector.Criteria);
                //    return !subSelector.Matches(Document, elm);
                //case PseudoClassType.NthChild:
                //case PseudoClassType.NthOfType:
                //    return PseudoSelectors.IsNthChild(elm, selector.Criteria);
                //case PseudoClassType.NthLastChild:
                //case PseudoClassType.NthLastOfType:
                //    return PseudoSelectors.IsNthLastChild(elm, selector.Criteria);
                //case PseudoClassType.FirstOfType:
                //    return PseudoSelectors.IsFirstOfType(elm, selector.Criteria);
                //case PseudoClassType.LastOfType:
                //    return PseudoSelectors.IsLastOfType(elm, selector.Criteria);
                //case PseudoClassType.FirstChild:
                //    return elm.PreviousElementSibling == null;
                //case PseudoClassType.LastChild:
                //    return elm.NextElementSibling == null;
                //case PseudoClassType.OnlyChild:
                //    return PseudoSelectors.IsOnlyChild(elm);
                //case PseudoClassType.OnlyOfType:
                //    return PseudoSelectors.IsOnlyOfType(elm);
                //case PseudoClassType.Empty:
                //    return PseudoSelectors.IsEmpty(elm);
                //case PseudoClassType.Parent:
                //    return PseudoSelectors.IsParent(elm);
                case PseudoClassType.Visible:
                    return PseudoSelectors.IsVisible(elm);
                case PseudoClassType.Hidden:
                    return !PseudoSelectors.IsVisible(elm);
                //case PseudoClassType.Header:
                //    return PseudoSelectors.IsHeader(elm);
                case PseudoClassType.All:
                    return true;
                //case PseudoClassType.Extension:
                //    return PseudoSelectors.IsExtension(elm, -1, clause.Criteria);

                default:
                    throw new NotImplementedException("Unimplemented position type selector");
            }
        }

 
        #endregion

        #region private methods

        /// <summary>
        /// Map a list to its siblings or adjacent elements if needed. Ignore other traversal types.
        /// </summary>
        /// <param name="traversalType"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        protected IEnumerable<IDomObject> GetAdjacentOrSiblings(TraversalType traversalType, IEnumerable<IDomObject> list)
        {
            IEnumerable<IDomObject> sourceList;
            switch (traversalType)
            {
                case TraversalType.Adjacent:
                    sourceList = GetAdjacentElements(list);
                    break;
                case TraversalType.Sibling:
                    sourceList = GetSiblings(list);
                    break;
                default:
                    sourceList = list;
                    break;
            }
            return sourceList;
        }

        protected IEnumerable<IDomObject> GetAllElements(IEnumerable<IDomObject> list)
        {
            foreach (var item in list)
            {
                yield return item;
                foreach (var descendant in GetDescendantElements(item))
                {
                    yield return descendant;
                }
            }
        }

        /// <summary>
        /// Map a list to its children or descendants, if needed.
        /// </summary>
        /// <param name="traversalType"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        protected IEnumerable<IDomObject> GetAllChildOrDescendants(TraversalType traversalType, IEnumerable<IDomObject> list)
        {
            IEnumerable<IDomObject> sourceList;
            switch (traversalType)
            {
                case TraversalType.All:
                    sourceList = GetAllElements(list);
                    break;
                case TraversalType.Child:
                    sourceList = GetChildElements(list);
                    break;
                case TraversalType.Descendent:
                    sourceList = GetDescendantElements(list);
                    break;
                default:
                    sourceList = list;
                    break;
            }
            return sourceList;
        }


        protected IEnumerable<IDomObject> GetTraversalTargetElements(TraversalType traversalType, IEnumerable<IDomObject> list)
        {
            IEnumerable<IDomObject> sourceList;
            switch (traversalType)
            {
                case TraversalType.Filter:
                    sourceList = list;
                    break;
                case TraversalType.Child:

                    sourceList = GetChildElements(list);
                    break;
                case TraversalType.Adjacent:
                    sourceList = GetAdjacentElements(list);
                    break;
                case TraversalType.Sibling:
                    sourceList = GetSiblings(list);
                    break;
                case TraversalType.Descendent:
                    throw new InvalidOperationException("TraversalType.Descendant should not be found at this point.");
                case TraversalType.All:
                    throw new InvalidOperationException("TraversalType.All should not be found at this point.");
                default:
                    throw new NotImplementedException("Unimplemented traversal type.");
            }
            return sourceList;
        }

        /// <summary>
        /// Return all children of each element in the list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        protected IEnumerable<IDomElement> GetChildElements(IEnumerable<IDomObject> list)
        {
            foreach (var item in list)
            {
                foreach (var child in item.ChildElements)
                {
                    yield return child;
                }
            }
        }

        /// <summary>
        /// Return all descendants of each element in the list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<IDomElement> GetDescendantElements(IEnumerable<IDomObject> list)
        {
            foreach (var item in list)
            {
                foreach (var child in GetDescendantElements(item))
                {
                    yield return child;
                }
            }


        }

        public static IEnumerable<IDomElement> GetDescendantElements(IDomObject element)
        {
            foreach (var child in element.ChildElements)
            {
                yield return child;
                foreach (var grandChild in GetDescendantElements(child))
                {
                    yield return grandChild;
                }
            }
            
        }
        protected IEnumerable<IDomElement> GetAdjacentElements(IEnumerable<IDomObject> list)
        {
            return CQ.Map(list, item =>
            {
                return item.NextElementSibling;
            });
        }

        protected IEnumerable<IDomElement> GetSiblings(IEnumerable<IDomObject> list)
        {
            foreach (var item in list)
            {

                IDomContainer parent = item.ParentNode;
                int index = item.Index + 1;
                int length = parent.ChildNodes.Count;

                while (index < length)
                {
                    IDomElement node = parent.ChildNodes[index] as IDomElement;
                    if (node != null)
                    {
                        yield return node;
                    }
                    index++;
                }
            }
        }
        
        /// <summary>
        /// Return true if any text node descendant of the source element contains the specified text
        /// </summary>
        /// <param name="source"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        protected bool ContainsText(IDomElement source, string text)
        {
            foreach (IDomObject e in source.ChildNodes)
            {
                if (e.NodeType == NodeType.TEXT_NODE)
                {
                    if (((IDomText)e).NodeValue.IndexOf(text) >= 0)
                    {
                        return true;
                    }
                }
                else if (e.NodeType == NodeType.ELEMENT_NODE)
                {
                    if (ContainsText((IDomElement)e, text))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Adds a new selector for just the attribute value. Used to chain with the indexed attribute exists selector.
        /// </summary>
        /// <param name="selector"></param>
        //protected void InsertAttributeValueSelector(SelectorClause fromSelector)
        //{
        //    SelectorClause newSel = new SelectorClause();
        //    newSel.TraversalType = TraversalType.Filter;
        //    newSel.SelectorType = SelectorType.Attribute;
        //    newSel.AttributeName = fromSelector.AttributeName;
        //    newSel.AttributeValue = fromSelector.AttributeValue;
        //    newSel.AttributeSelectorType = fromSelector.AttributeSelectorType;
        //    newSel.CombinatorType = CombinatorType.Chained;
        //    newSel.NoIndex = true;
        //    int insertAt = activeSelectorId + 1;
        //    if (insertAt >= ActiveSelectors.Count)
        //    {
        //        ActiveSelectors.Add(newSel);
        //    }
        //    else
        //    {
        //        ActiveSelectors.Insert(insertAt, newSel);
        //    }
        //}


        #endregion
    }
}
