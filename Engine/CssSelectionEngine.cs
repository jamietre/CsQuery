using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jtc.CsQuery.ExtensionMethods;

namespace Jtc.CsQuery.Engine
{
    public class CssSelectionEngine
    {
        protected NthChild NthChildMatcher
        {
            get
            {
                return _NthChildMatcher.Value;
            }
        }
        private Lazy<NthChild> _NthChildMatcher = new Lazy<NthChild>();

        protected IDomRoot Document { get; set; }

        /// <summary>
        /// The current selection list being acted on
        /// </summary>
        protected List<CsQuerySelector> ActiveSelectors;
        protected int activeSelectorId;
        
        public CsQuerySelectors Selectors { get; set; }
        
        /// <summary>
        /// Select from DOM using index. First non-class/tag/id selector will result in this being passed off to GetMatches
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public IEnumerable<IDomObject> Select(IDomRoot document, IEnumerable<IDomObject> context)
        {
            if (Selectors == null )
            {
                throw new Exception("No selectors provided.");
            }
            if (Selectors.Count == 0)
            {
                yield break;
            }
            Document = document;
            IEnumerable<IDomObject> lastResult = null;
            HashSet<IDomObject> output = new HashSet<IDomObject>();
            IEnumerable<IDomObject> selectionSource = context;
            // This is a bit of a hack, but ensures that fragments get selected manually. We need a way to have a separate index still bound to the 
            // main DOM ideally so fragments can have indexes
            bool useIndex = context.IsNullOrEmpty() || !context.First().IsDisconnected;

            // Copy the list because it may change during the process
            ActiveSelectors = new List<CsQuerySelector>(Selectors);

            for (activeSelectorId = 0; activeSelectorId < ActiveSelectors.Count; activeSelectorId++)
            {
                var selector = ActiveSelectors[activeSelectorId];
                SelectorType type = selector.SelectorType;

                // Determine what kind of combining method we will use with previous selection results

                if (activeSelectorId != 0)
                {
                    switch (selector.CombinatorType)
                    {
                        case CombinatorType.Cumulative:
                            //
                            //selectionSource = lastChained;
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
                    if (type.HasFlag(SelectorType.Attribute))
                    {
                        key = "!" + (char)DomData.TokenID(selector.AttributeName);
                        type &= ~SelectorType.Attribute;
                        if (selector.AttributeValue != null)
                        {
                            InsertAttributeValueSelector(selector);
                        }
                    }
                    else if (type.HasFlag(SelectorType.Tag))
                    {
                        key = "+" + (char)DomData.TokenID(selector.Tag, true);
                        type &= ~SelectorType.Tag;
                    }
                    else if (type.HasFlag(SelectorType.ID))
                    {
                        key = "#" + (char)DomData.TokenID(selector.ID);
                        type &= ~SelectorType.ID;
                    }
                    else if (type.HasFlag(SelectorType.Class))
                    {
                        key = "." + (char)DomData.TokenID(selector.Class);
                        type &= ~SelectorType.Class;
                    }
#endif
                }
                // If we can use an indexed selector, do it here
                if (key != String.Empty)
                {
                    int depth = 0;
                    bool descendants = true;

                    switch (selector.TraversalType)
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
                else if (type.HasFlag(SelectorType.Elements))
                {
                    type &= ~SelectorType.Elements;
                    HashSet<IDomObject> source = new HashSet<IDomObject>(selectionSource);
                    //source.IntersectWith(selectionSource);
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
                // TODO - GetMatch should work if passed with no selectors (returning nothing), now it returns eveyrthing
                if ((type & ~(SelectorType.SubSelectorNot | SelectorType.SubSelectorHas)) != 0)
                {
                    IEnumerable<IDomObject> finalSelectWithin = interimResult
                        ?? (selector.CombinatorType == CombinatorType.Chained ? lastResult : null)
                        ?? selectionSource;

                    // if there are no temporary results (b/c there was no indexed selector) then use the whole set
                    interimResult = GetMatch(document.ChildElements, finalSelectWithin, selector);

                }
                // there must be a better way to do this! Need to adjust the selector engine logic to be able to return something other than what it's looking it.

                if (type.HasFlag(SelectorType.SubSelectorHas) || type.HasFlag(SelectorType.SubSelectorNot))
                {
                    bool isHasSelector = type.HasFlag(SelectorType.SubSelectorHas);

                    IEnumerable<IDomObject> subSelectWithin = interimResult
                        ?? (selector.CombinatorType == CombinatorType.Chained ? lastResult : null)
                        ?? selectionSource;


                    //IEnumerable<IDomObject> subSelectWithin = interimResult ?? lastResult ?? selectionSource;
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
                foreach (IDomObject item in output.OrderBy(item => item.Path, StringComparer.Ordinal))
                {
                    yield return item;
                }
            }
            ActiveSelectors.Clear();
        }
        /// <summary>
        /// Adds a new selector for just the attribute value. Used to chain with the indexed attribute exists selector.
        /// </summary>
        /// <param name="selector"></param>
        protected void InsertAttributeValueSelector(CsQuerySelector fromSelector)
        {
            CsQuerySelector newSel = new CsQuerySelector();
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
        /// <summary>
        /// Parse out the contents of a function 
        /// </summary>
        /// <param name="sel"></param>
        /// <returns></returns>
        protected string ParseFunction(ref string sel)
        {
            int subPos = sel.IndexOfAny(new char[] { '(' });
            if (subPos < 0)
            {
                throw new Exception("Bad 'contains' selector.");
            }
            subPos++;
            int pos = subPos;
            int startPos = -1;
            int endPos = -1;
            int step = 0;
            bool finished = false;
            bool quoted = false;

            char quoteChar = ' ';
            while (pos < sel.Length && !finished)
            {
                char current = sel[pos];
                switch (step)
                {
                    case 0:
                        if (current == ' ')
                        {
                            pos++;
                        }
                        else
                        {
                            step = 1;
                        }
                        break;
                    case 1:
                        if (current == '\'' || current == '"')
                        {
                            quoteChar = current;
                            quoted = true;
                            startPos = pos + 1;
                            step = 2;
                        }
                        else
                        {
                            startPos = current;
                            step = 3;
                        }
                        pos++;
                        break;
                    case 2:
                        if (quoted && current == quoteChar)
                        {
                            endPos = pos;
                            pos++;
                            step = 3;
                        }
                        pos++;
                        break;
                    case 3:
                        if (current == ')')
                        {
                            finished = true;
                        }
                        else
                        {
                            pos++;
                        }
                        break;
                }
            }

            string result = sel.SubstringBetween(startPos, endPos);
            if (sel.Length > pos)
            {
                sel = sel.Substring(pos + 1);
            }
            else
            {
                sel = String.Empty;
            }
            return result;
        }

        protected class MatchElement
        {
            public MatchElement(IDomObject element)
            {
                Initialize(element, 0);
            }
            public MatchElement(IDomObject element, int depth)
            {
                Initialize(element, depth);
            }
            protected void Initialize(IDomObject element, int depth)
            {
                Object = element;
                Depth = depth;
            }
            public IDomObject Object { get; set; }
            public int Depth { get; set; }
            public IDomElement Element { get { return (IDomElement)Object; } }
        }

        public IEnumerable<IDomObject> GetMatch(IEnumerable<IDomObject> baseList, IEnumerable<IDomObject> list, CsQuerySelector selector)
        {
            // Maintain a hashset of every element already searched. Since result sets frequently contain items which are
            // children of other items in the list, we would end up searching the tree repeatedly
            HashSet<IDomObject> uniqueElements = null;

            Stack<MatchElement> stack = null;
            IEnumerable<IDomObject> curList = list ?? baseList;
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
            if (selector.SelectorType.HasFlag(SelectorType.Position) && selector.IsResultListPosition())
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
                        IDomElement elm = current.Element;
                        for (int j = elm.ChildNodes.Count - 1; j >= 0; j--)
                        {
                            IDomObject obj = elm[j];
                            if (selector.TraversalType == TraversalType.Child && !uniqueElements.Add(obj))
                            {
                                continue;
                            }
                            if (obj.NodeType == NodeType.ELEMENT_NODE)
                            //if (obj is IDomElement)
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

        #region selection matching main code
        /// <summary>
        /// Test obj for a match with selector. matchIndex is the current index for items returned by the selector, and depth is the current
        /// node depth.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="obj"></param>
        /// <param name="matchIndex"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        protected bool Matches(CsQuerySelector selector, IDomObject obj, int depth)
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
                        throw new Exception("No AttributeSelectorType set");
                }
                if (!match)
                {
                    return false;
                }
            }
            if (selector.SelectorType.HasFlag(SelectorType.Position) &&
                !MatchesDOMPosition(elm, selector.PositionType,
                selector.PositionType == PositionType.NthChild ? selector.Criteria : null))
            {
                return false;
            }

            if (selector.SelectorType.HasFlag(SelectorType.Contains) &&
                !ContainsText(elm, selector.Criteria))
            {
                return false;
            }
            return true;
        }
        protected IEnumerable<IDomObject> GetResultPositionMatches(IEnumerable<IDomObject> list, CsQuerySelector selector)
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
                    throw new Exception("Unimplemented position type selector");
            }
        }
        #endregion
        #region utility functions

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
