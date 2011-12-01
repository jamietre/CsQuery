using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery
{
    /// <summary>
    /// Interface for objects that can contain other objects. Note that to allow some consistency with how DOM
    /// objects are used in the browser DOM, many methods are part of the base IDomObject interface so that they
    /// can be used (and return null/missing data) on elements to which they don't apply. So in actuality the only 
    /// unique methods are nonstandard ones.
    /// </summary>
    public interface IDomContainer : IDomObject
    {
        IEnumerable<IDomObject> CloneChildren();
    }

    /// <summary>
    /// Base class for Dom object that contain other elements
    /// </summary>
    public abstract class DomContainer<T> : DomObject<T>, IDomContainer where T : IDomObject, IDomContainer, new()
    {
        public DomContainer()
        {

        }
        public DomContainer(IEnumerable<IDomObject> elements): base()
        {
            ChildNodes.AddRange(elements);
        }


        public abstract IEnumerable<IDomObject> CloneChildren();
        /// <summary>
        /// Returns all children (including inner HTML as objects);
        /// </summary>
        public override NodeList ChildNodes
        {
            get
            {
                if (_ChildNodes == null)
                {
                    _ChildNodes = new NodeList(this);
                }
                return _ChildNodes;
            }
        }
        protected NodeList _ChildNodes = null;
        // Avoids creating children object when testing
        public override bool HasChildren
        {
            get
            {
                return _ChildNodes != null && ChildNodes.Count > 0;
            }
        }
        
        public override IDomObject FirstChild
        {
            get
            {
                if (HasChildren)
                {
                    return ChildNodes[0];
                }
                else
                {
                    return null;
                }
            }
        }
        public override IDomObject LastChild
        {
            get
            {
                if (HasChildren)
                {
                    return ChildNodes[ChildNodes.Count - 1];
                }
                else
                {
                    return null;
                }
            }
        }
        public override void AppendChild(IDomObject item)
        {
            ChildNodes.Add(item);
        }
        public override void RemoveChild(IDomObject item)
        {
            ChildNodes.Remove(item);
        }
        public override void InsertBefore(IDomObject newNode, IDomObject referenceNode)
        {
            if (referenceNode.ParentNode != this)
            {
                throw new Exception("The reference node is not a child of this node");
            }
            ChildNodes.Insert(referenceNode.Index, newNode);
        }
        public override void InsertAfter(IDomObject newNode, IDomObject referenceNode)
        {
            if (referenceNode.ParentNode != this)
            {
                throw new Exception("The reference node is not a child of this node");
            }
            ChildNodes.Insert(referenceNode.Index + 1, newNode);
        }
        /// <summary>
        /// Returns all elements
        /// </summary>
        public override IEnumerable<IDomElement> ChildElements
        {
            get
            {
                if (HasChildren)
                {
                    foreach (IDomObject obj in ChildNodes)
                    {
                        var elm = obj as IDomElement;
                        if (elm != null)
                        {
                            yield return elm;
                        }
                    }
                }
                yield break;
            }
        }
        public IDomObject this[int index]
        {
            get
            {
                return ChildNodes[index];
            }
        }

        public override string Render()
        {
            StringBuilder sb = new StringBuilder();
            Render(sb);
            return sb.ToString();
        }
        public override void Render(StringBuilder sb)
        {
            Render(sb, Document==null ? 
                CsQuery.DefaultDomRenderingOptions :
                Document.DomRenderingOptions);
        }
        public override void Render(StringBuilder sb, DomRenderingOptions options)
        {
            if (HasChildren)
            {
                foreach (IDomObject e in ChildNodes)
                {
                    e.Render(sb, options);
                }
            }
        }

        // Just didn't use the / and the +. A three character ID will permit over 250,000 possible children at each level
        // so that should be plenty
        

        public override int DescendantCount()
        {
            int count = 0;
            if (HasChildren)
            {
                foreach (IDomObject obj in ChildNodes)
                {
                    count += 1 + obj.DescendantCount();
                }
            }
            return count;
        }
    }

}
