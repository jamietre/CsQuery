using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.Utility;
using CsQuery.Utility.StringScanner;
using CsQuery.Utility.StringScanner.Patterns;

namespace CsQuery.Engine
{
    public class PseudoSelectors
    {
        #region private properties

        private static NthChild NthChildMatcher
        {
            get
            {
                return new NthChild();
            }
        }
        
        #endregion

        public static bool IsNthChild(IDomElement obj, string criteria)
        {
            return NthChildMatcher.IndexMatches(obj.ElementIndex, criteria);
        }

        public static bool IsNthChildOfType(IDomElement obj, string criteria)
        {
            string[] crit = criteria.Split('|');
            if (crit.Length != 2)
            {
                throw new Exception("Invalid criteria was passed to IsNthChildsOfType; it must be \"type|equation\"");
            }
            // get the index just for this type
            int typeIndex=0;
            foreach (var item in obj.ParentNode.ChildElements) {
                if (item == obj) {
                    break;
                }
                if (item.NodeName == obj.NodeName) {
                    typeIndex++;
                }
            }

            return NthChildMatcher.IndexMatches(typeIndex, crit[1], crit[0]);
        }

        public static IEnumerable<IDomObject> NthChilds(IDomElement elm, string criteria)
        {
            return NthChildMatcher.GetMatchingChildren(elm, criteria);
        }

        public static IEnumerable<IDomObject> NthChildsOfType(IDomElement elm, string criteria)
        {
            string[] crit = criteria.Split('|');
            if (crit.Length!=2) {
                throw new Exception("Invalid criteria was passed to NthChildsOfType; it must be \"type|equation\"");
            }
            return NthChildMatcher.GetMatchingChildren(elm, crit[1], crit[0]);
        }

        public static bool IsFirstOfType(IDomObject elm, string type) {
            if (!String.IsNullOrEmpty(type))
            {
                return FirstOfTypeImpl(elm.ParentNode, type) == elm;
            }
            else
            {
                return FirstOfTypeImpl(elm.ParentNode).Contains(elm);
            }
        }

        public static IEnumerable<IDomObject> FirstOfType(IDomObject parent, string type)
        {
            if (!String.IsNullOrEmpty(type))
            {
                return Enumerate(FirstOfTypeImpl(parent, type));
            }
            else
            {
                return FirstOfTypeImpl(parent);
            }
           
        }
        private static IEnumerable<IDomObject> FirstOfTypeImpl(IDomObject parent)
        {
            HashSet<string> Types = new HashSet<string>();
            foreach (var child in parent.ChildElements)
            {
                if (!Types.Contains(child.NodeName))
                {
                    Types.Add(child.NodeName);
                    yield return child;
                }
            }
        }
        private static IDomObject FirstOfTypeImpl(IDomObject parent, string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentException("Type is not defined.");
            }
            foreach (var child in parent.ChildElements)
            {
                if (child.NodeName == type)
                {
                    return child;
                }
            }
            return null;
        }
        public static bool IsLastOfType(IDomObject elm, string type)
        {
            if (!String.IsNullOrEmpty(type))
            {
                return LastOfTypeImpl(elm.ParentNode, type) == elm;
            }
            else
            {
                return LastOfTypeImpl(elm.ParentNode).Contains(elm);
            }
        }

        public static IEnumerable<IDomObject> LastOfType(IDomObject parent, string type)
        {
            if (!String.IsNullOrEmpty(type))
            {
                return Enumerate(LastOfTypeImpl(parent, type));
            }
            else
            {
                return LastOfTypeImpl(parent);
            }
        }
        private static IEnumerable<IDomObject> LastOfTypeImpl(IDomObject parent)
        {
            IDictionary<string, IDomObject> Types = new Dictionary<string, IDomObject>();
            foreach (var child in parent.ChildElements)
            {
                Types[child.NodeName] = child;
            }
            return Types.Values;
        }
        private static IDomObject LastOfTypeImpl(IDomObject parent, string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentException("Type is not defined.");
            }
            IDomObject last = null;
            foreach (var child in parent.ChildElements)
            {
                if (child.NodeName == type)
                {
                    last= child;
                }
            }
            return last;
        }

        public static bool IsFirstChild(IDomObject elm)
        {
            return FirstChild(elm.ParentNode) == elm;
        }

        public static IDomObject FirstChild(IDomObject parent)
        {
            return parent.FirstElementChild;
        }


        public static bool IsLastChild(IDomObject elm)
        {
            return LastChild(elm.ParentNode) == elm;
        }

        public static IDomObject LastChild(IDomObject parent)
        {
            return parent.LastElementChild;
        }

        public static IEnumerable<IDomObject> OddElements(IEnumerable<IDomObject> list)
        {
            int index=0;
            foreach (var child in list)
            {
                if (index % 2 != 0)
                {
                    yield return child;
                }
                index++;
            }
        }
        public static IEnumerable<IDomObject> EvenElements(IEnumerable<IDomObject> list)
        {
            int index = 0;
            foreach (var child in list)
            {
                if (index % 2 == 0)
                {
                    yield return child;
                }
                index++;
            }
        }

        public static IDomObject ElementAtIndex(IEnumerable<IDomObject> list, int index)
        {
            if (index < 0)
            {
                index = list.Count() + index;
            }
            bool ok = true;
            IEnumerator<IDomObject> enumerator = list.GetEnumerator();
            for (int i = 0; i <= index && ok; i++)
            {
                ok = enumerator.MoveNext();
            }
            if (ok)
            {
                return enumerator.Current;
            }
            else
            {
                return null;
            }
        }

        public static IEnumerable<IDomObject> IndexGreaterThan(IEnumerable<IDomObject> list, int position)
        {
            int index = 0;
            foreach (IDomObject obj in list)
            {
                if (index++ >  position)
                {
                    yield return obj;
                }
            }
        }
        public static IEnumerable<IDomObject> IndexLessThan(IEnumerable<IDomObject> list, int position)
        {
            int index = 0;
            foreach (IDomObject obj in list)
            {
                if (index++ < position)
                {
                    yield return obj;
                }
            }
        }
        /// <summary>
        /// Yield nothing if obj is null, or the object if not
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<IDomObject> Enumerate(IDomObject obj)
        {
            if (obj == null)
            {
                yield break;
            }
            else
            {
                yield return obj;
            }

        }
    }
}
