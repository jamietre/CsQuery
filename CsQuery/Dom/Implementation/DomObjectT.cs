using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Implementation
{

    /// <summary>
    /// Base class for anything that exists in the DOM
    /// </summary>
    /// 
    public abstract class DomObject<T> : DomObject, IDomObject<T> where T : IDomObject, new()
    {
        public DomObject()
        {
            
        }

        public abstract new T Clone();
        /// <summary>
        /// This is called by the base class DomObject, and ensures that the typed Clone implementations get called when
        /// the object is accessed through the IDomObject interface.
        /// </summary>
        /// <returns></returns>
        protected override IDomObject CloneImplementation()
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
        
    }
    
}
