using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.Utility;

namespace CsQuery.Engine
{
    public class CssSelectionEngine
    {
        #region private properties


        protected IDomDocument Document;
        protected List<Selector> ActiveSelectors;
        protected int activeSelectorId;

        #endregion

        #region public properties
        /// <summary>
        /// The current selection list being acted on
        /// </summary>

        public SelectorChain Selectors { get; set; }
        #endregion

        #region public methods
        /// <summary>
        /// Select from DOM using index. First non-class/tag/id selector will result in this being passed off to GetMatches
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public IEnumerable<IDomObject> Select(IDomDocument document, IEnumerable<IDomObject> context)
        {
            if (Selectors == null )
            {
                throw new ArgumentException("No selectors provided.");
            }
            if (Selectors.Count == 0)
            {
                yield break;
            }
            Document = document;
            IEnumerable<IDomObject> lastResult = null;
            HashSet<IDomObject> output = new HashSet<IDomObject>();
            IEnumerable<IDomObject> selectionSource = context;

            // Disable the index if there is no context (e.g. disconnected elements)
            bool useIndex = context.IsNullOrEmpty() || !context.First().IsDisconnected;

            // Copy the list because it may change during the process
            ActiveSelectors = new List<Selector>(Selectors);

            // Deal with filter/all selectors: if the first selector is a pseudoclass, & there's no context,
            // then convert it to a filter, and add "all" selector first

            var firstSelector = ActiveSelectors[0];
            if (firstSelector.SelectorType == SelectorType.PseudoClass && 
                firstSelector.TraversalType == TraversalType.All && 
                context==null)
            {
                var selector = new Selector();
                selector.SelectorType = SelectorType.All;
                selector.TraversalType = TraversalType.All;
                selector.CombinatorType = CombinatorType.Root;
                ActiveSelectors.Insert(0, selector);

                firstSelector.TraversalType = TraversalType.Filter;
                firstSelector.CombinatorType = CombinatorType.Chained;
            }

            for (activeSelectorId = 0; activeSelectorId < ActiveSelectors.Count; activeSelectorId++)
            {
                var selector = ActiveSelectors[activeSelectorId];
                CombinatorType combinatorType = selector.CombinatorType;
                SelectorType selectorType = selector.SelectorType;
                TraversalType traversalType = selector.TraversalType;

                // Determine what kind of combining method we will use with previous selection results

                if (activeSelectorId != 0)
                {
                    switch (combinatorType)
                    {
                        case CombinatorType.Cumulative:
                            // do nothing
                            break;
                        case CombinatorType.Root:
                            selectionSource = context;
                            if (lastResult != null)
                            {
                                output.AddRange(lastResult);
                                lastResult = null;
                            }
                            break;
                        case CombinatorType.Chained:

                            // If the selector used the adjacent combinator, grab the next element for each
                            if (lastResult!=null) {
                                switch (traversalType)
                                {
                                    case TraversalType.Adjacent:
                                        selectionSource = CQ.Map(lastResult, item =>
                                        {
                                            return item.NextElementSibling;
                                        });
                                        break;
                                    case TraversalType.Sibling:
                                        selectionSource = GetSiblings(lastResult);
                                        break;
                                    default:
                                        selectionSource = lastResult;
                                        break;
                                }
                            }
                            else
                            {
                                selectionSource =  null;
                            }

                            lastResult = null;
                            break;
                        // default (chained): leave lastresult alone
                    }
                }

                HashSet<IDomObject> tempResult = null;
                IEnumerable<IDomObject> interimResult = null;

                string key = "";
                if (useIndex && !selector.NoIndex)
                {

#if DEBUG_PATH

                    if (type.HasFlag(SelectorType.Attribute))
                    {
                        key = "!" + selector.AttributeName;
                        type &= ~SelectorType.Attribute;
                        if (selector.AttributeValue != null)
                        {
                            InsertAttributeValueSelector(selector);
                        }
                    } 
                    else if (type.HasFlag(SelectorType.Tag))
                    {
                        key = "+"+selector.Tag;
                        type &= ~SelectorType.Tag;
                    }
                    else if (type.HasFlag(SelectorType.ID))
                    {
                        key = "#" + selector.ID;
                        type &= ~SelectorType.ID;
                    }
                    else if (type.HasFlag(SelectorType.Class))
                    {
                        key = "." + selector.Class;
                        type &= ~SelectorType.Class;
                    }

#else
                    if (selectorType.HasFlag(SelectorType.Attribute))
                    {
                        key = "!" + (char)DomData.TokenID(selector.AttributeName,true);
                        selectorType &= ~SelectorType.Attribute;
                        if (selector.AttributeValue != null)
                        {
                            InsertAttributeValueSelector(selector);
                        }
                    }
                    else if (selectorType.HasFlag(SelectorType.Tag))
                    {
                        key = "+" + (char)DomData.TokenID(selector.Tag, true);
                        selectorType &= ~SelectorType.Tag;
                    }
                    else if (selectorType.HasFlag(SelectorType.ID))
                    {
                        key = "#" + (char)DomData.TokenID(selector.ID);
                        selectorType &= ~SelectorType.ID;
                    }
                    else if (selectorType.HasFlag(SelectorType.Class))
                    {
                        key = "." + (char)DomData.TokenID(selector.Class);
                        selectorType &= ~SelectorType.Class;
                    }
#endif
                }

                // If part of the selector was indexed, key will not be empty. Return initial set from the
                // index. If any selectors remain after this they will be searched the hard way.
                

                if (key != String.Empty)
                {
                    int depth = 0;
                    bool descendants = true;

                    switch (traversalType)
                    {
                        case TraversalType.Child:
                            depth = selector.ChildDepth; ;
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

                    // This is the main index access point 

                    if (selectionSource == null)
                    {
                        interimResult = document.QueryIndex(key + DomData.indexSeparator, depth, descendants);
                    }
                    else
                    {
                        interimResult = new HashSet<IDomObject>();
                        foreach (IDomObject obj in selectionSource)
                        {
                            ((HashSet<IDomObject>)interimResult)
                                .AddRange(document.QueryIndex(key + DomData.indexSeparator + obj.Path,
                                    depth, descendants));
                        }
                    }
                }
                else if (selectorType.HasFlag(SelectorType.Elements))
                {
                    selectorType &= ~SelectorType.Elements;
                    HashSet<IDomObject> source = new HashSet<IDomObject>(selectionSource);
                    interimResult = new HashSet<IDomObject>();

                    foreach (IDomObject obj in selectionSource)
                    {
                        key = DomData.indexSeparator + obj.Path;
                        HashSet<IDomObject> srcKeys = new HashSet<IDomObject>(document.QueryIndex(key));
                        foreach (IDomObject match in selector.SelectElements)
                        {
                            if (srcKeys.Contains(match))
                            {
                                ((HashSet<IDomObject>)interimResult).Add(match);
                            }
                        }
                    }
                }

                

                if ((selectorType & ~(SelectorType.SubSelectorNot | SelectorType.SubSelectorHas)) != 0)
                {
                    IEnumerable<IDomObject> finalSelectWithin = 
                        interimResult
                        ?? (combinatorType == CombinatorType.Chained ? lastResult : null) 
                        ?? selectionSource
                        ?? document.ChildElements;

                    // if there are no temporary results (b/c there was no indexed selector) then use the whole set
                    interimResult = GetMatches(finalSelectWithin, selector);

                }

                // Deal with subselectors: has() and not() test for the presence of a selector within the children of
                // an element. This is essentially similar to the manual selection above.

                if (selectorType.HasFlag(SelectorType.SubSelectorHas) 
                    || selectorType.HasFlag(SelectorType.SubSelectorNot))
                {
                    bool isHasSelector = selectorType.HasFlag(SelectorType.SubSelectorHas);

                    IEnumerable<IDomObject> subSelectWithin = interimResult
                        ?? (combinatorType == CombinatorType.Chained ? lastResult : null)
                        ?? selectionSource;

                    // subselects are a filter. start a new interim result.

                    HashSet<IDomObject> filteredResults = new HashSet<IDomObject>();

                    foreach (IDomObject obj in subSelectWithin)
                    {
                        bool match = true;
                        foreach (var sub in selector.SubSelectors)
                        {
                            List<IDomObject> listOfOne = new List<IDomObject>();
                            listOfOne.Add(obj);

                            bool has = !sub.Select(document, listOfOne).IsNullOrEmpty();

                            match &= isHasSelector == has;
                        }
                        if (match)
                        {
                            filteredResults.Add(obj);
                        }
                    }
                    interimResult = filteredResults;
                }


                tempResult = new HashSet<IDomObject>();
                if (lastResult != null)
                {
                    tempResult.AddRange(lastResult);
                }
                if (interimResult != null)
                {
                    tempResult.AddRange(interimResult);
                }
                lastResult = tempResult;
            }


            if (lastResult != null)
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
        /// Return all elements matching a selector, within a domain baseList, starting from list.
        /// </summary>
        /// <param name="baseList"></param>
        /// <param name="list"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        protected IEnumerable<IDomObject> GetMatches(IEnumerable<IDomObject> list, Selector selector)
        {
            // Maintain a hashset of every element already searched. Since result sets frequently contain items which are
            // children of other items in the list, we would end up searching the tree repeatedly
            HashSet<IDomObject> uniqueElements = null;

            Stack<MatchElement> stack = null;
            IEnumerable<IDomObject> curList = list;
            HashSet<IDomObject> temporaryResults = new HashSet<IDomObject>();

            // The unique list has to be reset for each sub-selector
            uniqueElements = new HashSet<IDomObject>();

            if (selector.SelectorType == SelectorType.HTML)
            {
                HtmlParser.DomElementFactory factory = new HtmlParser.DomElementFactory(Document);

                foreach (var obj in factory.CreateObjects(selector.Html))
                {
                    yield return obj;
                }
                yield break;
            }

            // Result-list position selectors are simple -- skip out of main matching code if so
            if (selector.SelectorType.HasFlag(SelectorType.PseudoClass) && selector.IsResultListPosition)
            {
                foreach (var obj in GetResultPositionMatches(curList, selector))
                {
                    yield return obj;
                }
                yield break;
            }
            // Otherwise, try to match each element individually
            stack = new Stack<MatchElement>();

            foreach (var e in curList)
            {
                // We must check everything again when looking for specific depth of children
                // otherwise - no point - skip em

                if (selector.TraversalType != TraversalType.Child && uniqueElements.Contains(e))
                {
                    continue;
                }
                stack.Push(new MatchElement(e, 0));
                int matchIndex = 0;
                while (stack.Count != 0)
                {
                    var current = stack.Pop();

                    if (Matches(selector, current.Object, current.Depth))
                    {
                        temporaryResults.Add(current.Object);
                        matchIndex++;
                    }
                    // Add children to stack (in reverse order, so they are processed in the correct order when popped)

                    // Don't keep going to children if the target depth is < the depth. Though the match would still fail,
                    // stuff would end up the unique list which we might need to test later if it appears directly in the source list
                    // causing it to be ignored.

                    if (selector.TraversalType != TraversalType.Filter &&
                        current.Object is IDomElement &&
                        (selector.TraversalType != TraversalType.Child || selector.ChildDepth > current.Depth))
                    {
                        SelectorType selectorType = selector.SelectorType;
                        IDomElement elm = current.Element;

                        if (selector.IsDomIndexPosition &&
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
                            IDomObject obj = elm[j];
                            if (selector.TraversalType == TraversalType.Child && !uniqueElements.Add(obj))
                            {
                                continue;
                            }
                            if (obj.NodeType == NodeType.ELEMENT_NODE)
                            {
                                stack.Push(new MatchElement(obj, current.Depth + 1));
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
        /// Test 
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="obj"></param>
        /// <param name="matchIndex"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        protected bool Matches(Selector selector, IDomObject obj, int depth)
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
            if (!(obj is IDomElement))
            {
                return false;
            }
            IDomElement elm = (IDomElement)obj;
            // Check each selector from easier/more specific to harder. e.g. ID is going to eliminate a lot of things.

            if (selector.SelectorType.HasFlag(SelectorType.ID) &&
                selector.ID != elm.Id)
            {
                return false;
            }
            if (selector.SelectorType.HasFlag(SelectorType.Class) &&
                !elm.HasClass(selector.Class))
            {
                return false;
            }
            if (selector.SelectorType.HasFlag(SelectorType.Tag) &&
                !String.Equals(elm.NodeName, selector.Tag, StringComparison.CurrentCultureIgnoreCase))
            {
                return false;
            }

            
            if (selector.SelectorType.HasFlag(SelectorType.Attribute))
            {
                return AttributeSelectors.MatchesAttribute(selector, elm);
            }

            if (selector.SelectorType.HasFlag(SelectorType.PseudoClass)) {
                return selector.TraversalType == TraversalType.Filter && 
                    MatchesPseudoClass(elm, selector.PseudoClassType,
                    selector.Criteria);
            }

            if (selector.SelectorType.HasFlag(SelectorType.Contains) &&
                !ContainsText(elm, selector.Criteria))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Return all position-type matches. These are selectors that are keyed to the position within the selection
        /// set itself.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        protected IEnumerable<IDomObject> GetResultPositionMatches(IEnumerable<IDomObject> list, Selector selector)
        {
            switch (selector.PseudoClassType)
            {
                case PseudoClassType.Odd:
                    return PseudoSelectors.OddElements(list);
                case PseudoClassType.Even:
                    return PseudoSelectors.EvenElements(list);
                case PseudoClassType.First:
                    return PseudoSelectors.Enumerate(list.FirstOrDefault());
                case PseudoClassType.Last:
                    return PseudoSelectors.Enumerate(list.LastOrDefault());
                case PseudoClassType.IndexEquals:
                    return PseudoSelectors.Enumerate(PseudoSelectors.ElementAtIndex(list, selector.PositionIndex));
                case PseudoClassType.IndexGreaterThan:
                    return PseudoSelectors.IndexGreaterThan(list, selector.PositionIndex);
                case PseudoClassType.IndexLessThan:
                    return PseudoSelectors.IndexLessThan(list, selector.PositionIndex);
                case PseudoClassType.All:
                    return list;
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
        protected IEnumerable<IDomObject> GetPseudoClassMatches(IDomElement elm, Selector selector)
        {
            IEnumerable<IDomObject> results;
            switch (selector.PseudoClassType)
            {
                case PseudoClassType.NthChild:
                    results= PseudoSelectors.NthChilds(elm,selector.Criteria);
                    break;
                case PseudoClassType.NthOfType:
                    results= PseudoSelectors.NthChildsOfType(elm,selector.Criteria);
                    break;
                case PseudoClassType.FirstOfType:
                    results=PseudoSelectors.FirstOfType(elm, selector.Criteria);
                    break;
                case PseudoClassType.LastOfType:
                    results=PseudoSelectors.LastOfType(elm, selector.Criteria);
                    break;
                case PseudoClassType.FirstChild:
                    results=PseudoSelectors.Enumerate(PseudoSelectors.FirstChild(elm));
                    break;
                case PseudoClassType.LastChild:
                    results=PseudoSelectors.Enumerate(PseudoSelectors.LastChild(elm));
                    break;
                case PseudoClassType.OnlyChild:
                    results = PseudoSelectors.Enumerate(PseudoSelectors.OnlyChild(elm));
                    break;
                case PseudoClassType.OnlyOfType:
                    results = PseudoSelectors.OnlyOfType(elm, selector.Criteria);
                    break;
                case PseudoClassType.Empty:
                    results= PseudoSelectors.Empty(elm.ChildElements);
                    break;
                case PseudoClassType.Parent:
                    results = PseudoSelectors.Parent(elm.ChildElements);
                    break;
                case PseudoClassType.Visible:
                    results = PseudoSelectors.Visible(elm.ChildElements);
                    break;
                case PseudoClassType.Hidden:
                    results = PseudoSelectors.Hidden(elm.ChildElements);
                    break;
                case PseudoClassType.Header:
                    results = PseudoSelectors.Headers(elm.ChildElements);
                    break;
                case PseudoClassType.All:
                    results=elm.ChildElements;
                    break;
                default:
                    throw new NotImplementedException("Unimplemented position type selector");
            }

            foreach (var item in results)
            {
                yield return item;
            }

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
        /// Return true if an element matches a specific DOM position-type filter
        /// </summary>
        /// <param name="elm"></param>
        /// <param name="type"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        protected bool MatchesPseudoClass(IDomElement elm, PseudoClassType type, string criteria)
        {
            switch (type)
            {
                case PseudoClassType.FirstOfType:
                    return PseudoSelectors.IsFirstOfType(elm, criteria);
                case PseudoClassType.LastOfType:
                    return PseudoSelectors.IsLastOfType(elm, criteria);
                case PseudoClassType.FirstChild:
                    return elm.PreviousElementSibling == null;
                case PseudoClassType.LastChild:
                    return elm.NextElementSibling == null;
                case PseudoClassType.NthChild:
                    return PseudoSelectors.IsNthChild(elm, criteria);
                case PseudoClassType.NthOfType:
                    return PseudoSelectors.IsNthChildOfType(elm, criteria);
                case PseudoClassType.OnlyChild:
                    return PseudoSelectors.IsOnlyChild(elm);
                case PseudoClassType.OnlyOfType:
                    return PseudoSelectors.IsOnlyOfType(elm);
                case PseudoClassType.Empty:
                    return PseudoSelectors.IsEmpty(elm);
                case PseudoClassType.Parent:
                    return PseudoSelectors.IsParent(elm);
                case PseudoClassType.Visible:
                    return PseudoSelectors.IsVisible(elm);
                case PseudoClassType.Hidden:
                    return !PseudoSelectors.IsVisible(elm);
                case PseudoClassType.Header:
                    return PseudoSelectors.IsHeader(elm);
                case PseudoClassType.All:
                    return true;
                default:
                    throw new NotImplementedException("Unimplemented position type selector");
            }
        }

 
        #endregion

        #region private methods

        private bool ContainsText(IDomElement obj, string text)
        {
            foreach (IDomObject e in obj.ChildNodes)
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
        protected void InsertAttributeValueSelector(Selector fromSelector)
        {
            Selector newSel = new Selector();
            newSel.TraversalType = TraversalType.Filter;
            newSel.SelectorType = SelectorType.Attribute;
            newSel.AttributeName = fromSelector.AttributeName;
            newSel.AttributeValue = fromSelector.AttributeValue;
            newSel.AttributeSelectorType = fromSelector.AttributeSelectorType;
            newSel.CombinatorType = CombinatorType.Chained;
            newSel.NoIndex = true;
            int insertAt = activeSelectorId + 1;
            if (insertAt >= ActiveSelectors.Count)
            {
                ActiveSelectors.Add(newSel);
            }
            else
            {
                ActiveSelectors.Insert(insertAt, newSel);
            }
        }


        #endregion
    }
}
