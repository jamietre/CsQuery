using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using CsQuery.HtmlParser;
using CsQuery.Utility;

namespace CsQuery.Implementation
{
    /// <summary>
    /// A FORM element.
    /// </summary>
    ///
    /// <url>
    /// http://dev.w3.org/html5/spec/single-page.html#the-form-element
    /// </url>

    public class HtmlFormElement : DomElement, IHTMLFormElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>

        public HtmlFormElement()
            : base(HtmlData.tagFORM)
        {
        }

        /// <summary>
        /// A name or keyword giving a browsing context for UAs to use when following the hyperlink.
        /// </summary>

        public string Target
        {
            get
            {
                return GetAttribute("target");
            }
            set
            {
                SetAttribute("target", value);
            }
        }

        public string AcceptCharset
        {
            get
            {
                return GetAttribute("acceptcharset");
            }
            set
            {
                SetAttribute("acceptcharset", value);
            }
        }

        public string Action
        {
            get
            {
                return GetAttribute("action");
            }
            set
            {
                SetAttribute("action", value);
            }
        }

        public string Autocomplete
        {
            get
            {
                return GetAttribute("autocomplete");
            }
            set
            {
                SetAttribute("autocomplete", value);
            }
        }

        public string Enctype
        {
            get
            {
                return GetAttribute("enctype");
            }
            set
            {
                SetAttribute("enctype", value);
            }
        }

        public string Encoding
        {
            get
            {
                return GetAttribute("encoding");
            }
            set
            {
                SetAttribute("encoding", value);
            }
        }

        public string Method
        {
            get
            {
                return GetAttribute("method");
            }
            set
            {
                SetAttribute("method", value);
            }
        }

        public bool NoValidate
        {
            get
            {
                return HasAttribute("novalidate");
            }
            set
            {
                SetProp("novalidate",value);
            }
        }

        public INodeList<IDomElement> Elements
        {
            get {
                return new NodeList<IDomElement>(Document.QuerySelectorAll(":input"));
            }
        }

        public IList<IDomElement> ToList()
        {
            return new List<IDomElement>(this).AsReadOnly();
        }


        public int Length
        {
            get { return Elements.Length; }
        }

        public IDomElement Item(int index)
        {
            return Elements[index];
        }

        [IndexerName("Indexer")]
        public new IDomElement this[int index]
        {
            get { return Item(index); }
        }

        int IReadOnlyCollection<IDomElement>.Count
        {
            get { return Elements.Length; }
        }

        public IEnumerator<IDomElement> GetEnumerator()
        {
            return Elements.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
