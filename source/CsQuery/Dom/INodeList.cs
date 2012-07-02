using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery
{
    public interface INodeList: IList<IDomObject>, ICollection<IDomObject>, IEnumerable<IDomObject>
    {
        /// <summary>
        /// The number of nodes in this INodeList
        /// </summary>

        int Length { get; }

        /// <summary>
        /// Adds a sequence of elements to the end of this INodeList
        /// </summary>
        ///
        /// <param name="elements">
        /// An IEnumerable&lt;IDomObject&gt; of items to append to this.
        /// </param>

        void AddRange(IEnumerable<IDomObject> elements);

        /// <summary>
        /// Add a node but do not attempt to clean up duplicate IDs or remove it from an existing DOM.
        /// This is required for the parser, but normally when you are using "Add" you want it to removed
        /// the ID from disconnected elements. This can also result in nodes appearing in more than one
        /// place in the DOM and should generally not be used by clients.
        /// 
        /// This may be used by end users, but should be used with caution.
        /// </summary>
        ///
        /// <param name="item">
        /// The object to add to this INodeList
        /// </param>

        void AddAlways(IDomObject item);
    }
}
