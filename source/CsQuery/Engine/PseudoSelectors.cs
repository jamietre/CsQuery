using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using CsQuery.HtmlParser;
using CsQuery.StringScanner;
using CsQuery.StringScanner.Patterns;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.Engine
{
    public class PseudoSelectors: IDictionary<string,Type>
    {

        #region contructors

        static PseudoSelectors()
        {
            Items = new PseudoSelectors();
            PopulateInnerSelectors();
        }

        /// <summary>
        /// Default constructor/.
        /// </summary>
        ///
        /// <exception cref="Exception">
        /// Throws an exception if an instance has already been assigned to the static Items property.
        /// This class should instantiate itself as a singleton.
        /// </exception>

        public PseudoSelectors()
        {
            if (Items!=null) {
                throw new Exception("You can only create one instance of the PseudoSelectors class.");
            }
            InnerSelectors = new Dictionary<string, Type>();
        }

        private static void PopulateInnerSelectors()
        {
            string nameSpace = "CsQuery.Engine.PseudoClassSelectors";
            var assy = Assembly.GetExecutingAssembly();
            bool foundTypes=false;
            foreach (var t in assy.GetTypes())
            {
                if (t.IsClass && t.Namespace!=null && 
                    !t.IsAbstract &&
                    t.Namespace.StartsWith(nameSpace))
                {
                    if (t.GetInterface("IPseudoSelector") != null)
                    {
                        foundTypes = true;
                        Items.Add(Objects.FromCamelCase(t.Name), t);
                    }
                    
                }
            }
            if (!foundTypes)
            {
                throw new InvalidOperationException("Could not find any default PseudoClassSelectors. Did you change a namespace?");
            }
        }

        #endregion

        #region private properties

        private IDictionary<string, Type> InnerSelectors;

        #endregion


        #region public properties

        /// <summary>
        /// Static instance of the PseudoSelectors singleton.
        /// </summary>

        public static PseudoSelectors Items { get; protected set; }

        #endregion

        #region public methods

        /// <summary>
        /// Gets an instance of a named pseudoselector
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when the pseudoselector does not exist
        /// </exception>
        ///
        /// <param name="name">
        /// The name of the pseudoselector
        /// </param>
        ///
        /// <returns>
        /// A new instance
        /// </returns>

        public IPseudoSelector GetInstance(string name) 
        {
            Type ps;
            if (InnerSelectors.TryGetValue(name, out ps))
            {
                return (IPseudoSelector)Activator.CreateInstance(ps);
            }
            else
            {
                throw new ArgumentException(String.Format("Attempt to use nonexistent pseudoselector :{0}", name));
            }
        }

        /// <summary>
        /// Try to gets an instance of a named pseudoselector.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the pseudoselector.
        /// </param>
        /// <param name="instance">
        /// [out] The new instance.
        /// </param>
        ///
        /// <returns>
        /// true if succesful, false if a pseudoselector of that name doesn't exist.
        /// </returns>

        public bool TryGetInstance(string name, out IPseudoSelector instance) {
            Type ps;
            if (InnerSelectors.TryGetValue(name, out ps))
            {
                instance = (IPseudoSelector)Activator.CreateInstance(ps);
                return true;
            }
            else
            {
                instance = null;
                return false;
            }
        }

        #endregion


        private static NthChildMatcher NthChildMatcher
        {
            get
            {
                return new NthChildMatcher();
            }
        }


        #region CSS3 pseudoselectors

        /// <summary>
        /// Criteria should be the formula, optionally preceded by a node type filter e.g. "span|2n"
        /// </summary>
        /// <param name="elm"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        //public static bool IsNthChild(IDomObject obj, string criteria)
        //{
        //    return obj.NodeType != NodeType.ELEMENT_NODE ? false :
        //        NthChildMatcher.IsNthChildOfTypeImpl((IDomElement)obj, criteria, null,false);
        //}
        /// <summary>
        /// Criteria should be the formula, optionally preceded by a node type filter e.g. "span|2n"
        /// </summary>
        /// <param name="elm"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        //public static bool IsNthLastChild(IDomObject obj, string criteria)
        //{
        //    //string[] c = GetCriteria(criteria);
        //    return obj.NodeType != NodeType.ELEMENT_NODE ? false :
        //        NthChildMatcher.IsNthChildOfTypeImpl((IDomElement)obj, criteria,null,true);
        //}

        private static string[] GetCriteria(string criteria)
        {

            return criteria.Split('|');
        }
        //private static bool IsNthChildOfTypeImpl(IDomElement obj, string criteria, bool fromLast = false)
        //{
            

        //    string[] crit = criteria.Split('|');
        //    string formula = crit[0];
        //    string onlyNodeName = "";

        //    if (crit.Length == 2)
        //    {
        //        onlyNodeName = crit[1].ToUpper();
                
        //    }            

        //    return NthChildMatcher.IndexMatches(IndexOfTypeOnly(obj,onlyNodeName,fromLast),formula,onlyNodeName,fromLast);
        //}

       
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

        //public static bool IsFirstOfType(IDomObject elm, string type) {
        //    if (!String.IsNullOrEmpty(type))
        //    {
        //        return FirstOfTypeImpl(elm.ParentNode, type) == elm;
        //    }
        //    else
        //    {
        //        return FirstOfTypeImpl(elm.ParentNode).Contains(elm);
        //    }
        //}

        //public static IEnumerable<IDomObject> FirstOfType(IDomObject parent, string type)
        //{
        //    if (!String.IsNullOrEmpty(type))
        //    {
        //        return Enumerate(FirstOfTypeImpl(parent, type));
        //    }
        //    else
        //    {
        //        return FirstOfTypeImpl(parent);
        //    }
           
        //}
        //private static IEnumerable<IDomObject> FirstOfTypeImpl(IDomObject parent)
        //{
        //    HashSet<string> Types = new HashSet<string>();
        //    foreach (var child in parent.ChildElements)
        //    {
        //        if (!Types.Contains(child.NodeName))
        //        {
        //            Types.Add(child.NodeName);
        //            yield return child;
        //        }
        //    }
        //}
        //private static IDomObject FirstOfTypeImpl(IDomObject parent, string type)
        //{
        //    if (string.IsNullOrEmpty(type))
        //    {
        //        throw new ArgumentException("Type is not defined.");
        //    }
        //    foreach (var child in parent.ChildElements)
        //    {
        //        if (child.NodeName == type)
        //        {
        //            return child;
        //        }
        //    }
        //    return null;
        //}
        //public static bool IsLastOfType(IDomObject elm, string type)
        //{
        //    if (!String.IsNullOrEmpty(type))
        //    {
        //        return LastOfTypeImpl(elm.ParentNode, type) == elm;
        //    }
        //    else
        //    {
        //        return LastOfTypeImpl(elm.ParentNode).Contains(elm);
        //    }
        //}

        //public static IEnumerable<IDomObject> LastOfType(IDomObject parent, string type)
        //{
        //    if (!String.IsNullOrEmpty(type))
        //    {
        //        return Enumerate(LastOfTypeImpl(parent, type));
        //    }
        //    else
        //    {
        //        return LastOfTypeImpl(parent);
        //    }
        //}
        //private static IEnumerable<IDomElement> LastOfTypeImpl(IDomObject parent)
        //{
        //    IDictionary<string, IDomElement> Types = new Dictionary<string, IDomElement>();
        //    foreach (var child in parent.ChildElements)
        //    {
        //        Types[child.NodeName] = child;
        //    }
        //    return Types.Values;
        //}
        //private static IDomObject LastOfTypeImpl(IDomObject parent, string type)
        //{
        //    if (string.IsNullOrEmpty(type))
        //    {
        //        throw new ArgumentException("Type must be defined for LastOfTypeImpl.");
        //    }

        //    IDomObject last = null;
        //    foreach (var child in parent.ChildElements)
        //    {
        //        if (child.NodeName == type)
        //        {
        //            last= child;
        //        }
        //    }
        //    return last;
        //}

        //public static bool IsFirstChild(IDomObject elm)
        //{
        //    return FirstChild(elm.ParentNode) == elm;
        //}

        //public static IDomObject FirstChild(IDomObject parent)
        //{
        //    return parent.FirstElementChild;
        //}


        //public static bool IsLastChild(IDomObject elm)
        //{
        //    return LastChild(elm.ParentNode) == elm;
        //}

        //public static IDomObject LastChild(IDomObject parent)
        //{
        //    return parent.LastElementChild;
        //}

        //public static bool IsOnlyChild(IDomObject elm)
        //{
        //    return OnlyChild(elm.ParentNode) == elm;
        //}

        //public static IDomObject OnlyChild(IDomObject parent)
        //{
        //    // DOM does not consider the HTML node to be an "only child" so we don't eithter - return nothing for the document node
        //    return parent==parent.Document ? null :
        //        parent.ChildElements.SingleOrDefaultAlways();
        //}

        //public static bool IsOnlyOfType(IDomObject elm)
        //{
        //    return OnlyOfTypeImpl(elm.ParentNode, elm.NodeName)!=null;
        //}

        /// <summary>
        /// Return all child elements of parent that are the only children of their type (or a specific type) within parent
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        //public static IEnumerable<IDomObject> OnlyOfType(IDomObject parent, string type)
        //{
        //    if (!String.IsNullOrEmpty(type))
        //    {
        //        return Enumerate(OnlyOfTypeImpl(parent, type));
        //    }
        //    else
        //    {
        //        return OnlyOfTypeImpl(parent);
        //    }
        //}
        ///// <summary>
        ///// When there's no type, it must return all children that are the only one of that type
        ///// </summary>
        ///// <param name="parent"></param>
        ///// <returns></returns>
        //public static IEnumerable<IDomObject> OnlyOfTypeImpl(IDomObject parent)
        //{
        //    IDictionary<string, IDomElement> Types = new Dictionary<string, IDomElement>();
        //    foreach (var child in parent.ChildElements)
        //    {
        //        if (Types.ContainsKey(child.NodeName))
        //        {
        //            Types[child.NodeName] = null;
        //        }
        //        else
        //        {
        //            Types[child.NodeName] = child;
        //        }
        //    }
        //    // if the value is null, there was more than one of the type
        //    return Types.Values.Where(item=>item!=null);
        //}
        //public static IDomObject OnlyOfTypeImpl(IDomObject parent, string type)
        //{
        //    if (string.IsNullOrEmpty(type))
        //    {
        //        throw new ArgumentException("Type must be defined for OnlyOfType.");
        //    }
        //    return parent.ChildElements
        //        .Where(item=>item.NodeName==type)
        //        .SingleOrDefaultAlways();
        //}


        /// <summary>
        /// Element nodes and non-empty text nodes are considered to be children; empty text nodes, comments,
        /// and processing instructions don’t count as children. A text node is considered empty if it has a data 
        /// length of zero; so, for example, a text node with a single space isn’t empty.
        /// </summary>
        /// <param name="elm"></param>
        /// <returns></returns>
        //public static bool IsEmpty(IDomObject elm)
        //{
        //    // try to optimize this by checking for the least labor-intensive things first
        //    if (elm.HasChildren)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return IsReallyEmpty(elm);
        //    }
        //}

        /// <summary>
        /// Return all child nodes that are empty
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        //public static IEnumerable<IDomObject> Empty(IEnumerable<IDomObject> list)
        //{
        //    return list.Where(item => IsEmpty(item));
        //}

        /// <summary>
        /// Return true of the node is a parent.
        /// Element nodes and non-empty text nodes are considered to be children; empty text nodes, comments,
        /// and processing instructions don’t count as children. A text node is considered empty if it has a data 
        /// length of zero; so, for example, a text node with a single space isn’t empty.
        /// </summary>
        /// <param name="elm"></param>
        /// <returns></returns>
        //public static bool IsParent(IDomObject elm)
        //{
        //    // try to optimize this by checking for the least labor-intensive things first

        //    if (!elm.HasChildren)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return !IsReallyEmpty(elm);
        //    }
        //}

        //public static IEnumerable<IDomObject> Parent(IEnumerable<IDomObject> list)
        //{
        //    return list.Where(item => !IsEmpty(item));
        //}
        //private static bool IsReallyEmpty(IDomObject elm)
        //{
        //    return !elm.ChildNodes
        //           .Where(item => item.NodeType == NodeType.ELEMENT_NODE ||
        //               (item.NodeType == NodeType.TEXT_NODE &&
        //               !String.IsNullOrEmpty(item.NodeValue)))
        //           .Any();
        //}

        /// <summary>
        /// Return all elements of "list" that match selector
        /// </summary>
        /// <param name="list"></param>
        /// <param name="document"></param>
        /// <param name="criteria"></param>
        ///// <returns></returns>
        //public static IEnumerable<IDomObject> Has(IEnumerable<IDomObject> list, IDomDocument document, Selector selector)
        //{
        //    foreach (IDomObject element in list)
        //    {
        //        if (selector.Select(document, element).Any()) {
        //            yield return element;
        //        }
        //    }
        //}
        //public static IEnumerable<IDomObject> Not(IEnumerable<IDomObject> list, IDomDocument document, Selector selector)
        //{
        //    foreach (IDomObject element in list)
        //    {
        //        if (!selector.Select(document, element).Any())
        //        {
        //            yield return element;
        //        }
        //    }
        //}
        
        #endregion

        #region Selection set position pseudoselectors (jQuery additions)

        //public static IEnumerable<IDomObject> Extension(IDomObject obj, int index, string data) {
        //    var childs = obj.ChildNodes;
        //    for (int i=0;i<childs.Count;i++) {
        //        if (IsExtension(childs[i],i,data)) {
        //            yield return childs[i];
        //        }
        //    }
        //}

        //public static bool IsExtension(IDomObject obj, int index, string data)
        //{

        //    string[] parms = data.Split('|');
        //    string args = parms.Length ==1 ? 
        //        null : 
        //        parms[1];
            
        //    var func = GetExtension(parms[0]);
        //    return func(obj, args);
        //}

        //public static IEnumerable<IDomObject> OddElements(IEnumerable<IDomObject> list)
        //{
        //    int index = 0;
        //    foreach (var child in list)
        //    {
        //        if (index % 2 != 0)
        //        {
        //            yield return child;
        //        }
        //        index++;
        //    }
        //}

        //public static IEnumerable<IDomObject> EvenElements(IEnumerable<IDomObject> list)
        //{
        //    int index = 0;
        //    foreach (var child in list)
        //    {
        //        if (index % 2 == 0)
        //        {
        //            yield return child;
        //        }
        //        index++;
        //    }
        //}

        //public static IDomObject ElementAtIndex(IEnumerable<IDomObject> list, int index)
        //{
        //    if (index < 0)
        //    {
        //        index = list.Count() + index;
        //    }
        //    bool ok = true;
        //    IEnumerator<IDomObject> enumerator = list.GetEnumerator();
        //    for (int i = 0; i <= index && ok; i++)
        //    {
        //        ok = enumerator.MoveNext();
        //    }
        //    if (ok)
        //    {
        //        return enumerator.Current;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        //public static IEnumerable<IDomObject> IndexGreaterThan(IEnumerable<IDomObject> list, int position)
        //{
        //    int index = 0;
        //    foreach (IDomObject obj in list)
        //    {
        //        if (index++ >  position)
        //        {
        //            yield return obj;
        //        }
        //    }
        //}
        //public static IEnumerable<IDomObject> IndexLessThan(IEnumerable<IDomObject> list, int position)
        //{
        //    int index = 0;
        //    foreach (IDomObject obj in list)
        //    {
        //        if (index++ < position)
        //        {
        //            yield return obj;
        //        }
        //    }
        //}

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
            if (el.NodeNameID == HtmlData.tagINPUT && el.Type == "hidden")
            {
                return true;
            }

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

        //public static bool IsHeader(IDomObject el)
        //{
        //    var nodeName = el.NodeName;
        //    return nodeName[0] == 'H'
        //        && nodeName.Length == 2
        //        && nodeName[1] >= '0'
        //        && nodeName[1] <= '9';

        //}
        //public static IEnumerable<IDomObject> Headers(IEnumerable<IDomObject> list)
        //{
        //    return list.Where(item => IsHeader(item));
        //}

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


        private void ValidateType(Type value)
        {
            if (value.GetInterface("IPseudoSelector")==null)
            {
                throw new ArgumentException("The type must implement IPseudoSelector.");
            }
        }
        public void Add(string key, Type value)
        {
            ValidateType(value);
            InnerSelectors.Add(key,value);
        }

        public bool ContainsKey(string key)
        {
            return InnerSelectors.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return InnerSelectors.Keys; }
        }

        public bool Remove(string key)
        {
            return InnerSelectors.Remove(key);
        }

        public bool TryGetValue(string key, out Type value)
        {
            return InnerSelectors.TryGetValue(key, out value);
        }

        public ICollection<Type> Values
        {
            get {return InnerSelectors.Values; }
        }

        public Type this[string key]
        {
            get
            {
                return InnerSelectors[key];
            }
            set
            {
                ValidateType(value);
                InnerSelectors[key] = value;
                
            }
        }

        public void Add(KeyValuePair<string, Type> item)
        {
            ValidateType(item.Value);
            InnerSelectors.Add(item);

        }

        public void Clear()
        {
            InnerSelectors.Clear();
        }

        public bool Contains(KeyValuePair<string, Type> item)
        {
            return InnerSelectors.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, Type>[] array, int arrayIndex)
        {
            InnerSelectors.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get {return InnerSelectors.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, Type> item)
        {
            return InnerSelectors.Remove(item);
        }

        public IEnumerator<KeyValuePair<string, Type>> GetEnumerator()
        {
            return InnerSelectors.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
