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
        protected struct CacheInfo
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
        public string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                _Text = value;
                CheckForSimpleNumber();
                if (!IsJustNumber)
                {
                    formula = Equations.CreateEquation<int>(value);

                    if (!ParsedEquationCache.TryGetValue(value, out cacheInfo))
                    {
                        cacheInfo = new CacheInfo();
                        cacheInfo.MatchingIndices = new HashSet<int>();
                        ParsedEquationCache[value] = cacheInfo;
                    }


                }
            }
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

                while (index++ < MatchOnlyIndex && child != null)
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
        protected void CheckForSimpleNumber()
        {
            int matchIndex;
            if (Int32.TryParse(Text, out matchIndex))
            {
                MatchOnlyIndex = matchIndex;
                IsJustNumber = true;

            }
        }
        #endregion
    }
}
