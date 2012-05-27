using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.Utility;

namespace CsQuery.Engine
{
    public class CssSelectionEngine
    {
        #region private properties
        private Lazy<NthChild> _NthChildMatcher = new Lazy<NthChild>();

        protected IDomDocument Document;
        protected List<Selector> ActiveSelectors;
        protected int activeSelectorId;

        protected NthChild NthChildMatcher
        {
            get
            {
                return _NthChildMatcher.Value;
            }
        }
        
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
                            selectionSource = lastResult;
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
                        key = "!" + (char)DomData.TokenID(selector.AttributeName);
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
                // TODO - GetMatch should work if passed with no selectors (returning nothing), now it returns everything
                // 12/10/11 - this todo is not verified, much has changed since it was written. TODO confirm this and
                // fix if needed. If having the conversation with self again, remove comments and forget it. This is
                // an example of why comments can do more harm than good.

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
            if (selector.SelectorType.HasFlag(SelectorType.Position) && selector.IsResultListPosition)
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
                        if (selector.TraversalType == TraversalType.Child
                            && selector.ChildDepth == current.Depth + 1
                            && selector.IsDomIndexPosition)
                        {
                            temporaryResults.AddRange(GetDomPositionMatches(elm, selector));
                            selectorType &= ~SelectorType.Position;
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
            bool match = true;

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
                string value;
                match = elm.TryGetAttribute(selector.AttributeName, out value);
                if (!match ||
                    (match && selector.AttributeSelectorType.IsOneOf(AttributeSelectorType.NotExists, AttributeSelectorType.NotEquals)))
                {
                    return false;
                }

                switch (selector.AttributeSelectorType)
                {
                    case AttributeSelectorType.Exists:
                        break;
                    case AttributeSelectorType.Equals:
                        match = selector.AttributeValue == value;
                        break;
                    case AttributeSelectorType.StartsWith:
                        match = value.Length >= selector.AttributeValue.Length &&
                            value.Substring(0, selector.AttributeValue.Length) == selector.AttributeValue;
                        break;
                    case AttributeSelectorType.Contains:
                        match = value.IndexOf(selector.AttributeValue) >= 0;
                        break;
                    case AttributeSelectorType.ContainsWord:
                        match = ContainsWord(value, selector.AttributeValue);
                        break;
                    case AttributeSelectorType.NotEquals:
                        match = value.IndexOf(selector.AttributeValue) == 0;
                        break;
                    case AttributeSelectorType.EndsWith:
                        int len = selector.AttributeValue.Length;
                        match = value.Length >= len &&
                            value.Substring(value.Length - len) == selector.AttributeValue;
                        break;
                    default:
                        throw new InvalidOperationException("No AttributeSelectorType set");
                }
                if (!match)
                {
                    return false;
                }
            }

            if (selector.SelectorType.HasFlag(SelectorType.Other))
            {
                return IsVisible(elm);
            }

