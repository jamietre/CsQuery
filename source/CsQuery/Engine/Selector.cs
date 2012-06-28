using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.Engine;


namespace CsQuery.Engine
{
    public class Selector : IEnumerable<SelectorClause>
    {
        #region constructors
        
        public Selector()
        {

        }

        public Selector(SelectorClause selector)
        {
            Clauses.Add(selector);
        }

        public Selector(IEnumerable<SelectorClause> selectors)
        {
            Clauses.AddRange(selectors);
        }
        /// <summary>
        /// Create a new selector from any string
        /// </summary>
        /// <param name="selector"></param>
        public Selector(string selector)   
        {
            var parser = new SelectorParser();
            Clauses.AddRange(parser.Parse(selector));
        }
        /// <summary>
        /// Create a new selector from DOM elements
        /// </summary>
        /// <param name="elements"></param>
        public Selector(IEnumerable<IDomObject> elements ) {

            SelectorClause sel = new SelectorClause();
            sel.SelectorType = SelectorType.Elements;
            sel.SelectElements = elements;
            Clauses.Add(sel);
        }
        public Selector(IDomObject element)
        {

            SelectorClause sel = new SelectorClause();
            sel.SelectorType = SelectorType.Elements;
            sel.SelectElements = new List<IDomObject>();
            ((List<IDomObject>)sel.SelectElements).Add(element);
            Clauses.Add(sel);
        }

        #endregion

        #region public methods

        public void Add(SelectorClause clause)
        {

            // TODO: We'd like to prevent duplicate clauses, but in order to do so, they need to be combined into 
            // complete selectors (e.g. sets bounded by CombinatorType.Root). That really should be the definition of a 
            // selector, e.g. each part separated by a comma

            //if (clause.CombinatorType == CombinatorType.Root && Clauses.Contains(clause))
            //{
            //    return;
            //}
            Clauses.Add(clause);

        }

        #endregion

        #region private properties

        protected CssSelectionEngine _Engine;
        protected SelectorParser _selectorParser;
        protected List<SelectorClause> _Clauses;

        protected CssSelectionEngine GetEngine(IDomDocument document)
        {
            
            var engine = new CssSelectionEngine(document);
            engine.Selectors = this;
            return engine;
        }
        protected List<SelectorClause> Clauses
        {
            get
            {
                if (_Clauses == null)
                {
                    _Clauses = new List<SelectorClause>();
                }
                return _Clauses;
            }
        } 
        protected IEnumerable<SelectorClause> SelectorsClone
        {
            get
            {
                if (Count > 0)
                {
                    foreach (var selector in Clauses)
                    {
                        yield return selector.Clone();
                    }
                }
                else
                {
                    yield break;
                }
            }
        }

        #endregion

        #region public properties
        public int Count
        {
            get
            {
                return Clauses.Count;
            }
        }
        public SelectorClause this[int index]
        {
            get
            {
                return Clauses[index];
            }
        }
        #endregion

        #region public methods


        /// <summary>
        /// Insert a selector clause at the specified position
        /// </summary>
        /// <param name="index"></param>
        /// <param name="selector"></param>
        public void Insert(int index, SelectorClause clause, CombinatorType combinatorType = CombinatorType.Chained)
        {
            if (combinatorType == CombinatorType.Root && Clauses.Count!=0) {
                throw new ArgumentException("Combinator type can only be root if there are no other selectors.");
            }

            if (Clauses.Count > 0 && index == 0)
            {
                Clauses[0].CombinatorType = combinatorType;
                clause.CombinatorType = CombinatorType.Root;
                clause.TraversalType = TraversalType.All;

            }
            Clauses.Insert(index, clause);

        }

        public IEnumerable<IDomObject> Is(IDomDocument root, IDomObject element)
        {
            List<IDomObject> list = new List<IDomObject>();
            list.Add(element);
            return Select(root, list);
        }
        public IEnumerable<IDomObject> Select(IDomDocument document)
        {
            return Select(document, (IEnumerable<IDomObject>)null);
        }
        public IEnumerable<IDomObject> Select(IDomDocument document, IDomObject context)
        {
            return Select(document, Objects.Enumerate(context));
        }
        public IEnumerable<IDomObject> Select(IDomDocument document, IEnumerable<IDomObject> context)
        {
            return GetEngine(document).Select(context);
        }

        /// <summary>
        /// Return only elements matching this selector
        /// </summary>
        /// <param name="document"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public IEnumerable<IDomObject> Filter(IDomDocument document, IEnumerable<IDomObject> sequence)
        {
            // This needs to be two steps - returning the selection set directly will cause the sequence
            // to be ordered in DOM order, and not its original order.

            HashSet<IDomObject> matches = new HashSet<IDomObject>(GetFilterSelector().Select(document, sequence));
            
            foreach (var item in sequence) {
                 if (matches.Contains(item))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Test if a single element matches this selector
        /// </summary>
        /// <param name="document"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public bool Matches(IDomDocument document, IDomObject element)
        {
            return GetFilterSelector().Select(document, element).Any();
        }

        /// <summary>
        /// Return only elements from the sequence that do not match this selector.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public IEnumerable<IDomObject> Except(IDomDocument document, IEnumerable<IDomObject> sequence)
        {
            HashSet<IDomObject> matches = new HashSet<IDomObject>(GetFilterSelector().Select(document, sequence));
            foreach (var item in sequence)
            {
                if (!matches.Contains(item))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Returns a clone of this selector with all "root" combinators mapped to "filter", e.g. so it can be applied to 
        /// a sequence.
        /// </summary>
        protected Selector GetFilterSelector() {
             var clone = Clone();

             clone.Where(item => item.CombinatorType == CombinatorType.Root)
                    .ForEach(item => item.TraversalType = TraversalType.Filter);
             
            return clone;

        }
        /// <summary>
        /// Return a clone of this selector
        /// </summary>
        /// <returns></returns>
        public Selector Clone()
        {
            Selector clone = new Selector(SelectorsClone);
            return clone;
        }
        public override string ToString()
        {
            string output = "";
            bool first=true;
            foreach (var selector in this)
            {
                if (!first) {
                    if (selector.CombinatorType == CombinatorType.Root)
                    {
                        output += ",";
                    }
                    else if (selector.CombinatorType == CombinatorType.And)
                    {
                        output += "&";
                    }
                }
                output+=selector.ToString();
                first = false;
            }
            return output;
        }
        #endregion

        #region interface members

        public IEnumerator<SelectorClause> GetEnumerator()
        {
            return Clauses.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Clauses.GetEnumerator();
        }

        #endregion
    }
}
