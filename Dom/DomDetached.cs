using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Jtc.CsQuery
{
    /// <summary>
    /// Defines a root node for detached elements, so they can refer to another root node
    /// </summary>

    public interface IDomDetached : IDomRoot
    {
        string Text { get; set; }
    }


    /// <summary>
    /// Used for literal text (not part of a tag)
    /// </summary>
    public class DomDetached : DomRoot
    {
        public DomDetached(DomRoot root): base()
        {
            innerDom = root;
        }
        public override DomRoot Dom
        {
            get
            {
                return null;
            }
        }
        public override string GetString(int index)
        {
            return innerDom.GetString(index);
        }
        protected DomRoot innerDom;

        public override CsQuery Owner
        {
            get
            {
                return null;
            }
        }
        public override IEnumerable<IDomObject> CloneChildren()
        {
            yield break;
        }
    }
}
