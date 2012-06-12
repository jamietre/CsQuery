using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using CsQuery.ExtensionMethods;
using CsQuery.EquationParser;
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

        /// <summary>
        /// When true, the current equation is just a number, and the MatchOnlyIndex value should be used directly
        /// </summary>
        protected bool IsJustNumber;
        protected int MatchOnlyIndex;
        /// <summary>
        /// Only nodes with this name will be included in the count to determine if an index matches the equation
        /// </summary>
        protected string OnlyNodeName
        {
            get
            {
                return _OnlyNodeName;
            }
            set
            {
                _OnlyNodeName = !String.IsNullOrEmpty(value) ?
                    value.ToUpper() :
                    null;
            }

        }
        private string _OnlyNodeName;

        protected bool FromLast;

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
        /// Return true if the index matches the formula provided
        /// </summary>
        /// <param name="index"></param>
        /// <param name="formulaText"></param>
        /// <param name="onlyNodeName">Only include nodes of this type</param>
        /// <param name="fromLast">Count from the last element instead of the first</param>
        /// <returns></returns>
        public bool IndexMatches(int index, string formulaText, string onlyNodeName=null, bool fromLast=false)
        {
            OnlyNodeName = onlyNodeName;
            FromLast = fromLast;
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
        /// <param name="obj">The parent object</param>
        /// <param name="formula">The formula for determining n</param>
        /// <param name="onlyNodeName">The type of node to match</param>
        /// <returns></returns>
        public IEnumerable<IDomObject> GetMatchingChildren(IDomElement obj, string formula, string onlyNodeName=null, bool fromLast=false)
        {
            OnlyNodeName = onlyNodeName;
            FromLast = fromLast;
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
                IDomElement child = GetNthChild(obj,MatchOnlyIndex);

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
                UpdateCacheInfo(obj.ChildNodes.Count);

                int elementIndex = 1;
                int newActualIndex=-1;

                IDomElement el = GetNextChild(obj, -1, out newActualIndex);
                while (newActualIndex >= 0)
                {
                    if (cacheInfo.MatchingIndices.Contains(elementIndex))
                    {
                        yield return el;
                    }
                    el = GetNextChild(obj, newActualIndex, out newActualIndex);
                    elementIndex++;
                }
            }
        }

        #endregion

        #region public static methods

        /// <summary>
        /// Return the correct child from a list based on an index, and the fromLast setting
        /// </summary>
        /// <param name="nodeList"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static IDomObject GetEffectiveChild(INodeList nodeList, int index, bool fromLast)
        {
            if (fromLast)
            {
                return nodeList[nodeList.Length - index - 1];
            }
            else
            {
                return nodeList[index];
            }
        }

        #endregion

        #region private methods

        private IDomElement GetNthChild(IDomElement parent, int index)
        {
            int newActualIndex;
            int elementIndex = 1;
            IDomElement nthChild = GetNextChild(parent,-1, out newActualIndex);

            while (nthChild != null && elementIndex != index)
            {
                nthChild = GetNextChild(parent, newActualIndex, out newActualIndex);
                elementIndex++;
            }
            return nthChild;
        }

        private IDomElement GetNextChild(IDomElement parent, int currentIndex, out int newIndex)
        {

            int index = currentIndex;
            

            var children = parent.ChildNodes;
            int count = children.Count;
            IDomObject effectiveNextChild = null;

            while (++index < count) {
                effectiveNextChild= GetEffectiveChild(children, index);
                if (effectiveNextChild.NodeType == NodeType.ELEMENT_NODE)
                {
                    break;
                }
            }
             

            if (index < count)
            {
                newIndex = index;
                return (IDomElement)effectiveNextChild;
            }
            else
            {
                newIndex = -1;
                return null;
            }
        }

        /// <summary>
        /// Return the correct child from a list based on an index, and the current "FromLast" setting
        /// </summary>
        /// <param name="nodeList"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private IDomObject GetEffectiveChild(INodeList nodeList, int index)
        {
            return GetEffectiveChild(nodeList, index, FromLast);
        }
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
        /// <summary>
        /// Replaces _Text with the correct equation for "even" and "odd"
        /// </summary>
        /// <param name="equation"></param>
        /// <returns></returns>
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

            string cacheKey = (String.IsNullOrEmpty(OnlyNodeName) ? "" : OnlyNodeName + "|") +
                (FromLast ? "1|" : "0|") +
                equation;

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
