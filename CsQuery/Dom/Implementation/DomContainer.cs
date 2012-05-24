using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.ExtensionMethods;

namespace CsQuery.Implementation
{
    

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
        public override INodeList ChildNodes
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
        protected NodeList _ChildNodes;
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
        public override IDomElement FirstElementChild
        {
            get
            {
                if (HasChildren)
                {
                    int index=0;
                    while (index < ChildNodes.Count && ChildNodes[index].NodeType != NodeType.ELEMENT_NODE)
                    {
                        index++;
                    }
                    if (index < ChildNodes.Count)
                    {
                        return (IDomElement)ChildNodes[index];
                    }
                }
                return null;
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
        public override IDomElement LastElementChild
        {
            get
            {
                if (HasChildren)
                {
                    int index = ChildNodes.Count-1;
                    while (index >=0 && ChildNodes[index].NodeType != NodeType.ELEMENT_NODE)
                    {
                        index--;
                    }
                    if (index >=0)
                    {
                        return (IDomElement)ChildNodes[index];
                    }
                }
                return null;
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
                throw new InvalidOperationException("The reference node is not a child of this node");
            }
            ChildNodes.Insert(referenceNode.Index, newNode);
        }
        public override void InsertAfter(IDomObject newNode, IDomObject referenceNode)
        {
            if (referenceNode.ParentNode != this)
            {
                throw new InvalidOperationException("The reference node is not a child of this node");
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


        public override string Render()
        {
            StringBuilder sb = new StringBuilder();
            Render(sb);
            return sb.ToString();
        }
        public override void Render(StringBuilder sb)
        {
            Render(sb, Document==null ? 
                CQ.DefaultDomRenderingOptions :
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
        #region interface members
        IDomObject IDomObject.Clone()
        {
            return Clone();
        }

        IDomNode IDomNode.Clone()
        {
            return Clone();
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
        #endregion
    }

}