            if (selector.SelectorType.HasFlag(SelectorType.Position) &&
                selector.TraversalType == TraversalType.Filter && 
                !MatchesDOMPosition(elm, selector.PositionType,
                selector.PositionType == PositionType.NthChild ? selector.Criteria : null))
            {
                return false;
            }
            // remove this so it doesn't get re-run
            // selector.SelectorType &= ~SelectorType.Position;

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
            switch (selector.PositionType)
            {
                case PositionType.First:
                    IDomObject first = list.FirstOrDefault();
                    if (first != null)
                    {
                        yield return first;
                    }
                    break;
                case PositionType.Last:
                    IDomObject last = list.LastOrDefault();
                    if (last != null)
                    {
                        yield return last;
                    }
                    break;
                case PositionType.IndexEquals:
                    int critIndex = selector.PositionIndex;
                    if (critIndex < 0)
                    {
                        critIndex = list.Count() + critIndex;
                    }
                    bool ok = true;
                    IEnumerator<IDomObject> enumerator = list.GetEnumerator();
                    for (int i = 0; i <= critIndex && ok; i++)
                    {
                        ok = enumerator.MoveNext();
                    }
                    if (ok)
                    {
                        yield return enumerator.Current;
                    }
                    else
                    {
                        yield break;
                    }
                    break;
                case PositionType.IndexGreaterThan:
                    int index = 0;
                    foreach (IDomObject obj in list)
                    {
                        if (index++ > selector.PositionIndex)
                        {
                            yield return obj;
                        }
                    }
                    break;
                case PositionType.IndexLessThan:
                    int indexLess = 0;
                    foreach (IDomObject obj in list)
                    {
                        if (indexLess++ < selector.PositionIndex)
                        {
                            yield return obj;
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
            }
            yield break;
        }
        /// <summary>
        /// Determine if an element matches a position-type filter
        /// </summary>
        /// <param name="elm"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        protected IEnumerable<IDomObject> GetDomPositionMatches(IDomElement elm, Selector selector)
        {
            if (selector.PositionType == PositionType.NthChild)
            {
                return NthChildMatcher.GetMatchingChildren(elm,selector.Criteria);
            }
            else
            {
                return GetSimpleDomPostionMatches(elm,selector.PositionType);
            }
        }     
        /// <summary>
        /// Return DOM position matches (other than Nth Child)
        /// </summary>
        /// <param name="elm"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        protected IEnumerable<IDomObject> GetSimpleDomPostionMatches(IDomContainer elm, PositionType position)
        {
            if (position == PositionType.FirstChild)
            {
                IDomObject child = elm.FirstChild;
                if (child.NodeType != NodeType.ELEMENT_NODE)
                {
                    child = child.NextElementSibling;
                }
                if (child != null)
                {
                    yield return child;
                }
            }
            else if (position == PositionType.LastChild)
            {
                IDomObject child = elm.LastChild;
                if (child.NodeType != NodeType.ELEMENT_NODE)
                {
                    child = child.PreviousElementSibling;
                }
                if (child != null)
                {
                    yield return child;
                }
            }
            else
            {
                int index = 0;

                foreach (var child in elm.ChildNodes)
                {
                    switch (position)
                    {
                        case PositionType.Odd:
                            if (index % 2 != 0)
                            {
                                yield return child;
                            }
                            break;
                        case PositionType.Even:
                            if (index % 2 == 0)
                            {
                                yield return child;
                            }
                            break;
                        case PositionType.All:
                            yield return child;
                            break;
                        default:
                            throw new NotImplementedException("Unimplemented position type selector");
                    }
                }
            }

        }
        /// <summary>
        /// Return true if an element matches a specific DOM position-type filter
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="position"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        protected bool MatchesDOMPosition(IDomElement obj, PositionType position, string criteria)
        {
            switch (position)
            {
                case PositionType.FirstChild:
                    return obj.PreviousElementSibling == null;
                case PositionType.LastChild:
                    return obj.NextElementSibling == null;
                case PositionType.Odd:
                    return obj.ElementIndex % 2 != 0;
                case PositionType.Even:
                    return obj.ElementIndex % 2 == 0;
                case PositionType.All:
                    return true;
                case PositionType.NthChild:
                    return NthChildMatcher.IndexMatches(obj.ElementIndex, criteria);
                default:
                    throw new NotImplementedException("Unimplemented position type selector");
            }
        }

        /// <summary>
        /// Tests visibility by inspecting "display", "height" and "width" css & properties for object & all parents.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected bool IsVisible(IDomElement obj)
        {
            IDomObject el = obj;
            while (el != null && el.NodeType == NodeType.ELEMENT_NODE)
            {
                if (ElementIsItselfHidden((IDomElement)el))
                {
                    return false;
                }
                el = el.ParentNode;
            }
            return true;
        }
        protected bool ElementIsItselfHidden(IDomElement el)
        {
            if (el.HasStyles)
            {
                if (el.Style["display"] == "none")
                {
                    return true;
                }
                double? wid = el.Style.NumberPart("width");
                double? height = el.Style.NumberPart("height");
                if (wid == 0 || height == 0)
                {
                    return true;
                }
            }
            string widthAttr,heightAttr;
            widthAttr = el.GetAttribute("width");
            heightAttr = el.GetAttribute("height");

            return widthAttr=="0" || heightAttr=="0"; 

        }
        #endregion

        #region private methods
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

        protected bool ContainsWord(string text, string word)
        {
            HashSet<string> words = new HashSet<string>(word.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            return words.Contains(text);
        }
        protected bool ContainsText(IDomElement obj, string text)
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

        #endregion
    }
}
