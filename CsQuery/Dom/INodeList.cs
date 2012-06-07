using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery
{
    public interface INodeList: IList<IDomObject>, ICollection<IDomObject>, IEnumerable<IDomObject>
    {
        int Length { get; }
        void AddRange(IEnumerable<IDomObject> elements);
        /// <summary>
        /// Add a node but do not attempt to clean up duplicate IDs. This is required for the parser, but normally
        /// when you are using "Add" you want it to removed the ID from disconnected elements.
        /// </summary>
        /// <param name="item"></param>
        void AddAlways(IDomObject item);
    }
}
