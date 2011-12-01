using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery
{
    public interface IDomNode<T> : IDomNode where T : IDomNode
    {
        T Clone();
    }
    public interface IDomNode
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
    }
}
