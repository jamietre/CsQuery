using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Engine
{
    public interface IPseudoSelector
    {
        /// <summary>
        /// Gets or sets criteria (or parameter) data passed with the pseudoselector
        /// </summary>

        public string Criteria {get;set;}

        /// <summary>
        /// Gets or sets zero-based index of this object.T
        /// </summary>

        public int Index { get; set; }

        public bool Matches(IDomObject obj); 
    }
}
