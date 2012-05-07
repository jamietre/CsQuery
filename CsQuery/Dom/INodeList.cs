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
    }
}
