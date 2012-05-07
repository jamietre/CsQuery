using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Engine
{
     ///<summary>
     /// Wrapper class used by the engine to store info on the selector stack.
     ///</summary>
    public class MatchElement
    {
        public MatchElement(IDomObject element)
        {
            Initialize(element, 0);
        }
        public MatchElement(IDomObject element, int depth)
        {
            Initialize(element, depth);
        }
        protected void Initialize(IDomObject element, int depth)
        {
            Object = element;
            Depth = depth;
        }
        public IDomObject Object { get; set; }
        public int Depth { get; set; }
        public IDomElement Element { get { return (IDomElement)Object; } }
    }

}
