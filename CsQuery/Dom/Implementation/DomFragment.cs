using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.ExtensionMethods;

namespace CsQuery.Implementation
{
    /// <summary>
    /// Special node type to represent the DOM.
    /// </summary>
    public class DomFragment : DomDocument, IDomFragment
    {

        public DomFragment()
            : base()
        {
        }
        public DomFragment(IEnumerable<IDomObject> elements): base(elements)
        {
        
        }
        public DomFragment(char[] html): base(html)
        {
            
        }
        
        public override NodeType NodeType
        {
            get { return  NodeType.DOCUMENT_FRAGMENT_NODE; }
        }
        
    }
    
}
