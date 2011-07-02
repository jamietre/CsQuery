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
        protected bool CurrentExists
        {
            get
            {
                return (_Current != null);
            }
        }
        protected void ParseSelector(string selector)
        {
            string sel = _Selector.Trim();
            
            if (IsHtml)
            {
                Current.Html = sel;
                //sel = String.Empty;
                return;
            }
            StringScanner scanner = new StringScanner(sel);
            scanner.StopChars = " :.=#,*()[]^'\"";
            scanner.Next();
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
                                    // add anothre selector for actual "button" elements
                                    Selectors.Add(Current);
                                    _Current = null;
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
                        if (!scanner.AtEnd)
                        {
                            scanner.Expect(" ,");
                            scanner.Next();
                        }
                        break;
                    case ',':
                        if (!CurrentExists)
                        {
                            scanner.ThrowUnexpectedCharacterException();
                        }
                        Selectors.Add(_Current);
                        _Current = null;
                        scanner.Next();
                        scanner.SkipWhitespace();
                        break;
                    default:
                        Current.SelectorType |= SelectorType.Tag;
                        scanner.Prev();
                        Current.Tag = scanner.Seek();
                        break;
                }
            }
            if (_Current != null)
            {
                Selectors.Add(Current);
            }
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

        public IEnumerable<DomElement> GetMatches(IEnumerable<DomElement> list)
        {
            // Maintain a hashset of every element already searched. Since result sets frequently contain items which are
            // children of other items in the list, we would end up searching the tree repeatedly
            HashSet<DomElement> uniqueElements = new HashSet<DomElement>();
            
            var stack = new Stack<DomElement>();

            foreach (var e in list)
            {
                if (uniqueElements.Add(e))
                {
                    stack.Push(e);
                    while (stack.Count != 0)
                    {
                        var current = stack.Pop();
                        if (Matches(current)) yield return current;
                        for (int i = current._Children.Count - 1; i >= 0; i--)
                        {
                            DomObject obj = current._Children[i];
                            if (obj is DomElement && uniqueElements.Add((DomElement)obj))
                            {
                                stack.Push((DomElement)obj);
                            }
                        }
                    }
                }
            }
        }
        protected bool Matches(DomElement obj) 
        {
            bool match = true;
            foreach (CsQuerySelector selector in this)
            {
                if (selector.SelectorType.HasFlag(SelectorType.All))
                {
                    break;
                }
                if (selector.SelectorType.HasFlag(SelectorType.Tag) && 
                    !String.Equals(obj.Tag, selector.Tag, StringComparison.CurrentCultureIgnoreCase))
                {
                    match = false; continue;
                }
                if (selector.SelectorType.HasFlag(SelectorType.ID) &&
                    selector.ID != obj.ID) 
                {
                    match = false; continue;
                }
                if (selector.SelectorType.HasFlag(SelectorType.Attribute))
                {
                    string value;
                    match = obj.TryGetAttribute(selector.AttributeName, out value);
                    if (!match)
                    {
                        if (selector.AttributeSelectorType == AttributeSelectorType.NotExists)
                        {
                            match = true;
                        }
                        continue;
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
                        default:
                            throw new Exception("No AttributeSelectorType set");
                    }
                    if (!match) continue;
                }
                if (selector.SelectorType.HasFlag(SelectorType.Class) &&
                    !obj.HasClass(selector.Class))
                {
                    match = false; continue;
                }
                if (selector.SelectorType.HasFlag(SelectorType.Contains) &&
                    !ContainsText(obj, selector.Contains)) 
                {
                    match = false; continue;
                }
                match = true;
                break;
            }
            return match;
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
        NotExists=5
    }
    public class CsQuerySelector
    {
        public CsQuerySelector()
        {
            AttributeSelectorType = AttributeSelectorType.Equals;
        }
        public SelectorType SelectorType { get; set; }
        public AttributeSelectorType AttributeSelectorType { get; set; }
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
