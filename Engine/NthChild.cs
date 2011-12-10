using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Jtc.CsQuery.ExtensionMethods;
using Jtc.CsQuery.Utility.EquationParser;
namespace Jtc.CsQuery.Engine
{
    public class CacheInfo
    {
        public HashSet<int> MatchingIndices;
        public int NextIterator;
        public int MaxIndex;
    }
    /// <summary>
    /// Figure out if an index matches an Nth Child, or return a list of all matching elements from a list.
    /// </summary>
    public class NthChild
    {
       /// <summary>
       /// NthChild is expensive but easy to cache. Just save a list of matching element IDs for a given
       /// equation along with the last index this list represents, update it if needed.
       /// </summary>
        private static ConcurrentDictionary<string, CacheInfo> ParsedEquationCache =
            new ConcurrentDictionary<string, CacheInfo>();
        protected CacheInfo CacheInfo;
        protected bool cached = false;
        protected IEquation<int> Formula;
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
                   Formula = Equations.CreateEquation<int>(value);

                   if (!ParsedEquationCache.TryGetValue(value, out CacheInfo))
                   {
                       CacheInfo = new CacheInfo();
                       CacheInfo.MatchingIndices = new HashSet<int>();
                       ParsedEquationCache[value] = CacheInfo;
                   }
                  
                   
               }
            }
        }
        protected string _Text;
        protected string parsedFormula;
        protected bool IsJustNumber;
        protected int MatchOnlyIndex;

        public bool IndexMatches(int index, string formula)
        {
            Text = formula;
            if (IsJustNumber)
            {
                return MatchOnlyIndex-1 == index;
            }
            else
            {
                var matchIndex = index += 1; // nthchild is 1 based indices
                if (index > CacheInfo.MaxIndex)
                {
                    
                    UpdateCacheInfo( matchIndex);
                }
                return CacheInfo.MatchingIndices.Contains(matchIndex);       
            }
                
        }
        protected void UpdateCacheInfo(int lastIndex)
        {
            if (CacheInfo.MaxIndex >= lastIndex)
            {
                return;
            }

            int iterator = CacheInfo.NextIterator;
            int val = -1;
            while (val < lastIndex && iterator <= lastIndex)
            {
                Formula.SetVariable("n", iterator);
                val = Formula.Value;
                CacheInfo.MatchingIndices.Add(val);
                iterator++;
            }
            CacheInfo.MaxIndex = lastIndex;
            CacheInfo.NextIterator = iterator;
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
                    if (CacheInfo.MatchingIndices.Contains(index))
                    {
                        yield return child;
                    }
                    index++;
                }
            }
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
    }
}
