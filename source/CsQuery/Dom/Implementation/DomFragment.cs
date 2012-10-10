using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CsQuery.ExtensionMethods;
using CsQuery.HtmlParser;

namespace CsQuery.Implementation
{
    /// <summary>
    /// An incomplete document fragment
    /// </summary>
    public class DomFragment : DomDocument, IDomFragment
    {
        /// <summary>
        /// Creates a new fragment in a given context.
        /// </summary>
        ///
        /// <param name="elements">
        /// The elements.
        /// </param>
        /// <param name="context">
        /// (optional) the context. If omitted, will be automatically determined.
        /// </param>
        /// <param name="docType">
        /// (optional) type of the document.
        /// </param>
        ///
        /// <returns>
        /// A new fragment
        /// </returns>

        public static IDomDocument Create(string html,
           string context=null,
           DocType docType = DocType.Default)
        {
            var factory = new ElementFactory();
            factory.FragmentContext = context;
            factory.HtmlParsingMode = HtmlParsingMode.Fragment;
            factory.HtmlParsingOptions = HtmlParsingOptions.AllowSelfClosingTags;
            factory.DocType = docType;
            
            using (var reader = new StringReader(html))
            {
                return factory.Parse(new StringReader(html));
            }
        }

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
