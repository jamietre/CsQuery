using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jtc.CsQuery.ExtensionMethods;
using Jtc.CsQuery.Utility.EquationParser;
namespace Jtc.CsQuery.Engine
{
    /// <summary>
    /// Figure out if an index matches an Nth Child, or return a list of all matching elements from a list.
    /// DON'T MAKE FUN! This is dirty. It works.
    /// </summary>
    public class NthChild
    {
        protected IEquation<int> Formula
        {
            get
            {
                if (_Formula == null)
                {
                    _Formula = Equations.CreateEquation<int>();
                }
                return _Formula;
            }
        }
        private IEquation<int> _Formula;
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
                   Formula.Parse(value);
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

            index = index + 1; // nthchild is 1 based indices

            int iterator = 0;
            int val = -1;
            while (val < index && iterator <= index)
            {
                Formula.SetVariable<int>("n", iterator);
                val = Formula.Calculate();
                iterator++;
            }
            return val == index;
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
                int iterator = 0;
                int nextValid = -1, lastValid = -1;
                foreach (var child in obj.ChildElements)
                {
                    while (nextValid < index)
                    {
                        lastValid = nextValid;
                        Formula.SetVariable("n", iterator++);
                        nextValid = Formula.Calculate();
                        if (nextValid <= lastValid)
                        {
                            yield break;
                        }
                    }
                    if (nextValid == index)
                    {
                        yield return child;
                    }
                    index++;
                }
            }
        }
        protected void CheckForSimpleNumber()
        {
            if (Int32.TryParse(Text, out MatchOnlyIndex))
            {
                IsJustNumber = true;
            }
        }
    }
}
