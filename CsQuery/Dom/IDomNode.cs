using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery
{
    public interface IDomNode : ICloneable 
    {
        NodeType NodeType { get; }

        string NodeName { get; set; }
        string NodeValue { get; set; }

        bool HasChildren { get; }
        INodeList ChildNodes { get; }
        IEnumerable<IDomElement> ChildElements { get; }

        bool Complete { get; }
        string Render();
        void Render(StringBuilder sb);
        void Render(StringBuilder sb, DomRenderingOptions options);
        void Remove();

        bool IsIndexed { get; }
        bool IsDisconnected { get; }

        new IDomNode Clone();
    }

    //public interface IDomNode<T> : IDomNode where T : IDomNode
    //{
    //    new IDomNode<T> Clone();
    //}
}
