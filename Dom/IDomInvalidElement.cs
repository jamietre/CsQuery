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
}
