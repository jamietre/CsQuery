using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Implementation
{
    public class DomInvalidElement : DomText, IDomInvalidElement
    {
        public DomInvalidElement()
            : base()
        {
        }
        public DomInvalidElement(string text)
            : base(text)
        {

        }
        //public DomInvalidElement(int domTextIndex)
        //    : base(domTextIndex)
        //{
        //}
        public override string Render()
        {
            if (Document != null &&
                Document.DomRenderingOptions.HasFlag(DomRenderingOptions.RemoveMismatchedCloseTags))
            {
                return String.Empty;
            }
            else
            {
                return base.Render();
            }
        }

    }
    
}
