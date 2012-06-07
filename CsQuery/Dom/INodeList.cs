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
        /// Add a node but do not attempt to clean up duplicate IDs or remove it from an existing DOM. This is required for the parser, but normally
        /// when you are using "Add" you want it to removed the ID from disconnected elements. This can also result in nodes appearing in more than
        /// one place in the DOM and should generally not be used by clients.
        /// </summary>
        /// <param name="item"></param>
        void AddAlways(IDomObject item);
    }
}
