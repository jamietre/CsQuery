using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using CsQuery.ExtensionMethods;
using CsQuery.Utility.EquationParser;
namespace CsQuery.Engine
{

    /// <summary>
    /// Figure out if an index matches an Nth Child, or return a list of all matching elements from a list.
    /// </summary>
    public class NthChild
    {
        #region private properties

        /// <summary>
        /// A structure to keep information about what has been calculated so far for a given equation string.
        /// NthChild is expensive so we cache a list of matching element IDs for a given equation along with the 
        /// last index this list represents and the iteration. The next time it's called we can either reference
        /// the list of matches so far, or update it only from the point where we stopped last time.
        /// </summary>
        protected class CacheInfo
        {
            public HashSet<int> MatchingIndices;
            public int NextIterator;
            public int MaxIndex;
        }

        private static ConcurrentDictionary<string, CacheInfo> ParsedEquationCache =
            new ConcurrentDictionary<string, CacheInfo>();
        
        protected CacheInfo cacheInfo;
        protected bool cached = false;
        protected IEquation<int> formula;
        
        protected string _Text;
        protected bool IsJustNumber;
        protected int MatchOnlyIndex;
        #endregion

        #region public properties/methods
        /// <summary>
        /// The formula for this nth child selector
        /// </summary>
        protected string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                _Text = value;
                CheckForSimpleNumber(value);

                if (!IsJustNumber)
                {
                    ParseEquation(value);
                }
            }
        }

        /// <summary>
        /// Keep this private so changes can only me made by the GetMatchingChildren methods. This must be set before
        /// Text.
        /// </summary>
        protected string OnlyNodeName { get; set; }
        public bool IndexMatches(int index, string formulaText, string type)
        {
            OnlyNodeName = type;
            return IndexMatches(index, formulaText);
        }
        public bool IndexMatches(int index, string formulaText)
        {
            Text = formulaText;
            if (IsJustNumber)
            {
                return MatchOnlyIndex-1 == index;
            }
            else
            {
                var matchIndex = index += 1; // nthchild is 1 based indices
                if (index > cacheInfo.MaxIndex)
                {
                    
                    UpdateCacheInfo( matchIndex);
                }
                return cacheInfo.MatchingIndices.Contains(matchIndex);       
            }
                
        }

        /// <summary>
        /// Return nth children that match type
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="formula"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<IDomObject> GetMatchingChildren(IDomElement obj, string formula, string type)
        {
            OnlyNodeName = type;
            return GetMatchingChildren(obj, formula);

        }

        public IEnumerable<IDomObject> GetMatchingChildren(IDomElement obj, string formula)
        {
            Text = formula;
            return GetMatchingChildren(obj);
        }
        public IEnumerable<IDomObject> GetMatchingChildren(IDomElement obj)
        {
            if (!obj.HasChildren)
            {
                yield break;
            }
            else if (IsJustNumber)
            {
                int index = 1;
                IDomElement child = obj.FirstChild.NodeType == NodeType.ELEMENT_NODE ?
                    (IDomElement)obj.FirstChild :
                    obj.FirstChild.NextElementSibling;

                while (index++ < MatchOnlyIndex 
                    && child != null 
                    && (String.IsNullOrEmpty(OnlyNodeName) || child.NodeName == OnlyNodeName))
                {
                    child = child.NextElementSibling;
                }

                if (child != null)
                {
                    yield return child;
                }
                else
                {
                    yield break;
                }
            }
            else
            {

                int index = 1;
                UpdateCacheInfo(obj.ChildNodes.Count);
                foreach (var child in obj.ChildElements)
                {
                    if (cacheInfo.MatchingIndices.Contains(index))
                    {
                        yield return child;
                    }
                    index++;
                }
            }
        }
        #endregion

        #region private methods
        protected void UpdateCacheInfo(int lastIndex)
        {
            if (cacheInfo.MaxIndex >= lastIndex)
            {
                return;
            }

            int iterator = cacheInfo.NextIterator;
            int val = -1;
            while (val < lastIndex && iterator <= lastIndex)
            {
                formula.SetVariable("n", iterator);
                val = formula.Value;
                cacheInfo.MatchingIndices.Add(val);
                iterator++;
            }
            cacheInfo.MaxIndex = lastIndex;
            cacheInfo.NextIterator = iterator;
        }
        protected void CheckForSimpleNumber(string equation)
        {
            int matchIndex;
            if (Int32.TryParse(equation, out matchIndex))
            {
                MatchOnlyIndex = matchIndex;
                IsJustNumber = true;

            }
        }
        protected string  CheckForEvenOdd(string equation)
        {
            switch (_Text)
            {
                case "odd":
                    return "2n+1";
                case "even":
                    return "2n";
                default:
                    return equation;
            }
        }
        protected void ParseEquation(string equation)
        {
            equation = CheckForEvenOdd(equation);
            formula = Equations.CreateEquation<int>(equation);

            string cacheKey = (String.IsNullOrEmpty(OnlyNodeName) ? "" : OnlyNodeName + "|") + equation;

            if (!ParsedEquationCache.TryGetValue(cacheKey, out cacheInfo))
            {
                cacheInfo = new CacheInfo();
                cacheInfo.MatchingIndices = new HashSet<int>();
                ParsedEquationCache[equation] = cacheInfo;
            }

        }
        #endregion
    }
}
