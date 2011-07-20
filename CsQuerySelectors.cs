using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Jtc.ExtensionMethods;

namespace Jtc.CsQuery
{
    public class CsQuerySelectors : IEnumerable<CsQuerySelector>
    { 
        protected int CurrentPos = 0;

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

        public bool IsHtml
        {
            get
            {
                return !String.IsNullOrEmpty(Selector) && Selector[0] == '<';
            }
        }
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
        StringScanner scanner;
        protected void ParseSelector(string selector)
        {
            string sel = _Selector.Trim();
            
            if (IsHtml)
            {
                Current.Html = sel;
                Current.SelectorType = SelectorType.HTML;
                Selectors.Add(Current);
                return;
            }
            scanner = new StringScanner(sel);
            scanner.StopChars = " >:.=#$|,*()[]^'\"";
            scanner.Next();
            
            StartNewSelector();
            
            while (!scanner.AtEnd) {

                switch (scanner.Current)
                {
                    case '*':
                        Current.SelectorType = SelectorType.All;
                        scanner.End();
                        break;
                    case '<':
                        // not selecting - creating html
                        Current.Html = sel;
                        scanner.End();
                        break;
                    case ':':
                        string key = scanner.Seek().ToLower();
                        switch (key)
                        {
                            case "checkbox":
                            case "button":
                            case "file":
                            case "text":
                                Current.SelectorType |= SelectorType.Attribute;
                                Current.AttributeSelectorType = AttributeSelectorType.Equals;
                                Current.AttributeName = "type";
                                Current.AttributeValue = key;

                                if (key == "button" && !Current.SelectorType.HasFlag(SelectorType.Tag))
                                {
                                    StartNewSelector(CombinatorType.Cumulative);
                                    Current.SelectorType = SelectorType.Tag;
                                    Current.Tag = "button";
                                }
                                else
                                {
                                    FinishSelector();
                                }
                                break;
                            case "checked":
                            case "selected":
                            case "disabled":
                                Current.SelectorType |= SelectorType.Attribute;
                                Current.AttributeSelectorType = AttributeSelectorType.Exists;
                                Current.AttributeName = key;
                                FinishSelector();
                                break;
                            case "enabled":
                                Current.SelectorType |= SelectorType.Attribute;
                                Current.AttributeSelectorType = AttributeSelectorType.NotExists;
                                Current.AttributeName = "disabled";
                                FinishSelector();
                                break;
                            case "contains":
                                StartNewSelector();
                                Current.SelectorType |= SelectorType.Contains;
                                Current.TraversalType = TraversalType.Descendent;
                                scanner.Expect('(');
                                scanner.AllowQuoting();
                                Current.Criteria= scanner.Seek(")");
                                scanner.Next();
                                FinishSelector();
                                break;
                            case "eq":
                            case "gt":
                            case "lt":
                                StartNewPositionSelector();
                                switch(key) {
                                    case "eq": Current.PositionType=PositionType.IndexEquals; break;
                                    case "lt": Current.PositionType = PositionType.IndexLessThan; break;
                                    case "gt": Current.PositionType = PositionType.IndexGreaterThan; break;
                                }
                                scanner.Expect('(');
                                scanner.AllowQuoting();
                                Current.PositionIndex = Convert.ToInt32(scanner.Seek(")"));
                                scanner.Next();
                                FinishSelector();
                                break;
                            case "even":
                                StartNewPositionSelector();
                                Current.PositionType = PositionType.Even;
                                FinishSelector();
                                break;
                            case "odd":
                                StartNewPositionSelector();
                                Current.PositionType = PositionType.Odd;
                                FinishSelector();
                                break;
                            case "first":
                                StartNewPositionSelector();
                                Current.PositionType = PositionType.Odd;
                                FinishSelector();
                                break;
                            case "last":
                                StartNewPositionSelector();
                                Current.PositionType = PositionType.Last;
                                FinishSelector();
                                break;
                            default:
                                throw new Exception("Unknown selector :\""+key+"\"");

                        }
                        break;
                    case '.':
                        Current.SelectorType = SelectorType.Class;
                        Current.Class = scanner.Seek();
                        StartNewSelector();

                        break;
                    case '#':
                        Current.SelectorType = SelectorType.ID;
                        Current.ID = scanner.Seek();
                        StartNewSelector();
                        break;
                    case '[':
                        
                        Current.AttributeName = scanner.Seek();
                        Current.SelectorType |= SelectorType.Attribute;
                        
                        bool finished = false;
                        while (!scanner.AtEnd && !finished)
                        {
                            switch (scanner.Current)
                            {
                                case '=':
                                    scanner.AllowQuoting();
                                    Current.AttributeValue = scanner.Seek("]");
                                    if (Current.AttributeSelectorType == 0)
                                    {
                                        Current.AttributeSelectorType = AttributeSelectorType.Equals;
                                    }
                                    finished = true;
                                    break;
                                case '^':
                                    Current.AttributeSelectorType = AttributeSelectorType.StartsWith;
                                    break;
                                case '*':
                                    Current.AttributeSelectorType = AttributeSelectorType.Contains;
                                    break;
                                case '~':
                                    Current.AttributeSelectorType = AttributeSelectorType.ContainsWord;
                                    break;
                                case '$':
                                    Current.AttributeSelectorType = AttributeSelectorType.EndsWith;
                                    break;
                                case '!':
                                    Current.AttributeSelectorType = AttributeSelectorType.NotEquals;
                                    break;
                                case ']':
                                    Current.AttributeSelectorType = AttributeSelectorType.Exists;
                                    finished = true;
                                    break;
                                default:
                                    scanner.ThrowUnexpectedCharacterException("Malformed attribute selector.");
                                    break;
                            }
                            scanner.Next();
                            
                        }
                        //if (!scanner.AtEnd)
                        //{
                        //    scanner.Expect(" ,");
                        //    scanner.Next();
                        //}
                        break;
                    case ',':
                        
                        StartNewSelector(CombinatorType.Root);
                        // thre should be a selector closed from the previous line
                        if (Selectors.Count == 0)
                        {
                            scanner.ThrowUnexpectedCharacterException();
                        }

                        scanner.SkipWhitespace();
                        scanner.Next();
                        
                        break;
                    case '>':
                        StartNewSelector();

                        if (Selectors.Count == 0)
                        {
                            scanner.ThrowUnexpectedCharacterException();
                        }

                        Current.TraversalType = TraversalType.Child;
                        Current.ChildDepth = 1;

                        scanner.SkipWhitespace();
                        scanner.Next();
                        break;
                    case ' ':
                        // if a ">" or "," is later found, it will be overridden.
                        StartNewSelector(CombinatorType.Chained);
                        Current.TraversalType = TraversalType.Descendent;

                        scanner.SkipWhitespace();
                        scanner.Next();
                        
                        break;
                    default:
                        Current.SelectorType = SelectorType.Tag;
                        scanner.Prev();
                        Current.Tag = scanner.Seek();
                        break;
                }
            }
            // Close any open selectors
            StartNewSelector();

        }
        // Like StartnewSelector, but will force a selector to finish even if not complete 
        protected void FinishSelector()
        {
            if (Current.SelectorType == 0)
            {
                Current.SelectorType = SelectorType.All;
            }
            StartNewSelector();
        }
        protected void StartNewPositionSelector() {

            StartNewSelector();
            Current.SelectorType = SelectorType.Position;
            Current.TraversalType = TraversalType.Filter;

        }
        protected void StartNewSelector()
        {
            StartNewSelector(0);

        }
        /// <summary>
        /// Start a new selector. If current one exists and is complete, it is closed first.
        /// </summary>
        /// <param name="type"></param>
        protected void StartNewSelector(CombinatorType type)
        {
            CombinatorType defaultType = CombinatorType.Chained;
            if (_Current != null && Current.IsComplete)
            {
                Selectors.Add(Current);
                _Current = null;
                defaultType = CombinatorType.Chained;
            }

            Current.CombinatorType = type==0?defaultType : type;
        }
        protected string ParseFunction(ref string sel)
        {
            int subPos = sel.IndexOfAny(new char[] { '(' });
            if (subPos < 0)
            {
                throw new Exception("Bad 'contains' selector.");
            }
            subPos++;
            int pos = subPos;
            int startPos=-1;
            int endPos = -1;
            int step = 0;
            bool finished = false;
            bool quoted=false;

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
                        } else {
                            step=1;
                        }
                        break;
                    case 1:
                        if (current == '\'' || current == '"')
                        {
                            quoteChar = current;
                            quoted=true;
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
                        if (quoted && current==quoteChar)
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

            string result = sel.SubstringBetween(startPos,endPos);
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

        public IEnumerable<IDomObject> Select(DomRoot root)
        {
            return Select(root, null);
        }
        DomRoot Dom;
        /// <summary>
        /// Select from DOM using index. First non-class/tag/id selector will result in this being passed off to GetMatches
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public IEnumerable<IDomObject> Select(DomRoot root, IEnumerable<IDomObject> selectWithin)
        {
            Dom = root;
            IEnumerable<IDomObject> lastResult = null;
            IEnumerable<IDomObject> selectionSource = selectWithin;
            

            for (int selIndex = 0; selIndex < Selectors.Count; selIndex++)
            {
                var selector = Selectors[selIndex];

                // Determine what kind of combining method we will use with previous selection results

                if (selIndex != 0)
                {
                    switch (selector.CombinatorType)
                    {
                        case CombinatorType.Cumulative:
                            //
                            //selectionSource = lastChained;
                            break;
                        case CombinatorType.Root:
                            selectionSource= selectWithin;
                            break;
                        case CombinatorType.Chained:
                            selectionSource = lastResult;
                            lastResult = null;
                            break;
                        // default (chained): leave lastresult alone
                    }
                }
                if (selector.TraversalType == TraversalType.Child) {
                    var newSource = new HashSet<IDomObject>();
                    foreach (IDomObject el in selectionSource) {
                        if (el is IDomContainer) {
                            newSource.AddRange(((IDomContainer)el).Children);
                        }
                    }
                    selectionSource=newSource;
                }

                HashSet<IDomObject> tempResult = null;
                string key = String.Empty;
                if (selector.SelectorType.HasFlag(SelectorType.Tag))
                {
                    key = selector.Tag;
                    selector.SelectorType &= ~SelectorType.Tag;
                }
                else if (selector.SelectorType.HasFlag(SelectorType.ID))
                {
                    key = "#" + selector.ID;
                    selector.SelectorType &= ~SelectorType.ID;
                }
                else if (selector.SelectorType.HasFlag(SelectorType.Class))
                {
                    key = "." + selector.Class;
                    selector.SelectorType &= ~SelectorType.Class;
                }
                // If we can use an indexed selector, do it here
                if (key != String.Empty)
                {
                    int depth=0;
                    bool descendants=true;

                    switch(selector.TraversalType) {
                        case TraversalType.Child:
                            depth=1;
                            descendants=false;
                            break;
                        case TraversalType.Filter:
                            depth=0;
                            descendants=false;
                            break;
                        case TraversalType.Descendent:
                            depth=1;
                            descendants=true;
                            break;
                    }

                    if (lastResult != null)
                    {
                        tempResult = new HashSet<IDomObject>();
                        tempResult.AddRange(lastResult);
                    }
                    
                    if (selectionSource == null)
                    {
                        if (tempResult != null) {
                            tempResult.AddRange(root.SelectorXref.GetRange(key + ">",depth,descendants));
                            lastResult=tempResult;
                        } else {
                            lastResult = root.SelectorXref.GetRange(key + ">",depth,descendants);
                        }
                    }
                    else
                    {
                        if (tempResult == null)
                        {
                            tempResult = new HashSet<IDomObject>();
                        }

                        if (selector.CombinatorType == CombinatorType.Cumulative)
                        {
                            tempResult.AddRange(lastResult);
                        }

                        foreach (IDomObject obj in selectionSource)
                        {
                            tempResult.AddRange(root.SelectorXref.GetRange(key + ">" + obj.Path,depth,descendants));
                        }
                        lastResult = tempResult;
                    }
                }
                if (selector.SelectorType != 0)
                {
                    // if there are no temporary results (b/c there was no indexed selector) then use the whole set
                    lastResult = GetMatch(root.Elements, lastResult ?? selectionSource ,selector);
                }
            }
            // If there's another subquery within this selector, or any subselects, pass it on to the matching engine
            //if (selector.SelectorType != 0)
            //{
            //    return ReorderSelection(root, GetMatches(root.Elements, lastResult, 0));
            //}
            //else if (Selectors.Count > 1)
            //{
            //    return ReorderSelection(root,GetMatches(root.Elements, lastResult, 1));
            //}
            //else
           // {
                return ReorderSelection(root, lastResult);
            //}
        }
        /// <summary>
        /// Because selectors may not always return the items in DOM order (e.g. "OR" queries or chained full-dom selectors) fix that now. If elements in this
        /// selection aren't part of the dom, just return them (they are going to be added).
        /// </summary>
        /// <param name="elements"></param>
        protected IEnumerable<IDomObject> ReorderSelection(DomRoot root, IEnumerable<IDomObject> elements)
        {
            SortedSet<string> ordered = new SortedSet<string>(StringComparer.Ordinal);
            foreach (var e in elements)
            {
                if (ReferenceEquals(e.Dom, root))
                {
                    ordered.Add(e.Path);
                }
                else
                {
                    yield return e;
                }
            }
            foreach (var key in ordered)
            {
                yield return root.SelectorXref[">"+key];
            }

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
            public DomElement Element { get { return (DomElement)Object; } }
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
                DomElementFactory factory = new DomElementFactory();

                foreach (var obj in factory.CreateElements(selector.Html))
                {
                    yield return obj;
                }
                yield break;
            }

            // Position selectors are simple -- skip out of main matching code if so
            if (selector.SelectorType == SelectorType.Position)
            {
                foreach (var obj in GetPositionMatches(curList, selector))
                {
                    yield return obj;
                }
                yield break;
            }

            stack = new Stack<MatchElement>();
            int depth = 0;
            foreach (var e in curList)
            {
                if (uniqueElements.Add(e))
                {
                    stack.Push(new MatchElement(e, depth));
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

                        if (selector.TraversalType != TraversalType.Filter &&
                            current.Object is DomElement)
                        {
                            DomElement elm = current.Element;
                            for (int j = elm.Count - 1; j >= 0; j--)
                            {
                                IDomObject obj = elm[j];
                                if (obj is DomElement && uniqueElements.Add((DomElement)obj))
                                {
                                    stack.Push(new MatchElement(obj, current.Depth + 1));
                                }
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

        public IEnumerable<IDomObject> GetMatches(IEnumerable<IDomObject> list)
        {
            return GetMatches(list, null, 0);
        }
        /// <summary>
        /// Primary selection engine: returns a subset of "list" based on selectors.
        /// This can be optimized -- class and id selectors should be added to a global hashtable when the dom is built
        /// </summary>
        /// <param name="list"></param>
        /// <param name="recurse"></param>
        /// <returns></returns>
        public IEnumerable<IDomObject> GetMatches(IEnumerable<IDomObject> baseList, IEnumerable<IDomObject> list, int firstSelector)
        {

            // Maintain a hashset of every element already searched. Since result sets frequently contain items which are
            // children of other items in the list, we would end up searching the tree repeatedly
            HashSet<IDomObject> uniqueElements = null;

            Stack<MatchElement> stack = null;
            IEnumerable<IDomObject> curList = list ?? baseList;
            HashSet<IDomObject> temporaryResults = new HashSet<IDomObject>();

            bool simple = false;

            for (int i=firstSelector;i<Selectors.Count;i++)
            {
                var selector = Selectors[i];
                
                // If there is only one selector, and it's possible to know its results before all are found, allow it to be yielded directly
                if (selector.SelectorType.IsOneOf(SelectorType.Position, SelectorType.HTML) )
                {
                    simple = false;
                }
                else
                {
                    simple = firstSelector == Selectors.Count - 1;
                }
                

                // For chained combinators, clear the temporary list - we only want the results from each successive round. Otherwise,
                // reset the source to the original list but keep the results of the previous round.
                
                //if (i != firstSelector) {
                //    if (selector.CombinatorType == CombinatorType.Chained)
                //    {
                //        curList = temporaryResults;
                //        temporaryResults = new HashSet<IDomObject>();
                //    }  else {
                //        curList = baseList;
                //    }
                //}

                // The unique list has to be reset for each sub-selector
                uniqueElements = new HashSet<IDomObject>();

                if (selector.SelectorType == SelectorType.HTML)
                {
                    DomElementFactory factory = new DomElementFactory();

                    foreach (var obj in factory.CreateElements(selector.Html))
                    {
                        temporaryResults.Add(obj);
                    }
                    continue;
                }

                // Position selectors are simple -- skip out of main matching code if so
                if (selector.SelectorType == SelectorType.Position)
                {
                    foreach (var obj in GetPositionMatches(curList, selector)) {
                        temporaryResults.Add(obj);
                    }
                    continue;
                }

                stack = new Stack<MatchElement>();
                int depth = 0;
                foreach (var e in curList)
                {
                    if (uniqueElements.Add(e))
                    {
                        stack.Push(new MatchElement(e,depth));
                        int matchIndex = 0;
                        while (stack.Count != 0)
                        {
                            var current = stack.Pop();

                            if (Matches(selector, current.Object,current.Depth ))
                            {
                                if (simple)
                                {
                                    yield return current.Object;
                                }
                                else
                                {
                                    temporaryResults.Add(current.Object);
                                }   
                                matchIndex++;
                            }
                            // Add children to stack (in reverse order, so they are processed in the correct order when popped)
                            
                            if (selector.TraversalType != TraversalType.Filter && 
                                current.Object is DomElement)
                            {
                                DomElement elm = current.Element;
                                for (int j = elm.Count - 1; j >= 0; j--)
                                {
                                    IDomObject obj = elm[j];
                                    if (obj is DomElement && uniqueElements.Add((DomElement)obj))
                                    {
                                        stack.Push(new MatchElement(obj,current.Depth+1));
                                    }
                                }
                            }
                        }
                    }
                }


            }
            // for complex cases, return each final result
            if (!simple)
            {
                foreach (var obj in temporaryResults)
                {
                    yield return obj;
                }
            }
        }
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
                    if (selector.ChildDepth != depth) {
                        return false;
                    }
                    break;
                case TraversalType.Descendent:
                    if (depth==0) {
                        return false;
                    }
                    break;
            }
            if (selector.SelectorType.HasFlag(SelectorType.All))
            {
                return true;
            }
            if (!(obj is DomElement)) { 
                return false;
            }
            DomElement elm = (DomElement)obj;

            if (selector.SelectorType.HasFlag(SelectorType.Tag) &&
                !String.Equals(elm.Tag, selector.Tag, StringComparison.CurrentCultureIgnoreCase))
            {
                //match = false; continue;
                return false;
            }
            if (selector.SelectorType.HasFlag(SelectorType.ID) &&
                selector.ID != elm.ID) 
            {
                //match = false; continue;
                return false;
            }
            if (selector.SelectorType.HasFlag(SelectorType.Attribute))
            {
                string value;
                match = elm.TryGetAttribute(selector.AttributeName, out value);
                if (!match)
                {
                    if (selector.AttributeSelectorType.IsOneOf(AttributeSelectorType.NotExists, AttributeSelectorType.NotEquals))
                    {
                        match = true;
                    }
                    return match;
                }

                switch(selector.AttributeSelectorType) {
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
                        match = ContainsWord(value,selector.AttributeValue);
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
        
            if (selector.SelectorType.HasFlag(SelectorType.Class) &&
                !elm.HasClass(selector.Class))
            {
                match = false; 
                return match;
            }
            if (selector.SelectorType.HasFlag(SelectorType.Contains) &&
                !ContainsText(elm, selector.Criteria)) 
            {
                match = false; 
                return match;
            }
            match = true;
            return match;
        }
        protected IEnumerable<IDomObject> GetPositionMatches(IEnumerable<IDomObject> list, CsQuerySelector selector) 
        {

            switch (selector.PositionType)
            {
                case PositionType.First:
                    yield return list.First();
                    break;
                case PositionType.Last:
                    yield return list.Last();
                    break;
                case PositionType.Odd:
                case PositionType.Even:
                    foreach (var obj in GetOddEventMatches(list, selector.PositionType))
                    {
                        yield return obj;
                    }
                    break;
                case PositionType.IndexEquals:
                    int critIndex = selector.PositionIndex;
                    if (critIndex < 0)
                    {
                        critIndex = list.Count() + critIndex;
                    }
                    bool ok=true;
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
                    int index=0;
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
                        if (indexLess++< selector.PositionIndex)
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
        }
        protected IEnumerable<IDomObject> GetOddEventMatches(IEnumerable<IDomObject> list, PositionType positionType)
        {
            int index=0;
            foreach (IDomObject obj in list) {
                index++;
                switch (positionType)
                {
                    case PositionType.Even:
                        if (index % 2 == 0) {
                            yield return obj;
                        }
                        break;
                    case PositionType.Odd:
                        if (index % 2 != 0) {
                            yield return obj;
                        }
                        break;
                    default:
                        throw new Exception("Unexpected criteria '" + positionType.ToString() + "'");
                }
            }
        }
        protected bool MatchesPosition(string criteria, int index)
        {
            switch (criteria)
            {
                case "even":
                    return index % 2 == 0;
                case "odd":
                    return index % 2 != 0;
                case "first":
                    return index==0;
                case "last":
                    return true;
                default:
                    return Convert.ToInt32(criteria) == index;

            }
        }
        protected bool ContainsWord(string text, string word)
        {
            HashSet<string> words = new HashSet<string>(word.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            return words.Contains(text);
        }
        protected bool ContainsText(DomElement obj, string text)
        {
            foreach (IDomObject e in obj.Children)
            {
                if (e is DomText)
                {
                    if (((IDomText)e).Text.IndexOf(text) > 0)
                    {
                        return true;
                    }
                }
                else
                {
                    if (ContainsText((DomElement)e, text))
                    {
                        return true;
                    }
                }
            }
            return false;

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
    
    [Flags]
    public enum SelectorType
    {
        All = 1,
        Tag = 2,
        ID=4,
        Class=8,
        Attribute=16,
        Contains =32,
        Position = 64,
        HTML = 128
    }
    public enum AttributeSelectorType
    {
        Exists=1,
        Equals=2,
        StartsWith=3,
        Contains=4,
        NotExists=5,
        ContainsWord=6,
        EndsWith=7,
        NotEquals=8
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
        Descendent=3,
        Child=4
    }
    public enum PositionType
    {
        All = 1,
        Even = 2,
        Odd = 3,
        First = 4,
        Last = 5,
        IndexEquals = 6,
        IndexLessThan=7,
        IndexGreaterThan=8
    }
    public class CsQuerySelector
    {
        public CsQuerySelector()
        {
            SelectorType=0;
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


        /// <summary>
        /// Selection tag name
        /// </summary>
        public string Tag {
            get;set;
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

    }
}
