using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.ExtensionMethods;
using CsQuery.HtmlParser;

namespace CsQuery.Implementation
{
    /// <summary>
    /// An incomplete document fragment
    /// </summary>
    public class DomFragment : DomDocument, IDomFragment
    {

        public DomFragment()
            : base()
        {
        }
        //public DomFragment(IEnumerable<IDomObject> elements): base(elements)
        //{
        
        //}
        //public DomFragment(char[] html, HtmlParsingMode htmlParsingMode)
        //    : base(html, htmlParsingMode)
        //{
            
        //}
        
        public override NodeType NodeType
        {
            get { return  NodeType.DOCUMENT_FRAGMENT_NODE; }
        }

        /// <summary>
        /// Gets a value indicating whether this object is indexed. 
        /// </summary>

        public override bool IsIndexed
        {
            get
            {
                return true;
            }
        }
        public override bool IsFragment
        {
            get
            {
                return true;
            }
        }

        public override IDomDocument CreateNew()
        {
            return CreateNew<IDomFragment>();
        }
    }
    
}
