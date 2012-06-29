using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery
{
    /// <summary>
    /// DOCTYPE node
    /// </summary>
    public interface IDomDocumentType : IDomSpecialElement
    {
        DocType DocType { get; set; }
    }

}
