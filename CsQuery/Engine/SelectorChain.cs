using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using CsQuery.ExtensionMethods;
using CsQuery.Engine;


namespace CsQuery.Engine
{
    public class SelectorChain : IEnumerable<Selector>
    {
        #region constructors
        public SelectorChain(IEnumerable<Selector> selectors)
        {
            Selectors.AddRange(selectors);
        }
        /// <summary>
        /// Create a new selector from any string
        /// </summary>
        /// <param name="selector"></param>
        public SelectorChain(string selector)   
        {
            var parser = new SelectorParser();
            Selectors.AddRange(parser.Parse(selector));
        }
        /// <summary>
        /// Create a new selector from DOM elements
        /// </summary>
        /// <param name="elements"></param>
        public SelectorChain(IEnumerable<IDomObject> elements ) {

            Selector sel = new Selector();
            sel.SelectorType = SelectorType.Elements;
            sel.SelectElements = elements;
            Selectors.Add(sel);
        }
        public SelectorChain(IDomObject element)
        {

            Selector sel = new Selector();
            sel.SelectorType = SelectorType.Elements;
            sel.SelectElements = new List<IDomObject>();
            ((List<IDomObject>)sel.SelectElements).Add(element);
            Selectors.Add(sel);
        }
        #endregion

        #region private properties
        
        protected CssSelectionEngine _Engine;
        protected SelectorParser _selectorParser;

        protected CssSelectionEngine Engine
        {
            get
            {
                if (_Engine == null)
                {
                    _Engine = new CssSelectionEngine();
                    _Engine.Selectors = this;
                }
                return _Engine;
            }
        }
        protected List<Selector> Selectors
        {
            get
            {
                if (_Selectors == null)
                {
                    _Selectors = new List<Selector>();
                }
                return _Selectors;
            }
        } protected List<Selector> _Selectors = null;
        protected IEnumerable<Selector> SelectorsClone
        {
            get
            {
                if (Count > 0)
                {
                    foreach (var selector in Selectors)
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
                return Selectors.Count;
            }
        }
        public Selector this[int index]
        {
            get
            {
                return Selectors[index];
            }
        }
        #endregion

        #region public methods
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
            return Engine.Select(document, context);
        }
        public SelectorChain Clone()
        {
            SelectorChain clone = new SelectorChain(SelectorsClone);
            return clone;

        }
        public override string ToString()
        {
            string output = "";
            bool first=true;
            foreach (var selector in this)
            {
                if (!first && selector.CombinatorType == CombinatorType.Root) {
                    output+=", ";
                }
                output+=selector.ToString();
                first = false;
            }
            return output;
        }
        #endregion

        #region interface members

        public IEnumerator<Selector> GetEnumerator()
        {
            return Selectors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Selectors.GetEnumerator();
        }

        #endregion
    }
}
