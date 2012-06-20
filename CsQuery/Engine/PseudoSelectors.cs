using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.HtmlParser;
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

        /// <summary>
        /// Criteria should be the formula, optionally preceded by a node type filter e.g. "span|2n"
        /// </summary>
        /// <param name="elm"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static bool IsNthChild(IDomElement obj, string criteria)
        {
            return IsNthChildOfTypeImpl(obj, criteria, false);
        }
        /// <summary>
        /// Criteria should be the formula, optionally preceded by a node type filter e.g. "span|2n"
        /// </summary>
        /// <param name="elm"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static bool IsNthLastChild(IDomElement obj, string criteria)
        {
            return IsNthChildOfTypeImpl(obj, criteria,true);
        }

        private static bool IsNthChildOfTypeImpl(IDomElement obj, string criteria, bool fromLast = false)
        {
            

            string[] crit = criteria.Split('|');
            string formula = crit[0];
            string onlyNodeName = "";

            if (crit.Length == 2)
            {
                onlyNodeName = crit[1].ToUpper();
                
            }            

            return NthChildMatcher.IndexMatches(IndexOfTypeOnly(obj,onlyNodeName,fromLast),formula,onlyNodeName,fromLast);
        }

        /// <summary>
        /// Return the index of obj within its siblings, including only elements with the same node name
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static int IndexOfTypeOnly(IDomElement obj, string onlyNodeName, bool fromLast=false)
        {
            // get the index just for this type
            int typeIndex = 0;
            ushort matchNodeId = HtmlData.TokenID(onlyNodeName);
            var childNodes = obj.ParentNode.ChildNodes;
            int length = childNodes.Count;
            bool onlyNodes = !String.IsNullOrEmpty(onlyNodeName);

            for (int index=0;index<length;index++) {
                var el = NthChild.GetEffectiveChild(childNodes,index,fromLast);
                if (el.NodeType == NodeType.ELEMENT_NODE)
                {
                    if (!onlyNodes || el.NodeNameID == matchNodeId)
                    {
                        if (ReferenceEquals(el, obj))
                        {
                            return typeIndex;
                        }
                        typeIndex++;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Criteria should be the formula, optionally preceded by a node type filter e.g. "span|2n"
        /// </summary>
        /// <param name="elm"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static IEnumerable<IDomObject> NthChilds(IDomElement elm, string criteria)
        {
            return NthChildsOfTypeImpl(elm, criteria,false);
        }

        /// <summary>
        /// Criteria should be the formula, optionally preceded by a node type filter e.g. "span|2n"
        /// </summary>
        /// <param name="elm"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static IEnumerable<IDomObject> NthLastChilds(IDomElement elm, string criteria)
        {
            return NthChildsOfTypeImpl(elm, criteria, true);
        }
        public static IEnumerable<IDomObject> NthLastChildsOfType(IDomElement elm, string criteria)
        {
            return NthChildsOfTypeImpl(elm, criteria, true);
        }

        private static IEnumerable<IDomObject> NthChildsOfTypeImpl(IDomElement elm, string criteria, bool fromLast = false)
        {
            string onlyNodeName=null;
            string formula = null;
            
            if (!String.IsNullOrEmpty(criteria))
            {
                string[] crit = criteria.Split('|');
                if (crit.Length > 2)
                {
                    throw new ArgumentOutOfRangeException("Invalid criteria was passed to NthChildsOfType; it must be \"type|equation\"");
                }

                formula = crit[0];

                if (crit.Length == 2)
                {
                    onlyNodeName = crit[1].ToUpper();
                }
            }
            
            return NthChildMatcher.GetMatchingChildren(elm, formula, onlyNodeName, fromLast);
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
            // DOM does not consider the HTML node to be an "only child" so we don't eithter - return nothing for the document node
            return parent==parent.Document ? null :
                parent.ChildElements.SingleOrDefaultAlways();
        }

        public static bool IsOnlyOfType(IDomObject elm)
        {
            return OnlyOfTypeImpl(elm.ParentNode, elm.NodeName)!=null;
        }

        /// <summary>
        /// Return all child elements of parent that are the only children of their type (or a specific type) within parent
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
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
                .Where(item=>item.NodeName==type)
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
            if (elm.HasChildren)
            {
                return false;
            }
            else
            {
                return IsReallyEmpty(elm);
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

        /// <summary>
        /// Return true of the node is a parent.
        /// Element nodes and non-empty text nodes are considered to be children; empty text nodes, comments,
        /// and processing instructions don’t count as children. A text node is considered empty if it has a data 
        /// length of zero; so, for example, a text node with a single space isn’t empty.
        /// </summary>
        /// <param name="elm"></param>
        /// <returns></returns>
        public static bool IsParent(IDomObject elm)
        {
            // try to optimize this by checking for the least labor-intensive things first

            if (!elm.HasChildren)
            {
                return false;
            }
            else
            {
                return !IsReallyEmpty(elm);
            }
        }

        public static IEnumerable<IDomObject> Parent(IEnumerable<IDomObject> list)
        {
            return list.Where(item => !IsEmpty(item));
        }
        private static bool IsReallyEmpty(IDomObject elm)
        {
            return !elm.ChildNodes
                   .Where(item => item.NodeType == NodeType.ELEMENT_NODE ||
                       (item.NodeType == NodeType.TEXT_NODE &&
                       !String.IsNullOrEmpty(item.NodeValue)))
                   .Any();
        }

        /// <summary>
        /// Return all elements of "list" that match selector
        /// </summary>
        /// <param name="list"></param>
        /// <param name="document"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static IEnumerable<IDomObject> Has(IEnumerable<IDomObject> list, IDomDocument document, Selector selector)
        {
            foreach (IDomObject element in list)
            {
                if (selector.Select(document, element).Any()) {
                    yield return element;
                }
            }
        }
        public static IEnumerable<IDomObject> Not(IEnumerable<IDomObject> list, IDomDocument document, Selector selector)
        {
            foreach (IDomObject element in list)
            {
                if (!selector.Select(document, element).Any())
                {
                    yield return element;
                }
            }
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
