using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.Utility;
using CsQuery.StringScanner;
using CsQuery.StringScanner.Patterns;
using CsQuery.ExtensionMethods.Internal;

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

        #region CSS3 pseudoselectors

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
                if (item.TagName == obj.TagName) {
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
        private static IEnumerable<IDomElement> LastOfTypeImpl(IDomObject parent)
        {
            IDictionary<string, IDomElement> Types = new Dictionary<string, IDomElement>();
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
                throw new ArgumentException("Type must be defined for LastOfTypeImpl.");
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

        public static bool IsOnlyChild(IDomObject elm)
        {
            return OnlyChild(elm.ParentNode) == elm;
        }

        public static IDomObject OnlyChild(IDomObject parent)
        {
            return parent.ChildElements.SingleOrDefaultAlways();
        }

        public static bool IsOnlyOfType(IDomObject elm)
        {
            return OnlyOfTypeImpl(elm.ParentNode, elm.TagName)!=null;
        }

        public static IEnumerable<IDomObject> OnlyOfType(IDomObject parent, string type)
        {
            if (!String.IsNullOrEmpty(type))
            {
                return Enumerate(OnlyOfTypeImpl(parent, type));
            }
            else
            {
                return OnlyOfTypeImpl(parent);
            }
        }
        /// <summary>
        /// When there's no type, it must return all children that are the only one of that type
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static IEnumerable<IDomObject> OnlyOfTypeImpl(IDomObject parent)
        {
            IDictionary<string, IDomElement> Types = new Dictionary<string, IDomElement>();
            foreach (var child in parent.ChildElements)
            {
                if (Types.ContainsKey(child.NodeName))
                {
                    Types[child.NodeName] = null;
                }
                else
                {
                    Types[child.NodeName] = child;
                }
            }
            // if the value is null, there was more than one of the type
            return Types.Values.Where(item=>item!=null);
        }
        public static IDomObject OnlyOfTypeImpl(IDomObject parent, string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentException("Type must be defined for OnlyOfType.");
            }
            return parent.ChildElements
                .Where(item=>item.TagName==type)
                .SingleOrDefaultAlways();
        }


        /// <summary>
        /// Element nodes and non-empty text nodes are considered to be children; empty text nodes, comments,
        /// and processing instructions don’t count as children. A text node is considered empty if it has a data 
        /// length of zero; so, for example, a text node with a single space isn’t empty.
        /// </summary>
        /// <param name="elm"></param>
        /// <returns></returns>
        public static bool IsEmpty(IDomObject elm)
        {
            // try to optimize this by checking for the least labor-intensive things first
            bool simpleEmpty = !elm.HasChildren ||
                elm.ChildNodes.Count==0;

            if (simpleEmpty)
            {
                return true;
            }
            else
            {
                return !elm.ChildNodes
                    .Where(item => item.NodeType == NodeType.TEXT_NODE && 
                        !String.IsNullOrEmpty(item.NodeValue))
                    .Any();
            }
        }

        /// <summary>
        /// Return all child nodes that are empty
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<IDomObject> Empty(IEnumerable<IDomObject> list)
        {
            return list.Where(item => IsEmpty(item));
        }

        #endregion

        #region Selection set position pseudoselectors (jQuery additions)

        public static IEnumerable<IDomObject> OddElements(IEnumerable<IDomObject> list)
        {
            int index = 0;
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

        #endregion

        #region special pseuedo selectors (jquery) 

        
        /// <summary>
        /// Return all child nodes that are visible
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<IDomObject> Visible(IEnumerable<IDomObject> list)
        {
            return list.Where(item => IsVisible(item));
        }

        public static IEnumerable<IDomObject> Hidden(IEnumerable<IDomObject> list)
        {
            return list.Where(item => !IsVisible(item));
        }

        /// <summary>
        /// Tests visibility by inspecting "display", "height" and "width" css & properties for object & all parents.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsVisible(IDomObject obj)
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

        private static bool ElementIsItselfHidden(IDomElement el)
        {
            if (el.HasStyles)
            {
                if (el.Style["display"] == "none" || el.Style.NumberPart("opacity")==0)
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
            string widthAttr, heightAttr;
            widthAttr = el.GetAttribute("width");
            heightAttr = el.GetAttribute("height");

            return widthAttr == "0" || heightAttr == "0";

        }

        public static bool IsHeader(IDomObject el)
        {
            var nodeName = el.NodeName;
            return nodeName[0] == 'H'
                && nodeName.Length == 2
                && nodeName[1] >= '0'
                && nodeName[1] <= '9';

        }
        public static IEnumerable<IDomObject> Headers(IEnumerable<IDomObject> list)
        {
            return list.Where(item => IsHeader(item));
        }

        #endregion
  
        #region utility functions
        
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
        #endregion
    }
}
