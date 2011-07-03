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
        /// <summary>
        /// Always treats this selector as new HTML
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="isHtml"></param>
        public CsQuerySelectors(string selector, bool isHtml)
        {
            IsHtml = isHtml;
            Selector = selector;

        }
        public bool IsHtml = false;
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
                //sel = String.Empty;
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

                        Current.SelectorType |= SelectorType.All;
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
                                Current.SelectorType |= SelectorType.Attribute;
                                Current.AttributeSelectorType = AttributeSelectorType.Equals;
                                Current.AttributeName = "type";
                                Current.AttributeValue = key;
                                if (key == "button" && !Current.SelectorType.HasFlag(SelectorType.Tag))
                                {
                                    StartNewSelector(CombinatorType.Cumulative);
                                    Current.SelectorType |= SelectorType.Tag;
                                    Current.Tag = "button";
                                }
                                break;
                            case "checked":
                            case "selected":
                            case "disabled":
                                Current.SelectorType |= SelectorType.Attribute;
                                Current.AttributeSelectorType = AttributeSelectorType.Exists;
                                Current.AttributeName = key;
                                break;
                            case "contains":
                                Current.SelectorType |= SelectorType.Contains;
                                scanner.Expect('(');
                                scanner.AllowQuoting();

                                Current.Contains = scanner.Seek(")");
                                scanner.Next();
                                break;
                            case "enabled":
                                Current.SelectorType |= SelectorType.Attribute;
                                Current.AttributeSelectorType = AttributeSelectorType.NotExists;
                                Current.AttributeName = "disabled";
                                break;
                            default:
                                throw new Exception("Unknown selector :\""+key+"\"");

                        }
                        break;
                    case '.':
                        Current.SelectorType |= SelectorType.Class;
                        Current.Class = scanner.Seek();

                        break;
                    case '#':
                        Current.SelectorType |= SelectorType.ID;
                        Current.ID = scanner.Seek();
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
                        // thre should be a selector already
                        StartNewSelector(CombinatorType.Cumulative);
                        scanner.SkipWhitespace();
                        scanner.Next();
                        if (Selectors.Count==0)
                        {
                            scanner.ThrowUnexpectedCharacterException();
                        }
                        break;
                    case '>':
                        StartNewSelector(CombinatorType.Child);

                        if (Selectors.Count == 0)
                        {
                            scanner.ThrowUnexpectedCharacterException();
                        }
                        scanner.SkipWhitespace();
                        scanner.Next();
                        break;
                    case ' ':
                        StartNewSelector(CombinatorType.Descendant);
                        scanner.SkipWhitespace();
                        scanner.Next();
                        break;
                    default:
                        Current.SelectorType |= SelectorType.Tag;
                        scanner.Prev();
                        Current.Tag = scanner.Seek();
                        break;
                }
            }
            StartNewSelector();

        }
        protected void StartNewSelector()
        {
            // First selector should always be "cumulative"
            if (Selectors.Count == 0)
            {
                StartNewSelector(CombinatorType.Cumulative);
            }
            else
            {
                StartNewSelector(CombinatorType.Descendant);
            }

        }
        /// <summary>
        /// Start a new selector. If current one exists and is complete, 
        /// </summary>
        /// <param name="type"></param>
        protected void StartNewSelector(CombinatorType type)
        {
            if (_Current != null && Current.IsComplete)
            {
                Selectors.Add(Current);
                _Current = null;
            }

            Current.CombinatorType = type;
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
        protected string GetNextPart(ref string sel)
        {
            string result;
            int subPos = sel.IndexOfAny(new char[] { ' ', ':', '.', '=', '#', ',','*','(',')','[',']','^','\'' });
            if (subPos < 0)
            {
                result = sel.Trim();
                sel = String.Empty;
                return result;
            }
            else
            {
                result = sel.Substring(0, subPos).Trim();
                sel = sel.Substring(subPos).Trim();
                return result;
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

        /// <summary>
        /// Primary selection engine: returns a subset of "list" based on selectors.
        /// This can be optimized -- class and id selectors should be added to a global hashtable when the dom is built
        /// </summary>
        /// <param name="list"></param>
        /// <param name="recurse"></param>
        /// <returns></returns>
        public IEnumerable<DomElement> GetMatches(IEnumerable<DomElement> list)
        {
            // Maintain a hashset of every element already searched. Since result sets frequently contain items which are
            // children of other items in the list, we would end up searching the tree repeatedly
            HashSet<DomElement> uniqueElements=null;
            
            Stack<DomElement> stack = null;
            IEnumerable<DomElement> curList = list;
            HashSet<DomElement> temporaryResults = new HashSet<DomElement>();

            bool simple = Selectors.Count == 1;
            for (int i=0;i<Selectors.Count;i++)
            {
                var selector = Selectors[i];
                // The unique list has to be reset for each sub-selector
                uniqueElements = new HashSet<DomElement>();
                // For progressive combinatores, start with the previous round's results for each successive selection.
                // Otherwise always search the original heirarchy and add to results (cumulate)
                if (selector.CombinatorType != CombinatorType.Cumulative)
                {
                    curList = temporaryResults;
                    temporaryResults = new HashSet<DomElement>();
                }

                stack = new Stack<DomElement>();
                foreach (var e in curList)
                {
                    if (uniqueElements.Add(e))
                    {
                        stack.Push(e);
                        while (stack.Count != 0)
                        {
                            var current = stack.Pop();
                            if (Matches(selector, current))
                            {
                                if (simple)
                                {
                                    yield return current;
                                }
                                else
                                {
                                    temporaryResults.Add(current);
                                }
                            }
                            // For child -never go below first level.
                            if (selector.CombinatorType != CombinatorType.Child || stack.Count==0)
                            {
                                for (int j = current._Children.Count - 1; j >= 0; j--)
                                {
                                    DomObject obj = current._Children[j];
                                    if (obj is DomElement && uniqueElements.Add((DomElement)obj))
                                    {
                                        stack.Push((DomElement)obj);
                                    }
                                }
                            }
                        }
                    }
                }

            }
            if (!simple)
            {
                foreach (var e in temporaryResults)
                {
                    yield return e;
                }
            }
        }
        protected bool Matches(CsQuerySelector selector,DomElement obj) 
        {
            bool match = true;
            if (selector.SelectorType.HasFlag(SelectorType.All))
            {
                return true;
            }
            if (selector.SelectorType.HasFlag(SelectorType.Tag) && 
                !String.Equals(obj.Tag, selector.Tag, StringComparison.CurrentCultureIgnoreCase))
            {
                //match = false; continue;
                return false;
            }
            if (selector.SelectorType.HasFlag(SelectorType.ID) &&
                selector.ID != obj.ID) 
            {
                //match = false; continue;
                return false;
            }
            if (selector.SelectorType.HasFlag(SelectorType.Attribute))
            {
                string value;
                match = obj.TryGetAttribute(selector.AttributeName, out value);
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
                !obj.HasClass(selector.Class))
            {
                match = false; 
                return match;
            }
            if (selector.SelectorType.HasFlag(SelectorType.Contains) &&
                !ContainsText(obj, selector.Contains)) 
            {
                match = false; 
                return match;
            }
            match = true;
            return match;
        }
        protected bool ContainsWord(string text, string word)
        {
            HashSet<string> words = new HashSet<string>(word.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            return words.Contains(text);
        }
        protected bool ContainsText(DomElement obj, string text)
        {
            foreach (DomObject e in obj.Children)
            {
                if (e is DomLiteral)
                {
                    if (e.Html.IndexOf(text) > 0)
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
        Contains =32
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
        NotEquals=8,


    }
    public enum CombinatorType
    {
        Cumulative = 1,
        Descendant = 2,
        Child = 3
    }
    public class CsQuerySelector
    {
        public CsQuerySelector()
        {
            SelectorType=0;
            AttributeSelectorType = AttributeSelectorType.Equals;
            CombinatorType = CombinatorType.Descendant;
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


        public string Tag = null;
        public string Contains
        {
            get
            {
                return _Contains;
            }
            set
            {
                //if (value[0]=='\'' || value[0]=='"') {
                //    char quoteChar = value[0];
                //    if (value[value.Length - 1] == quoteChar)
                //    {
                //        _Contains = value.Substring(1, value.Length - 2);
                //    }
                //    else
                //    {
                //        _Contains = value;
                //    }
                //  }
                _Contains=value;
                
            }
        }protected string _Contains = null;
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
