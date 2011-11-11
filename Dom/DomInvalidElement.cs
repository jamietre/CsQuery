using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery
{

    /// <summary>
    /// Sn element that will be rendered as text because it was determined to be a mismatched tag
    /// </summary>
    public interface IDomInvalidElement : IDomText
    {

    }

    public class DomInvalidElement : DomText, IDomInvalidElement
    {
        public DomInvalidElement()
            : base()
        {
        }
        //public DomInvalidElement(CsQuery owner)
        //    : base(owner)
        //{
        //}
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
            if (Dom != null &&
                Dom.DomRenderingOptions.HasFlag(DomRenderingOptions.RemoveMismatchedCloseTags))
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
